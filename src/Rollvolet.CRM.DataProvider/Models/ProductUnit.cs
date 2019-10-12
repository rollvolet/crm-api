using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class ProductUnit
    {
        [Column("Id")]
        public int Id { get; set; }

        [Column("Code")]
        public string Code { get; set; }

        [Column("NameNed")]
        public string NameNed { get; set; }

        [Column("NameFra")]
        public string NameFra { get; set; }
    }

}