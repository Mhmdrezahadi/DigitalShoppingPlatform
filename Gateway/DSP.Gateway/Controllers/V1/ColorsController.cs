
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DSP.Gateway.Sevices;
using System;
using System.Collections.Generic;
using DSP.Gateway.Data;

namespace DSP.Gateway.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,SuperAdmin")]
    public class ColorsController : ControllerBase
    {
        private readonly ColorHttpService _colorHttpService;
        public ColorsController(ColorHttpService colorHttpService)
        {
            _colorHttpService = colorHttpService;
        }
        /// <summary>
        /// افزودن رنگ
        /// </summary>
        /// <remarks>وارد نشود id برای افزودن </remarks>
        /// <param name="color"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("Admin/Colors")]
        public ActionResult<bool> AddColor(ProductColorDTO color)
        {
            bool res = _colorHttpService.AddColor(color);

            return Ok(res);
        }

        /// <summary>
        /// حذف رنگ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpDelete("Admin/Colors/{id}")]
        public ActionResult<bool> RemoveColor(Guid id)
        {
            bool res = _colorHttpService.RemoveColor(id);

            return Ok(res);
        }

        /// <summary>
        /// ویرایش رنگ
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("Admin/Colors")]
        public ActionResult<bool> EditColor(ProductColorDTO color)
        {
            bool res = _colorHttpService.EditColor(color);

            return Ok(res);
        }

        /// <summary>
        /// لیست رنگ ها
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("Admin/Colors")]
        public ActionResult<List<ProductColorDTO>> ColorsList()
        {
            List<ProductColorDTO> dto = _colorHttpService.GetColorsList();

            return Ok(dto);
        }
    }
}
