

using DSP.ProductService.Data;

namespace DSP.ProductService.Services
{
    public interface ISellService
    {
        Task<Guid> AddDevice(Guid userId, FastPricingForCreateDTO dto);
        Task<List<FastPricingKeysAndDDsToReturnDTO>> FastPricingKeysAndValues(Guid catId);
        Task<FastPricingToReturnDTO> FastPricingValues(FastPricingForCreateDTO dto);
        Task<FastPricingToReturnDTO> MyDevice(Guid userId, Guid id);
        Task<List<FastPricingToReturnDTO>> MyDeviceList(Guid userId);
        Task<bool> SellRequest(SellRequestDTO dto);
        Task<List<SellRequestToReturnDTO>> MySellRequests(Guid userId);
        List<SellRequestStatusCountDTO> SellRequestStatusCount();
        bool RemoveDevice(Guid deviceId, Guid userId);
        bool UpdateDevice(Guid deviceId, Guid userId, FastPricingForCreateDTO dto);
    }
}