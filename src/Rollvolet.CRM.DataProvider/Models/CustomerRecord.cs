using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using Rollvolet.CRM.DataProvider.Extensions;

namespace Rollvolet.CRM.DataProvider.Models
{
    public abstract class CustomerRecord
    {
        [Column("DataID")]
        public int DataId { get; set; } // the public ID for Contact / Building

        [Column("ID")]
        public int Number { get; set; } // the public ID for Customer ; the relative number for Contact / Building

        [Column("ParentID")]
        public int CustomerId { get; set; } // the ID of the Customer for Contact / Building

        [Column("AanspreekID")]
        public int? HonorificPrefixId { get; set; }

        [Column("Naam")]
        public string Name { get; set; }

        [Column("Adres1")]
        public string Address1 { get; set; }

        [Column("Adres2")]
        public string Address2 { get; set; }

        [Column("Adres3")]
        public string Address3 { get; set; }

        [Column("PostcodeID")]
        public int? PostalCodeId { get; set; }

        [Column("TaalID")]
        public int? LanguageId { get; set; }

        [Column("LandId")]
        public int? CountryId { get; set; }

        [Column("Prefix")] // used for firstName in Contact / Building
        public string Prefix { get; set; }

        [Column("Suffix")]
        public string Suffix { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("email2")]
        public string Email2 { get; set; }

        [Column("URL")]
        public string Url { get; set; }

        [Column("PrintPrefix")]
        public bool PrintPrefix { get; set; }

        [Column("PrintSuffix")]
        public bool PrintSuffix { get; set; }

        [Column("PrintVoor")]
        public bool PrintInFront { get; set; }

        [Column("Opmerking")]
        public string Comment { get; set; }

        [Column("RegistratieDatum")]
        public DateTime Created { get; set; }


        // Included resources
        public HonorificPrefix HonorificPrefix { get; set; }
        public Country Country { get; set; }
        public Language Language { get; set; }
        public IEnumerable<Telephone> Telephones { get; set; }


        // Embedded properties
        [Column("Postcode")]
        public string EmbeddedPostalCode { get; set; }

        [Column("Gemeente")]
        public string EmbeddedCity { get; set; }

        [Column("ZoekNaam")]
        public string SearchName { get; set; }


        public static string CalculateSearchName(string name)
        {
            if (name != null)
            {
                var searchName = name.ToUpper();
                searchName = Regex.Replace(searchName, @"\s+", "");
                searchName = searchName.FilterDiacritics();
                return searchName;
            }

            return null;

        }
    }
}