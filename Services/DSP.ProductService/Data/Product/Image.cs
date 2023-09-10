﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace DSP.ProductService.Data
{
    public class Image : BaseEntity<Guid>
    {
        public string ImageUrl_L { get; set; }
        public string ImageUrl_M { get; set; }
        public string ImageUrl_S { get; set; }
        public Product Product { get; set; }
        public Guid? ProductId { get; set; }
    }
    public class ImageConfiguartion : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.CreatedAt).HasDefaultValueSql("getdate()");
            builder.Property(p => p.UpdatedAt).HasDefaultValueSql("getdate()");
        }

    }
}
