using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using Sembo.Models;

// Much easier to understand. It's like a #DEFINE in C++, but with all the .Net Power.
using APIResponse = System.Collections.Generic.List<Sembo.Models.HotelByCountry>;

namespace Sembo.Controllers
{
    /// <summary>
    /// Default (and unique) controller that returns a <see cref="HotelStats"/> list
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class GetHotelStatsController : ControllerBase
    {
        #region Fields

        /// <summary>
        /// Configuration service
        /// </summary>
        private readonly IConfiguration _config;

        /// <summary>
        /// Log service
        /// </summary>
        private readonly ILogger<GetHotelStatsController> _logger;

        /// <summary>
        /// SHA1 APi Key
        /// </summary>
        private readonly string apiKey;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="logger"></param>
        public GetHotelStatsController(IConfiguration config, ILogger<GetHotelStatsController> logger)
        {
            _config = config;

            _logger = logger;

            apiKey = _config.GetValue<string>(GlobalData.APIKEYNAME);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the final URL adding the countryISOCode
        /// </summary>
        /// <param name="countryISOCode">A two digit country ISO Code</param>
        /// <returns></returns>
        string GetCountryURL(string countryISOCode)
        {
            return GlobalData.SEMBOURLBASE + countryISOCode + GlobalData.SEMBOURLGETDATA;
        }

        /// <summary>
        /// Main and UNIQUE Get Entry Point
        /// </summary>
        /// <returns>A List of <see cref="HotelStats"/></returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = new List<HotelStats>();

            // For all the countries, a task is created

            var tasks = GlobalData.countries.Keys.Select(async isoCode =>
            {
                // Get the data from API
                var hotelStat = await GetHotelByCountry(isoCode);

                // Add it to the result
                result.Add(hotelStat);
            });

            // Wait to finish

            await Task.WhenAll(tasks);

            return Ok(result);
        }

        /// <summary>
        /// Calls to Sembo's API to get the data needed
        /// </summary>
        /// <param name="isoCountry"></param>
        /// <returns></returns>
        async Task<HotelStats> GetHotelByCountry(string isoCountry)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    // Add API Key

                    httpClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);

                    string url = GetCountryURL(isoCountry);

                    using (var response = await httpClient.GetAsync(url))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            // Process the data

                            var result = ComputeRequeriments(isoCountry, apiResponse);

                            return result;
                        }
                        else
                        {
                            // Service unavailable. Returns an empty object

                            return new HotelStats("Service unavailable");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new HotelStats("Exception: " + ex.Message);
            }
        }

        /// <summary>
        /// Fills the <see cref="HotelStats"/> instance
        /// </summary>
        /// <param name="isoCountry"></param>
        /// <param name="apiResponse"></param>
        /// <returns></returns>
        HotelStats ComputeRequeriments(string isoCountry, string apiResponse)
        {
            // Convert to a List

            var hotelByCountryResult = JsonSerializer.Deserialize<APIResponse>(apiResponse);

            // Sort hotels by score and take only 3
            // BUT as hotels can be repeated, i should take the DISTINCT

            // First i get the MAXIMUN score for each hotel

            var hotelsByMaximumScore = from s in hotelByCountryResult
                                       group s by s.name into g
                                       select new { Name = g.Key, MaxScore = g.Max(s => s.score) };

            // Then, order the result

            var top3HotelByScore = hotelsByMaximumScore.OrderByDescending(a => a.MaxScore).Take(3);

            // Add a new item to the list

            var result = new HotelStats(

                country: GlobalData.ISO2Country(isoCountry),

                score: hotelByCountryResult.Average(a => a.score),

                top: string.Join(',', top3HotelByScore.Select(a => a.Name))    // Joins string, separated by a ,

                );

            

            return result;
        }
    }

    #endregion
}

