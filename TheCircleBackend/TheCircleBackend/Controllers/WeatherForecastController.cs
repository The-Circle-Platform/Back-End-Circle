//JWT authentication was implemented based on this tutorial: https://www.c-sharpcorner.com/article/jwt-authentication-and-authorization-in-net-6-0-with-identity-framework/

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheCircleBackend.Domain.AuthModels;
using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IRepo;
using Microsoft.Extensions.Logging;
using TheCircleBackend.Helper;
using System.Security.Claims;
using TheCircleBackend.DomainServices.IHelpers;
using TheCircleBackend.Domain.DTO.EncryptedPayload;

namespace TheCircleBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
       

        [HttpGet(Name = "GetWeatherForecast")]
        public string Get()
        {
            return "Hello";
        }
    }
}