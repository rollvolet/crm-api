using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class TelephoneType
    {
        [Column("TelTypeId")]
        public int Id { get; set; }

        [Column("TypeTel")]
        public string Name { get; set; }
    }

}