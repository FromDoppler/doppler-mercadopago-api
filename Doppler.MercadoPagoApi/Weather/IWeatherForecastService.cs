using System.Collections.Generic;

namespace Doppler.HelloMicroservice.Weather
{
    public interface IWeatherForecastService
    {
        IEnumerable<WeatherForecast> GetForecasts();
    }
}
