using DSP.Gateway.Data;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SellController : ControllerBase
    {
        private readonly SellHttpService _sellService;
        private readonly ManageHttpService _manageService;

        public SellController(SellHttpService sellHttpService, ManageHttpService manageHttpService)
        {
            _sellService = sellHttpService;
            _manageService = manageHttpService;
        }
        /// <summary>
        /// دریافت لیست دستگاه های من
        /// </summary>
        /// <returns></returns>
        [HttpGet("App/MyDeviceList")]
        [HttpGet("Web/MyDeviceList")]
        public async Task<ActionResult<List<FastPricingToReturnDTO>>> MyDeviceList()
        {
            var userName = User.GetUserName();
            var userId = User.GetUserId();
            var roles = User.GetRoles();

            List<FastPricingToReturnDTO> ls = await _sellService.MyDeviceList(userId);

            return Ok(ls);
        }

        /// <summary>
        /// دریافت دستگاه من
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("App/MyDevice/{id}")]
        [HttpGet("Web/MyDevice/{id}")]
        public async Task<ActionResult<FastPricingToReturnDTO>> MyDevice(Guid id)
        {
            var userName = User.GetUserName();
            var userId = User.GetUserId();
            var roles = User.GetRoles();

            FastPricingToReturnDTO dto = await _sellService.MyDevice(userId, id);

            return Ok(dto);
        }

        /// <summary>
        /// لیست درخواست های فروش
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("Admin/SellRequest")]
        public async Task<ActionResult<PagedList<SellRequestToReturnDTO>>> SellRequestList([FromQuery] PaginationParams<SellRequestSearch> pagination)
        {
            PagedList<SellRequestToReturnDTO> ls = await _manageService.SellRequestList(pagination);

            return Ok(ls);
        }
        /// <summary>
        /// دریافت جزئیات یک درخواست فروش
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("Admin/SellRequest/{id}")]
        public async Task<ActionResult<SellRequestToReturnDTO>> SellRequest(Guid id)
        {
            SellRequestToReturnDTO dto = await _manageService.SellRequest(id);

            return Ok(dto);
        }

        /// <summary>
        /// دریافت دستگاه موجود در یک درخواست فروش
        /// </summary>
        /// <param name="reqId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("Admin/DeviceInSellRequest/{reqId}")]
        public async Task<ActionResult<FastPricingToReturnDTO>> DeviceInSellRequest(Guid reqId)
        {

            FastPricingToReturnDTO dto = await _manageService.DeviceInSellRequest(reqId);

            return Ok(dto);
        }


        /// <summary>
        /// تغییر استاتوس یک درخواست فروش
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("Admin/SellRequest/{id}")]
        public async Task<ActionResult<bool>> ChangeSellRequestStatus(Guid id, SellRequestStatusDTO dto)
        {
            bool res = await _manageService.ChangeSellRequestStatus(id, dto);

            return Ok(res);
        }

        /// <summary>
        /// تعداد لیست درخواست های فروش بر اساس استاتوس
        /// </summary>
        /// <returns></returns>
        [HttpGet("Admin/SellRequestStatusCount")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public ActionResult<List<SellRequestStatusCountDTO>> SellRequestStatusCount()
        {
            List<SellRequestStatusCountDTO> ls = _sellService.SellRequestStatusCount();

            return Ok(ls);
        }


        /// <summary>
        /// دریافت کلید ها و مقدار های قیمت گذاری بدون افزودن دستگاه
        /// </summary>
        /// <remarks>
        ///  دریافت کلید ها و مقدار های پیشنهادی برای قیمت گذاری
        /// Get fast pricing keys and values from both tables AND DD
        /// </remarks>
        /// <returns></returns>
        [HttpGet("App/FastPricingKeysAndValues/{catId}")]
        [HttpGet("Web/FastPricingKeysAndValues/{catId}")]
        [HttpGet("Admin/FastPricingKeysAndValues/{catId}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<FastPricingKeysAndDDsToReturnDTO>>> FastPricingKeysAndValues(int catId)
        {
            List<FastPricingKeysAndDDsToReturnDTO> ls = await _sellService.FastPricingKeysAndValues(catId);

            return Ok(ls);
        }

        /// <summary>
        /// دریافت کلید ها و مقدار های قیمت گذاری با افزودن دستگاه
        /// </summary>
        /// <remarks>
        /// دریافت کلید ها و مقدار های پیشنهادی برای افزودن دستگاه 
        /// </remarks>
        /// <param name="catId"></param>
        /// <returns></returns>
        [HttpGet("App/DeviceKeysAndValues/{catId}")]
        [HttpGet("Web/DeviceKeysAndValues/{catId}")]
        [HttpGet("Admin/DeviceKeysAndValues/{catId}")]
        public async Task<ActionResult<List<FastPricingKeysAndDDsToReturnDTO>>> DeviceKeysAndValues(int catId)
        {
            List<FastPricingKeysAndDDsToReturnDTO> ls = await _sellService.FastPricingKeysAndValues(catId);

            return Ok(ls);
        }

        /// <summary>
        /// قیمت گذاری سریع به همراه افزودن دستگاه
        /// </summary>
        /// <remarks>
        /// افزودن دستگاه بر اساس کلید های دریافتی و مقدارهای پیشنهادی
        /// </remarks>
        /// <returns></returns>
        [HttpPost("App/Device")]
        [HttpPost("Web/Device")]
        public async Task<ActionResult<Guid>> AddDevice(FastPricingForCreateDTO dto)
        {
            var userName = User.GetUserName();
            var userId = User.GetUserId();
            var roles = User.GetRoles();

            Guid resultId = await _sellService.AddDevice(userId, dto);
            return Ok(resultId);
        }

        /// <summary>
        /// حذف دستگاه من
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        [HttpDelete("App/Device/{deviceId}")]
        [HttpDelete("Web/Device/{deviceId}")]
        public ActionResult<bool> RemoveDevice(Guid deviceId)
        {
            var userName = User.GetUserName();
            var userId = User.GetUserId();
            var roles = User.GetRoles();

            bool result = _sellService.RemoveDevice(deviceId, userId);

            return Ok(result);
        }
        /// <summary>
        /// به روز رسانی دستگاه من
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("App/Device/{deviceId}")]
        [HttpPut("Web/Device/{deviceId}")]
        public ActionResult<bool> UpdateDevice(Guid deviceId, FastPricingForCreateDTO dto)
        {
            var userName = User.GetUserName();
            var userId = User.GetUserId();
            var roles = User.GetRoles();

            bool result = _sellService.UpdateDevice(deviceId, userId, dto);

            return Ok(result);
        }

        /// <summary>
        ///  قیمت گذاری سریع بدون افزودن دستگاه
        /// </summary>
        /// <remarks>
        /// Post fast pricing values => by updating values table => dto contains value id and exact value
        /// </remarks>
        /// <returns></returns>
        [HttpPost("App/FastPricingValues")]
        [HttpPost("Web/FastPricingValues")]
        [AllowAnonymous]
        public async Task<ActionResult<FastPricingToReturnDTO>> FastPricingValues(FastPricingForCreateDTO dto)
        {
            FastPricingToReturnDTO res = await _sellService.FastPricingValues(dto);

            return Ok(res);
        }

        /// <summary>
        /// درخواست فروش یک دستگاه به تل بال
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("App/SellRequest")]
        [HttpPost("Web/SellRequest")]
        public async Task<ActionResult<bool>> SellRequest(SellRequestDTO dto)
        {
            bool res = await _sellService.SellRequest(dto);

            return Ok(res);
        }
        /// <summary>
        /// لیست درخواست های فروش من
        /// </summary>
        /// <returns></returns>
        [HttpGet("App/MySellRequests")]
        [HttpGet("Web/MySellRequests")]
        public async Task<ActionResult<List<SellRequestToReturnDTO>>> MySellRequests()
        {
            var userName = User.GetUserName();
            var userId = User.GetUserId();
            var roles = User.GetRoles();

            List<SellRequestToReturnDTO> ls = await _sellService.MySellRequests(userId);

            return Ok(ls);
        }
    }
}
