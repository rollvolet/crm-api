using Microsoft.EntityFrameworkCore;
using Rollvolet.CRM.DataProvider.Models;

namespace Rollvolet.CRM.DataProvider.Contexts
{
    public class CrmContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Building> Buildings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerEntity>()
                .HasDiscriminator<string>("DataType")
                .HasValue<Customer>("KLA")
                .HasValue<Contact>("CON")
                .HasValue<Building>("GEB");

            modelBuilder.Entity<CustomerEntity>()
                .HasKey(b => b.DataId)
                .HasName("DataID");

            modelBuilder.Entity<CustomerEntity>()
                .ToTable("tblData", schema: "dbo");
        }

        public CrmContext(DbContextOptions<CrmContext> options) : base(options)
        {
        }
    }
}