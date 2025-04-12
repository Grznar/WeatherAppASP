using System;
using System.Collections.Generic;

namespace WeatherApp.Application.Common.Dto
{
    public class LocationMeasurementsVM
    {
        public string CityOrPSC { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        // Poslední (nejnovější) měření – pro rychlý přehled
        public WeatherMesrVM LastMeasurement { get; set; } = new WeatherMesrVM();
        // Všechna měření k dané lokaci (chronologicky seřazená vzestupně)
        public List<WeatherMesrVM> Measurements { get; set; } = new List<WeatherMesrVM>();
    }
}
