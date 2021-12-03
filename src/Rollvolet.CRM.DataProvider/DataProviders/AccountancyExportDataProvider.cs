using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.DataProvider.Extensions;
using Rollvolet.CRM.Domain.Configuration;
using Rollvolet.CRM.Domain.Contracts;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;
using Rollvolet.CRM.Domain.Utils;

namespace Rollvolet.CRM.DataProviders
{
    public class AccountancyExportDataProvider : IAccountancyExportDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;
        private readonly AccountancyConfiguration _accountancyConfig;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger _logger;

        public AccountancyExportDataProvider(CrmContext context, IMapper mapper,
                                        IOptions<AccountancyConfiguration> accountancyConfiguration,
                                        IFileStorageService fileStorageService,
                                        ILogger<AccountancyExportDataProvider> logger)
        {
            _context = context;
            _mapper = mapper;
            _accountancyConfig = accountancyConfiguration.Value;
            _fileStorageService = fileStorageService;
            _logger = logger;

            _accountancyConfig.WinbooksExportLocation = _fileStorageService.EnsureDirectory(_accountancyConfig.WinbooksExportLocation);
        }

        public async Task<Paged<AccountancyExport>> GetAllAsync(QuerySet query)
        {
            var source = _context.AccountancyExports
                            .Sort(query)
                            .Filter(query);

            var accountancyExports = await source.ForPage(query).ToListAsync();

            var mappedAccountancyExports = _mapper.Map<IEnumerable<AccountancyExport>>(accountancyExports);

            var count = await source.CountAsync();

            return new Paged<AccountancyExport>() {
                Items = mappedAccountancyExports,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }

        public async Task<AccountancyExport> GetByIdAsync(int id, QuerySet query = null)
        {
            var accountancyExport = await _context.AccountancyExports.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (accountancyExport == null)
            {
                _logger.LogError($"No accountancy-export found with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<AccountancyExport>(accountancyExport);
        }

        public async Task<AccountancyExport> CreateAsync(AccountancyExport accountancyExport)
        {
            var accountancyExportRecord = _mapper.Map<DataProvider.Models.AccountancyExport>(accountancyExport);

            _context.AccountancyExports.Add(accountancyExportRecord);

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await GenerateExportFilesAsync((int) accountancyExport.FromNumber, (int) accountancyExport.UntilNumber, accountancyExport.IsDryRun);
                    await _context.SaveChangesAsync();

                    transaction.Commit();

                    return _mapper.Map<AccountancyExport>(accountancyExportRecord);
                }
                catch (InvalidDataException e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }

        private async Task GenerateExportFilesAsync(int fromNumber, int untilNumber, bool isDryRun)
        {
            var invoiceQuery = _context.Invoices
                                    .Where(i => i.Number >= fromNumber && i.Number <= untilNumber && i.BookingDate == null)
                                    .Include(i => i.VatRate);
            var invoiceRecords = await invoiceQuery.ToListAsync();
            var customerRecordIds = invoiceRecords.Select(i => i.CustomerId).Where(id => id != null).Distinct().ToList();
            var customerRecords = await _context.Customers
                                                .Where(c => customerRecordIds.Contains(c.Number)) // Export all customer, also previously exported ones
                                                .Include(c => c.Country)
                                                .ToListAsync();

            // TODO validate if there are gaps that have already been booked

            // Invoices export

            var invoiceLines = new List<object>();
            var now = DateTimeOffset.UtcNow.UtcDateTime;
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

            if (isDryRun)
                _logger.LogInformation($"Starting dry run of accountancy export. Simulating booking of {invoiceRecords.Count} invoices and {customerRecords.Count} customers..");
            else
                _logger.LogInformation($"Starting accountancy export. {invoiceRecords.Count} invoices and {customerRecords.Count} will be booked.");

            foreach (var invoiceRecord in invoiceRecords)
            {
                EnsureValidityForExport(invoiceRecord);
                invoiceLines.AddRange(GenerateInvoiceExportLines(invoiceRecord));
                _logger.LogDebug($"Exported invoice {invoiceRecord.Number} for accountancy export.");

                if (!isDryRun)
                    invoiceRecord.BookingDate = now;
            }

            using (var memoryStream = new MemoryStream())
            using (var writer = new StreamWriter(memoryStream))
            using (var csv = new CsvWriter(writer))
            {
                csv.Configuration.HasHeaderRecord = false;
                csv.Configuration.Delimiter = ",";
                csv.Configuration.ShouldQuote = (field, ctx) => { return false; };
                csv.WriteRecords(invoiceLines);
                writer.Flush();
                memoryStream.Position = 0;
                await _fileStorageService.UploadDocumentAsync(_accountancyConfig.WinbooksExportLocation, _accountancyConfig.WinbooksInvoicesFile, memoryStream);
                memoryStream.Position = 0;
                var timestampedFile = $"{timestamp}-{_accountancyConfig.WinbooksInvoicesFile}";
                await _fileStorageService.UploadDocumentAsync(_accountancyConfig.WinbooksExportLocation, timestampedFile, memoryStream);
            }

            // Customers export

            var customerLines = new List<object>();

            foreach (var customerRecord in customerRecords)
            {
                customerLines.Add(GenerateCustomerExportLine(customerRecord));
                _logger.LogDebug($"Exported customer {customerRecord.Number} for accountancy export.");

                if (!isDryRun)
                    customerRecord.BookingDate = now;
            }

            using (var memoryStream = new MemoryStream())
            using (var writer = new StreamWriter(memoryStream))
            using (var csv = new CsvWriter(writer))
            {
                csv.Configuration.HasHeaderRecord = false;
                csv.Configuration.Delimiter = ",";
                csv.Configuration.ShouldQuote = (field, ctx) => { return false; };
                csv.WriteRecords(customerLines);
                writer.Flush();
                memoryStream.Position = 0;
                await _fileStorageService.UploadDocumentAsync(_accountancyConfig.WinbooksExportLocation, _accountancyConfig.WinbooksCustomersFile, memoryStream);
                memoryStream.Position = 0;
                var timestampedFile = $"{timestamp}-{_accountancyConfig.WinbooksCustomersFile}";
                await _fileStorageService.UploadDocumentAsync(_accountancyConfig.WinbooksExportLocation, timestampedFile, memoryStream);
            }
        }

        private void EnsureValidityForExport(DataProvider.Models.Invoice invoice)
        {
            if (invoice.Number == null)
                throw new InvalidDataException($"Invoice {invoice.Id} doesn't have an invoice date and cannot be exported.");
            if (invoice.InvoiceDate == null)
                throw new InvalidDataException($"Invoice {invoice.Id} doesn't have an invoice date and cannot be exported.");
            if (invoice.Amount == null)
                throw new InvalidDataException($"Invoice {invoice.Id} doesn't have an amount and cannot be exported.");
            if (invoice.Vat == null)
                throw new InvalidDataException($"Invoice {invoice.Id} doesn't have a VAT amount and cannot be exported.");
            if (invoice.TotalAmount == null)
                throw new InvalidDataException($"Invoice {invoice.Id} doesn't have a total amount and cannot be exported.");
        }

        private IEnumerable<object> GenerateInvoiceExportLines(DataProvider.Models.Invoice invoice)
        {
            var invoiceDate = (DateTime) invoice.InvoiceDate;
            var invoiceDateStr = invoiceDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            var dueDate = invoice.DueDate != null ? (DateTime) invoice.DueDate : (DateTime) invoice.InvoiceDate;
            var dueDateStr = dueDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture);

            var startYear = Int32.Parse(_accountancyConfig.WinbooksStart);
            var bookYear = GetBookYear(invoiceDate, startYear);
            var startBookYear = Int32.Parse(_accountancyConfig.WinbooksBookYear);
            var startMonth = startBookYear >= 100 ? startBookYear / 100 : 0;
            var period = GetPeriod(invoiceDate, startMonth);

            var amount = Math.Round((double) invoice.Amount, 2);
            var totalAmount = Math.Round((double) invoice.TotalAmount, 2);
            var vatAmount = Math.Round((double) invoice.Vat, 2);

            if (invoice.IsCreditNote)
            {
                amount *= -1;
                totalAmount *= -1;
                vatAmount *= -1;
            }

            var grossAmountSalesLine = new {
                DocType = "1",
                DBKCode = _accountancyConfig.WinbooksDiary,
                DBKType = "2",
                DocNumber = ((int) invoice.Number).ToString("D5"), // format with minimal 5 digits
                DocOrder = "001",
                OPCode = "",
                AccountGL = "400000",
                AccountRP = invoice.CustomerId.ToString(),
                BookYear = bookYear,
                Period = period,
                Date = invoiceDateStr,
                DateDoc = invoiceDateStr,
                DueDate = dueDateStr,
                Comment = "",
                CommentText = "",
                Amount = "0.000",
                AmountEUR = FormatDecimal(totalAmount),
                VATBase = FormatDecimal(amount),
                VATCode = "",
                CurrAmount = "0.000",
                CurrCode = "",
                CurEURBase = "0.000",
                VATTax = FormatDecimal(vatAmount),
                VATInput = "",
                CurrRate = "0.00000",
                RemindLev = "0",
                MatchNo = "",
                OldDate = "    ",
                IsMatched = "T",
                IsLocked = "F",
                IsImported = "F",
                IsPositve = "T",
                IsTemp = "F",
                MemoType = " ",
                IsDoc = "F",
                DocStatus = " ",
                DICFrom = "",
                CODAKey = ""
            };

            var account = GetAccount(invoice.VatRate);
            var vatCode = GetVatCode(invoice.VatRate);

            var netAmountSalesLine = new {
                DocType = "3",
                DBKCode = _accountancyConfig.WinbooksDiary,
                DBKType = "2",
                DocNumber = ((int) invoice.Number).ToString("D5"), // format with minimal 5 digits
                DocOrder = "002",
                OPCode = "",
                AccountGL = account,
                AccountRP = invoice.CustomerId.ToString(),
                BookYear = bookYear,
                Period = period,
                Date = invoiceDateStr,
                DateDoc = invoiceDateStr,
                DueDate = dueDateStr,
                Comment = "",
                CommentText = "",
                Amount = "0.000",
                AmountEUR = FormatDecimal(amount * -1),
                VATBase = "0.000",
                VATCode = "",
                CurrAmount = "0.000",
                CurrCode = "",
                CurEURBase = "0.000",
                VATTax = "0.000",
                VATInput = vatCode,
                CurrRate = "0.00000",
                RemindLev = "0",
                MatchNo = "",
                OldDate = "    ",
                IsMatched = "T",
                IsLocked = "F",
                IsImported = "F",
                IsPositve = "T",
                IsTemp = "F",
                MemoType = " ",
                IsDoc = "F",
                DocStatus = " ",
                DICFrom = "",
                CODAKey = ""
            };

            var docType = GetDocType(invoice.VatRate);

            var vatLine = new {
                DocType = docType,
                DBKCode = _accountancyConfig.WinbooksDiary,
                DBKType = "2",
                DocNumber = ((int) invoice.Number).ToString("D5"), // format with minimal 5 digits
                DocOrder = "VAT",
                OPCode = "FIXED",
                AccountGL = docType == "4" ? "" : "451000",
                AccountRP = invoice.CustomerId.ToString(),
                BookYear = bookYear,
                Period = period,
                Date = invoiceDateStr,
                DateDoc = invoiceDateStr,
                DueDate = dueDateStr,
                Comment = "",
                CommentText = "",
                Amount = "0.000",
                AmountEUR = FormatDecimal(vatAmount * -1),
                VATBase = FormatDecimal(invoice.IsCreditNote ? amount : Math.Abs(amount)),
                VATCode = vatCode,
                CurrAmount = "0.000",
                CurrCode = "",
                CurEURBase = "0.000",
                VATTax = "0.000",
                VATInput = "",
                CurrRate = "0.00000",
                RemindLev = "0",
                MatchNo = "",
                OldDate = "    ",
                IsMatched = "F",
                IsLocked = "F",
                IsImported = "F",
                IsPositve = "T",
                IsTemp = "F",
                MemoType = " ",
                IsDoc = "F",
                DocStatus = " ",
                DICFrom = "",
                CODAKey = ""
            };

            return new object[] { grossAmountSalesLine, netAmountSalesLine, vatLine };
        }

        private object GenerateCustomerExportLine(DataProvider.Models.Customer customer)
        {
            return new {
                Number = customer.Number,
                Type = "1",
                Name1 = FormatAsName(customer.Name),
                Name2 = "",
                CivName1 = "",
                CivName2 = "",
                Address1 = FormatAsName(customer.Address1),
                Address2 =FormatAsName(customer.Address2),
                VATCat = customer.IsCompany ? "1" : "3",
                Country = customer.Country != null ? customer.Country.Code : "??",
                VatNumber = FormatVatNumber(customer.VatNumber),
                PayCode = "",
                TelNumber = "",
                FaxNumber = "",
                BnkAccount = "",
                ZipCode = FormatAsName(customer.EmbeddedPostalCode),
                City = FormatAsName(customer.EmbeddedCity),
                DefitPost = "",
                Lang = "",
                Category = "",
                Central = "",
                VatCode = "",
                Currency = "EUR",
                LastRemDev = "",
                LastRemDat = "",
                TotDeb1 = "0.000",
                TotCre1 = "0.000",
                TotDebTmp1 = "0.000",
                TotCreTmp1 = "0.000",
                TotDeb2 = "0.000",
                TotCre2 = "0.000",
                TotDebTmp2 = "0.000",
                TotCreTmp2 = "0.000",
                IsLocked = "F",
                MemoType = "",
                IsDoc = "T",
                F28150 = ""
            };
        }


        private string GetBookYear(DateTime invoiceDate, int startYear)
        {
            var index = invoiceDate.Year - startYear + 1;
            return "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"[index].ToString();
        }

        private string GetPeriod(DateTime invoiceDate, int startMonth)
        {
            var month = invoiceDate.Month;
            var diff = month - startMonth + 1;

            if (diff <= 0)
                diff += 12;

            return diff.ToString("D2"); // format with 2 digits
        }

        private string GetAccount(DataProvider.Models.VatRate vatRate)
        {
            var vatCode = vatRate != null ? vatRate.Code : null;

            if (vatCode == "6")
                return "700000";
            else if (vatCode == "12")
                return "701000";
            else if (vatCode == "21")
                return "702000";
            else if (vatCode == "vry")
                return "703000";
            else if (vatCode == "exp")
                return "704000";
            else if (vatCode == "m")
                return "705000";
            else if (vatCode == "e")
                return "706000";
            else if (vatCode == "0")
                return "707000";
            else
                return "??????";
        }

        private string GetVatCode(DataProvider.Models.VatRate vatRate)
        {
            var vatCode = vatRate != null ? vatRate.Code : null;

            if (vatCode == "6")
                return "211200";
            else if (vatCode == "12")
                return "211300";
            else if (vatCode == "21")
                return "211400";
            else if (vatCode == "vry")
                return "244600";
            else if (vatCode == "exp")
                return "231000";
            else if (vatCode == "m")
                return "212000";
            else if (vatCode == "e")
                return "221000";
            else if (vatCode == "0")
                return "211100";
            else
                return "??????";
        }

        private string GetDocType(DataProvider.Models.VatRate vatRate)
        {
            var vatCode = vatRate != null ? vatRate.Code : null;

            if (vatCode == "6" || vatCode == "12" || vatCode == "21")
                return "3";
            else if (vatCode == "vry" || vatCode == "exp" || vatCode == "m" || vatCode == "e" || vatCode == "0")
                return "4";
            else
                return "?";
        }

        private string FormatAsName(string name)
        {
            if (name != null)
            {
                var formattedName = name.Replace(',', ' ').ReplaceLineBreaks("-");
                return formattedName.Length >= 40 ? formattedName.Substring(0, 40) : formattedName;
            }
            else
            {
                return "";
            }
        }

        private string FormatVatNumber(string vatNumber)
        {
            if (vatNumber != null && vatNumber.Length >= 2)
            {
                var country = vatNumber.Substring(0, 2).ToUpper();
                if (country == "BE")
                {
                    vatNumber = vatNumber.Substring(2).Trim();
                    // VAT number used to be 000.000.000 but must be 0000.000.000 now
                    return vatNumber.Length <= 11 ? $"0{vatNumber}" : vatNumber;
                }
            }

            return "";
        }

        private string FormatDecimal(double number)
        {
            return number.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}