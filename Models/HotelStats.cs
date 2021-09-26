using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sembo.Models
{
    /// <summary>
    /// Api Response for my client
    /// I make the class not allow modifications in the fields. To do this, it forces the necessary values ​​to be indicated in the constructor ( private set)
    /// 
    /// </summary>
    public class HotelStats
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public HotelStats(string country,decimal score,string top)
        {
            Country = country;
            AverageScore = score;
            TopHotels = top;
        }

        /// <summary>
        /// Constructor for an ErrorMessage
        /// </summary>
        /// <param name="errorMessage"></param>
        public HotelStats(string errorMessage)
        {
            this.ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Simple boolean to make API consume easier & cleaner
        /// </summary>
        public bool ServiceError => ErrorMessage != null;

        /// <summary>
        /// Country name
        /// </summary>
        public string Country { get; private set; }

        /// <summary>
        /// Average score for this copuntry
        /// </summary>
        public decimal AverageScore { get; private set; }

        /// <summary>
        /// Top hotel list sortered by score
        /// </summary>
        public string TopHotels { get; private set; }

        /// <summary>
        /// a TimeStamp to check if parallelism is working fine
        /// </summary>
        public DateTime TimeStamp { get; private set; } = DateTime.Now;

        public string ErrorMessage { get; private set; }
    }
}
