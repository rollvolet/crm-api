using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class VatRate
    {
        [Column("BtwId")]
        public int Id { get; set; }

        [Column("BTWCode")]
        public string Code { get; set; }

        [Column("BtwTarief")]
        public double Rate { get; set; }

        [Column("BTWTekst")]
        public string Name { get; set; }

        [Column("VolgOrde")]
        public short? Order { get; set; }    

        [Column("GeldigTot")]
        public DateTime ExpirationDate { get; set; }            
    }
  
}