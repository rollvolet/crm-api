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
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.DataProvider.Extensions;
using Rollvolet.CRM.Domain.Configuration;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.DataProviders
{
    public class AccountancyExportDataProvider : IAccountancyExportDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public AccountancyExportDataProvider(CrmContext context, IMapper mapper, ILogger<AccountancyExportDataProvider> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Paged<AccountancyExport>> GetAllAsync(QuerySet query)
        {
            var source = _context.AccountancyExports
                            .Sort(query)
                            .Filter(query);

            var accountancyExports = source.ForPage(query).AsEnumerable();

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

        public async Task<AccountancyExport> CreateAsync(AccountancyExport accountancyExport, AccountancyConfiguration configuration)
        {
            var accountancyExportRecord = _mapper.Map<DataProvider.Models.AccountancyExport>(accountancyExport);

            _context.AccountancyExports.Add(accountancyExportRecord);

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await GenerateExportFilesAsync((int) accountancyExport.FromNumber, (int) accountancyExport.UntilNumber, accountancyExport.IsDryRun, configuration);
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

        private async Task GenerateExportFilesAsync(int fromNumber, int untilNumber, bool isDryRun, AccountancyConfiguration configuration)
        {
            var invoiceQuery = _context.Invoices
                                    .Where(i => i.Number >= fromNumber && i.Number <= untilNumber && i.BookingDate == null)
                                    .Include(i => i.VatRate);
            var invoiceRecords = await invoiceQuery.ToListAsync();
            var customerRecords = await invoiceQuery.Select(i => i.Customer)
                                                .Where(c => c != null && c.BookingDate == null)
                                                .Include(c => c.Country)
                                                .Distinct().ToListAsync();

            // TODO validate if there are gaps that have already been booked

            // Invoices export

            var invoiceLines = new List<object>();
            var now = DateTime.Now;

            if (isDryRun)
                _logger.LogInformation($"Starting dry run of accountancy export. Simulating booking of {invoiceRecords.Count} invoices and {customerRecords.Count} customers..");
            else
                _logger.LogInformation($"Starting accountancy export. {invoiceRecords.Count} invoices and {customerRecords.Count} will be booked.");

            foreach (var invoiceRecord in invoiceRecords)
            {
                EnsureValidityForExport(invoiceRecord);
                invoiceLines.AddRange(GenerateInvoiceExportLines(invoiceRecord, configuration));
                _logger.LogDebug($"Exported invoice {invoiceRecord.Number} for accountancy export.");

                if (!isDryRun)
                    invoiceRecord.BookingDate = now;
            }

            using (var writer = new StreamWriter($"{configuration.WinbooksExportLocation}{configuration.WinbooksInvoicesFile}"))
            using (var csv = new CsvWriter(writer))
            {
                csv.Configuration.HasHeaderRecord = false;
                csv.Configuration.Delimiter = ",";
                csv.Configuration.ShouldQuote = (field, ctx) => { return false; };
                csv.WriteRecords(invoiceLines);
            }

            // Customers export

            var customerLines = new List<object>();

            foreach (var customerRecord in customerRecords)
            {
                customerLines.Add(GenerateCustomerExportLine(customerRecord, configuration));
                _logger.LogDebug($"Exported customer {customerRecord.Number} for accountancy export.");

                if (!isDryRun)
                    customerRecord.BookingDate = now;
            }

            using (var writer = new StreamWriter($"{configuration.WinbooksExportLocation}{configuration.WinbooksCustomersFile}"))
            using (var csv = new CsvWriter(writer))
            {
                csv.Configuration.HasHeaderRecord = false;
                csv.WriteRecords(customerLines);
            }
        }

        private void EnsureValidityForExport(DataProvider.Models.Invoice invoice)
        {
            if (invoice.Number == null)
                throw new InvalidDataException($"Invoice {invoice.Id} doesn't have an invoice date and cannot be exported.");
            if (invoice.InvoiceDate == null)
                throw new InvalidDataException($"Invoice {invoice.Id} doesn't have an invoice date and cannot be exported.");
            if (invoice.DueDate == null)
                throw new InvalidDataException($"Invoice {invoice.Id} doesn't have a due date and cannot be exported.");
            if (invoice.Amount == null)
                throw new InvalidDataException($"Invoice {invoice.Id} doesn't have an amount and cannot be exported.");
            if (invoice.Vat == null)
                throw new InvalidDataException($"Invoice {invoice.Id} doesn't have a VAT amount and cannot be exported.");
            if (invoice.TotalAmount == null)
                throw new InvalidDataException($"Invoice {invoice.Id} doesn't have a total amount and cannot be exported.");
        }

        private IEnumerable<object> GenerateInvoiceExportLines(DataProvider.Models.Invoice invoice, AccountancyConfiguration configuration)
        {
            var invoiceDate = (DateTime) invoice.InvoiceDate;
            var invoiceDateStr = invoiceDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            var dueDate = (DateTime) invoice.DueDate;
            var dueDateStr = dueDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture);

            var startYear = Int32.Parse(configuration.WinbooksStart);
            var bookYear = GetBookYear(invoiceDate, startYear);
            var startBookYear = Int32.Parse(configuration.WinbooksBookYear);
            var startMonth = startBookYear >= 100 ? startBookYear / 100 : 0;
            var period = GetPeriod(invoiceDate, startMonth);

            var amount = (double) invoice.Amount;
            var totalAmount = (double) invoice.TotalAmount;
            var vatAmount = (double) invoice.Vat;

            if (invoice.IsCreditNote)
            {
                amount *= -1;
                totalAmount *= -1;
                vatAmount *= -1;
            }

            var grossAmountSalesLine = new {
                DocType = "1",
                DBKCode = configuration.WinbooksDiary,
                DBKType = "2",
                DocNumber = ((int) invoice.Number).ToString("D5"), // format with 5 digits
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
                AmountEUR = totalAmount.ToString("0.000"),
                VATBase = amount.ToString("0.000"),
                VATCode = "",
                CurrAmount = "0.000",
                CurrCode = "",
                CurEURBase = "0.000",
                VATTax = vatAmount.ToString("0.000"),
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
                DBKCode = configuration.WinbooksDiary,
                DBKType = "2",
                DocNumber = ((int) invoice.Number).ToString("D5"), // format with 5 digits
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
                AmountEUR = (amount * -1).ToString("0.000"),
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
                DBKCode = configuration.WinbooksDiary,
                DBKType = "2",
                DocNumber = ((int) invoice.Number).ToString("D5"), // format with 5 digits
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
                AmountEUR = (vatAmount * -1).ToString("0.000"),
                VATBase = (invoice.IsCreditNote ? amount : Math.Abs(amount)).ToString("0.000"),
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

        private object GenerateCustomerExportLine(DataProvider.Models.Customer customer, AccountancyConfiguration configuration)
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
                var formattedName = name.Replace(',', ' ');
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
                    return vatNumber;
                }
            }

            return "";
        }
    }
}