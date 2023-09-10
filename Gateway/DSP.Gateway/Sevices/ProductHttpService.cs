
using DSP.Gateway.Data;
using DSP.Gateway.Utilities;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Polly;
using System.Net.Mime;
using System.Text;
using System.Web;

namespace DSP.Gateway.Sevices
{
    public class ProductHttpService
    {
        public HttpClient Client { get; }

        public ProductHttpService(HttpClient client)
        {
            client.BaseAddress = new Uri("http://localhost:5301/DSP/ProductService/");
            Client = client;
        }
        //TODO image
        public Task<CreatedImageToReturnDTO> AddImage(IFormFile image)
        {
            throw new NotImplementedException();
        }

        public async Task<Guid?> AddProduct(ProductForCreateDTO dto)
        {
            var data = JsonConvert.SerializeObject(dto);

            var polly = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(3, pause => TimeSpan.FromSeconds(5));

            await polly.ExecuteAsync(async () =>
            {
                var response = await Client.PostAsync("Product", new StringContent(data, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
                var productId = JsonConvert.DeserializeObject<Guid>(await response.Content.ReadAsStringAsync());
                return productId;
            });
            return null;
        }
        //TODO Fix HttpRequest
        public async Task<List<ProductToReturnDTO>> CompareTwoProduct(List<Guid> productIds)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            foreach (var item in productIds)
            {
                query["productIds"] = item.ToString();
            }
            string queryString = query.ToString();
            var response = await Client.GetAsync($"CompareTwoProduct{queryString}");
            var productKeyValues = JsonConvert.DeserializeObject<List<ProductToReturnDTO>>
                (await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();
            return productKeyValues;
        }

        public async Task<bool> EditProduct(Guid productId, ProductForCreateDTO dto)
        {
            var data = JsonConvert.SerializeObject(dto);

            var polly = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(3, pause => TimeSpan.FromSeconds(5));

            await polly.ExecuteAsync(async () =>
            {
                var response = await Client.PostAsync($"Product/{productId}", new StringContent(data, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
                bool result = JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync());
                return result;
            });
            return false;
        }

        public async Task<ProductKeysValuesToReturnDTO> GetProductKeysAndValues(Guid id)
        {
            var response = await Client.GetAsync($"ProductKeysValues/{id}");
            var productKeyValues = JsonConvert.DeserializeObject<ProductKeysValuesToReturnDTO>
                (await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();
            return productKeyValues;
        }

        public async Task<ProductToReturnDTO> GetProducts(Guid id)
        {
            var response = await Client.GetAsync($"Products/{id}");
            var post = JsonConvert.DeserializeObject<ProductToReturnDTO>
                (await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();
            return post;
        }

        public async Task<ProductToReturnDTO> GetProductsForAdmin(Guid id)
        {
            var response = await Client.GetAsync($"ProductsForAdmin/{id}");
            var post = JsonConvert.DeserializeObject<ProductToReturnDTO>
                (await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();
            return post;
        }

        //TODO
        public void GetSubcategories(List<int> list, CategoryToReturnDTO category)
        {
            throw new NotImplementedException();
        }

        public async Task<List<PriceLogDTO>> ProductPriceLog(Guid productId)
        {
            var response = await Client.GetAsync($"ProductPriceLog/{productId}");
            var post = JsonConvert.DeserializeObject<List<PriceLogDTO>>
                (await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();
            return post;
        }

        public async Task<List<PriceLogDTO>> ProductPriceLogInDateRange(Guid productId)
        {
            var response = await Client.GetAsync($"ProductPriceLogOfWeek/{productId}");
            var post = JsonConvert.DeserializeObject<List<PriceLogDTO>>
                (await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();
            return post;
        }
        //TODO image
        public Task<bool> RemoveImage(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveProduct(Guid productId)
        {
            var response = await Client.DeleteAsync($"Product/{productId}");
            bool result = JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();
            return result;
        }

        public async Task<PagedList<ProductToReturnDTO>> SearchInProducts(PaginationParams<ProductSearch> pagination)
        {
            var response = await Client.GetAsync("Products");
            var posts = JsonConvert.DeserializeObject<PagedList<ProductToReturnDTO>>
                (await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();
            return posts;
        }

        public async Task<PagedList<ProductToReturnDTO>> SearchInProductsForAdmin(PaginationParams<ProductSearch> pagination)
        {
            var response = await Client.GetAsync("ProductsForAdmin");
            var posts = JsonConvert.DeserializeObject<PagedList<ProductToReturnDTO>>
                (await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();
            return posts;
        }

        public async Task<bool> SetProductStatus(Guid productId, Status status)
        {
            var response = await Client.PatchAsync($"Product/{productId}?status={status}", null);
            var result = JsonConvert.DeserializeObject<bool>
                (await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();
            return result;
        }

        public async Task<bool> AddColor(ProductColorDTO color)
        {
            var data = JsonConvert.SerializeObject(color);

            var polly = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(3, pause => TimeSpan.FromSeconds(5));

            bool result = false;
            await polly.ExecuteAsync(async () =>
            {
                var response = await Client.PostAsync("Colors", new StringContent(data, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
                result = JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync());
            });
            return result;
        }

        public async Task<bool> RemoveColor(Guid id)
        {
            var response = await Client.DeleteAsync($"Colors/{id}");
            var result = JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();
            return result;
        }

        public async Task<bool> EditColor(ProductColorDTO color)
        {
            var data = JsonConvert.SerializeObject(color);

            var polly = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(3, pause => TimeSpan.FromSeconds(5));
            bool result = false;
            await polly.ExecuteAsync(async () =>
            {
                var response = await Client.PutAsync("Colors", new StringContent(data, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
                result = JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync());
            });
            return result;
        }

        public async Task<List<ProductColorDTO>> GetColorsList()
        {
            var response = await Client.GetAsync("Colors");
            var posts = JsonConvert.DeserializeObject<List<ProductColorDTO>>(await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();
            return posts;
        }
    }
}
