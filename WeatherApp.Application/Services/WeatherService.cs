using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace WeatherApp.Application.Services
{
    public interface IWeatherService
    {
        Task<WeatherData> GetWeatherAsync(double lat, double lon);
    }

    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public WeatherService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<WeatherData> GetWeatherAsync(double lat, double lon)
        {
            string apiKey = _configuration["OpenWeatherMap:ApiKey"];
            // units=metric – teploty ve stupních Celsia; lang=cz – čeština
            string url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat.ToString(CultureInfo.InvariantCulture)}&lon={lon.ToString(CultureInfo.InvariantCulture)}&appid={apiKey}&units=metric&lang=en";

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<JsonElement>(json);

                double temperature = data.GetProperty("main").GetProperty("temp").GetDouble();
                string description = data.GetProperty("weather")[0].GetProperty("description").GetString() ?? "Unknown";

                return new WeatherData
                {
                    Temperature = temperature,
                    Description = description
                };
            }
            return new WeatherData
            {
                Temperature = 0,
                Description = "Can't read weather"
            };
        }
    }
}
