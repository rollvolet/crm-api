using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class HonorificPrefix
    {
        [Column("AanspreekID")]
        public int Id { get; set; }

        [Column("TaalId")]
        public int LanguageId { get; set; }

        [Column("Omschrijving")]
        public string Name { get; set; }
    }
}