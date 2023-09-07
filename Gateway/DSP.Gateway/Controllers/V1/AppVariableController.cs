using DSP.Gateway.Data;
using DSP.Gateway.Sevices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace DSP.Gateway.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AppVariableController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly ManageHttpService _manageHttpService;
        private readonly IUserService _userService;
        private readonly PaymentHttpService _paymentHttpService;
        private readonly ILogger<AppVariableController> _logger;
        public AppVariableController(IWebHostEnvironment env,
            ManageHttpService manageHttpService,
            IUserService memberService, PaymentHttpService paymentHttpService,
            ILogger<AppVariableController> logger)
        {
            _env = env;
            _manageHttpService = manageHttpService;
            _userService = memberService;
            _paymentHttpService = paymentHttpService;
            _logger = logger;
        }

        /// <summary>
        /// افزودن سوال متداول جدید
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("Admin/FAQ")]
        public async Task<ActionResult<bool>> FAQ(FAQToCreateDTO dto)
        {
            bool res = await _manageHttpService.FAQ(dto);

            return Ok(res);
        }
        /// <summary>
        /// مرتب کردن سوالات متداول
        /// </summary>
        /// <param name="arrangeIds"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("Admin/FAQ/Arrange")]
        public async Task<ActionResult<FAQToReturnDTO>> ArrangeFAQs(List<int> arrangeIds)
        {
            List<FAQToReturnDTO> ls = await _manageHttpService.ArrangeFAQs(arrangeIds);

            return Ok(ls);
        }

        /// <summary>
        /// حذف یک سوال متدوال
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpDelete("Admin/FAQ")]
        public async Task<ActionResult<bool>> RemoveFAQ(Guid id)
        {
            bool res = await _manageHttpService.RemoveFAQ(id);

            return Ok(res);
        }

        /// <summary>
        /// ویرایش درباره ما
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("Admin/AboutUs")]
        public async Task<ActionResult<bool>> AboutUs(AppVariableDTO dto)
        {
            bool res = await _manageHttpService.UpdateAboutUs(dto);

            return Ok(res);
        }

        /// <summary>
        /// ویرایش حریم خصوصی و امنیت
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("Admin/SecurityAndPrivacy")]
        public async Task<ActionResult<bool>> SecurityAndPrivacy(AppVariableDTO dto)
        {
            bool res = await _manageHttpService.SecurityAndPrivacy(dto);

            return Ok(res);
        }

        /// <summary>
        /// ویرایش شرایط وضوابط
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("Admin/TermsAndConditions")]
        public async Task<ActionResult<bool>> TermsAndCondition(AppVariableDTO dto)
        {
            bool res = await _manageHttpService.TermsAndCondition(dto);

            return Ok(res);
        }

        /// <summary>
        /// لیست شهرهای یک استان
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        [HttpGet("Admin/CitiesOfState")]
        [HttpGet("App/CitiesOfState")]
        [HttpGet("Web/CitiesOfState")]
        public async Task<ActionResult<List<CityDTO>>> CitiesOfState(Guid stateId)
        {
            List<CityDTO> ls = await _userService.GetCitiesOfState(stateId);
            return Ok(ls);
        }

        /// <summary>
        /// لیست استان ها
        /// </summary>
        /// <returns></returns>
        [HttpGet("App/StatesList")]
        [HttpGet("Web/StatesList")]
        [HttpGet("Admin/StatesList")]
        public async Task<ActionResult<List<ProvinceDTO>>> StatesList()
        {
            List<ProvinceDTO> ls = await _userService.GetStatesList();
            return Ok(ls);
        }

        /// <summary>
        /// لیست درگاه های پرداخت
        /// </summary>
        /// <returns></returns>
        [HttpGet("App/SupportedPayments")]
        [HttpGet("Web/SupportedPayments")]
        public ActionResult<List<SupportedPayment>> GetSupportedPayments()
        {
            List<SupportedPayment> ls = _paymentHttpService.GetSupportedPayments();

            return Ok(ls);
        }

        [HttpGet("Admin/Environment")]
        [AllowAnonymous]
        public ActionResult<Dictionary<string, string>> GetEnvironment()
        {

            var dic = new Dictionary<string, string>();

            dic.Add("Environment.GetEnvironmentVariable", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));

            dic.Add("_env.IsDevelopment()", _env.IsDevelopment().ToString());

            dic.Add("_env.EnvironmentName", _env.EnvironmentName);

            return Ok(dic);
        }

        [HttpGet("Admin/LogMessage")]
        [AllowAnonymous]
        public ActionResult LogMessage(string message)
        {
            _logger.LogError(message);

            throw new Exception(message);
        }
    }
}
