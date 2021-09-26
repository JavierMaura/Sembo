using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sembo.Shared.Models
{
    /// <summary>
    /// Api Response for my client
    /// Previously i made the class not allow modifications in the fields. To do this, it forces the necessary values ​​to be indicated in the constructor ( private set)
    /// But as i moved to a shared class library, and as it's used for deserealization, i have to remove this indications.
    /// </summary>
    public class HotelStats
    {
        /// <summary>
        /// Constructor used for deserealization
        /// </summary>
        public HotelStats()
        {

        }
        /// <summary>
        /// Default constructor
        /// </summary>
        public HotelStats(string country,decimal score,string top)
        {
            Country = country;
            AverageScore = score;
            TopHotels = top;

            TimeStamp = DateTime.Now;
        }

        /// <summary>
        /// Constructor for an ErrorMessage
        /// </summary>
        /// <param name="errorMessage"></param>
        public HotelStats(string country, string errorMessage,int numTries)
        {
            Country = country;

            this.ErrorMessage = errorMessage;

            TimeStamp = DateTime.Now;

            NumTries = numTries;
        }

        /// <summary>
        /// Simple boolean to make API consume easier & cleaner
        /// </summary>
        public bool ServiceError => ErrorMessage != null;

        /// <summary>
        /// Country name
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Average score for this copuntry
        /// </summary>
        public decimal AverageScore { get; set; }

        /// <summary>
        /// Top hotel list sortered by score
        /// </summary>
        public string TopHotels { get; set; }

        /// <summary>
        /// a TimeStamp to check if parallelism is working fine
        /// </summary>
        public DateTime TimeStamp { get;  set; } 

        /// <summary>
        /// Error message recived from the API
        /// </summary>
        public string ErrorMessage { get; set; }

        public int NumTries { get; set; }
    }
}
