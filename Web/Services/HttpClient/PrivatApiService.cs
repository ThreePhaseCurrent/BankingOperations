using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Web.Models;

namespace Web.Services.HttpClient
{
    public class PrivatApiService
    {
        private readonly System.Net.Http.HttpClient _httpClient;

        public PrivatApiService(System.Net.Http.HttpClient httpClient)
        {
            _httpClient             = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.privatbank.ua/");
        }

        public async Task<PrivatApiResponse[]> LoadInfo()
        {
            const string uri = "/p24api/pubinfo?json&exchange&coursid=5";

            var response          = await _httpClient.GetAsync(uri);
            var responseString    = await response.Content.ReadAsStringAsync();
            var deserializeObject = JsonConvert.DeserializeObject<PrivatApiResponse[]>(responseString);
            return deserializeObject;
        }
    }
}