using Microsoft.AspNetCore.Mvc;
using TheCircleBackend.DBInfra.Repo;
using TheCircleBackend.Domain.DTO;
using TheCircleBackend.Domain.DTO.EncryptedPayload;
using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IHelpers;
using TheCircleBackend.DomainServices.IRepo;

namespace TheCircleBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VodController : ControllerBase
    {
        private readonly IVodRepo vodRepo;
        private readonly ISecurityService securityService;

        public VodController(IVodRepo vodRepo, ISecurityService securityService)
        {
            this.vodRepo = vodRepo;
            this.securityService = securityService;
        }

        [HttpGet]
        public IActionResult GetVods()
        {
            var vods = vodRepo.GetVods().ToList();
            return Ok(vods);
        }

        [HttpGet("{id}")]
        public IActionResult GetVodById(int id)
        {
            var vod = vodRepo.GetVodById(id);
            return Ok(vod);
        }

        [HttpPost]
        public IActionResult AddVod(VodDTO vod)
        {
            //Vod vod = new() {
            //    Title = vodDTO.Title, 
            //    ContentType = vodDTO.ContentType, 
            //    Data = Convert.FromBase64String(vodDTO.Data)
            //};
            

            var nodeKey = this.securityService.GetVideoServerPublicKey();
            bool holdsIntegrity = this.securityService.HoldsIntegrity(vod.OriginalData, Convert.FromBase64String(vod.Signature), nodeKey);
            Console.WriteLine("VIDEO STREAM INTEGRITY CHECK");
            Console.WriteLine(holdsIntegrity);

            if (holdsIntegrity)
            {

                var VideoExists = this.vodRepo.VideoExists(vod.OriginalData.Title);
                Console.WriteLine("VIDEO ALREADY EXISTS CHECK");
                Console.WriteLine(VideoExists);
                if (VideoExists)
                {
                    return BadRequest("Video already exists");
                }

                var persitenceVod = new Vod()
                {
                    ContentType = vod.OriginalData.ContentType,
                    Data = vod.OriginalData.Data,
                    Title = vod.OriginalData.Title
                };
                this.vodRepo.AddVod(persitenceVod);
                return Ok("added");
            }
            return BadRequest("Request does not hold integrity");

           
        }
    }
}