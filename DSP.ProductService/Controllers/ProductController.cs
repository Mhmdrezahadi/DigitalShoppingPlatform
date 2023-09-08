
using DSP.ProductService.Data;
using DSP.ProductService.Services;
using DSP.ProductService.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DSP.ProductService.Controllers
{
    [ApiController]
    [Route("DSP/ProductService/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IManageService _manageService;
        public ProductController(IProductService productService, IManageService manageService)
        {
            _productService = productService;
            _manageService = manageService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("Products")]
        public async Task<ActionResult<PagedList<ProductToReturnDTO>>> SearchInProducts([FromQuery] PaginationParams<ProductSearch> pagination)
        {
            PagedList<ProductToReturnDTO> result = await _productService.SearchInProducts(pagination);

            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("ProductsForAdmin")]
        public async Task<ActionResult<PagedList<ProductToReturnDTO>>> SearchInProductsForAdmin([FromQuery] PaginationParams<ProductSearch> pagination)
        {
            PagedList<ProductToReturnDTO> result = await _productService.SearchInProductsForAdmin(pagination);

            return Ok(result);
        }

        [HttpGet("ProductsForAdmin/{id}")]
        public ActionResult<ProductToReturnDTO> GetProductsForAdmin(Guid id)
        {
            ProductToReturnDTO dto = _productService.GetProductsForAdmin(id);

            return Ok(dto);
        }
        [HttpGet("Products/{id}")]
        public async Task<ActionResult<ProductToReturnDTO>> GetProducts(Guid id)
        {
            ProductToReturnDTO dto = await _productService.GetProducts(id);

            return Ok(dto);
        }
 
        [HttpGet("ProductKeysValues/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductKeysValuesToReturnDTO>> GetProductKeysAndValues(Guid id)
        {
            ProductKeysValuesToReturnDTO dto = await _productService.GetProductKeysAndValues(id);

            return Ok(dto);
        }

        [HttpGet("CompareTwoProduct")]
        public async Task<ActionResult<List<ProductToReturnDTO>>> CompareTwoProduct(List<Guid> productIds)
        {
            List<ProductToReturnDTO> ls = await _productService.CompareTwoProduct(productIds);

            return Ok(ls);
        }

        [HttpGet("ProductPriceLog")]
        public async Task<ActionResult<List<PriceLogDTO>>> ProductPriceLog(Guid productId)
        {
            List<PriceLogDTO> ls = await _productService.ProductPriceLog(productId);

            return Ok(ls);
        }

        [HttpGet("ProductPriceLogOfWeek")]
        public async Task<ActionResult<List<PriceLogDTO>>> ProductPriceLogOfWeek(Guid productId)
        {
            DateTime fromDT = DateTime.Now.AddDays(-7);
            DateTime toDT = DateTime.Now;

            List<PriceLogDTO> ls = await _productService.ProductPriceLogInDateRange(productId, fromDT, toDT);

            return Ok(ls);
        }

        [HttpGet("ProductPriceLogOfMonth")]
        public async Task<ActionResult<List<PriceLogDTO>>> ProductPriceLogOfMonth(Guid productId)
        {
            DateTime fromDT = DateTime.Now.AddDays(-30);
            DateTime toDT = DateTime.Now;

            List<PriceLogDTO> ls = await _productService.ProductPriceLogInDateRange(productId, fromDT, toDT);

            return Ok(ls);
        }

        [HttpGet("PropertyKeys")]
        public async Task<ActionResult<List<PropertyKeyDTO>>> GetPropertyKeys(int catId)
        {
            List<PropertyKeyDTO> ls = await _manageService.GetPropertyKeys(catId);

            return Ok(ls);
        }

        [HttpPost("Product")]
        public async Task<ActionResult<Guid>> AddProduct([FromBody] ProductForCreateDTO dto)
        {
            Guid id = await _productService.AddProduct(dto);
            return Ok(id);
        }

        [HttpPost("Image")]
        public async Task<ActionResult<CreatedImageToReturnDTO>> AddImage(IFormFile Image)
        {
            CreatedImageToReturnDTO dto = await _productService.AddImage(Image);

            return Ok(dto);
        }

        [HttpDelete("Image/{id}")]
        public async Task<ActionResult<bool>> RemoveImage(Guid id)
        {
            bool res = await _productService.RemoveImage(id);

            return Ok(res);
        }

        [HttpPut("Product/{productId}")]
        public async Task<ActionResult<bool>> EditProduct(Guid productId, [FromBody] ProductForCreateDTO dto)
        {
            bool res = await _productService.EditProduct(productId, dto);

            return Ok(res);
        }

        [HttpDelete("Product/{productId}")]
        public async Task<ActionResult<bool>> RemoveProduct(Guid productId)
        {
            bool res = await _productService.RemoveProduct(productId);

            return Ok(res);
        }

        [HttpPatch("Product/{productId}")]
        public async Task<ActionResult<bool>> SetProductStatus(Guid productId, Status status)
        {
            bool res = await _productService.SetProductStatus(productId, status);

            return Ok(res);
        }
    }
}
