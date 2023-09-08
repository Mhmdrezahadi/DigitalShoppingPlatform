using DSP.ProductService.Data;

namespace DSP.ProductService.Services
{
    public interface ICategoryService
    {
        Task<List<int>> FlattenedTree(int CategoryId);
        Task<List<int>> PathToRoot(int categoryId);
        Task<List<int>> GetRootNodesIds();
        Task FirstRunAfterBoot();
        List<int> GetTreeFromCache(int CategoryId);
        Task<List<CategoryToReturnDTO>> RootCategories();
        Task<List<CategoryToReturnDTO>> BrandsOfCategory(int rootId);
        Task<List<CategoryToReturnDTO>> AllBrands();
        Task<List<CategoryToReturnDTO>> ModelsOfBrand(int brandCategoryid);
        Task<List<int>> PathToLeaf(int rootId);
        Task<List<int>> MyPathToLeaf(List<int> path, int rootId, bool isVerified);
        Task<bool> AddCategory(CategoryForSetDTO dto);
        Task<bool> DeleteCategory(int id);
        Task<bool> UpdateCategory(int catId, CategoryForSetDTO category);
        Task<List<CategoryToReturnDTO>> AllModels();
        Task<List<CategoryToReturnDTO>> CategoryArrange(int? parentId, List<int> arrangeIds);
        Task<List<CategoryToReturnDTO>> GetSubCategories(int id);
        Task<List<CategoryToReturnDTO>> GetParentCategories(int id);
        List<CategoryWithBrandDTO> GetCategoriesWithBrands();
    }
}
