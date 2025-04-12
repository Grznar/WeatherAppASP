using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http;

namespace WeatherApp.Application.Services
{
    public class GeocodingResult
    {
        public string lat { get; set; }
        public string lon { get; set; }
        public string display_name { get; set; }
    }

    public class GeocodingService
    {
        private readonly HttpClient _httpClient;

        public GeocodingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GeocodingResult?> GetCoordinatesAsync(string locationName)
        {
            // Použijeme WebUtility.UrlEncode, které je standardní v .NET Core
            var query = WebUtility.UrlEncode(locationName);
            var url = $"https://nominatim.openstreetmap.org/search?q={query}&format=json&addressdetails=1&limit=1";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent", "WeatherApp/1.0 (AsKronak@seznam.cz)");

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var results = JsonSerializer.Deserialize<GeocodingResult[]>(jsonResponse, options);
            if (results != null && results.Length > 0)
                return results[0];

            return null;
        }
    }
}
