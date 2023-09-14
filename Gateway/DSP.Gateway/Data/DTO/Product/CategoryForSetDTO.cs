
namespace DSP.Gateway.Data
{
    public class CategoryForSetDTO
    {
        public IFormFile Img { get; set; }

        public string Name { get; set; }

        public Guid? ParentCategoryId { get; set; }
    }
}
