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

            if ((DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 1000000000 > dto.Request.TimeStamp))
            {
                return BadRequest("Request timeout");
            }


            var user = await _userManager.FindByNameAsync(dto.Request.UserName);
            if (user != null)
            {
                var webUser = _websiteUserRepo.GetByUserName(dto.Request.UserName);
                var keys = securityService.GetKeys(webUser.Id);
                Console.WriteLine(keys.privKey);
                var isSameUser = securityService.HoldsIntegrity(dto.Request, Convert.FromBase64String(dto.Signature),
                    keys.pubKey);
                if (!isSameUser)
                {
                    return Unauthorized();
                }
                var serverKeys = securityService.GetServerKeys();
                var userDTO = new WebsiteUserDTORequest()
                {
                    Id = webUser.Id,
                    UserName = webUser.UserName,
                    IsOnline = webUser.IsOnline,
                };
                var PayLoad = new
                {
                    WebsiteUser = userDTO,
                    IsVerified = isSameUser
                };
                var serverSignature = securityService.SignData(PayLoad, serverKeys.privKey);
                AuthOutRegisterDTO authOut = new()
                {
                    Signature = serverSignature,
                    SenderUserId = webUser.Id,
                    OriginalLoad = PayLoad
                };

                Console.WriteLine(isSameUser);
                return Ok(authOut);
            }

            return NotFound();

            
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(AuthRegisterDTO request)
        {
            if ((DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 10000 > request.OriginalRegisterData.TimeStamp))
            {
                return BadRequest("Request timeout");
            }
            var keyPair = securityService.GetServerKeys();
            var claimedAdmin = await _userManager.FindByNameAsync(request.OriginalRegisterData.UsernameOfAdmin);
            var userRoles = await _userManager.GetRolesAsync(claimedAdmin);
            if (!userRoles.Contains("Admin"))
            {
                return Forbid();
            }

            var adminWebUser = _websiteUserRepo.GetByUserName(request.OriginalRegisterData.UsernameOfAdmin);
            var adminKeys = securityService.GetKeys(adminWebUser.Id);
            Console.WriteLine(Convert.ToBase64String(request.Signature));
            var isAdmin =
                securityService.HoldsIntegrity(request.OriginalRegisterData, request.Signature, adminKeys.pubKey);

            if (!isAdmin)
            {
                return Forbid();
            }

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
            string fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".txt";
            StreamWriter sw = new StreamWriter(fileName);
            sw.WriteLine(KeyPair.privKey);
            sw.Close();
            System.Net.Mail.Attachment privateKey = new System.Net.Mail.Attachment(fileName);
            privateKey.Name = "PrivateKey.txt";  // set name here
            string fileName2 = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".txt";
            StreamWriter sw2 = new StreamWriter(fileName2);
            sw2.WriteLine(KeyPair.pubKey);
            sw2.Close();
            System.Net.Mail.Attachment publicKey = new System.Net.Mail.Attachment(fileName2);
            publicKey.Name = "PublicKey.txt";
            List<System.Net.Mail.Attachment> attachments = new List<System.Net.Mail.Attachment>();
            attachments.Add(privateKey);
            attachments.Add(publicKey);
            string emailbody =
                $" Hello {dto.Username} An account has been created by a TheCircle admin using: \n email: {dto.Email} \n Username: {dto.Username} \n In the attachments of this email you will find your private and public keys which you can use to authenticate yourself";
            mailer.SendMail(dto.Email, "The Circle Account Creation", emailbody, "The Circle Team", attachments);

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
            //if ((DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 10000 > request.OriginalRegisterData.TimeStamp))
            //{
            //    return BadRequest("Request timeout");
            //}
            //var claimedAdmin = await _userManager.FindByNameAsync(request.OriginalRegisterData.UsernameOfAdmin);
            //var userRoles = await _userManager.GetRolesAsync(claimedAdmin);
            //if (!userRoles.Contains("Admin"))
            //{
            //    return Forbid();
            //}

            //var adminWebUser = _websiteUserRepo.GetByUserName(request.OriginalRegisterData.UsernameOfAdmin);
            //var adminKeys = securityService.GetKeys(adminWebUser.Id);
            //Console.WriteLine(Convert.ToBase64String(request.Signature));
            //var isAdmin =
            //    securityService.HoldsIntegrity(request.OriginalRegisterData, request.Signature, adminKeys.pubKey);

            //if (!isAdmin)
            //{
            //    return Forbid();
            //}

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
            string fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".txt";
            StreamWriter sw = new StreamWriter(fileName);
            sw.WriteLine(KeyPair.privKey);
            sw.Close();
            System.Net.Mail.Attachment privateKey = new System.Net.Mail.Attachment(fileName);
            privateKey.Name = "PrivateKey.txt";  // set name here
            string fileName2 = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".txt";
            StreamWriter sw2 = new StreamWriter(fileName2);
            sw2.WriteLine(KeyPair.pubKey);
            sw2.Close();
            System.Net.Mail.Attachment publicKey = new System.Net.Mail.Attachment(fileName2);
            publicKey.Name = "PublicKey.txt";
            List<System.Net.Mail.Attachment> attachments = new List<System.Net.Mail.Attachment>();
            attachments.Add(privateKey);
            attachments.Add(publicKey);
            string emailbody =
                $" Hello {dto.Username} An account has been created by a TheCircle admin using: \n email: {dto.Email} \n Username: {dto.Username} \n In the attachments of this email you will find your private and public keys which you can use to authenticate yourself";
            mailer.SendMail(dto.Email, "The Circle Account Creation", emailbody, "The Circle Team", attachments);
            var Response = new Response { Status = "Success", Message = "User created successfully!" };

            //Creates signature
            var keyPair = securityService.GetServerKeys();
            var Signature = securityService.SignData(Response, keyPair.privKey);

            AuthOutRegisterDTO authOut = new AuthOutRegisterDTO()
            {
                OriginalLoad = Response,
                Signature = Signature,
            };
            return Ok(authOut);
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
 