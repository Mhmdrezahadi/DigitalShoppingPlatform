
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace DSP.ProductService.Data
{
    public class BasketDetail : BaseEntity<Guid>
    {
        public Basket Basket { get; set; }
        public Guid BasketId { get; set; }
        public Product Product { get; set; }
        public Guid ProductId { get; set; }
        public Color Color { get; set; }
        public Guid ColorId { get; set; }
        public int Count { get; set; }
        public decimal Amount { get; set; }
        public double Discount { get; set; }
    }
    public class BasketDetailConfiguartion : IEntityTypeConfiguration<BasketDetail>
    {
        public void Configure(EntityTypeBuilder<BasketDetail> builder)
        {
            builder.Property(p => p.Amount).HasColumnType("decimal(18,2)");

            builder.Property(p => p.CreatedAt).HasDefaultValueSql("getdate()");
            builder.Property(p => p.UpdatedAt).HasDefaultValueSql("getdate()");

            //builder.HasOne(p => p.Basket)
            //    .WithMany(p => p.BasketDetails)
            //    .HasForeignKey(p => p.BasketId)
            //    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
