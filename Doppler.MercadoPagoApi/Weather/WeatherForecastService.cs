using System;
using System.Collections.Generic;
using System.Linq;

namespace Doppler.HelloMicroservice.Weather
{
    public class WeatherForecastService : IWeatherForecastService
    {
        private readonly DataService _dataService;
        public WeatherForecastService(DataService dataService)
        {
            _dataService = dataService;
        }

        public IEnumerable<WeatherForecast> GetForecasts()
        {
            var Summaries = _dataService.GetData();
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });
        }
    }
}
