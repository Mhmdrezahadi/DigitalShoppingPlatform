using DSP.ProductService.Data;
using DSP.ProductService.Services;
using DSP.ProductService.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace DSP.ProductService.Controllers
{
    [ApiController]
    [Route("DSP/ProductService/Order")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [HttpGet("AddToBasket/{userId}")]
        public async Task<ActionResult<int>> AddToBasket(Guid userId, [Required] Guid ItemId, [Required] Guid colorId)
        {
            int result = await _orderService.AddToBasket(userId, ItemId, colorId);

            return Ok(result);
        }

        [HttpGet("ChangeProductsCountInBasket/{userId}")]
        public ActionResult<BasketCountToReturnDTO> ChangeProductsCountInBasket(Guid userId, [Required] bool action, [Required] Guid ItemId, [Required] Guid ColorId)
        {
            BasketCountToReturnDTO result = _orderService.ChangeProductsCountInBasket(userId, action, ItemId, ColorId);

            return Ok(result);
        }

        [HttpDelete("RemoveFromBasket/{userId}")]
        public async Task<ActionResult<BasketCountToReturnDTO>> Basket(
            Guid userId,
            [Required][FromQuery] Guid itemId,
            [Required][FromQuery] Guid colorId)
        {
            BasketCountToReturnDTO result = await _orderService.RemoveFromBasket(userId, itemId, colorId);

            return Ok(result);
        }

        [HttpGet("BasketItemCount/{userId}")]
        public async Task<ActionResult<int>> BasketItemCount(Guid userId)
        {
            int itemCount = await _orderService.GetBasketItemCount(userId);
            return Ok(itemCount);
        }

        [HttpPut("BasketAddress/{userId}")]
        public async Task<ActionResult<bool>> SetBasketAddress(Guid userId, BasketAddressDTO order)
        {
            bool result = await _orderService.SetBasketAddress(userId, order);

            return Ok(result);
        }

        [HttpGet("OrdersList")]
        public async Task<ActionResult<PagedList<OrderToReturnDTO>>> OrdersList([FromQuery] PaginationParams<OrderSearch> pagination)
        {
            PagedList<OrderToReturnDTO> ls = await _orderService.OrdersList(pagination);

            return Ok(ls);
        }

        [HttpGet("OrderStatusCount")]
        public ActionResult<List<OrderStatusCountDTO>> OrderStatusCount()
        {
            List<OrderStatusCountDTO> ls = _orderService.OrderStatusCount();

            return Ok(ls);
        }

        [HttpGet("Orders/{id}")]
        public async Task<ActionResult<OrderToReturnDTO>> OrderDetail(Guid id)
        {
            OrderToReturnDTO dto = await _orderService.OrderDetail(id);

            return Ok(dto);
        }

        [HttpGet("OrderItems/{orderId}")]
        public async Task<ActionResult<List<OrderDetailToReturnDTO>>> OrderItems(Guid orderId)
        {
            List<OrderDetailToReturnDTO> ls = await _orderService.OrderItems(orderId);

            return Ok(ls);
        }

        [HttpPut("OrderStatus/{id}")]
        public async Task<ActionResult<bool>> ChangeOrderStatus(Guid id, [Required] OrderStatusDTO dto)
        {
            bool res = await _orderService.ChangeOrderStatus(id, dto);

            return Ok(res);
        }

        [HttpGet("MyBasket/{userId}")]
        public async Task<ActionResult<BasketToReturnDTO>> MyBasket(Guid userId)
        {
            BasketToReturnDTO dto = await _orderService.MyBasket(userId);

            return Ok(dto);
        }

        [HttpGet("IsProductInMyBasket/{userId}")]
        public async Task<ActionResult<bool>> IsProductInMyBasket( Guid userId, [Required] Guid productId, [Required] Guid colorId)
        {
            bool res = await _orderService.IsProductInMyBasket(userId, productId, colorId);

            return Ok(res);
        }

        [HttpGet("MyOrders/{userId}")]
        public async Task<ActionResult<List<MyOrderToReturnDTO>>> MyOrders(Guid userId)
        {
            List<MyOrderToReturnDTO> dto = await _orderService.MyOrders(userId);

            return Ok(dto);
        }

        [HttpGet("MyOrderDetails/{userId}/{orderId}")]
        public async Task<ActionResult<MyOrderToReturnDTO>> MyOrderDetails(Guid userId, Guid orderId)
        {
            MyOrderToReturnDTO dto = _orderService.MyOrderDetails(userId, orderId);

            return Ok(dto);
        }


        [HttpPost("CheckoutTheBasket/{userId}")]
        public async Task<ActionResult<CheckOutResponse>> CheckoutTheBasket(Guid userId, BasketCheckOutDTO dto)
        {
            _logger.LogError("execute CheckouTheBasket");

            CheckOutResponse result = await _orderService.CheckoutTheBasket(userId, dto);

            return Ok(result);
        }

        [HttpGet("VerifyCheckOut/{trackingCode}")]
        public async Task<ActionResult> VerifyCheckOut(string trackingCode, [Required][FromQuery] string Authority, [Required][FromQuery] string Status)
        {
            var url = $"RedirectUrl?trackingCode={trackingCode}&authority={Authority}";
            try
            {
                await _orderService.VerifyCheckOut(trackingCode, Authority, Status);
                return Redirect(url);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Redirect(url);
            }
        }

        [HttpGet("CheckoutResult")]
        public ActionResult<CheckOutResultDTO> PaymentResponse([Required][FromQuery] string trackingCode, [Required][FromQuery] string authority)
        {
            CheckOutResultDTO dto = _orderService.CheckOutResult(trackingCode, authority);

            return Ok(dto);
        }
    }
}
