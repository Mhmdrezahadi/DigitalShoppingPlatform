using System;
using System.ComponentModel.DataAnnotations;

namespace DSP.Gateway.Data
{
    public class ColorDTO
    {
        public Guid DetailId { get; set; }
        public Guid? Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }
    }
}
