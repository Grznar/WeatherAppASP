using System.Collections.Generic;

namespace WeatherApp.Application.Common.Dto
{
    public class WeatherOverviewVM
    {
        public List<LocationMeasurementsVM> Locations { get; set; } = new List<LocationMeasurementsVM>();
    }
}
