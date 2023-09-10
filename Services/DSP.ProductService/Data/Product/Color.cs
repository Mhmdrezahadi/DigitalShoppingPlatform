using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace DSP.ProductService.Data
{
    public class Color : BaseEntity<Guid>
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsVerified { get; set; } = true;
        public ICollection<ProductDetail> ProductDetails { get; set; }
    }
    public class ColorConfiguartion : IEntityTypeConfiguration<Color>
    {
        public void Configure(EntityTypeBuilder<Color> builder)
        {
            builder.Property(p => p.CreatedAt).HasDefaultValueSql("getdate()");
            builder.Property(p => p.UpdatedAt).HasDefaultValueSql("getdate()");

            builder.Property(p => p.IsVerified).HasDefaultValue(true);
        }
    }
}
