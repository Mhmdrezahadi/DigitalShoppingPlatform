﻿using System;

namespace DSP.ProductService.Data
{
    public class ProductImageToReturnDTO
    {
        public Guid Id { get; set; }
        public string ImageUrl_L { get; set; }
        public string ImageUrl_M { get; set; }
        public string ImageUrl_S { get; set; }
    }
}
