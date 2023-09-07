using DSP.Gateway.Data;
using DSP.Gateway.Sevices;
using DSP.Gateway.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DSP.Gateway.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : ControllerBase
    {
        #region Configurations
        private readonly IUserService _userService;
        private readonly PaymentHttpService _paymentHttpService;
        private readonly IWebHostEnvironment _env;

        public UserController(IUserService memberService, IWebHostEnvironment env, PaymentHttpService paymentHttpService)
        {
            _userService = memberService;
            _paymentHttpService = paymentHttpService;
            _env = env;
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

        #endregion
        #region APIs
        /// <summary>
        /// دریافت کد ورود با پیامک
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <returns></returns>
        [HttpGet("App/GetOtp")]
        [HttpGet("Web/GetOtp")]
        [HttpGet("Admin/GetOtp")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> GetOtp([FromQuery][Required] string mobileNumber)
        {
            OtpResponseDTO result = await _userService.GetOtp(mobileNumber);

            return Ok(result);
        }

        /// <summary>
        /// ورود با شماره تلفن و کد پیامکی
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>نتیجه ورود</returns>
        /// <response code="200">return user associated with mobile number</response>
        /// <response code="400">if verfication code invalid</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        [HttpPost("App/Auth")]
        [HttpPost("Web/Auth")]
        public async Task<ActionResult<LoginResultDTO>> Auth(AuthDTO dto)
        {
            LoginResultDTO lr = await _userService.Auth(dto);

            return Ok(lr);
        }
        /// <summary>
        /// ورود
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("Admin/Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResultDTO>> LogIn([FromBody] UserForLoginDTO user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            LoginResultDTO result = await _userService.Login(user);

            return Ok(result);
        }
        /// <summary>
        /// آیا پروفایل تکمیل شده است؟
        /// </summary>
        /// <returns></returns>
        [HttpGet("App/IsProfileCompleted")]
        [HttpGet("Web/IsProfileCompleted")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> IsProfileCompleted()
        {
            var userName = User.GetUserName();
            var userId = User.GetUserId();
            var roles = User.GetRoles();

            bool result = await _userService.IsProfileCompleted(userId);
            return Ok(result);
        }

        /// <summary>
        /// تکمیل پروفایل
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("App/Profile")]
        [HttpPut("Web/Profile")]
        public async Task<ActionResult<bool>> Profile([FromBody] ProfileToUpdateDTO dto)
        {
            var userId = User.GetUserId();

            bool result = await _userService.SetProfile(userId, dto);

            return Ok(result);
        }

        /// <summary>
        /// ویرایش تصویر پروفایل
        /// </summary>
        /// <remarks>
        /// آدرس تصویر آپلود شده
        /// </remarks>
        /// <param name="Img"></param>
        /// <returns></returns>
        [HttpPut("App/ProfilePicture")]
        [HttpPut("Web/ProfilePicture")]
        public async Task<ActionResult<List<string>>> ProfilePicture(IFormFile Img)
        {
            var userId = User.GetUserId();

            List<string> result = await _userService.ProfilePicture(userId, Img);

            return Ok(result);
        }

        /// <summary>
        /// دریافت پروفایل
        /// </summary>
        /// <returns></returns>
        [HttpGet("App/Profile")]
        [HttpGet("Web/Profile")]
        public async Task<ActionResult<ProfileToReturnDTO>> Profile()
        {
            var userName = User.GetUserName();
            var userId = User.GetUserId();
            var roles = User.GetRoles();

            ProfileToReturnDTO dto = await _userService.GetProfile(userId);
            return Ok(dto);
        }

        /// <summary>
        /// لایک کردن یا آنلایک کردن محصول
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        [HttpGet("App/LikeProduct/{productId}")]
        [HttpGet("Web/LikeProduct/{productId}")]
        public async Task<ActionResult<bool>> LikeOrUnlike(Guid productId, [Required] bool action)
        {
            var userName = User.GetUserName();
            var userId = User.GetUserId();
            var roles = User.GetRoles();

            bool response = await _userService.LikeOrUnlike(userId, productId, action);

            return Ok(response);
        }

        /// <summary>
        /// آیا محصول توسط من لایک شده است؟
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpGet("App/IsProductLikedByMe")]
        [HttpGet("Web/IsProductLikedByMe")]
        public async Task<ActionResult<bool>> IsProductLikedByMe(Guid productId)
        {
            var userName = User.GetUserName();
            var userId = User.GetUserId();
            var roles = User.GetRoles();

            bool response = await _userService.IsProductLikedByMe(userId, productId);

            return Ok(response);
        }

        /// <summary>
        /// تعداد کالاهایی که من لایک کرده ام
        /// </summary>
        /// <returns></returns>
        [HttpGet("App/LikedProductsCount")]
        [HttpGet("Web/LikedProductsCount")]
        public async Task<ActionResult<int>> LikedProductsCount()
        {
            var userName = User.GetUserName();
            var userId = User.GetUserId();
            var roles = User.GetRoles();

            int res = await _userService.LikedProductsCount(userId);

            return Ok(res);
        }


        /// <summary>
        /// لیست آدرس های من
        /// </summary>
        /// <returns></returns>
        [HttpGet("App/UserAdressList")]
        [HttpGet("Web/UserAdressList")]
        public async Task<ActionResult<List<AddressToReturnDTO>>> UserAdressList()
        {
            var userName = User.GetUserName();
            var userId = User.GetUserId();
            var roles = User.GetRoles();

            List<AddressToReturnDTO> ls = await _userService.UserAdressList(userId);
            return Ok(ls);
        }

        /// <summary>
        /// لیست آدرس های یک کاربر
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("Admin/UserAdressList")]
        public async Task<ActionResult<List<AddressToReturnDTO>>> GetUserAdressList(Guid userId)
        {
            List<AddressToReturnDTO> ls = await _userService.UserAdressList(userId);
            return Ok(ls);
        }
        /// <summary>
        /// ثبت آدرس
        /// </summary>
        /// <returns></returns>
        [HttpPost("App/Address")]
        [HttpPost("Web/Address")]
        public async Task<ActionResult<Guid>> Address(AddressForCreateDTO dto)
        {
            var userName = User.GetUserName();
            var userId = User.GetUserId();
            var roles = User.GetRoles();

            Guid res = await _userService.PostAddress(userId, dto);

            return Ok(res);
        }

        /// <summary>
        /// حذف آدرس من
        /// </summary>
        /// <remarks>
        /// اگر در هیچ سفارشی استفاده نشده باشد میتوان حذف کرد
        /// </remarks>
        /// <param name="addressId"></param>
        /// <returns></returns>
        [HttpDelete("App/Address")]
        [HttpDelete("Web/Address")]
        public async Task<ActionResult<bool>> Address(Guid addressId)
        {
            var userName = User.GetUserName();
            var userId = User.GetUserId();
            var roles = User.GetRoles();

            bool res = await _userService.DeleteAddress(userId, addressId);

            return Ok(res);
        }

        /// <summary>
        /// ویرایش آدرس من
        /// </summary>
        /// <remarks>
        /// اگر در هیچ سفارشی استفاده نشده باشد میتوان ویرایش کرد
        /// </remarks>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("App/Address")]
        [HttpPut("Web/Address")]
        public async Task<ActionResult<bool>> Address(AddressToReturnDTO dto)
        {
            var userName = User.GetUserName();
            var userId = User.GetUserId();
            var roles = User.GetRoles();

            bool res = await _userService.EditAddress(userId, dto);

            return Ok(res);
        }

        /// <summary>
        /// لیست کاربر ها
        /// </summary>
        /// <returns></returns>
        [HttpGet("Admin/UserList")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult<PagedList<UserToReturnDTO>>> GetUsers([FromQuery] PaginationParams<UserSearch> pagination)
        {
            PagedList<UserToReturnDTO> ls = await _userService.GetUsers(pagination);

            return Ok(ls);
        }
        /// <summary>
        /// دریافت اطلاعات یک کاربر
        /// </summary>
        /// <returns></returns>
        [HttpGet("Admin/User/{userId}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult<UserToReturnDTO>> GetUser(Guid userId)
        {
            UserToReturnDTO dto = await _userService.GetUser(userId);

            return Ok(dto);
        }

        /// <summary>
        /// لیست تراکنش های یک کاربر
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("Admin/UserPayments/{userId}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public ActionResult<List<UserPaymentToReturnDTO>> UserPayments(Guid userId)
        {
            List<UserPaymentToReturnDTO> ls = _paymentHttpService.UserPayments(userId);

            return Ok(ls);
        }
        #endregion
    }
}
