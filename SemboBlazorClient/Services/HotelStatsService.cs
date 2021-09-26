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
    public class HotelStatsService
    {
        [Inject]
        private HttpClient Http { get; set; }

        public HotelStatsService(HttpClient client)
        {
            Http = client;
        }

        public async Task<APIResponse> GetHotelStats()
        {
            var data = await Http.GetFromJsonAsync<APIResponse>("https://localhost:44392/GetHotelStats");

            return data;
        }
    }
}
