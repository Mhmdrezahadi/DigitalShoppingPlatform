using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace DSP.ProductService.Data
{
    public class Basket : BaseEntity<Guid>
    {
        public Basket()
        {
            BasketDetails = new HashSet<BasketDetail>();
        }
        public Guid UserId { get; set; }
        public DateTime DT { get; set; }
        public Guid? AddressId { get; set; }
        ///// <summary>
        ///// کد پیگیری 
        ///// </summary>
        //public string Code { get; set; }
        public decimal Price { get; set; }
        public double Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalPrice { get; set; }
        public string Description { get; set; }
        public TimeSpan Due { get; set; }
        public ICollection<BasketDetail> BasketDetails { get; set; }
    }
    public class BasketConfiguartion : IEntityTypeConfiguration<Basket>
    {
        public void Configure(EntityTypeBuilder<Basket> builder)
        {

            builder.Property(p => p.Price).HasColumnType("decimal(18,2)");
            builder.Property(p => p.Tax).HasColumnType("decimal(18,2)");
            builder.Property(p => p.TotalPrice).HasColumnType("decimal(18,2)");

            builder.Property(p => p.CreatedAt).HasDefaultValueSql("getdate()");
            builder.Property(p => p.UpdatedAt).HasDefaultValueSql("getdate()");
        }
    }
}
