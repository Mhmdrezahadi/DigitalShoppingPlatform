
namespace DSP.ProductService.Data
{
    public class CategoryForSetDTO
    {
        public IFormFile Img { get; set; }

        public string Name { get; set; }

        public int? ParentCategoryId { get; set; }
    }
}
