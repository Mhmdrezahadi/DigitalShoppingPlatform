

using Microsoft.OpenApi.Models;

namespace DSP.Gateway.Data
{
    public class FastPricingDDsToReturnDTO
    {
        public Guid Id { get; set; }
        public string Label { get; set; }
        public double? MinRate { get; set; }
        public double? MaxRate { get; set; }
        public string ErrorTitle { get; set; }
        public string ErrorDiscription { get; set; }
        public OperationType OperationType { get; set; }
    }
}
