using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LoRaWeatherStation.DataModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LoRaWeatherStation.Service
{
    public class SensorRecorder : IHostedService, IDisposable
    {
        private readonly ILogger<SensorRecorder> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        private LoRaModem _modem;

        public SensorRecorder(ILogger<SensorRecorder> logger, IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
        }
        
        public Task StartAsync(CancellationToken cancelStartToken)
        {
            _logger.LogInformation("Starting sensor recorder service");

            var modemSerialPort = _configuration.GetValue<string>("LRRC_LORA_DEVICE");
            if (string.IsNullOrEmpty(modemSerialPort))
            {
                _logger.LogWarning("Sensor recording will be disabled, because serial port of LoRa modem was not provided. For sensor recording to work, please ensure that environment variable 'LRRC_LORA_DEVICE' is set");
                return Task.CompletedTask;
            }
            
            _modem = new LoRaModem(modemSerialPort);
            _modem.DataReceived += OnSensorDataReceived;
            _modem.Open();

            return Task.CompletedTask;
        }

        private void OnSensorDataReceived(object sender, LoRaDataReceivedEventArgs e)
        {
            var data = e.Data;
            _logger.LogDebug("Received LoRa packet ({length} bytes)", data.Length);
            if (data.Length != 15 || data[0] != 0x42 || data[14] != 0xFF)
            {
                _logger.LogWarning("Dropped LoRa packet because of invalid structure ({length} bytes)", data.Length);
                return;
            }

            using var serviceScope = _serviceScopeFactory.CreateScope();
            using var dbContext = serviceScope.ServiceProvider.GetRequiredService<WeatherStationContext>();
            
            var id = data[1];
            var sensor = dbContext.Find<Sensor>((long) id);
            if (sensor == null)
            {
                sensor = new Sensor()
                {
                    Id = id,
                    Name = $"Unnamed Sensor #{id}",
                    SupportedValues = SensorValues.Temperature | SensorValues.Humidity | SensorValues.Pressure,
                };
                dbContext.Add(sensor);
                _logger.LogInformation("Adding sensor entry {sensor}, because sensor data is from new sensor", sensor);
            }

            var temperature = BitConverter.ToSingle(data, 2);
            var humidity = BitConverter.ToSingle(data, 6);
            var pressure = BitConverter.ToSingle(data, 10);
            var now = DateTime.UtcNow;
            var record = new SensorRecord()
            {
                Sensor = sensor, 
                RecordTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, DateTimeKind.Utc), 
                Temperature = temperature, 
                Humidity = humidity,
                Pressure = pressure,
            };
            _logger.LogInformation("Received sensor data {record}", record);

            if (dbContext.Records.Where(r => r.Sensor == sensor).Any(r => r.RecordTime == record.RecordTime))
            {
                _logger.LogInformation("Dropping record, as one is already in database");
                return;
            }
            
            dbContext.Add(record);
            dbContext.SaveChanges();
            _logger.LogInformation("Added sensor data to database");
        }

        public Task StopAsync(CancellationToken cancelGracefulStopAndKillInsteadToken)
        {
            _logger.LogInformation("Stopping sensor recorder service");
            
            _modem?.Dispose();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _modem?.Dispose();
        }
    }
}