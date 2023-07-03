using Microsoft.AspNetCore.Mvc;
using TheCircleBackend.DBInfra.Repo;
using TheCircleBackend.Domain.DTO;
using TheCircleBackend.Domain.DTO.EncryptedPayload;
using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IRepo;

namespace TheCircleBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VodController : ControllerBase
    {
        private readonly IVodRepo vodRepo;

        public VodController(IVodRepo vodRepo)
        {
            this.vodRepo = vodRepo;
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
        public IActionResult AddVod(Vod vod)
        {
            //Vod vod = new() {
            //    Title = vodDTO.Title, 
            //    ContentType = vodDTO.ContentType, 
            //    Data = Convert.FromBase64String(vodDTO.Data)
            //};
            this.vodRepo.AddVod(vod);
            return Ok("added");
        }
    }
}