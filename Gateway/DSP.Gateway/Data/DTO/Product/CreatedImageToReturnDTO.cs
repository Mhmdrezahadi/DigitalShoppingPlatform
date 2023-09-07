using System;

namespace DSP.Gateway.Data
{
    public class CreatedImageToReturnDTO
    {
        public Guid Id { get; set; }
        public string ImageUrl_L { get; set; }
        public string ImageUrl_M { get; set; }
        public string ImageUrl_S { get; set; }
    }
}
