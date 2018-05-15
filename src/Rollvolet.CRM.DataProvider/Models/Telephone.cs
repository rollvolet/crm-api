using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class Telephone
    {

        [Column("DataId")]
        public int CustomerRecordId { get; set; }

        [Column("TelTypeId")]
        public int TelephoneTypeId { get; set; }

        [Column("LandId")]
        public int CountryId { get; set; }

        [Column("Zonenr")]
        public string Area { get; set; }

        [Column("Telnr")]
        public string Number { get; set; }

        [Column("TelMemo")]
        public string Memo { get; set; }

        [Column("Volgorde")]
        public short? Order { get; set; }


        // Include resources
        public CustomerRecord CustomerRecord { get; set; }
        public Country Country { get; set; }
        public TelephoneType TelephoneType { get; set; }


        public string ComposedId
        {
            get
            {
                return $"{CustomerRecordId}-{TelephoneTypeId}-{CountryId}-{Area}-{Number}";
            }
        }

        public static string[] DecomposeId(string composedId)
        {
           return composedId.Split('-', StringSplitOptions.None);
        }

        public static int DecomposeCustomerId(string composedId)
        {
            var idParts = DecomposeId(composedId);
            return int.Parse(idParts[0]);
        }

        public static int DecomposeTelephoneTypeId(string composedId)
        {
            var idParts = DecomposeId(composedId);
            return int.Parse(idParts[1]);
        }

        public static int DecomposeCountryId(string composedId)
        {
            var idParts = DecomposeId(composedId);
            return int.Parse(idParts[2]);
        }

        public static string DecomposeArea(string composedId)
        {
            var idParts = DecomposeId(composedId);
            return idParts[3];
        }

        public static string DecomposeNumber(string composedId)
        {
            var idParts = DecomposeId(composedId);
            return idParts[4];
        }

        public static string SerializeNumber(string number)
        {
            if (number != null)
            {
                if (number.Length == 6)
                    return $"{number.Substring(0, 2)}.{number.Substring(2, 2)}.{number.Substring(4, 2)}";
                else
                    return $"{number.Substring(0, 3)}.{number.Substring(3, 2)}.{number.Substring(5, 2)}";
            }
            else
            {
                return null;
            }
        }
    }

}