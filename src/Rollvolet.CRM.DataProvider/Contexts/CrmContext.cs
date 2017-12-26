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
        public DbSet<WayOfEntry> WaysOfEntry { get; set; }

        public CrmContext(DbContextOptions<CrmContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Customer / contact / building

            modelBuilder.Entity<CustomerRecord>()
                .ToTable("tblData", schema: "dbo");

            modelBuilder.Entity<CustomerRecord>()
                .HasDiscriminator<string>("DataType")
                .HasValue<Customer>("KLA")
                .HasValue<Contact>("CON")
                .HasValue<Building>("GEB");

            modelBuilder.Entity<CustomerRecord>()
                .HasKey(b => b.DataId) // primary key
                .HasName("TblData$PrimaryKey"); // name of the primary key constraint

            modelBuilder.Entity<Customer>()
                .HasOne(e => e.HonorificPrefix)
                .WithOne()
                .HasForeignKey((Customer e) => new { e.HonorificPrefixId, e.LanguageId });

            modelBuilder.Entity<Contact>()
                .HasOne(e => e.Customer)
                .WithMany(e => e.Contacts)
                .HasForeignKey(e => e.CustomerId)
                .HasPrincipalKey(e => e.Number);

            modelBuilder.Entity<Building>()
                .HasOne(e => e.Customer)
                .WithMany(e => e.Buildings)
                .HasForeignKey(e => e.CustomerId)
                .HasPrincipalKey(e => e.Number);


            // Country

            modelBuilder.Entity<Country>()
                .ToTable("TblLand", schema: "dbo");

            modelBuilder.Entity<Country>()
                .HasKey(e => e.Id)
                .HasName("TblLand$PrimaryKey");


            // HonorificPrefix

            modelBuilder.Entity<HonorificPrefix>()
                .ToTable("TblAanspreekTitel", schema: "dbo");

            modelBuilder.Entity<HonorificPrefix>()
                .HasKey(e => new { e.Id, e.LanguageId })
                .HasName("TblAanspreekTitel$PrimaryKey");


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
                .HasPrincipalKey(e => e.DataId);

            modelBuilder.Entity<Request>()
                .HasOne(e => e.Building)
                .WithMany(e => e.Requests)
                .HasForeignKey(e => e.BuildingId)
                .HasPrincipalKey(e => e.Number);

            modelBuilder.Entity<Request>()
                .HasOne(e => e.Contact)
                .WithMany(e => e.Requests)
                .HasForeignKey(e => e.BuildingId)
                .HasPrincipalKey(e => e.Number);


            // Way of Entry

            modelBuilder.Entity<WayOfEntry>()
                .ToTable("TblAanmelding", schema: "dbo");
            
            modelBuilder.Entity<WayOfEntry>()
                .HasKey(e => e.Id)
                .HasName("TblAanmelding$PrimaryKey");
        }
    }
}