﻿using System;
using System.ComponentModel.DataAnnotations;

namespace DSP.ProductService.Data
{
    public class FastPricingValueToSetDTO
    {
        [Required]
        public Guid FastPricingDDId { get; set; }
        [Required]
        public Guid FastPricingKeyId { get; set; }
    }
}
