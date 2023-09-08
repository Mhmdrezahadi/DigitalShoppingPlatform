using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace DSP.ProductService.Data
{
    public class Warranty : BaseEntity
    {
        public string Title { get; set; }
        public int Months { get; set; }
        public string Info { get; set; }
        public ICollection<Product> Products { get; set; }
    }
    public class WarrantyConfiguartion : IEntityTypeConfiguration<Warranty>
    {
        public void Configure(EntityTypeBuilder<Warranty> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.CreatedAt).HasDefaultValueSql("getdate()");
            builder.Property(p => p.UpdatedAt).HasDefaultValueSql("getdate()");
        }

    }
}
