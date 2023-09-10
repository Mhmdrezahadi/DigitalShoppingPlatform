
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace DSP.ProductService.Data
{
    public class SellRequest : BaseEntity<Guid>
    {
        public DateTime DT { get; set; }
        public Device Device { get; set; }
        public Guid AddressId { get; set; }
        public string Code { get; set; }
        public string StatusDescription { get; set; }
        public SellRequestStatus SellRequestStatus { get; set; }
        public decimal AgreedPrice { get; set; }
    }

    public class SellRequestConfiguartion : IEntityTypeConfiguration<SellRequest>
    {
        public void Configure(EntityTypeBuilder<SellRequest> builder)
        {
            builder.HasKey(p => p.Id);

            builder.HasOne(p => p.Device)
                .WithOne(p => p.SellRequest)
                .HasForeignKey<SellRequest>(p => p.Id);

            //builder.HasOne(p => p.Address)
            //    .WithOne(p => p.SellRequest)
            //    .HasForeignKey<SellRequest>(p => p.AddressId);

            builder.Property(p => p.SellRequestStatus)
                .HasConversion(
                e => e.ToString(),
                s => Enum.Parse<SellRequestStatus>(s));

            builder.Property(p => p.AgreedPrice).HasColumnType("decimal(18,2)");

            builder.Property(p => p.CreatedAt).HasDefaultValueSql("getdate()");
            builder.Property(p => p.UpdatedAt).HasDefaultValueSql("getdate()");
        }
    }
}
