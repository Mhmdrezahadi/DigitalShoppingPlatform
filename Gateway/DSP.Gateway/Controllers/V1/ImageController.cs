using DSP.Gateway.Data.DTO;
using DSP.Gateway.Sevices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DSP.Gateway.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ImageController : ControllerBase
    {

        #region Configurations
        private readonly ImageHttpService _imageHttpService;
        public ImageController(ImageHttpService imageHttpService)
        {
            _imageHttpService = imageHttpService;
        }
        #endregion
        [HttpGet("Admin/Full/{id}")]
        [HttpGet("App/Full/{id}")]
        [HttpGet("Web/Full/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetImage(string id)
        {
            var image = await _imageHttpService.GetImage(id);
            return File(image.Full, "image/jpeg");
        }

        [HttpGet("Admin/Thumb/{id}")]
        [HttpGet("App/Thumb/{id}")]
        [HttpGet("Web/Thumb/{id}")]
        public async Task<IActionResult> GetImageThumb(string id)
        {
            var image = await _imageHttpService.GetImage(id);
            return File(image.Thumb, "image/jpeg");
        }
    }
}