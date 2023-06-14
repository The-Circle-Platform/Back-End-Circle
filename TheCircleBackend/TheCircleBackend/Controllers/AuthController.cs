using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Identity.Client;
using PasswordGenerator;
using TheCircleBackend.Domain.AuthModels;
using TheCircleBackend.Domain.DTO;
using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices;
using TheCircleBackend.DomainServices.IRepo;
using TheCircleBackend.DomainServices.IHelpers;
using TheCircleBackend.Domain.DTO.EncryptedPayload;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly IWebsiteUserRepo _websiteUserRepo;
    private readonly ISecurityService securityService;

    public AuthController(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        IWebsiteUserRepo _websiteUserRepo,
        ISecurityService securityService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        this._websiteUserRepo = _websiteUserRepo;
        this.securityService = securityService;
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(LoginDTO dto, [FromBody] UserIncomingDTO userIncomingDTO)
    {
        // Gets keypair of Admin
        var KeyPair = securityService.GetKeys(userIncomingDTO.SenderUserId);

        bool HoldsIntegrity = securityService.HoldsIntegrity(userIncomingDTO.OriginalUserRequest, userIncomingDTO.Signature, KeyPair.pubKey);

        if (!HoldsIntegrity)
        {
            return BadRequest();
        }

        var user = await _userManager.FindByNameAsync(dto.UserName);
        if (user != null && await _userManager.CheckPasswordAsync(user, dto.Password))
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = GetToken(authClaims);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }
        return Unauthorized();
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register(RegisterDTO dto)
    {
        var userExists = await _userManager.FindByNameAsync(dto.Username);
        if (userExists != null)
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

        IdentityUser user = new()
        {
            Email = dto.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = dto.Username
        };
        var pwd = new Password();
        var password = pwd.Next();
        Console.WriteLine(password);
        var result = await _userManager.CreateAsync(user, password);
        Console.WriteLine(result);

        if (!await _roleManager.RoleExistsAsync(UserRoles.User))
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));


        if (!result.Succeeded)
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

        if (await _roleManager.RoleExistsAsync(UserRoles.User))
        {
            await _userManager.AddToRoleAsync(user, UserRoles.User);
        }

        WebsiteUser newUser = new WebsiteUser();
        newUser.UserName = dto.Username;
        newUser.IsOnline = false;

        _websiteUserRepo.Add(newUser);

        Mailer mailer = new Mailer(_configuration);

        string emailbody =
            $"Hello {dto.Username} An account has been created by a TheCircle admin using: \n email: {dto.Email} \n Username: {dto.Username} \n Your generated password is: {password}";
        mailer.SendMail(dto.Email, "The Circle Account Creation", emailbody, "The Circle Team");
        return Ok(new Response { Status = "Success", Message = "User created successfully!" });

        return Ok(new Response { Status = "Success", Message = "User created successfully!" });
    }

    [HttpPost]
    [Route("register-admin")]
    public async Task<IActionResult> RegisterAdmin(RegisterDTO dto)
    {
        var pwd = new Password();
        var password = pwd.Next();
        Console.WriteLine(password);
        var userExists = await _userManager.FindByNameAsync(dto.Username);
        if (userExists != null)
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

        IdentityUser user = new()
        {
            Email = dto.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = dto.Username
        };
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

        if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
        if (!await _roleManager.RoleExistsAsync(UserRoles.User))
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

        if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
        {
            await _userManager.AddToRoleAsync(user, UserRoles.Admin);
        }
        if (await _roleManager.RoleExistsAsync(UserRoles.User))
        {
            await _userManager.AddToRoleAsync(user, UserRoles.User);
        }

        Mailer mailer = new Mailer(_configuration);

        string emailbody =
            $"Hello {dto.Username} An account has been created by a TheCircle admin using: \n email: {dto.Email} \n Username: {dto.Username} \n Your generated password is: {password}";
        mailer.SendMail(dto.Email, "The Circle Account Creation", emailbody, "The Circle Team");
        return Ok(new Response { Status = "Success", Message = "User created successfully!" });
    }

    private JwtSecurityToken GetToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

        return token;
    }
}