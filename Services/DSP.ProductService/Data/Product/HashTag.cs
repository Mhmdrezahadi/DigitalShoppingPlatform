using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace DSP.ProductService.Data
{
    public class HashTag : BaseEntity<Guid>
    {
        public string Text { get; set; }
    }
    public class HashTagConfiguartion : IEntityTypeConfiguration<HashTag>
    {
        public void Configure(EntityTypeBuilder<HashTag> builder)
        {
            builder.Property(p => p.CreatedAt).HasDefaultValueSql("getdate()");
            builder.Property(p => p.UpdatedAt).HasDefaultValueSql("getdate()");
        }
    }
}
