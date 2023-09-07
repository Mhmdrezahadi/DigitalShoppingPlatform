using DSP.Gateway.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DSP.Gateway.Sevices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using DSP.Gateway.Utilities;

namespace Tellbal.Controllers.V1.Shopping
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrderController : ControllerBase
    {
        private readonly OrderHttpService _orderHttpService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(OrderHttpService orderHttpService, ILogger<OrderController> logger)
        {
            _orderHttpService = orderHttpService;
            _logger = logger;
        }

        /// <summary>
        /// add product to basket
        /// </summary>
        /// <param name="ItemId"></param>
        /// <returns></returns>
        [HttpGet("App/AddToBasket")]
        [HttpGet("Web/AddToBasket")]
        public async Task<ActionResult<int>> AddToBasket([Required] Guid ItemId, [Required] Guid ColorId)
        {
            var userName = User.GetUserName();
            var userId = User.GetUserId();
            var roles = User.GetRoles();

            int result = await _orderHttpService.AddToBasket(userId, ItemId, ColorId);

            return Ok(result);
        }

        /// <summary>
        /// Change count of basket products
        /// </summary>
        /// <remarks>
        /// action = false و برای کاهش action = true  برای افزایش 
        /// </remarks>
        /// <param name="action"></param>
        /// <param name="ItemId"></param>
        /// <param name="ColorId"></param>
        /// <returns></returns>
        [HttpGet("App/ChangeProductsCountInBasket")]
        [HttpGet("Web/ChangeProductsCountInBasket")]
        public ActionResult<BasketCountToReturnDTO> ChangeProductsCountInBasket([Required] bool action, [Required] Guid ItemId, [Required] Guid ColorId)
        {
            var userName = User.GetUserName();
            var userId = User.GetUserId();
            var roles = User.GetRoles();

            BasketCountToReturnDTO result = _orderHttpService.ChangeProductsCountInBasket(userId, action, ItemId, ColorId);

            return Ok(result);
        }


        /// <summary>
        /// Remove an item from basket
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="colorId"></param>
        /// <returns></returns>
        [HttpDelete("App/RemoveFromBasket")]
        [HttpDelete("Web/RemoveFromBasket")]
        public async Task<ActionResult<BasketCountToReturnDTO>> Basket(
            [Required][FromQuery] Guid itemId,
            [Required][FromQuery] Guid colorId)
        {
            var userName = User.GetUserName();
            var userId = User.GetUserId();
            var roles = User.GetRoles();

            BasketCountToReturnDTO result = await _orderHttpService.RemoveFromBasket(userId, itemId, colorId);

            return Ok(result);
        }
        /// <summary>
        /// number of products in basket
        /// </summary>
        /// <returns></returns>
        [HttpGet("App/BasketItemCount")]
        [HttpGet("Web/BasketItemCount")]
        public async Task<ActionResult<int>> BasketItemCount()
        {
            var userName = User.GetUserName();
            var userId = User.GetUserId();
            var roles = User.GetRoles();

            int itemCount = await _orderHttpService.GetBasketItemCount(userId);
            return Ok(itemCount);
        }

        /// <summary>
        /// assign address to basket
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPut("App/BasketAddress")]
        [HttpPut("Web/BasketAddress")]
        public async Task<ActionResult<bool>> SetBasketAddress(BasketAddressDTO order)
        {
            var userName = User.GetUserName();
            var userId = User.GetUserId();
            var roles = User.GetRoles();

            bool result = await _orderHttpService.SetBasketAddress(userId, order);

            return Ok(result);
        }

        /// <summary>
        /// لیست سفارشات
        /// </summary>
        /// <returns></returns>
        [HttpGet("Admin/OrdersList")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult<PagedList<OrderToReturnDTO>>> OrdersList([FromQuery] PaginationParams<OrderSearch> pagination)
        {
            var userName = User.GetUserName();
            var userId = User.GetUserId();
            var roles = User.GetRoles();

            PagedList<OrderToReturnDTO> ls = await _orderHttpService.OrdersList(pagination);

            return Ok(ls);
        }

        /// <summary>
        /// تعداد لیست سفارشات بر اساس استاتوس
        /// </summary>
        /// <returns></returns>
        [HttpGet("Admin/OrderStatusCount")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public ActionResult<List<OrderStatusCountDTO>> OrderStatusCount()
        {
            List<OrderStatusCountDTO> ls = _orderHttpService.OrderStatusCount();

            return Ok(ls);
        }

        /// <summary>
        /// جزئیات یک سفارش
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Admin/Orders/{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult<OrderToReturnDTO>> OrderDetail(Guid id)
        {
            OrderToReturnDTO dto = await _orderHttpService.OrderDetail(id);

            return Ok(dto);
        }

        /// <summary>
        /// محصولات موجود در یک سفارش
        /// </summary>
        /// <returns></returns>
        [HttpGet("Admin/OrderItems/{orderId}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult<List<OrderDetailToReturnDTO>>> OrderItems(Guid orderId)
        {
            List<OrderDetailToReturnDTO> ls = await _orderHttpService.OrderItems(orderId);

            return Ok(ls);
        }

        /// <summary>
        /// تغییر وضعیت یک سفارش
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("Admin/OrderStatus/{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult<bool>> ChangeOrderStatus(Guid id, [Required] OrderStatusDTO dto)
        {
            bool res = await _orderHttpService.ChangeOrderStatus(id, dto);

            return Ok(res);
        }

        /// <summary>
        /// My basket
        /// </summary>
        /// <returns></returns>
        [HttpGet("App/MyBasket")]
        [HttpGet("Web/MyBasket")]
        public async Task<ActionResult<BasketToReturnDTO>> MyBasket()
        {
            var userName = User.GetUserName();
            var userId = User.GetUserId();
            var roles = User.GetRoles();

            BasketToReturnDTO dto = await _orderHttpService.MyBasket(userId);

            return Ok(dto);
        }

        /// <summary>
        /// Is product in my basket?
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="colorId"></param>
        /// <returns></returns>
        [HttpGet("App/IsProductInMyBasket")]
        [HttpGet("Web/IsProductInMyBasket")]
        public async Task<ActionResult<bool>> IsProductInMyBasket([Required] Guid productId, [Required] Guid colorId)
        {
            var userName = User.GetUserName();
            var userId = User.GetUserId();
            var roles = User.GetRoles();

            bool res = await _orderHttpService.IsProductInMyBasket(userId, productId, colorId);

            return Ok(res);
        }

        /// <summary>
        /// My orders list
        /// </summary>
        /// <returns></returns>
        [HttpGet("App/MyOrders")]
        [HttpGet("Web/MyOrders")]
        public async Task<ActionResult<List<MyOrderToReturnDTO>>> MyOrders()
        {
            var userName = User.GetUserName();
            var userId = User.GetUserId();
            var roles = User.GetRoles();

            List<MyOrderToReturnDTO> dto = await _orderHttpService.MyOrders(userId);

            return Ok(dto);
        }

        /// <summary>
        /// My order details
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet("App/MyOrderDetails/{orderId}")]
        [HttpGet("Web/MyOrderDetails/{orderId}")]
        public async Task<ActionResult<MyOrderToReturnDTO>> MyOrderDetails(Guid orderId)
        {
            var userName = User.GetUserName();
            var userId = User.GetUserId();
            var roles = User.GetRoles();


            MyOrderToReturnDTO dto = _orderHttpService.MyOrderDetails(userId, orderId);

            return Ok(dto);
        }

        [HttpGet("App/ApplyCouponToBasket/{basketId}")]
        [HttpGet("Web/ApplyCouponToBasket/{basketId}")]
        public async Task<ActionResult<bool>> ApplyCouponToBasket([FromQuery] string couponCode, [FromRoute] Guid basketId)
        {
            return Ok(Task.FromResult(true));
        }

        [HttpGet("App/RemoveCouponFromBasket/{basketId}")]
        [HttpGet("Web/RemoveCouponFromBasket/{basketId}")]
        public async Task<ActionResult<bool>> RemoveCouponFromBasket([FromRoute] Guid basketId)
        {
            return Ok(Task.FromResult(true));
        }

        /// <summary>
        /// checkout
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("App/CheckoutTheBasket")]
        [HttpPost("Web/CheckoutTheBasket")]
        public async Task<ActionResult<CheckOutResponse>> CheckoutTheBasket(BasketCheckOutDTO dto)
        {
            var userName = User.GetUserName();
            var userId = User.GetUserId();
            var roles = User.GetRoles();
            _logger.LogError("execute CheckouTheBasket");

            CheckOutResponse result = await _orderHttpService.CheckoutTheBasket(userId, dto);

            return Ok(result);
        }

        /// <summary>
        /// Verify checkout from payment gateway
        /// </summary>
        /// <param name="trackingCode"></param>
        /// <param name="Authority"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        [HttpGet("App/VerifyCheckOut/{trackingCode}")]
        [HttpGet("Web/VerifyCheckOut/{trackingCode}")]
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCheckOut(string trackingCode, [Required][FromQuery] string Authority, [Required][FromQuery] string Status)
        {
            var url = $"{"_siteSettings.PaymentSettings.RedirectUrl"}?trackingCode={trackingCode}&authority={Authority}";
            try
            {
                await _orderHttpService.VerifyCheckOut(trackingCode, Authority, Status);
                return Redirect(url);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Redirect(url);
            }


        }
        /// <summary>
        /// Checkout result
        /// </summary>
        /// <param name="authority"></param>
        /// <param name="trackingCode"></param>
        /// <returns></returns>
        [HttpGet("Web/CheckoutResult")]
        [HttpGet("App/CheckoutResult")]
        [AllowAnonymous]
        public ActionResult<CheckOutResultDTO> PaymentResponse([Required][FromQuery] string trackingCode, [Required][FromQuery] string authority)
        {
            CheckOutResultDTO dto = _orderHttpService.CheckOutResult(trackingCode, authority);

            return Ok(dto);
        }
    }
}
