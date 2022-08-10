using Microsoft.AspNetCore.Mvc;

namespace LoRaWeatherStation.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatusController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "healthy";
        }
    }
}