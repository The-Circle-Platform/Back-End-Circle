//JWT authentication was implemented based on this tutorial: https://www.c-sharpcorner.com/article/jwt-authentication-and-authorization-in-net-6-0-with-identity-framework/

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheCircleBackend.Domain.AuthModels;


namespace TheCircleBackend.Controllers
{
    [ApiController]
    [Authorize(Roles = UserRoles.Admin)]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
       
        public WeatherForecastController()
        {
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public string Get()
        {
            return "Hello";
        }
    }
}