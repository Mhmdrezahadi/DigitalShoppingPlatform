
using DSP.Gateway.Data;
using DSP.Gateway.Utilities;
using Newtonsoft.Json;

namespace DSP.Gateway.Sevices
{
    public class ProductHttpService
    {
        public HttpClient Client { get; }

        public ProductHttpService(HttpClient client)
        {
            client.BaseAddress = new Uri("http://localhost:5301/api");
            Client = client;

        }
        public Task<CreatedImageToReturnDTO> AddImage(IFormFile image)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> AddProduct(ProductForCreateDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProductToReturnDTO>> CompareTwoProduct(List<Guid> productIds)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EditProduct(Guid productid, ProductForCreateDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<ProductKeysValuesToReturnDTO> GetProductKeysAndValues(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<ProductToReturnDTO> GetProducts(Guid id)
        {
            var response = await Client.GetAsync("ProductsForAdmin");
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

        public void GetSubcategories(List<int> list, CategoryToReturnDTO category)
        {
            throw new NotImplementedException();
        }

        public Task<List<PriceLogDTO>> ProductPriceLog(Guid productId)
        {
            throw new NotImplementedException();
        }

        public Task<List<PriceLogDTO>> ProductPriceLogInDateRange(Guid productId, DateTime fromDT, DateTime toDT)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveImage(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveProduct(Guid productId)
        {
            throw new NotImplementedException();
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
            var response = await Client.GetAsync("Products");
            var posts = JsonConvert.DeserializeObject<PagedList<ProductToReturnDTO>>
                (await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();
            return posts;
        }

        public Task<bool> SetProductStatus(Guid productId, Status status)
        {
            throw new NotImplementedException();
        }
    }
}
