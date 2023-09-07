
using DSP.Gateway.Data;
using DSP.Gateway.Utilities;

namespace DSP.Gateway.Sevices
{
    public class OrderHttpService
    {
        public HttpClient Client { get; }
        public OrderHttpService(HttpClient client)
        {
            client.BaseAddress = new Uri("Order Base API Adress");
            Client = client;
        }
        public Task<int> AddToBasket(Guid userId, Guid itemId, Guid ColorId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ChangeOrderStatus(Guid id, OrderStatusDTO dto)
        {
            throw new NotImplementedException();
        }

        public BasketCountToReturnDTO ChangeProductsCountInBasket(Guid userId, bool action, Guid itemId, Guid colorId)
        {
            throw new NotImplementedException();
        }

        public CheckOutResultDTO CheckOutResult(string trackingCode, string authority)
        {
            throw new NotImplementedException();
        }

        public Task<CheckOutResponse> CheckoutTheBasket(Guid userId, BasketCheckOutDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreateBasket(Guid UserId)
        {
            throw new NotImplementedException();
        }

        public Task<Guid?> GetBasketId(Guid UserId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetBasketItemCount(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsProductInMyBasket(Guid userId, Guid productId, Guid colorId)
        {
            throw new NotImplementedException();
        }

        public Task<BasketToReturnDTO> MyBasket(Guid userId)
        {
            throw new NotImplementedException();
        }

        public MyOrderToReturnDTO MyOrderDetails(Guid userId, Guid orderId)
        {
            throw new NotImplementedException();
        }

        public Task<List<MyOrderToReturnDTO>> MyOrders(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<OrderToReturnDTO> OrderDetail(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<OrderDetailToReturnDTO>> OrderItems(Guid orderId)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<OrderToReturnDTO>> OrdersList(PaginationParams<OrderSearch> pagination)
        {
            throw new NotImplementedException();
        }

        public List<OrderStatusCountDTO> OrderStatusCount()
        {
            throw new NotImplementedException();
        }

        public Task<BasketCountToReturnDTO> RemoveFromBasket(Guid userId, Guid itemId, Guid colorId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetBasketAddress(Guid userId, BasketAddressDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task VerifyCheckOut(string trackingCode, string authority, string status)
        {
            throw new NotImplementedException();
        }
    }
}