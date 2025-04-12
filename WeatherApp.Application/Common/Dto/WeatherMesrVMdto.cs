using System.Collections.Generic;

namespace WeatherApp.Application.Common.Dto
{
    public class WeatherMesrVMdto
    {
        public List<WeatherMesrVM> Measurements { get; set; } = new List<WeatherMesrVM>();

        public string? City { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
    }
}
