using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class Memo
    {
        [Column("DataID")]
        public int CustomerId { get; set; }

        [Column("Memo")]
        public string Text { get; set; }
    }
  
}