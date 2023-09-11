using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSP.ProductService.Data
{
    public class Category : BaseEntity<Guid>
    {
        [Required]
        public string Name { get; set; }
        public int Level { get; set; }
        public string ImageUrl_L { get; set; }
        public string ImageUrl_M { get; set; }
        public string ImageUrl_S { get; set; }
        public Guid? ParentCategoryId { get; set; }
        [ForeignKey(nameof(ParentCategoryId))]
        public Category ParentCategory { get; set; }
        public ICollection<Category> ChildCategories { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<PropertyKey> PropertyKeys { get; set; }
        public FastPricingDefinition FastPricingDefinition { get; set; }
        public int Arrange { get; set; }
        public bool IsVerified { get; set; } = true;
    }
    public class CategoryConfiguartion : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired();

            builder.Property(p => p.CreatedAt).HasDefaultValueSql("getdate()");
            builder.Property(p => p.UpdatedAt).HasDefaultValueSql("getdate()");

            builder.Property(p => p.IsVerified).HasDefaultValue(true);
        }

    }
}
