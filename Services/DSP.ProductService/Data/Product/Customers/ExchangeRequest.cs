using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace DSP.ProductService.Data
{
    public class ExchangeRequest : BaseEntity<Guid>
    {
        public string Description { get; set; }
        public DateTime Time { get; set; }
        public Device Device { get; set; }
    }
    public class ExchangeRequestConfiguartion : IEntityTypeConfiguration<ExchangeRequest>
    {
        public void Configure(EntityTypeBuilder<ExchangeRequest> builder)
        {
            builder.HasKey(p => p.Id);
            builder.HasOne(p => p.Device)
                .WithOne(p => p.ExchangeRequest)
                .HasForeignKey<ExchangeRequest>(p => p.Id);

            builder.Property(p => p.CreatedAt).HasDefaultValueSql("getdate()");
            builder.Property(p => p.UpdatedAt).HasDefaultValueSql("getdate()");
        }
    }
}
