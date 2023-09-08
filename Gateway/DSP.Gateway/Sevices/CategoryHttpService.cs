
using DSP.Gateway.Data;
using Microsoft.Extensions.Caching.Memory;

namespace DSP.Gateway.Sevices
{
    public class CategoryHttpService
    {
        public HttpClient Client { get; }
        public CategoryHttpService(HttpClient client) 
        {
            client.BaseAddress = new Uri("Category Base API Adress");
            Client = client;
        }
        public Task<bool> AddCategory(CategoryForSetDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<List<CategoryToReturnDTO>> AllBrands()
        {
            throw new NotImplementedException();
        }

        public Task<List<CategoryToReturnDTO>> AllModels()
        {
            throw new NotImplementedException();
        }

        public Task<List<CategoryToReturnDTO>> BrandsOfCategory(int rootId)
        {
            throw new NotImplementedException();
        }

        public Task<List<CategoryToReturnDTO>> CategoryArrange(int? parentId, List<int> arrangeIds)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteCategory(int id)
        {
            throw new NotImplementedException();
        }

        public Task FirstRunAfterBoot()
        {
            throw new NotImplementedException();
        }

        public Task<List<int>> FlattenedTree(int CategoryId)
        {
            throw new NotImplementedException();
        }

        public List<CategoryWithBrandDTO> GetCategoriesWithBrands()
        {
            throw new NotImplementedException();
        }

        public Task<List<CategoryToReturnDTO>> GetParentCategories(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<int>> GetRootNodesIds()
        {
            throw new NotImplementedException();
        }

        public Task<List<CategoryToReturnDTO>> GetSubCategories(int id)
        {
            throw new NotImplementedException();
        }

        public List<int> GetTreeFromCache(int CategoryId)
        {
            throw new NotImplementedException();
        }

        public Task<List<CategoryToReturnDTO>> ModelsOfBrand(int brandCategoryid)
        {
            throw new NotImplementedException();
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

        public Task<List<CategoryToReturnDTO>> RootCategories()
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateCategory(int catId, CategoryForSetDTO category)
        {
            throw new NotImplementedException();
        }
    }
}
