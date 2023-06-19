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
using TheCircleBackend.Controllers;
using TheCircleBackend.Helper;
using TheCircleBackend.DomainServices.IHelpers;
using TheCircleBackend.Domain.DTO.EncryptedPayload;
using System.Security.Cryptography;
using System.Xml.Serialization;

namespace Controllers.AuthController
{



    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IWebsiteUserRepo _websiteUserRepo;
        private readonly ISecurityService securityService;
        private readonly LogHelper logHelper;

        public AuthController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            IWebsiteUserRepo _websiteUserRepo,
            ILogItemRepo logItemRepo,
            ILogger<AuthController> logger, ISecurityService securityService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            this._websiteUserRepo = _websiteUserRepo;
            this.logHelper = new LogHelper(logItemRepo, logger);
            this.securityService = securityService;
        }


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {

            var decryptedPassword = await this.securityService.DecryptMessage(dto.Password);
            Console.WriteLine(decryptedPassword);

                var user = await _userManager.FindByNameAsync(dto.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, decryptedPassword))
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




                // Retrieves user from database.
                var WebsiteUser = _websiteUserRepo.GetByUserName(user.UserName);
                var KeyPair = securityService.GetKeys(WebsiteUser.Id);
                var token = GetToken(authClaims, WebsiteUser.Id);

                 var userDTO = new WebsiteUserDTO()
                {
                    Id = WebsiteUser.Id,
                    UserName = WebsiteUser.UserName,
                    IsOnline = WebsiteUser.IsOnline,
                    FollowCount = WebsiteUser.FollowCount,
                    Balance = WebsiteUser.Balance,
                };


                //Create payload
                var PayLoad = new
                {
                    WebsiteUser = userDTO,
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    PrivateKey = KeyPair.privKey,
                    PublicKey = KeyPair.pubKey,
                };

                //Console.WriteLine(KeyPair.privKey);
                Console.WriteLine(KeyPair.pubKey);
                    //Signs signature
                    var ServerKeys = securityService.GetServerKeys();
                var Signature = securityService.SignData(PayLoad, ServerKeys.privKey);

                AuthOutRegisterDTO authOut = new()
                {
                    Signature = Signature,
                    SenderUserId = WebsiteUser.Id,
                    OriginalLoad = PayLoad
                };

                return Ok(authOut);
            }

            return Unauthorized();
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(AuthRegisterDTO request)
        {

            // Decrypts with admin keys
            var keyPair = securityService.GetKeys(request.SenderUserId);

            var HoldsIntegrity =
                securityService.HoldsIntegrity(request.OriginalRegisterData, request.Signature, keyPair.pubKey);

            var userExists = await _userManager.FindByNameAsync(request.OriginalRegisterData.Username);

            if (userExists != null || !HoldsIntegrity)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "User already exists!" });

            var dto = request.OriginalRegisterData;
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
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response
                    {
                        Status = "Error", Message = "User creation failed! Please check user details and try again."
                    });

            if (await _roleManager.RoleExistsAsync(UserRoles.User))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.User);
            }

            WebsiteUser newUser = new WebsiteUser();
            newUser.UserName = dto.Username;
            newUser.IsOnline = false;

            _websiteUserRepo.Add(newUser);

            // Creates new keypair and stores the keys in database
            var KeyPair = securityService.GenerateKeys();

            var Succeeded = securityService.StoreKeys(newUser.Id, KeyPair.privKey, KeyPair.pubKey);
            if (!Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response
                    {
                        Status = "Error", Message = "User creation failed! Please check user details and try again."
                    });
            }

            // Creates a mail to the user.
            Mailer mailer = new Mailer(_configuration);

            string emailbody =
                $"Hello {dto.Username} An account has been created by a TheCircle admin using: \n email: {dto.Email} \n Username: {dto.Username} \n Your generated password is: {password}";
            mailer.SendMail(dto.Email, "The Circle Account Creation", emailbody, "The Circle Team");

            // Encrypts data
            var Response = new Response { Status = "Success", Message = "User created successfully!" };

            //Creates signature
            var Signature = securityService.SignData(Response, keyPair.privKey);

            AuthOutRegisterDTO authOut = new AuthOutRegisterDTO()
                { 
                OriginalLoad = Response, 
                Signature = Signature,
                };
            return Ok(authOut);
        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin(AuthRegisterDTO request)
        {

            var keyPair = securityService.GetKeys(request.SenderUserId);

            var HoldsIntegrity =
                securityService.HoldsIntegrity(request.OriginalRegisterData, request.Signature, keyPair.pubKey);
            if (!HoldsIntegrity)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable,
                    new Response { Status = "Error", Message = "Data has been tampered" });
            }
            var dto = request.OriginalRegisterData;
            var pwd = new Password();
            var password = pwd.Next();
            Console.WriteLine(password);
            var userExists = await _userManager.FindByNameAsync(dto.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "User already exists!" });

            IdentityUser user = new()
            {
                Email = dto.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = dto.Username
            };
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response
                    {
                        Status = "Error", Message = "User creation failed! Please check user details and try again."
                    });

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
            var KeyPair = securityService.GenerateKeys();

            WebsiteUser newUser = new WebsiteUser();
            newUser.UserName = dto.Username;
            newUser.IsOnline = false;

            _websiteUserRepo.Add(newUser);
            var Succeeded = securityService.StoreKeys(newUser.Id, KeyPair.privKey, KeyPair.pubKey);

            if (!Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response
                    {
                        Status = "Error",
                        Message = "User creation failed! Please check user details and try again."
                    });
            }


            Mailer mailer = new Mailer(_configuration);

            string emailbody =
                $"Hello {dto.Username} An account has been created by a TheCircle admin using: \n email: {dto.Email} \n Username: {dto.Username} \n Your generated password is: {password}";
            mailer.SendMail(dto.Email, "The Circle Account Creation", emailbody, "The Circle Team");
            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims, int Id)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            token.Payload["id"] = Id;

            return token;
        }
    }
}
 