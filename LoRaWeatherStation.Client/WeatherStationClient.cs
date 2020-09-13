using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using LoRaWeatherStation.DataModel;
using Microsoft.AspNetCore.WebUtilities;

namespace LoRaWeatherStation.Client
{
    public class WeatherStationClient
    {
        private readonly HttpClient _httpClient;

        public WeatherStationClient(string baseUrl)
        {
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(baseUrl)
            };
        }
        
        public async Task<IReadOnlyList<Sensor>> GetSensors()
        {
            return await _httpClient.GetFromJsonAsync<Sensor[]>($"/sensors");            
        }
        
        public async Task<IReadOnlyList<Sensor>> GetSensorById(long id)
        {
            return await _httpClient.GetFromJsonAsync<Sensor[]>($"/sensors/{id}");            
        }
        
        public async Task<SensorRecord> GetCurrentSensorValues(Sensor sensor)
        {
            if (sensor == null)
                throw new ArgumentNullException(nameof(sensor));
            
            return await _httpClient.GetFromJsonAsync<SensorRecord>($"/sensors/{sensor.Id}/current");         
        }
        
        public async Task<SensorRecord> GetSensorValues(Sensor sensor, DateTime? minDate = null, DateTime? maxDate = null)
        {
            if (sensor == null)
                throw new ArgumentNullException(nameof(sensor));
            
            return await _httpClient.GetFromJsonAsync<SensorRecord>(QueryHelpers.AddQueryString($"/sensors/{sensor.Id}/values?", new Dictionary<string, string>
            {
                {"minDate", minDate?.ToString("O") ?? ""},
                {"maxDate", maxDate?.ToString("O") ?? ""}
            }));         
        }
        
        public async Task<IReadOnlyList<ForecastLocation>> GetLocations()
        {
            return await _httpClient.GetFromJsonAsync<ForecastLocation[]>($"/locations");            
        }
        
        public async Task<ForecastLocation> GetLocationById(long id)
        {
            return await _httpClient.GetFromJsonAsync<ForecastLocation>($"/locations/{id}");            
        }
        
        public async Task<ForecastData> GetCurrentWeather(ForecastLocation location)
        {
            if (location == null)
                throw new ArgumentNullException(nameof(location));
            
            return await _httpClient.GetFromJsonAsync<ForecastData>($"/locations/{location.Id}/weather");         
        }
        
        public async Task<ForecastData[]> GetWeatherForecast(ForecastLocation location, DateTime? day = null)
        {
            if (location == null)
                throw new ArgumentNullException(nameof(location));
            
            return await _httpClient.GetFromJsonAsync<ForecastData[]>(QueryHelpers.AddQueryString($"/locations/{location.Id}/forecast", new Dictionary<string, string>
            {
                {"day", day?.ToString("O") ?? ""},
            }));         
        }
    }
}