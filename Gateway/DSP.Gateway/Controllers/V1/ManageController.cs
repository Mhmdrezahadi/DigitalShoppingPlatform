using DSP.Gateway.Data;
using DSP.Gateway.Data.Pricing;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DSP.Gateway.Sevices;
using DSP.Gateway.Utilities;

namespace DSP.Gateway.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,SuperAdmin")]
    public class ManageController : ControllerBase
    {
        private readonly ManageHttpService _manageHttpService;

        public ManageController(ManageHttpService manageHttpService)
        {
            _manageHttpService = manageHttpService;
        }

        /// <summary>
        /// ایجاد ساختار کالا
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("Admin/DefinePropertyKeys")]
        public async Task<ActionResult<bool>> DefinePropertyKeys([FromBody] ProductKeysDefinitionsDTO dto)
        {
            var userName = User.GetUserName();
            var userId = User.GetUserId();
            var roles = User.GetRoles();

            bool res = await _manageHttpService.DefinePropertyKes(dto);

            return Ok(res);
        }

        /// <summary>
        /// افزودن ویژگی به ساختار کالا
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("Admin/AddToPropertyKeys")]
        public async Task<ActionResult<bool>> AddToPropertyKeys([FromBody] ProductKeysDefinitionsDTO dto)
        {
            var userName = User.GetUserName();
            var userId = User.GetUserId();
            var roles = User.GetRoles();

            bool res = await _manageHttpService.AddToPropertyKeys(dto);

            return Ok(res);
        }

        /// <summary>
        /// ویرایش ویژگی های ساختار کالا
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost("Admin/EditPropertyKeys")]
        public async Task<ActionResult<bool>> EditPropertyKeys([FromBody] List<PropertyKeyDTO> list)
        {
            var userName = User.GetUserName();
            var userId = User.GetUserId();
            var roles = User.GetRoles();

            bool res = await _manageHttpService.EditPropertyKeys(list);

            return Ok(res);
        }

        /// <summary>
        /// حذف ویژگی ها از ساختار کالا
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("Admin/PropertyKeys/{id}")]
        public async Task<ActionResult<bool>> RemovePropertyKeys(Guid id)
        {
            bool res = await _manageHttpService.RemovePropertyKeys(id);

            return Ok(res);
        }

        /// <summary>
        /// ایجاد یک ساختار قیمت گذاری
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("Admin/DefineFastPricingKey")]
        public async Task<ActionResult<bool>> DefineFastPricingKey(FastPricingDefinitionToCreateDTO dto)
        {
            bool res = await _manageHttpService.DefineFastPricingKey(dto);

            return Ok(res);
        }

        /// <summary>
        /// ویرایش یک ساختار قیمت گذاری
        /// </summary>
        /// <returns></returns>
        [HttpPut("Admin/DefineFastPricing/{Id}")]
        public async Task<ActionResult<bool>> EditFastPricing(Guid Id, FastPricingDefinitionToCreateDTO dto)
        {
            bool res = await _manageHttpService.EditFastPricing(Id, dto);

            return Ok(res);
        }
        /// <summary>
        ///  آیا در یک مدل قیمت گذاری تعریف شده است؟
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Admin/DefineFastPricing/IsModelDefined/{id}")]
        public async Task<ActionResult<bool>> IsModelDefined(int id)
        {
            bool res = await _manageHttpService.IsModelDefined(id);

            return Ok(res);
        }
        /// <summary>
        /// دریافت لیست ساختارهای قیمت گذاری
        /// </summary>
        /// <returns></returns>
        [HttpGet("Admin/FastPricingList")]
        public async Task<ActionResult<PagedList<FastPricingDefinitionToReturnDTO>>> FastPricingList([FromQuery] PaginationParams<FastPricingSearch> pagination)
        {
            PagedList<FastPricingDefinitionToReturnDTO> ls = await _manageHttpService.FastPricingList(pagination);

            return Ok(ls);
        }
        /// <summary>
        /// دریافت یک ساختار قیمت گذاری
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Admin/FastPricing/{id}")]
        public async Task<ActionResult<FastPricingDefinitionToReturnDTO>> FastPricing(Guid id)
        {
            FastPricingDefinitionToReturnDTO ls = await _manageHttpService.FastPricing(id);

            return Ok(ls);
        }

        /// <summary>
        /// حذف یک ساختار قیمت گذاری
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("Admin/FastPricingDefinition/{id}")]
        public async Task<ActionResult<bool>> RemoveFastPricingDefinition(Guid id)
        {
            bool res = await _manageHttpService.RemoveFastPricingDefinition(id);

            return Ok(res);
        }

        /// <summary>
        /// لیست تراکنش های مالی
        /// </summary>
        /// <returns></returns>
        [HttpGet("Admin/TransactionList")]
        public async Task<ActionResult<PagedList<TransactionToReturnDTO>>> TransactionList([FromQuery] PaginationParams<TransactionSearch> pagination)
        {
            PagedList<TransactionToReturnDTO> ls = await _manageHttpService.TransactionList(pagination);

            return Ok(ls);
        }

        /// <summary>
        /// جزئیات یک تراکنش
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Admin/Transaction/{id}")]
        public async Task<ActionResult<TransactionToReturnDTO>> GetTransaction(Guid id)
        {
            TransactionToReturnDTO dto = await _manageHttpService.GetTransaction(id);

            return Ok(dto);
        }
        /// <summary>
        /// موارد موجود در یک تراکنش
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        [HttpGet("Admin/TransactionItems/{transactionId}")]
        public async Task<ActionResult<TransactionItemDTO>> GetTransactionItems(Guid transactionId)
        {
            TransactionItemDTO dto = await _manageHttpService.GetTransactionItems(transactionId);

            return Ok(dto);
        }
    }
}
