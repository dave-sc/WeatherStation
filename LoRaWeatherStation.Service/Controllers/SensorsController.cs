using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoRaWeatherStation.DataModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LoRaWeatherStation.Service.Controllers
{
    [ApiController]
    [Route("sensors")]
    public class SensorsController : ControllerBase
    {
        private readonly ILogger<SensorsController> _logger;
        private readonly WeatherStationContext _dbContext;

        public SensorsController(ILogger<SensorsController> logger, WeatherStationContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sensors = await _dbContext.Sensors.ToArrayAsync();
            return Ok(sensors);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] long id)
        {
            var (success, sensor) = await TryGetSensorById(id);
            if (!success)
                return NotFound();

            return Ok(sensor);
        }
        
        [HttpPost]
        public async Task<IActionResult> Post(IEnumerable<Sensor> sensors)
        {
            var newSensors = sensors as IReadOnlyList<Sensor> ?? sensors.ToArray();
            var oldIds = _dbContext.Sensors.Select(s => s.Id).ToArray();
            var newIds = newSensors.Select(s => s.Id).ToArray();
            
            _dbContext.RemoveRange(await _dbContext.Sensors.Where(existingSensor => !newIds.Contains(existingSensor.Id)).ToArrayAsync());
            _dbContext.UpdateRange(newSensors.Where(newSensor => oldIds.Contains(newSensor.Id)));
            _dbContext.AddRange(newSensors.Where(newSensor => !oldIds.Contains(newSensor.Id)));
            await _dbContext.SaveChangesAsync();

            return Ok(await _dbContext.Sensors.ToListAsync());
        }
        
        [HttpGet("{id}/current")]
        public async Task<IActionResult> GetCurrent([FromRoute] long id)
        {
            var (success, sensor) = await TryGetSensorById(id);
            if (!success)
                return NotFound();
            
            var lastRecord = await _dbContext.Records.Where(r => r.Sensor == sensor).OrderByDescending(r => r.RecordTime).FirstOrDefaultAsync();
            if (lastRecord == null || lastRecord.RecordTime < DateTime.UtcNow.AddMinutes(-15))
                return Ok(new SensorRecord() {Sensor = sensor, RecordTime = DateTime.Now});

            return Ok(lastRecord);
        }
        
        [HttpGet("{id}/values")]
        public async Task<IActionResult> GetValues([FromRoute] long id, [FromQuery] DateTime? minDate = null, [FromQuery] DateTime? maxDate = null)
        {
            if (minDate == null)
                minDate = DateTime.MinValue;
            if (maxDate == null)
                maxDate = DateTime.MaxValue;

            var (success, sensor) = await TryGetSensorById(id);
            if (!success)
                return NotFound();
            
            var sensorRecords = await _dbContext.Records.Where(r => r.Sensor == sensor).Where(r => r.RecordTime >= minDate).Where(r => r.RecordTime <= maxDate).ToArrayAsync();
            return Ok(sensorRecords);
        }

        private async Task<(bool, Sensor)> TryGetSensorById(long id)
        {
            try
            {
                return (true, await _dbContext.Sensors.SingleAsync(sensor => sensor.Id == id));
            }
            catch (InvalidOperationException)
            {
                return (false, null);
            }
        }
    }
}