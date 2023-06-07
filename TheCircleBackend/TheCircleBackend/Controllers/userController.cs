using Microsoft.AspNetCore.Mvc;
using System.Net;
using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IRepo;

namespace TheCircleBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class userController : ControllerBase
    {

        private readonly IWebsiteUserRepo websiteUserRepo;

        public userController(IWebsiteUserRepo websiteUserRepo)
        {
            this.websiteUserRepo = websiteUserRepo;
        }

        [HttpGet]
        public IEnumerable<WebsiteUser> Get()
        {

            return websiteUserRepo.GetAllWebsiteUsers();

        }

        [HttpGet("{id}")]
        public IActionResult get(int id)
        {
            Console.Write("fuck this shit");
            Console.WriteLine(id);
            var user = websiteUserRepo.GetById(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);

        }

        [HttpPost]
        public IActionResult post(WebsiteUser user)
        {
            Console.WriteLine(user);
            try
            {
                this.websiteUserRepo.Add(user);
                return Ok("user added");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }

        [HttpPut("{id}")]
        public IActionResult put(WebsiteUser user, int id)
        {
            Console.WriteLine(id);
            try
            {
                websiteUserRepo.Update(user, id);
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }
    }
}
