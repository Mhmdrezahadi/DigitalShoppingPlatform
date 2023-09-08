using System;
using System.ComponentModel.DataAnnotations;

namespace DSP.ProductService.Data
{
    public class ProductColorDTO
    {
        public Guid? Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }
    }
}
