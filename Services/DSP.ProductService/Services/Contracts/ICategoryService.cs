using DSP.ProductService.Data;

namespace DSP.ProductService.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryToReturnDTO>> RootCategories();
        Task<List<CategoryToReturnDTO>> BrandsOfCategory(Guid rootId);
        Task<List<CategoryToReturnDTO>> AllBrands();
        Task<List<CategoryToReturnDTO>> ModelsOfBrand(Guid brandCategoryid);
        Task<List<Guid>> MyPathToLeaf(List<Guid> path, Guid rootId, bool isVerified);
        Task<Guid?> AddCategory(CategoryForSetDTO dto);
        Task<bool> DeleteCategory(Guid id);
        Task<Guid?> UpdateCategory(Guid catId, CategoryForSetDTO dto);
        Task<List<CategoryToReturnDTO>> AllModels();
        Task<List<CategoryToReturnDTO>> CategoryArrange(Guid? parentId, List<int> arrangeIds);
        Task<List<CategoryToReturnDTO>> GetSubCategories(Guid id);
        Task<List<CategoryToReturnDTO>> GetParentCategories(Guid id);
        List<CategoryWithBrandDTO> GetCategoriesWithBrands();
    }
}
