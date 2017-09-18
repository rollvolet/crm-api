using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class Language
    {
        [Column("TaalID")]
        public int Id { get; set; }

        [Column("Taalcode")]
        public string Code { get; set; }

        [Column("Taal")]
        public string Name { get; set; }
    }
  
}