using System;

namespace WeatherApp.Application.Common.Dto
{
    public class WeatherMesrVM
    {
        public int Id { get; set; }
        public string? CityOrPSC { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }

        public double Temperature { get; set; }
        public string? LoadStatus { get; set; }

        public string? Weather { get; set; }
        public DateTime MeasurementTime { get; set; }
    }
}
