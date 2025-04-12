using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WeatherApp.Application.Services
{
    public class RecaptchaService
    {
        private readonly string _secretKey;
        private readonly IHttpClientFactory _httpClientFactory;

        public RecaptchaService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _secretKey = configuration["Recaptcha:SecretKey"];
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> VerifyResponseAsync(string token)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.PostAsync($"https://www.google.com/recaptcha/api/siteverify?secret={_secretKey}&response={token}", null);
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var captchaResult = JsonSerializer.Deserialize<RecaptchaResponse>(jsonResponse, options);
            return captchaResult != null && captchaResult.Success;
        }
    }

    public class RecaptchaResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("challenge_ts")]
        public string Challenge_ts { get; set; }

        [JsonPropertyName("hostname")]
        public string Hostname { get; set; }

        [JsonPropertyName("error-codes")]
        public string[] ErrorCodes { get; set; }
    }
}