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

using APIResponse = System.Collections.Generic.List<Sembo.Models.HotelByCountry>;

namespace Sembo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GetHotelStatsController : ControllerBase
    {
        /// <summary>
        /// ISO Code and country
        /// </summary>
        readonly Dictionary<string, string> countries = new Dictionary<string, string>() { { "es", "Spain" },{ "fr", "France" } , { "it", "Italy" } };

        /// <summary>
        /// Ky name for the API in appsettings
        /// </summary>
        const string APIKEYNAME = "APIKey";

        /// <summary>
        /// URL to get the data. IMPORTANT DO NOT USE BEFORE CHANGE {xx} TO ISO COUNTRY
        /// </summary>
        const string SEMBOURL = "https://developers.sembo.com/sembo/hotels-test/countries/{xx}/hotels";

        /// <summary>
        /// Configuration service
        /// </summary>
        private readonly IConfiguration _config;

        /// <summary>
        /// Log service
        /// </summary>
        private readonly ILogger<GetHotelStatsController> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="logger"></param>
        public GetHotelStatsController(IConfiguration config,ILogger<GetHotelStatsController> logger)
        {
            _config = config;

            _logger = logger;

            var apiKey = _config.GetValue<string>(APIKEYNAME);
        }

        /// <summary>
        /// Get the final URL changing the {xx} internal code to the countryISOCode
        /// </summary>
        /// <param name="countryISOCode">A two digit country ISO Code</param>
        /// <returns></returns>
        string GetCountryURL(string countryISOCode)
        {
            return SEMBOURL.Replace("{xx}",countryISOCode);
        }

        /// <summary>
        /// Main and UNIQUE Get Entry Point
        /// </summary>
        /// <returns>A List of <see cref="HotelStats"/></returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = new List<HotelStats>();

            using (var httpClient = new HttpClient())
            {
                string isoCountry = "es";

                string url = GetCountryURL(isoCountry);

                using (var response = await httpClient.GetAsync(url))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
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

                        result.Add(new HotelStats()
                        {
                            Country = countries[isoCountry],

                            AverageScore = hotelByCountryResult.Average(a => a.score),  

                            TopHotels = string.Join(',', top3HotelByScore.Select(a=>a.Name))

                        });
                    }
                    else
                    {
                        // Service unavailable
                        return BadRequest(apiResponse);
                    }

                }
            }

            return Ok(result);
        }
    }
}
