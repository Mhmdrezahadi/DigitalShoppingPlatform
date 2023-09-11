
using DSP.ProductService.Data;
using DSP.ProductService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DSP.Gateway.Controllers
{
    [ApiController]
    [Route("DSP/ProductService")]

    public class CategoriesController : ControllerBase
    {
        #region Configurations
        private readonly ICategoryService _categoryHttpService;
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryHttpService = categoryService;
        }
        #endregion

        #region APIs

        [HttpGet("Categories/RootCategories")]
        public async Task<ActionResult<List<CategoryToReturnDTO>>> RootCategories()
        {
            List<CategoryToReturnDTO> ls = await _categoryHttpService.RootCategories();

            return Ok(ls);
        }

        [HttpGet("Categories/AllBrands")]
        public async Task<ActionResult<List<CategoryToReturnDTO>>> AllBrands()
        {
            List<CategoryToReturnDTO> brands = await _categoryHttpService.AllBrands();

            return Ok(brands);
        }

        [HttpGet("Categories/AllModels")]
        public async Task<ActionResult<List<CategoryToReturnDTO>>> AllModels()
        {
            List<CategoryToReturnDTO> models = await _categoryHttpService.AllModels();

            return Ok(models);
        }

        [HttpGet("Categories/BrandsOfCategory")]
        public async Task<ActionResult<List<CategoryToReturnDTO>>> BrandsOfCategory([FromQuery] Guid rootId)
        {
            List<CategoryToReturnDTO> ls = await _categoryHttpService.BrandsOfCategory(rootId);

            return Ok(ls);
        }

        [HttpGet("Categories/ModelsOfBrand")]
        public async Task<ActionResult<List<CategoryToReturnDTO>>> ModelsOfBrand([FromQuery] Guid brandCategoryid)
        {
            List<CategoryToReturnDTO> ls = await _categoryHttpService.ModelsOfBrand(brandCategoryid);

            return Ok(ls);
        }

        [HttpPost("Category")]
        public async Task<ActionResult<Guid>> AddCategory(
            [Required][FromForm] string Name,
            [FromForm] Guid? parentCategoryId,
            [Required] IFormFile Image)
        {
            CategoryForSetDTO dto = new CategoryForSetDTO
            {
                Name = Name,
                ParentCategoryId = parentCategoryId,
                Img = Image
            };
            var res = await _categoryHttpService.AddCategory(dto);

            return Ok(res);
        }

        [HttpDelete("Category/{id}")]
        public async Task<ActionResult<bool>> DeleteCategory(Guid id)
        {
            bool res = await _categoryHttpService.DeleteCategory(id);

            return Ok(res);
        }

        [HttpPut("Category/{catId}")]
        public async Task<ActionResult<bool>> UpdateCategory(
            Guid catId,
            [FromForm] string Name,
            [FromForm] Guid? parentCategoryId,
            IFormFile Image)
        {

            CategoryForSetDTO dto = new CategoryForSetDTO
            {
                Img = Image,
                Name = Name,
                ParentCategoryId = parentCategoryId
            };
            bool res = await _categoryHttpService.UpdateCategory(catId, dto);

            return Ok(res);
        }

        [HttpPut("Category/Arrange")]
        public async Task<ActionResult<List<CategoryToReturnDTO>>> CategoryArrange(Guid? parentId, List<int> arrangeIds)
        {
            List<CategoryToReturnDTO> ls = await _categoryHttpService.CategoryArrange(parentId, arrangeIds);

            return Ok(ls);
        }

        [HttpGet("Category/{id}/SubCategories")]
        public async Task<ActionResult<List<CategoryToReturnDTO>>> GetSubCategories(Guid id)
        {
            List<CategoryToReturnDTO> ls = await _categoryHttpService.GetSubCategories(id);

            return Ok(ls);
        }
        [HttpGet("Category/{id}/ParentCategories")]
        public async Task<ActionResult<List<CategoryToReturnDTO>>> GetParentCategories(Guid id)
        {
            List<CategoryToReturnDTO> ls = await _categoryHttpService.GetParentCategories(id);

            return Ok(ls);
        }

        [HttpGet("Category/CategoriesWithBrands")]
        public ActionResult<List<CategoryWithBrandDTO>> GetCategoriesWithBrands()
        {
            List<CategoryWithBrandDTO> ls = _categoryHttpService.GetCategoriesWithBrands();

            return Ok(ls);
        }
        #endregion

    }
}
