
using DSP.Gateway.Data;
using DSP.Gateway.Entities;
using DSP.Gateway.Utilities;
using Newtonsoft.Json;
using Polly;
using System;
using System.Text;

namespace DSP.Gateway.Sevices
{
    public class OrderHttpService
    {
        public HttpClient Client { get; }
        public OrderHttpService(HttpClient client)
        {
            client.BaseAddress = new Uri("http://localhost:5301/DSP/ProductService/Order/");
            Client = client;
        }
        public async Task<int> AddToBasket(Guid userId, Guid itemId, Guid colorId)
        {
            var response = await Client.GetAsync($"AddToBasket/{userId}?itemId={itemId}&colorId={colorId}");
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync());
            return result;
        }

        public async Task<bool> ChangeOrderStatus(Guid id, OrderStatusDTO dto)
        {
            var response = await Client.GetAsync($"OrderStatus/{id}?dto={dto}");
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync());
            return result;
        }

        public async Task<BasketCountToReturnDTO> ChangeProductsCountInBasket(Guid userId, bool action, Guid itemId, Guid colorId)
        {
            var response = await Client.GetAsync($"ChangeProductsCountInBasket/{userId}?action={action}&itemId={itemId}&ColorId={colorId}");
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<BasketCountToReturnDTO>(await response.Content.ReadAsStringAsync());
            return result;
        }

        public async Task<CheckOutResultDTO> CheckOutResult(string trackingCode, string authority)
        {
            var response = await Client.GetAsync($"CheckoutResult");
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<CheckOutResultDTO>(await response.Content.ReadAsStringAsync());
            return result;
        }

        public async Task<CheckOutResponse> CheckoutTheBasket(Guid userId, BasketCheckOutDTO dto)
        {
            var data = JsonConvert.SerializeObject(dto);

            var response = await Client.PostAsync($"CheckoutTheBasket/{userId}", new StringContent(data, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            var res = JsonConvert.DeserializeObject<CheckOutResponse>(await response.Content.ReadAsStringAsync());
            return res;
        }

        public Task<bool> CreateBasket(Guid UserId)
        {
            throw new NotImplementedException();
        }

        public Task<Guid?> GetBasketId(Guid UserId)
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetBasketItemCount(Guid userId)
        {
            var response = await Client.GetAsync($"BasketItemCount/{userId}");
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync());
            return result;
        }

        public async Task<bool> IsProductInMyBasket(Guid userId, Guid productId, Guid colorId)
        {
            var response = await Client.GetAsync($"IsProductInMyBasket/{userId}?productId={productId}&colorId={colorId}");
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync());
            return result;
        }

        public async Task<BasketToReturnDTO> MyBasket(Guid userId)
        {
            var response = await Client.GetAsync($"MyBasket/{userId}");
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<BasketToReturnDTO>(await response.Content.ReadAsStringAsync());
            return result;
        }

        public async Task<MyOrderToReturnDTO> MyOrderDetails(Guid userId, Guid orderId)
        {
            var response = await Client.GetAsync($"MyOrderDetails/{userId}/{orderId}");
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<MyOrderToReturnDTO>(await response.Content.ReadAsStringAsync());
            return result;
        }

        public async Task<List<MyOrderToReturnDTO>> MyOrders(Guid userId)
        {
            var response = await Client.GetAsync($"MyOrders/{userId}");
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<List<MyOrderToReturnDTO>>(await response.Content.ReadAsStringAsync());
            return result;
        }

        public async Task<OrderToReturnDTO> OrderDetail(Guid id)
        {
            var response = await Client.GetAsync($"Orders/{id}");
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<OrderToReturnDTO>(await response.Content.ReadAsStringAsync());
            return result;
        }

        public async Task<List<OrderDetailToReturnDTO>> OrderItems(Guid orderId)
        {
            var response = await Client.GetAsync($"OrderItems/{orderId}");
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<List<OrderDetailToReturnDTO>>(await response.Content.ReadAsStringAsync());
            return result;
        }

        public async Task<PagedList<OrderToReturnDTO>> OrdersList(PaginationParams<OrderSearch> pagination)
        {
            var response = await Client.GetAsync($"OrdersList");
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<PagedList<OrderToReturnDTO>>(await response.Content.ReadAsStringAsync());
            return result;
        }

        public async Task<List<OrderStatusCountDTO>> OrderStatusCount()
        {
            var response = await Client.GetAsync($"OrderStatusCount");
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<List<OrderStatusCountDTO>>(await response.Content.ReadAsStringAsync());
            return result;
        }

        public async Task<BasketCountToReturnDTO> RemoveFromBasket(Guid userId, Guid itemId, Guid colorId)
        {
            var response = await Client.DeleteAsync($"RemoveFromBasket/{userId}?itemId={itemId}&ColorId={colorId}");
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<BasketCountToReturnDTO>(await response.Content.ReadAsStringAsync());
            return result;
        }

        public async Task<bool> SetBasketAddress(Guid userId, BasketAddressDTO dto)
        {
            var data = JsonConvert.SerializeObject(dto);

            var response = await Client.PutAsync($"BasketAddress/{userId}", new StringContent(data, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            var res = JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync());
            return res;

        }

        public async Task VerifyCheckOut(string trackingCode, string authority, string status)
        {
            var response = await Client.GetAsync($"VerifyCheckOut/{trackingCode}?authority={authority}&status={status}");
            response.EnsureSuccessStatusCode();
        }
    }
}