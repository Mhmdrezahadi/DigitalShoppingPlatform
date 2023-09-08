using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace DSP.ProductService.Data
{
    public class ProductDetail : BaseEntity<Guid>
    {
        public Color Color { get; set; }
        public Guid ColorId { get; set; }
        public Product Product { get; set; }
        public Guid ProductId { get; set; }
        //public decimal Price { get; set; }
        //public double Discount { get; set; }
    }
    public class ProductDetailConfiguartion : IEntityTypeConfiguration<ProductDetail>
    {
        public void Configure(EntityTypeBuilder<ProductDetail> builder)
        {
            builder.Property(p => p.CreatedAt).HasDefaultValueSql("getdate()");
            builder.Property(p => p.UpdatedAt).HasDefaultValueSql("getdate()");
        }
    }
}
