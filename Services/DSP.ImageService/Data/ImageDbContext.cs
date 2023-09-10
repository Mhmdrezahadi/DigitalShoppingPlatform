using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace DSP.ImageService.Data
{
    public class ImageServiceDbContext : DbContext
    {
        public ImageServiceDbContext(DbContextOptions<ImageServiceDbContext> options)
                : base(options)
        {
        }

        public DbSet<Image> Images { get; set; }


    }

    public class Image
    {
        public Guid Id { get; set; }

        public Byte[] Full { get; set; }
        public Byte[] Thumb { get; set; }
        public DateTime TimeCreated { get; set; }

    }
}
