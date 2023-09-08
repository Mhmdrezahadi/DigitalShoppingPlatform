﻿
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace DSP.ProductService.Data
{
    public class OrderDetail : BaseEntity<Guid>
    {
        public Order Order { get; set; }
        public Guid OrderId { get; set; }
        public Product Product { get; set; }
        public Guid ProductId { get; set; }
        public Color Color { get; set; }
        public Guid ColorId { get; set; }
        public int Count { get; set; }
        public decimal Amount { get; set; }
        public double Discount { get; set; }

    }
    public class OrderDetailsConfiguartion : IEntityTypeConfiguration<OrderDetail>
    {
        public void Configure(EntityTypeBuilder<OrderDetail> builder)
        {
            builder.Property(p => p.Amount).HasColumnType("decimal(18,2)");

            builder.Property(p => p.CreatedAt).HasDefaultValueSql("getdate()");
            builder.Property(p => p.UpdatedAt).HasDefaultValueSql("getdate()");
        }
    }
}
