using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class SubmissionType
    {
        [Column("Code")]
        public string Id { get; set; }
        
        [Column("Tekst")]
        public string Name { get; set; }

        [Column("Volgorde")]
        public short? Order { get; set; }    
    }
  
}