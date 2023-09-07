
using DSP.Gateway.Data;
using DSP.Gateway.Utilities;

namespace DSP.Gateway.Sevices
{
    public class ProductHttpService
    {
        public HttpClient Client { get; }

        public ProductHttpService(HttpClient client)
        {
            client.BaseAddress = new Uri("Product Base API Adress");
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

        public Task<ProductToReturnDTO> GetProducts(Guid id)
        {
            throw new NotImplementedException();
        }

        public ProductToReturnDTO GetProductsForAdmin(Guid id)
        {
            throw new NotImplementedException();
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

        public Task<PagedList<ProductToReturnDTO>> SearchInProducts(PaginationParams<ProductSearch> pagination)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<ProductToReturnDTO>> SearchInProductsForAdmin(PaginationParams<ProductSearch> pagination)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetProductStatus(Guid productId, Status status)
        {
            throw new NotImplementedException();
        }
    }
}
