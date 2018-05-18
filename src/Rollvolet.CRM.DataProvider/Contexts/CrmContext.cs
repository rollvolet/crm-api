using Microsoft.EntityFrameworkCore;
using Rollvolet.CRM.DataProvider.Models;

namespace Rollvolet.CRM.DataProvider.Contexts
{
    public class CrmContext : DbContext
    {
        public DbSet<Building> Buildings { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<HonorificPrefix> HonorificPrefixes { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<PostalCode> PostalCodes { get; set; }
        public DbSet<Telephone> Telephones { get; set; }
        public DbSet<TelephoneType> TelephoneTypes { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<WayOfEntry> WayOfEntries { get; set; }
        public DbSet<Memo> Memos { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<CustomerTag> CustomerTags { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Deposit> Deposits { get; set; }
        public DbSet<DepositInvoiceHub> DepositInvoices { get; set; }
        public DbSet<WorkingHour> WorkingHours { get; set; }
        public DbSet<VatRate> VatRates { get; set; }
        public DbSet<SubmissionType> SubmissionTypes { get; set; }

        public CrmContext(DbContextOptions<CrmContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Customer / contact / building

            modelBuilder.Entity<CustomerRecord>()
                .ToTable("tblData", schema: "dbo")
                .HasDiscriminator<string>("DataType")
                .HasValue<Customer>("KLA")
                .HasValue<Contact>("CON")
                .HasValue<Building>("GEB");

            modelBuilder.Entity<CustomerRecord>()
                .HasKey(b => b.DataId) // primary key
                .HasName("TblData$PrimaryKey");

            modelBuilder.Entity<Customer>()
                .HasOne(e => e.HonorificPrefix)
                .WithMany()
                .HasForeignKey(e => new { e.HonorificPrefixId, e.LanguageId });

            modelBuilder.Entity<Customer>()
                .HasOne(e => e.Memo)
                .WithOne()
                .HasForeignKey<Memo>(e => e.CustomerId);

            modelBuilder.Entity<Contact>()
                .HasOne(e => e.Customer)
                .WithMany(e => e.Contacts)
                .HasForeignKey(e => e.CustomerId)
                .HasPrincipalKey(e => e.Number);

            modelBuilder.Entity<Contact>()
                .HasOne(e => e.HonorificPrefix)
                .WithMany()
                .HasForeignKey(e => new { e.HonorificPrefixId, e.LanguageId });

            modelBuilder.Entity<Building>()
                .HasOne(e => e.Customer)
                .WithMany(e => e.Buildings)
                .HasForeignKey(e => e.CustomerId)
                .HasPrincipalKey(e => e.Number);

            modelBuilder.Entity<Building>()
                .HasOne(e => e.HonorificPrefix)
                .WithMany()
                .HasForeignKey(e => new { e.HonorificPrefixId, e.LanguageId });


            // Country

            modelBuilder.Entity<Country>()
                .ToTable("TblLand", schema: "dbo");

            modelBuilder.Entity<Country>()
                .HasKey(e => e.Id)
                .HasName("TblLand$PrimaryKey");


            // HonorificPrefix

            modelBuilder.Entity<HonorificPrefix>()
                .ToTable("TblAanspreektitel", schema: "dbo");

            modelBuilder.Entity<HonorificPrefix>()
                .HasKey(e => new { e.Id, e.LanguageId })
                .HasName("TblAanspreektitel$PrimaryKey");


            // Language

            modelBuilder.Entity<Language>()
                .ToTable("TblTaal", schema: "dbo");

            modelBuilder.Entity<Language>()
                .HasKey(e => e.Id)
                .HasName("TblTaal$PrimaryKey");


            // PostalCode

            modelBuilder.Entity<PostalCode>()
                .ToTable("TblPostCode", schema: "dbo");

            modelBuilder.Entity<PostalCode>()
                .HasKey(e => e.Id)
                .HasName("TblPostCode$PrimaryKey");


            // Telephone

            modelBuilder.Entity<Telephone>()
                .ToTable("tblTel", schema: "dbo");

            modelBuilder.Entity<Telephone>()
                .HasKey(e => new { e.CustomerRecordId, e.TelephoneTypeId, e.CountryId, e.Area, e.Number })
                .HasName("tblTel$PrimaryKey");

            modelBuilder.Entity<Telephone>()
                .HasOne(e => e.CustomerRecord)
                .WithMany(e => e.Telephones)
                .HasForeignKey(e => e.CustomerRecordId)
                .HasPrincipalKey(e => e.DataId);


            // Telephone Type

            modelBuilder.Entity<TelephoneType>()
                .ToTable("TblTelType", schema: "dbo");

            modelBuilder.Entity<TelephoneType>()
                .HasKey(e => e.Id)
                .HasName("TblTelType$PrimaryKey");


            // Memo

            modelBuilder.Entity<Memo>()
                .ToTable("TblDataMemo", schema: "dbo");

            modelBuilder.Entity<Memo>()
                .HasKey(e => e.CustomerId);


            // Tag

            modelBuilder.Entity<Tag>()
                .ToTable("TblKeyWord", schema: "dbo");

            modelBuilder.Entity<Tag>()
                .HasKey(e => e.Id)
                .HasName("TblKeyWord$PrimaryKey");


            // CustomerTag

            modelBuilder.Entity<CustomerTag>()
                .ToTable("TblDataKeyWord", schema: "dbo");

            modelBuilder.Entity<CustomerTag>()
                .HasKey(e => new { e.CustomerId, e.TagId });

            modelBuilder.Entity<CustomerTag>()
                .HasOne(e => e.Customer)
                .WithMany(e => e.CustomerTags)
                .HasForeignKey(e => e.CustomerId);

            modelBuilder.Entity<CustomerTag>()
                .HasOne(e => e.Tag)
                .WithMany(e => e.CustomerTags)
                .HasForeignKey(e => e.TagId);


            // Request

            modelBuilder.Entity<Request>()
                .ToTable("TblAanvraag", schema: "dbo");

            modelBuilder.Entity<Request>()
                .HasKey(e => e.Id)
                .HasName("TblAanvraag$PrimaryKey");

            modelBuilder.Entity<Request>()
                .HasOne(e => e.Customer)
                .WithMany(e => e.Requests)
                .HasForeignKey(e => e.CustomerId)
                .HasPrincipalKey(e => e.Number);

            // The below configuration doesn't work since the FK may not include the CustomerId field on a derived type
            // As a consequence the Building and Contact need to be embedded manually in the Request resources

            // modelBuilder.Entity<Request>()
            //     .HasOne(e => e.Building)
            //     .WithMany()
            //     .HasForeignKey(e => new { e.RelativeBuildingId, e.CustomerId })
            //     .HasPrincipalKey(e => new { e.Number, e.CustomerId });

            // modelBuilder.Entity<Request>()
            //     .HasOne(e => e.Contact)
            //     .WithMany()
            //     .HasForeignKey(e => new { e.RelativeContactId, e.CustomerId })
            //     .HasPrincipalKey(e => new { e.Number, e.CustomerId });


            // Visit

            modelBuilder.Entity<Visit>()
                .ToTable("TblBezoek", schema: "dbo");

            modelBuilder.Entity<Visit>()
                .HasKey(e => e.Id)
                .HasName("TblBezoek$PrimaryKey");

            modelBuilder.Entity<Visit>()
                .HasOne(e => e.Request)
                .WithOne(e => e.Visit)
                .HasForeignKey<Visit>(e => e.RequestId);

            modelBuilder.Entity<Visit>()
                .HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId);


            // Way of Entry

            modelBuilder.Entity<WayOfEntry>()
                .ToTable("TblAanmelding", schema: "dbo");

            modelBuilder.Entity<WayOfEntry>()
                .HasKey(e => e.Id)
                .HasName("TblAanmelding$PrimaryKey");


            // Offer
            modelBuilder.Entity<Offer>()
                .ToTable("tblOfferte", schema: "dbo");

            modelBuilder.Entity<Offer>()
                .HasKey(e => e.Id)
                .HasName("tblOfferte$PrimaryKey");

            modelBuilder.Entity<Offer>()
                .HasQueryFilter(e => e.Currency == "EUR");

            modelBuilder.Entity<Offer>()
                .HasOne(e => e.Request)
                .WithOne(e => e.Offer)
                .HasForeignKey<Offer>(e => e.RequestId);

            modelBuilder.Entity<Offer>()
                .HasOne(e => e.Customer)
                .WithMany(e => e.Offers)
                .HasForeignKey(e => e.CustomerId)
                .HasPrincipalKey(e => e.Number);

            modelBuilder.Entity<Offer>()
                .HasOne(e => e.VatRate)
                .WithMany()
                .HasForeignKey(e => e.VatRateId);

            modelBuilder.Entity<Offer>()
                .HasOne(e => e.SubmissionType)
                .WithMany()
                .HasForeignKey(e => e.SubmissionTypeId);


            // VatRate

            modelBuilder.Entity<VatRate>()
                .ToTable("TblBtw", schema: "dbo");

            modelBuilder.Entity<VatRate>()
                .HasKey(e => e.Id)
                .HasName("TblBtw$PrimaryKey");


            // Submission type

            modelBuilder.Entity<SubmissionType>()
                .ToTable("TblVerzendType", schema: "dbo");

            modelBuilder.Entity<SubmissionType>()
                .HasKey(e => e.Id)
                .HasName("TblVerzendType$PrimaryKey");


            // Order

            modelBuilder.Entity<Order>()
                .ToTable("tblOfferte", schema: "dbo");

            modelBuilder.Entity<Order>()
                .HasKey(e => e.Id)
                .HasName("tblOfferte$PrimaryKey");

            modelBuilder.Entity<Order>()
                .HasQueryFilter(e => e.Currency == "EUR" && e.IsOrdered);

            modelBuilder.Entity<Order>()
                .HasOne(e => e.Customer)
                .WithMany(e => e.Orders)
                .HasForeignKey(e => e.CustomerId)
                .HasPrincipalKey(e => e.Number);

            modelBuilder.Entity<Order>()
                .HasOne(e => e.Offer)
                .WithOne(e => e.Order)
                .HasForeignKey<Order>(e => e.Id) // offer and order are stored in the same table on the same row
                .HasPrincipalKey<Offer>(e => e.Id);

            modelBuilder.Entity<Order>()
                .HasOne(e => e.VatRate)
                .WithMany()
                .HasForeignKey(e => e.VatRateId);

            modelBuilder.Entity<Order>()
                .HasMany(e => e.Deposits)
                .WithOne(e => e.Order)
                .HasForeignKey(e => e.OrderId);


            // Invoice

            modelBuilder.Entity<Invoice>()
                .ToTable("TblFactuur", schema: "dbo");

            modelBuilder.Entity<Invoice>()
                .HasKey(e => e.Id)
                .HasName("TblFactuur$PrimaryKey");

            modelBuilder.Entity<Invoice>()
                .HasQueryFilter(e => e.Currency == "EUR");

            modelBuilder.Entity<Invoice>()
                .HasOne(e => e.Customer)
                .WithMany(e => e.Invoices)
                .HasForeignKey(e => e.CustomerId)
                .HasPrincipalKey(e => e.Number);

            modelBuilder.Entity<Invoice>()
                .HasOne(e => e.Order)
                .WithOne(e => e.Invoice)
                .HasForeignKey<Invoice>(e => e.OrderId)
                .HasPrincipalKey<Order>(e => e.Id);

            modelBuilder.Entity<Invoice>()
                .HasOne(e => e.VatRate)
                .WithMany()
                .HasForeignKey(e => e.VatRateId);

            modelBuilder.Entity<Invoice>()
                .HasMany(e => e.Deposits)
                .WithOne(e => e.Invoice)
                .HasForeignKey(e => e.InvoiceId);


            // Invoice supplement
            modelBuilder.Entity<InvoiceSupplement>()
                .ToTable("TblFactuurExtra", schema: "dbo");

            modelBuilder.Entity<InvoiceSupplement>()
                .HasKey(e => e.Id)
                .HasName("TblFactuurExtra$PrimaryKey");

            // modelBuilder.Entity<InvoiceSupplement>()
            //     .HasQueryFilter(e => e.Currency == "EUR");

            modelBuilder.Entity<InvoiceSupplement>()
                .HasOne(e => e.Invoice)
                .WithMany(e => e.Supplements)
                .HasForeignKey(e => e.InvoiceId);


            // Deposit

            modelBuilder.Entity<Deposit>()
                .ToTable("TblVoorschot", schema: "dbo");

            modelBuilder.Entity<Deposit>()
                .HasKey(e => e.Id)
                .HasName("TblVoorschot$PrimaryKey");

            modelBuilder.Entity<Deposit>()
                .HasQueryFilter(e => e.Currency == "EUR" && e.IsDeposit);

            modelBuilder.Entity<Deposit>()
                .HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId)
                .HasPrincipalKey(e => e.Number);

            modelBuilder.Entity<Deposit>()
                .HasOne(e => e.Payment)
                .WithMany()
                .HasForeignKey(e => e.PaymentId);


            // Deposit invoices

            modelBuilder.Entity<DepositInvoiceHub>()
                .ToTable("TblVoorschotFactuur", schema: "dbo");

            modelBuilder.Entity<DepositInvoiceHub>()
                .HasKey(e => e.Id)
                .HasName("TblVoorschotFactuur$PrimaryKey");

            modelBuilder.Entity<DepositInvoiceHub>()
                .HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId)
                .HasPrincipalKey(e => e.Number);

            modelBuilder.Entity<DepositInvoiceHub>()
                .HasOne(e => e.Order)
                .WithMany(e => e.DepositInvoicesHubs)
                .HasForeignKey(e => e.OrderId)
                .HasPrincipalKey(e => e.Id);

            modelBuilder.Entity<DepositInvoiceHub>()
                .HasOne(e => e.Invoice)
                .WithMany(e => e.DepositInvoiceHubs)
                .HasForeignKey(e => e.InvoiceId)
                .HasPrincipalKey(e => e.Id);

            modelBuilder.Entity<DepositInvoiceHub>()
                .HasOne(e => e.DepositInvoice)
                .WithOne(e => e.MainInvoiceHub)
                .HasForeignKey<DepositInvoiceHub>(e => e.DepositInvoiceId)
                .HasPrincipalKey<Invoice>(e => e.Id);


            // Employee

            modelBuilder.Entity<Employee>()
                .ToTable("TblPersoneel", schema: "dbo");

            modelBuilder.Entity<Employee>()
                .HasKey(e => e.Id);


            // WorkingHour

            modelBuilder.Entity<WorkingHour>()
                .ToTable("tblWerkUren", schema: "dbo");

            modelBuilder.Entity<WorkingHour>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<WorkingHour>()
                .HasOne(e => e.Employee)
                .WithMany(e => e.WorkingHours)
                .HasForeignKey(e => e.EmployeeName)
                .HasPrincipalKey(e => e.FirstName);

            modelBuilder.Entity<WorkingHour>()
                .HasOne(e => e.Invoice)
                .WithMany(e => e.WorkingHours)
                .HasForeignKey(e => e.InvoiceId)
                .HasPrincipalKey(e => e.Id);


            // Payment

            modelBuilder.Entity<Payment>()
                .ToTable("TblParameters", schema: "dbo")
                .HasQueryFilter(e => e.Type == "BETALING");

            modelBuilder.Entity<Payment>()
                .HasKey(e => e.Id);
        }
    }
}