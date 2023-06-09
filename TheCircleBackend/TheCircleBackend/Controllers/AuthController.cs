using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TheCircleBackend.Domain.AuthModels;
using TheCircleBackend.Domain.DTO;

namespace TheCircleBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AuthUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<AuthUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._configuration = configuration;
        }

        //[HttpPost]
        //[Route("login")]
        //public async Task<IActionResult> Login(LoginDTO dto)
        //{
        //    var user = await _userManager.FindByNameAsync(dto.UserName);

        //}
    }
}
