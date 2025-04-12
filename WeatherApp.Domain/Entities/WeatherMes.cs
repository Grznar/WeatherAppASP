using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeatherApp.Domain.Entities
{
    public class WeatherMes
    {
        public int Id { get; set; }
        public string? CityOrPSC { get; set; }

        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }

        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        public double? Longitude { get; set; }
        public double? Latitude { get; set; }

        // Nové vlastnosti:
        public double Temperature { get; set; }   // Teplota ve stupních
        public string? LoadStatus { get; set; }     // Například "Zatížený" nebo "Normálně"

        public string? Weather { get; set; }        // Popis počasí
        public DateTime MeasurementTime { get; set; }
    }
}
