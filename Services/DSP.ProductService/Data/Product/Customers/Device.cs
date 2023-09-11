using DSP.ProductService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace DSP.ProductService.Data
{
    public class Device : BaseEntity<Guid>
    {
        public Category Category { get; set; }
        public Guid CategoryId { get; set; }
        public bool IsPriced { get; set; } = false;
        public SellRequest SellRequest { get; set; }
        public ExchangeRequest ExchangeRequest { get; set; }
        public RepairRequest RepairRequest { get; set; }
        public Guid UserId { get; set; }
        public ExactPricingValue ExactPricingValue { get; set; }
        public ICollection<FastPricingValue> FastPricingValues { get; set; }
        public bool IsVerified { get; set; }

    }
    public class DeviceConfiguartion : IEntityTypeConfiguration<Device>
    {
        public void Configure(EntityTypeBuilder<Device> builder)
        {
            builder.Property(p => p.CreatedAt).HasDefaultValueSql("getdate()");
            builder.Property(p => p.UpdatedAt).HasDefaultValueSql("getdate()");

            builder.Property(p => p.IsVerified).HasDefaultValue(true);
        }
    }
}
