using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using Sembo.Shared.Models;

// Much easier to understand. It's like a #DEFINE in C++, but with all the .Net Power.
using APIResponse = System.Collections.Generic.List<Sembo.Models.HotelByCountry>;
using Microsoft.AspNetCore.Cors;

namespace Sembo.Controllers
{
    /// <summary>
    /// Default (and unique) controller that returns a <see cref="HotelStats"/> list
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class GetHotelStatsController : ControllerBase
    {
        /// <summary>
        /// Try count due to API failure
        /// </summary>
        const int NUMTRIES = 3;

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
        [EnableCors("NonRestrictivePolicy")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // Createa a GUId to identify start and end logs

            string GUID = Guid.NewGuid().ToString();

            _logger.LogInformation(GUID + ". Get");

            var result = new SynchronizedCollection<HotelStats>();

            // For all the countries, a task is created

            var tasks = GlobalData.countries.Keys.Select(async isoCode =>
            {
                // Get the data from API
                var hotelStat = await GetHotelByCountryAsync(GUID, isoCode);

                // Add it to the result
                result.Add(hotelStat);

            });

            // Wait to finish

            await Task.WhenAll(tasks);

            // Simple log
            foreach (var item in result)
                _logger.LogInformation(GUID + $". Get. Result: {item.Country} {item.AverageScore}");

            return Ok(result);
        }

        /// <summary>
        /// Main loop with retry check
        /// </summary>
        /// <param name="isoCountry"></param>
        /// <returns></returns>
        async Task<HotelStats> GetHotelByCountryAsync(string logGUID, string isoCountry)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    // Add API Key

                    httpClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);

                    string url = GetCountryURL(isoCountry);

                    // Try it some times

                    for (int i = 0; i < NUMTRIES; i++)
                    {
                        var result = await GetHotelByCountryInLoopAsync(logGUID, httpClient, isoCountry, url);

                        if (result != null)
                        {
                            // Store the loop value when we get data

                            result.NumTries = i;

                            return result;

                        }
                    }

                    // If we can not access data, return Service unavailable

                    return new HotelStats(GlobalData.ISO2Country(isoCountry), "Service unavailable", NUMTRIES);
                }
            }
            catch (Exception ex)
            {
                // If we have an exception, return Exception

                return new HotelStats(GlobalData.ISO2Country(isoCountry), "Exception: " + ex.Message, -1);
            }
        }

        /// <summary>
        /// Calls to Sembo's API to get the data needed
        /// </summary>
        /// <param name="logGUID"></param>
        /// <param name="httpClient"></param>
        /// <param name="isoCountry"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        async Task<HotelStats> GetHotelByCountryInLoopAsync(string logGUID, HttpClient httpClient, string isoCountry, string url)
        {
            _logger.LogInformation(logGUID + $". GetHotelByCountryInLoop. Country:{isoCountry}");

            using (var response = await httpClient.GetAsync(url))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();

                _logger.LogInformation(logGUID + $". GetHotelByCountryInLoop. Country:{isoCountry} Response:{response.StatusCode}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    // Process the data

                    var result = ComputeRequeriments(logGUID, isoCountry, apiResponse);

                    return result;
                }
                else
                if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                {
                    // Service unavailable. let's try again after a little while...
                    await Task.Delay(TimeSpan.FromMilliseconds(100));

                }
                else
                {
                    // Maybe a server connection problema...So wait more time
                    await Task.Delay(TimeSpan.FromMilliseconds(300));

                }
            }

            return null;
        }

        /// <summary>
        /// Fills the <see cref="HotelStats"/> instance
        /// </summary>
        /// <param name="isoCountry"></param>
        /// <param name="apiResponse"></param>
        /// <returns></returns>
        HotelStats ComputeRequeriments(string logGUID, string isoCountry, string apiResponse)
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

