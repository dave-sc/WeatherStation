using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LoRaWeatherStation.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatusController : ControllerBase
    {
        private readonly ILogger<StatusController> _logger;

        public StatusController(ILogger<StatusController> logger)
        {
            _logger = logger;
        }
        
        [HttpGet]
        public string Get()
        {
            return "healthy";
        }
    }
}