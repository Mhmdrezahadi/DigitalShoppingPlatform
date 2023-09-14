
using DSP.Gateway.Data;
using DSP.Gateway.Entities;
using Newtonsoft.Json;
using Polly;
using System.Text;

namespace DSP.Gateway.Sevices
{
    public class SellHttpService
    {
        public HttpClient Client { get; }
        public SellHttpService(HttpClient client)
        {
            client.BaseAddress = new Uri("http://localhost:5301/DSP/ProductService/Sell/");
            Client = client;
        }
        public async Task<Guid> AddDevice(Guid userId, FastPricingForCreateDTO dto)
        {
            var data = JsonConvert.SerializeObject(dto);

            var response = await Client.PostAsync("Device/{userId}", new StringContent(data, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<Guid>(await response.Content.ReadAsStringAsync());
            return result;
        }

        public async Task<List<FastPricingKeysAndDDsToReturnDTO>> FastPricingKeysAndValues(Guid catId)
        {
            var response = await Client.GetAsync($"FastPricingKeysAndValues/{catId}");
            response.EnsureSuccessStatusCode();
            var pricing = JsonConvert.DeserializeObject<List<FastPricingKeysAndDDsToReturnDTO>>(await response.Content.ReadAsStringAsync());
            return pricing;
        }

        public async Task<FastPricingToReturnDTO> FastPricingValues(FastPricingForCreateDTO dto)
        {
            var data = JsonConvert.SerializeObject(dto);

            var response = await Client.PostAsync("FastPricingValues", new StringContent(data, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            var pricing = JsonConvert.DeserializeObject<FastPricingToReturnDTO>(await response.Content.ReadAsStringAsync());
            return pricing;
        }

        public async Task<FastPricingToReturnDTO> MyDevice(Guid userId, Guid id)
        {
            var response = await Client.GetAsync($"MyDevice/{userId}/{id}");
            response.EnsureSuccessStatusCode();
            var pricing = JsonConvert.DeserializeObject<FastPricingToReturnDTO>(await response.Content.ReadAsStringAsync());
            return pricing;
        }

        public async Task<List<FastPricingToReturnDTO>> MyDeviceList(Guid userId)
        {
            var response = await Client.GetAsync($"MyDeviceList/{userId}");
            response.EnsureSuccessStatusCode();
            var deviceList = JsonConvert.DeserializeObject<List<FastPricingToReturnDTO>>(await response.Content.ReadAsStringAsync());
            return deviceList;
        }

        public async Task<List<SellRequestToReturnDTO>> MySellRequests(Guid userId)
        {
            var response = await Client.GetAsync($"MySellRequests/{userId}");
            response.EnsureSuccessStatusCode();
            var sellRequests = JsonConvert.DeserializeObject<List<SellRequestToReturnDTO>>(await response.Content.ReadAsStringAsync());
            return sellRequests;
        }

        public async Task<bool> RemoveDevice(Guid deviceId, Guid userId)
        {
            var response = await Client.DeleteAsync($"MyDeviceList/{userId}/{deviceId}");
            response.EnsureSuccessStatusCode();
            var deviceList = JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync());
            return deviceList;
        }

        public async Task<bool> SellRequest(SellRequestDTO dto)
        {
            var data = JsonConvert.SerializeObject(dto);

            var response = await Client.PostAsync("SellRequest", new StringContent(data, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            var deviceList = JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync());
            return deviceList;
        }

        public async Task<List<SellRequestStatusCountDTO>> SellRequestStatusCount()
        {
            var response = await Client.GetAsync($"SellRequestStatusCount");
            response.EnsureSuccessStatusCode();
            var count = JsonConvert.DeserializeObject<List<SellRequestStatusCountDTO>>(await response.Content.ReadAsStringAsync());
            return count;
        }

        public async Task<bool> UpdateDevice(Guid deviceId, Guid userId, FastPricingForCreateDTO dto)
        {
            var data = JsonConvert.SerializeObject(dto);

            var response = await Client.PutAsync("Device/{userId}/{deviceId}", new StringContent(data, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            var deviceList = JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync());
            return deviceList;
        }
    }
}
