using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace DSP.ProductService.Data
{
    public class ExactPricingKey : BaseEntity<Guid>
    {
        public string Name { get; set; }
        public string ExactPricingKeyType { get; set; }
    }
    public class ExactPricingKeyConfiguration : IEntityTypeConfiguration<ExactPricingKey>
    {
        public void Configure(EntityTypeBuilder<ExactPricingKey> builder)
        {
            builder.Property(p => p.CreatedAt).HasDefaultValueSql("getdate()");
            builder.Property(p => p.UpdatedAt).HasDefaultValueSql("getdate()");
        }
    }
}
