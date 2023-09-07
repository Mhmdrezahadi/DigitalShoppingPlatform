using System;

namespace DSP.Gateway.Data
{
    public class FAQToReturnDTO
    {
        public Guid Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public int ArrnageId { get; set; }
    }
}
