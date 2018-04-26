using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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

        public string ComposedId 
        {
            get
            {
                return $"{Id}-{LanguageId}";
            }
        }

        public static string[] DecomposeId(string composedId)
        {
           return composedId.Split('-', StringSplitOptions.None);
        }

        public static int DecomposeEntityId(string composedId)
        {
            var idParts = DecomposeId(composedId);
            return int.Parse(idParts.FirstOrDefault());
        }

        public static int DecomposeLanguageId(string composedId)
        {
            var idParts = DecomposeId(composedId);
            return int.Parse(idParts.LastOrDefault());
        }
    }
}