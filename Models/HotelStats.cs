using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sembo.Models
{
    /// <summary>
    /// Api Response for my client
    /// </summary>
    public class HotelStats
    {

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
        public DateTime TimeStamp { get; set; } = DateTime.Now;
    }
}
