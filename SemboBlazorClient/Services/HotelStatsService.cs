using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Sembo.Shared.Models;

using APIResponse = System.Collections.Generic.List<Sembo.Shared.Models.HotelStats>;

namespace SemboBlazorClient.Services
{
    /// <summary>
    /// Services used for the app. 
    /// </summary>
    public class HotelStatsService
    {
        /// <summary>
        /// Web API base url
        /// </summary>
        const string APIBASEURL = "https://localhost:44364/";

        /// <summary>
        /// Injected from Blazor
        /// </summary>
        [Inject]
        private HttpClient Http { get; set; }

        public HotelStatsService(HttpClient client)
        {
            Http = client;
        }

        /// <summary>
        /// Get HotelStats from API
        /// </summary>
        /// <returns></returns>
        public async Task<APIResponse> GetHotelStats()
        {
            var data = await Http.GetFromJsonAsync<APIResponse>(APIBASEURL+"GetHotelStats");

            return data;
        }
    }
}
