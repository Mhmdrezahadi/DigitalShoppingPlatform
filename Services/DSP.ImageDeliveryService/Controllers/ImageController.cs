using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSP.ImageDeliveryService.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DSP.ImageDeliveryService.Controllers
{
    [Route("DSP/ImageDeliveryService/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly ImageServiceDbContext _db;

        public ImageController(ImageServiceDbContext db)
        {
            _db = db;
        }

        [Route("full/{id}")]
        public async Task<ActionResult<Image>> GetImage(string id)
        {
            var image=await _db.Images.FindAsync(Guid.Parse(id));
            return Ok(image);
        }

        [Route("thumb/{id}")]
        public async Task<ActionResult<Image>> GetImageThumb(string id)
        {
            var image = await _db.Images.FindAsync(Guid.Parse(id));
            return Ok(image);
        }
    }
}
