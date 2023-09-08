using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace DSP.ProductService.Data
{
    public class RepairRequest : BaseEntity<Guid>
    {
        public string Description { get; set; }
        public DateTime Time { get; set; }
        public Device Device { get; set; }
    }
    public class RepairRequestConfiguartion : IEntityTypeConfiguration<RepairRequest>
    {
        public void Configure(EntityTypeBuilder<RepairRequest> builder)
        {
            builder.HasKey(p => p.Id);
            builder.HasOne(p => p.Device)
                .WithOne(p => p.RepairRequest)
                .HasForeignKey<RepairRequest>(p => p.Id);

            builder.Property(p => p.CreatedAt).HasDefaultValueSql("getdate()");
            builder.Property(p => p.UpdatedAt).HasDefaultValueSql("getdate()");
        }
    }
}
