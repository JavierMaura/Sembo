using System;

namespace Sembo.Models
{
    /// <summary>
    /// Responde From Sembo's API
    /// </summary>
    public class HotelByCountry
    {
        /// <summary>
        /// A GUID
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// Hotel name
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Hotel ISO CounTRY 2 digits 
        /// </summary>
        public string isoCountryId { get; set; }
        /// <summary>
        /// Hotel score
        /// </summary>
        public decimal score { get; set; }
    }
}
// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 

