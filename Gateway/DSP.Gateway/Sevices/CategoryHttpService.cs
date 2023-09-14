
using DSP.Gateway.Data;
using DSP.Gateway.Utilities;
using DSP.ImageService.Protos;
using Grpc.Net.Client;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Polly;
using System.Net.Mime;
using System.Text;

namespace DSP.Gateway.Sevices
{
    public class CategoryHttpService
    {
        public HttpClient Client { get; }
        public CategoryHttpService(HttpClient client)
        {
            client.BaseAddress = new Uri("http://localhost:5301/DSP/ProductService/");
            Client = client;
        }
        //TODO Fix ImageService
        public async Task<bool> AddCategory(CategoryForSetDTO dto)
        {
            var data = JsonConvert.SerializeObject(dto);

            var response = await Client.PostAsync("Category", new StringContent(data, Encoding.UTF8, MediaTypeNames.Application.Json));
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<Guid?>(await response.Content.ReadAsStringAsync());

            if (result.HasValue)
            {
                //var polly = Policy.Handle<Exception>()
                //        .CircuitBreakerAsync(2, TimeSpan.FromSeconds(20));

                //await polly.ExecuteAsync(async () =>
                //{
                //});
                using var channel = GrpcChannel.ForAddress("https://localhost:5302");
                var uploadFileClient = new UploadFileService.UploadFileServiceClient(channel);
                await UploadImage.SendFile(uploadFileClient, dto.Img, result.ToString());
                return true;
            }
            return false;
        }

        public async Task<List<CategoryToReturnDTO>> AllBrands()
        {
            var response = await Client.GetAsync($"Categories/AllBrands");
            var allBrands = JsonConvert.DeserializeObject<List<CategoryToReturnDTO>>(await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();
            return allBrands;
        }

        public async Task<List<CategoryToReturnDTO>> AllModels()
        {
            var response = await Client.GetAsync($"Categories/AllModels");
            var allModels = JsonConvert.DeserializeObject<List<CategoryToReturnDTO>>(await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();
            return allModels;
        }

        public async Task<List<CategoryToReturnDTO>> BrandsOfCategory(int rootId)
        {
            var response = await Client.GetAsync($"Categories/BrandsOfCategory?rootId={rootId}");
            var brandsOfCategory = JsonConvert.DeserializeObject<List<CategoryToReturnDTO>>(await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();
            return brandsOfCategory;
        }

        public async Task<List<CategoryToReturnDTO>> CategoryArrange(int? parentId, List<int> arrangeIds)
        {
            var data = JsonConvert.SerializeObject(arrangeIds);

            var polly = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(3, pause => TimeSpan.FromSeconds(5));

            await polly.ExecuteAsync(async () =>
            {
                var response = await Client.PutAsync($"Category/Arrange?parentId={parentId}", new StringContent(data, Encoding.UTF8, MediaTypeNames.Application.Json));
                response.EnsureSuccessStatusCode();
                var result = JsonConvert.DeserializeObject<List<CategoryToReturnDTO>>(await response.Content.ReadAsStringAsync());
                return result;
            });
            return null;
        }

        public async Task<bool> DeleteCategory(int id)
        {
            var response = await Client.DeleteAsync($"Category/{id}");
            var result = JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();
            return result;
        }

        public Task FirstRunAfterBoot()
        {
            throw new NotImplementedException();
        }

        public Task<List<int>> FlattenedTree(int CategoryId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<CategoryWithBrandDTO>> GetCategoriesWithBrands()
        {
            var response = await Client.GetAsync($"Category/CategoriesWithBrands");
            var categoryWithBrands = JsonConvert.DeserializeObject<List<CategoryWithBrandDTO>>(await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();
            return categoryWithBrands;
        }

        public async Task<List<CategoryToReturnDTO>> GetParentCategories(int id)
        {

            var response = await Client.GetAsync($"Category/{id}/ParentCategories");
            var parentCategories = JsonConvert.DeserializeObject<List<CategoryToReturnDTO>>(await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();
            return parentCategories;
        }

        public Task<List<int>> GetRootNodesIds()
        {
            throw new NotImplementedException();
        }

        public async Task<List<CategoryToReturnDTO>> GetSubCategories(int id)
        {
            var response = await Client.GetAsync($"Category/{id}/SubCategories");
            var subCategories = JsonConvert.DeserializeObject<List<CategoryToReturnDTO>>(await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();
            return subCategories;
        }

        public List<int> GetTreeFromCache(int CategoryId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<CategoryToReturnDTO>> ModelsOfBrand(int brandCategoryid)
        {
            var response = await Client.GetAsync($"Categories/ModelsOfBrand?brandCategoryid={brandCategoryid}");
            var modelsOfBrand = JsonConvert.DeserializeObject<List<CategoryToReturnDTO>>(await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();
            return modelsOfBrand;
        }

        public Task<List<int>> MyPathToLeaf(List<int> path, int rootId, bool isVerified)
        {
            throw new NotImplementedException();
        }

        public Task<List<int>> PathToLeaf(int rootId)
        {
            throw new NotImplementedException();
        }

        public Task<List<int>> PathToRoot(int categoryId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<CategoryToReturnDTO>> RootCategories()
        {
            var response = await Client.GetAsync($"Categories/RootCategories");
            var rootCategories = JsonConvert.DeserializeObject<List<CategoryToReturnDTO>>(await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();
            return rootCategories;
        }
        // TODO Fix ImageService
        public Task<bool> UpdateCategory(int catId, CategoryForSetDTO category)
        {
            throw new NotImplementedException();
        }
    }
}
