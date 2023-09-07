using DSP.Gateway.Data;
using DSP.Gateway.Sevices;
using DSP.Gateway.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
namespace Tellbal.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ShopController : ControllerBase
    {
        private readonly ProductHttpService _productHttpService;
        private readonly ManageHttpService _manageHttpService;

        public ShopController(ProductHttpService productHttpService, ManageHttpService manageHttpService)
        {
            _productHttpService = productHttpService;
            _manageHttpService = manageHttpService;
        }

        /// <summary>
        /// لیست محصولات ، سرچ ، فیلتر
        /// </summary>
        /// <remarks>
        /// بدون متن جستجو تمامی کالاها را شامل می شود
        /// </remarks>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("App/Products")]
        [HttpGet("Web/Products")]
        [AllowAnonymous]
        public async Task<ActionResult<PagedList<ProductToReturnDTO>>> SearchInProducts([FromQuery] PaginationParams<ProductSearch> pagination)
        {
            return null;
        }

        /// <summary>
        /// لیست محصولات ، سرچ ، فیلتر
        /// </summary>
        /// <remarks>
        /// بدون متن جستجو تمامی کالاها را شامل می شود
        /// </remarks>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("Admin/Products")]
        public async Task<ActionResult<PagedList<ProductToReturnDTO>>> SearchInProductsForAdmin([FromQuery] PaginationParams<ProductSearch> pagination)
        {
            PagedList<ProductToReturnDTO> result = await _productHttpService.SearchInProductsForAdmin(pagination);

            return Ok(result);
        }

        /// <summary>
        /// دریافت جزئیات یک محصول
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Admin/Products/{id}")]
        public ActionResult<ProductToReturnDTO> GetProductsForAdmin(Guid id)
        {
            ProductToReturnDTO dto = _productHttpService.GetProductsForAdmin(id);

            return Ok(dto);
        }

        /// <summary>
        /// دریافت جزئیات یک محصول
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("App/Products/{id}")]
        [HttpGet("Web/Products/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductToReturnDTO>> GetProducts(Guid id)
        {
            ProductToReturnDTO dto = await _productHttpService.GetProducts(id);

            return Ok(dto);
        }
        /// <summary>
        /// دریافت کلید و مقدار های یک محصول
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("App/ProductKeysValues/{id}")]
        [HttpGet("Web/ProductKeysValues/{id}")]
        [HttpGet("Admin/ProductKeysValues/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductKeysValuesToReturnDTO>> GetProductKeysAndValues(Guid id)
        {
            ProductKeysValuesToReturnDTO dto = await _productHttpService.GetProductKeysAndValues(id);

            return Ok(dto);
        }

        /// <summary>
        /// مقایسه بین دو کالا
        /// </summary>
        /// <param name="productIds"></param>
        /// <returns></returns>
        [HttpGet("App/CompareTwoProduct")]
        [HttpGet("Web/CompareTwoProduct")]
        public async Task<ActionResult<List<ProductToReturnDTO>>> CompareTwoProduct(List<Guid> productIds)
        {
            List<ProductToReturnDTO> ls = await _productHttpService.CompareTwoProduct(productIds);

            return Ok(ls);
        }

        /// <summary>
        /// نمودار قیمت
        /// </summary>
        /// <remarks>
        /// for used products serve from the new one of this cat key val pair
        /// </remarks>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpGet("App/ProductPriceLog")]
        [HttpGet("Web/ProductPriceLog")]
        public async Task<ActionResult<List<PriceLogDTO>>> ProductPriceLog(Guid productId)
        {
            List<PriceLogDTO> ls = await _productHttpService.ProductPriceLog(productId);

            return Ok(ls);
        }

        /// <summary>
        /// نمودار هفتگی قیمت
        /// </summary>
        /// <remarks>
        /// for used products serve from the new one of this cat key val pair
        /// </remarks>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpGet("App/ProductPriceLogOfWeek")]
        [HttpGet("Web/ProductPriceLogOfWeek")]
        public async Task<ActionResult<List<PriceLogDTO>>> ProductPriceLogOfWeek(Guid productId)
        {
            DateTime fromDT = DateTime.Now.AddDays(-7);
            DateTime toDT = DateTime.Now;

            List<PriceLogDTO> ls = await _productHttpService.ProductPriceLogInDateRange(productId, fromDT, toDT);

            return Ok(ls);
        }

        /// <summary>
        /// نمودار ماهانه قیمت
        /// </summary>
        /// <remarks>
        /// for used products serve from the new one of this cat key val pair
        /// </remarks>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpGet("App/ProductPriceLogOfMonth")]
        [HttpGet("Web/ProductPriceLogOfMonth")]
        public async Task<ActionResult<List<PriceLogDTO>>> ProductPriceLogOfMonth(Guid productId)
        {
            DateTime fromDT = DateTime.Now.AddDays(-30);
            DateTime toDT = DateTime.Now;

            List<PriceLogDTO> ls = await _productHttpService.ProductPriceLogInDateRange(productId, fromDT, toDT);

            return Ok(ls);
        }

        /// <summary>
        /// لیست ویژگی های ساختار کالا
        /// </summary>
        /// <param name="catId"></param>
        /// <returns></returns>
        [HttpGet("Admin/PropertyKeys")]
        [HttpGet("App/PropertyKeys")]
        [HttpGet("Web/PropertyKeys")]
        public async Task<ActionResult<List<PropertyKeyDTO>>> GetPropertyKeys(int catId)
        {
            return null;
        }

        /// <summary>
        /// ثبت آگهی کالا
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("Admin/Product")]
        public async Task<ActionResult<Guid>> AddProduct([FromBody] ProductForCreateDTO dto)
        {
            return null;
        }

        /// <summary>
        /// ثبت یک عکس برای محصول
        /// </summary>
        /// <remarks>
        /// ابتدا عکس در سرور ذخیره و سپس با استفاده از آیدی در افزودن محصول استفاده شود
        /// </remarks>
        /// <returns></returns>
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("Admin/Image")]
        public async Task<ActionResult<CreatedImageToReturnDTO>> AddImage(IFormFile Image)
        {
            return null ;
        }

        /// <summary>
        /// حذف یک عکس از محصول
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpDelete("Admin/Image/{id}")]
        public async Task<ActionResult<bool>> RemoveImage(Guid id)
        {
            return true;
        }

        /// <summary>
        /// ویرایش کالا
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("Admin/Product/{productId}")]
        public async Task<ActionResult<bool>> EditProduct(Guid productId, [FromBody] ProductForCreateDTO dto)
        {
            return true;
        }
        /// <summary>
        /// حذف محصول
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpDelete("Admin/Product/{productId}")]
        public async Task<ActionResult<bool>> RemoveProduct(Guid productId)
        {
            return true;
        }
        /// <summary>
        /// تغییر استاتوس یک محصول
        /// </summary>
        /// <remarks>
        /// تغییر استاتوس به موجود ، ناموجود ، مخفی
        /// </remarks>
        /// <param name="productId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPatch("Admin/Product/{productId}")]
        public async Task<ActionResult<bool>> SetProductStatus(Guid productId, Status status)
        {
            return true;
        }

    }
}
