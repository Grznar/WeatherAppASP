using System.Globalization;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherApp.Application.Common.Dto;
using WeatherApp.Application.Common.Interfaces;
using WeatherApp.Application.Services;
using WeatherApp.Domain.Entities;

[Authorize]
public class WeatherController : Controller
{
    private readonly GeocodingService _geoService;
    private readonly IWeatherService _weatherService;
    private readonly IUnitOfWork _unitOfWork;

    public WeatherController(GeocodingService geoService, IWeatherService weatherService, IUnitOfWork unitOfWork)
    {
        _geoService = geoService;
        _weatherService = weatherService;
        _unitOfWork = unitOfWork;
    }

    [HttpPost]
    public async Task<IActionResult> MeasureManual(double latitude, double longitude)
    {
        // Get current weather based on the provided coordinates
        WeatherData weatherData = await _weatherService.GetWeatherAsync(latitude, longitude);
        string loadStatus = weatherData.Temperature > 25 ? "High Temperature Warning" : "No Warning";

        var measurement = new WeatherMes
        {
            CityOrPSC = $"{latitude}, {longitude}",
            Latitude = latitude,
            Longitude = longitude,
            Temperature = weatherData.Temperature,
            LoadStatus = loadStatus,
            Weather = weatherData.Description,
            MeasurementTime = DateTime.Now,
            ApplicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        };

        _unitOfWork.WeatherMes.Add(measurement);
        _unitOfWork.Save();

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> MeasureByCity(string city)
    {
        // Get coordinates using the geocoding service
        var geoResult = await _geoService.GetCoordinatesAsync(city);
        if (geoResult == null)
        {
            ModelState.AddModelError("", "City/ZIP Code not found.");
            return RedirectToAction("Index");
        }

        double lat = double.Parse(geoResult.lat, CultureInfo.InvariantCulture);
        double lon = double.Parse(geoResult.lon, CultureInfo.InvariantCulture);

        // If either coordinate is 0, consider it an invalid result
        if (lat == 0 || lon == 0)
        {
            ModelState.AddModelError("", "Invalid location – coordinates returned as 0.");
            return RedirectToAction("Index");
        }

        // Get current weather data for the obtained coordinates
        WeatherData weatherData = await _weatherService.GetWeatherAsync(lat, lon);
        string loadStatus = weatherData.Temperature > 25 ? "High Temperature Warning" : "No Warning";

        var measurement = new WeatherMes
        {
            CityOrPSC = city,
            Latitude = lat,
            Longitude = lon,
            Temperature = weatherData.Temperature,
            LoadStatus = loadStatus,
            Weather = weatherData.Description,
            MeasurementTime = DateTime.Now,
            ApplicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        };

        _unitOfWork.WeatherMes.Add(measurement);
        _unitOfWork.Save();

        return RedirectToAction("Index");
    }

    // Action to create a new measurement for an existing location (triggered by the button on the chart)
    [HttpGet]
    public async Task<IActionResult> NewMeasurement(int id)
    {
        // Find the existing measurement by ID
        var existingMeasurement = _unitOfWork.WeatherMes.Get(u => u.Id == id);
        if (existingMeasurement == null)
        {
            return RedirectToAction("Index");
        }

        double? lat = existingMeasurement.Latitude;
        double? lon = existingMeasurement.Longitude;
        if (lat == null || lon == null)
        {
            return RedirectToAction("Index");
        }

        WeatherData weatherData = await _weatherService.GetWeatherAsync(lat.Value, lon.Value);
        string loadStatus = weatherData.Temperature > 25 ? "High Temperature Warning" : "No Warning";

        var newMeasurement = new WeatherMes
        {
            CityOrPSC = existingMeasurement.CityOrPSC,
            Latitude = lat,
            Longitude = lon,
            Temperature = weatherData.Temperature,
            LoadStatus = loadStatus,
            Weather = weatherData.Description,
            MeasurementTime = DateTime.Now,
            ApplicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        };

        _unitOfWork.WeatherMes.Add(newMeasurement);
        _unitOfWork.Save();

        return RedirectToAction("Index");
    }

    // Index action: Group measurements by location and prepare the view model.
    public IActionResult Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var measurements = _unitOfWork.WeatherMes.GetAll(w => w.ApplicationUserId == userId).ToList();

        var overview = new WeatherOverviewVM();
        var groups = measurements.GroupBy(m => m.CityOrPSC);
        foreach (var group in groups)
        {
            var ordered = group.OrderBy(m => m.MeasurementTime).ToList();
            var last = ordered.Last();

            var locationVM = new LocationMeasurementsVM
            {
                CityOrPSC = group.Key,
                Latitude = last.Latitude,
                Longitude = last.Longitude,
                LastMeasurement = new WeatherMesrVM
                {
                    Id = last.Id,
                    CityOrPSC = last.CityOrPSC,
                    Latitude = last.Latitude,
                    Longitude = last.Longitude,
                    Temperature = last.Temperature,
                    LoadStatus = last.LoadStatus,
                    Weather = last.Weather,
                    MeasurementTime = last.MeasurementTime
                },
                Measurements = ordered.Select(m => new WeatherMesrVM
                {
                    Id = m.Id,
                    CityOrPSC = m.CityOrPSC,
                    Latitude = m.Latitude,
                    Longitude = m.Longitude,
                    Temperature = m.Temperature,
                    LoadStatus = m.LoadStatus,
                    Weather = m.Weather,
                    MeasurementTime = m.MeasurementTime
                }).ToList()
            };
            overview.Locations.Add(locationVM);
        }

        return View(overview);
    }
}
