
using DSP.Gateway.Data;

namespace DSP.Gateway.Sevices
{
    public class SellHttpService
    {
        public HttpClient Client { get; }
        public SellHttpService(HttpClient client)
        {
            client.BaseAddress = new Uri("Sell Base API Adress");
            Client = client;
        }
        public Task<Guid> AddDevice(Guid userId, FastPricingForCreateDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<List<FastPricingKeysAndDDsToReturnDTO>> FastPricingKeysAndValues(int catId)
        {
            throw new NotImplementedException();
        }

        public Task<FastPricingToReturnDTO> FastPricingValues(FastPricingForCreateDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<FastPricingToReturnDTO> MyDevice(Guid userId, Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<FastPricingToReturnDTO>> MyDeviceList(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<SellRequestToReturnDTO>> MySellRequests(Guid userId)
        {
            throw new NotImplementedException();
        }

        public bool RemoveDevice(Guid deviceId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SellRequest(SellRequestDTO dto)
        {
            throw new NotImplementedException();
        }

        public List<SellRequestStatusCountDTO> SellRequestStatusCount()
        {
            throw new NotImplementedException();
        }

        public bool UpdateDevice(Guid deviceId, Guid userId, FastPricingForCreateDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
