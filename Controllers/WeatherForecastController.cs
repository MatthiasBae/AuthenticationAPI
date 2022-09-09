using AuthenticateUserApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security;

namespace AuthenticateUserApi.Controllers {
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase {

        private readonly IWeatherDatabase WeatherDatabase;

        public WeatherForecastController(IWeatherDatabase weatherDatabase) {
            this.WeatherDatabase = weatherDatabase;
        }

        [HttpGet]
        public IActionResult Get() {
            var items = this.WeatherDatabase.GetForecasts();
            if(items == null) {
                return NotFound();
            }

            return Ok(items);
        }

        [HttpPost]
        public IActionResult Post(string date, int temperature) {
            var dateItem = DateTime.Parse(date);
            this.WeatherDatabase.AddForecast(dateItem,temperature);
            return Ok();
        }
    }
}