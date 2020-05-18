using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Web.Services.HttpClient
{
    /// <summary>
    ///   Типизированный HttpClient для получения коеф разницы 2х валют
    /// </summary>
    public class CurrencyExchangeService
    {
        private readonly System.Net.Http.HttpClient _httpClient;

        public CurrencyExchangeService(System.Net.Http.HttpClient httpClient)
        {
            _httpClient            = httpClient;
            httpClient.BaseAddress = new Uri("https://api.exchangeratesapi.io/");
        }

        /// <summary>
        ///   Получаем коеф для заданых валют
        /// </summary>
        /// <param name="base">Валюта из которой конвертируем</param>
        /// <param name="symbols">Валюта в которую конвертируем</param>
        /// <returns></returns>
        public async Task<decimal> FetchRate(string @base, string symbols)
        {
            if (symbols == "RUR")
            {
                symbols = "RUB";
            }
            
            if (@base == "RUR")
            {
                @base = "RUB";
            }
            
            var uri      = $@"/latest?base={@base}&symbols={symbols}";
            var response = await _httpClient.GetAsync(uri);

            var responseString = await response.Content.ReadAsStringAsync();

            var root = JObject.Parse(responseString);

            var rate = (decimal) root["rates"][$"{symbols}"];

            return rate;
        }
    }
}