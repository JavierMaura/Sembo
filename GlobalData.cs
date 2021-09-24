using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sembo
{
    /// <summary>
    /// Global data, const and vars should be here
    /// </summary>
    public static class GlobalData
    {
        /// <summary>
        /// ISO Code and country
        /// </summary>
        public static readonly Dictionary<string, string> countries = new Dictionary<string, string>() { 
            { "es", "Spain" }, { "fr", "France" }, { "it", "Italy" } 
        };

        /// <summary>
        /// Ky name for the API in appsettings
        /// </summary>
        public const string APIKEYNAME = "APIKey";

        /// <summary>
        /// URL to get the data. IMPORTANT DO NOT USE BEFORE CHANGE {xx} TO ISO COUNTRY
        /// </summary>
        public const string SEMBOURL = "https://developers.sembo.com/sembo/hotels-test/countries/{xx}/hotels";

        /// <summary>
        /// Returns the country name given a ISO
        /// </summary>
        /// <param name="iso"></param>
        /// <returns></returns>
        public static string ISO2Country(string iso)
        {
            return countries[iso];
        }
    }
}
