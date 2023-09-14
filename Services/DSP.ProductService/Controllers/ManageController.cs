using DSP.ProductService.Data;
using DSP.ProductService.Services;
using DSP.ProductService.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DSP.ProductService.Controllers
{
    [ApiController]
    [Route("DSP/ProductService/Manage")]
    public class ManageController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IManageService _manageService;
        public ManageController(IProductService productService, IManageService manageService)
        {
            _productService = productService;
            _manageService = manageService;
        }

        [HttpGet("PropertyKeys/{catId}")]
        public async Task<ActionResult<List<PropertyKeyDTO>>> GetPropertyKeys(Guid catId)
        {
            List<PropertyKeyDTO> ls = await _manageService.GetPropertyKeys(catId);

            return Ok(ls);
        }

        [HttpPost("DefinePropertyKeys")]
        public async Task<ActionResult<bool>> DefinePropertyKeys([FromBody] ProductKeysDefinitionsDTO dto)
        {

            bool res = await _manageService.DefinePropertyKes(dto);

            return Ok(res);
        }

        [HttpPost("AddToPropertyKeys")]
        public async Task<ActionResult<bool>> AddToPropertyKeys([FromBody] ProductKeysDefinitionsDTO dto)
        {
            var userName = User.GetUserName();
            var userId = User.GetUserId();
            var roles = User.GetRoles();

            bool res = await _manageService.AddToPropertyKeys(dto);

            return Ok(res);
        }

        [HttpPost("EditPropertyKeys")]
        public async Task<ActionResult<bool>> EditPropertyKeys([FromBody] List<PropertyKeyDTO> list)
        {
            var userName = User.GetUserName();
            var userId = User.GetUserId();
            var roles = User.GetRoles();

            bool res = await _manageService.EditPropertyKeys(list);

            return Ok(res);
        }

        [HttpDelete("PropertyKeys/{id}")]
        public async Task<ActionResult<bool>> RemovePropertyKeys(Guid id)
        {
            bool res = await _manageService.RemovePropertyKeys(id);

            return Ok(res);
        }

        [HttpPost("DefineFastPricingKey")]
        public async Task<ActionResult<bool>> DefineFastPricingKey(FastPricingDefinitionToCreateDTO dto)
        {
            bool res = await _manageService.DefineFastPricingKey(dto);

            return Ok(res);
        }

        [HttpPut("DefineFastPricing/{Id}")]
        public async Task<ActionResult<bool>> EditFastPricing(Guid Id, FastPricingDefinitionToCreateDTO dto)
        {
            bool res = await _manageService.EditFastPricing(Id, dto);

            return Ok(res);
        }

        [HttpGet("DefineFastPricing/IsModelDefined/{id}")]
        public ActionResult<bool> IsModelDefined(Guid id)
        {
            bool res = _manageService.IsModelDefined(id);

            return Ok(res);
        }

        [HttpGet("FastPricingList")]
        public async Task<ActionResult<PagedList<FastPricingDefinitionToReturnDTO>>> FastPricingList([FromQuery] PaginationParams<FastPricingSearch> pagination)
        {
            PagedList<FastPricingDefinitionToReturnDTO> ls = await _manageService.FastPricingList(pagination);

            return Ok(ls);
        }

        [HttpGet("FastPricing/{id}")]
        public async Task<ActionResult<FastPricingDefinitionToReturnDTO>> FastPricing(Guid id)
        {
            FastPricingDefinitionToReturnDTO ls = await _manageService.FastPricing(id);

            return Ok(ls);
        }

        [HttpDelete("FastPricingDefinition/{id}")]
        public ActionResult<bool> RemoveFastPricingDefinition(Guid id)
        {
            bool res = _manageService.RemoveFastPricingDefinition(id);

            return Ok(res);
        }

        [HttpGet("TransactionList")]
        public async Task<ActionResult<PagedList<TransactionToReturnDTO>>> TransactionList([FromQuery] PaginationParams<TransactionSearch> pagination)
        {
            PagedList<TransactionToReturnDTO> ls = await _manageService.TransactionList(pagination);

            return Ok(ls);
        }

        [HttpGet("Transaction/{id}")]
        public ActionResult<TransactionToReturnDTO> GetTransaction(Guid id)
        {
            TransactionToReturnDTO dto = _manageService.GetTransaction(id);

            return Ok(dto);
        }

        [HttpGet("TransactionItems/{transactionId}")]
        public ActionResult<TransactionItemDTO> GetTransactionItems(Guid transactionId)
        {
            TransactionItemDTO dto = _manageService.GetTransactionItems(transactionId);

            return Ok(dto);
        }
    }
}
