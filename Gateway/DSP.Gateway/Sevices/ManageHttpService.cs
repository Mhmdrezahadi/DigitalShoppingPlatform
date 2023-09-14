
using DSP.Gateway.Data;
using DSP.Gateway.Data.Pricing;
using DSP.Gateway.Utilities;
using Newtonsoft.Json;
using Polly;
using System.Collections.Generic;
using System.Text;

namespace DSP.Gateway.Sevices
{
    public class ManageHttpService
    {
        public HttpClient Client { get; }
        public ManageHttpService(HttpClient client)
        {
            client.BaseAddress = new Uri("http://localhost:5301/DSP/ProductService/Manage/");
            Client = client;
        }

        public async Task<bool> AddToPropertyKeys(ProductKeysDefinitionsDTO dto)
        {
            var data = JsonConvert.SerializeObject(dto);

            var response = await Client.PostAsync("AddToPropertyKeys", new StringContent(data, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync());
            return result;
        }

        public Task<List<FAQToReturnDTO>> ArrangeFAQs(List<int> arrangeIds)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ChangeSellRequestStatus(Guid id, SellRequestStatusDTO dto)
        {
            var data = JsonConvert.SerializeObject(dto);

            var response = await Client.PostAsync($"SellRequest/{id}", new StringContent(data, Encoding.UTF8, "application/json"));

            var pricing = JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync());

            response.EnsureSuccessStatusCode();
            return pricing;
        }

        public Task<bool> DefineFastPricingKey(FastPricingDefinitionToCreateDTO dto)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DefinePropertyKes(ProductKeysDefinitionsDTO dto)
        {
            var data = JsonConvert.SerializeObject(dto);

            var response = await Client.PostAsync("DefinePropertyKeys", new StringContent(data, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync());
            return result;
        }

        public async Task<FastPricingToReturnDTO> DeviceInSellRequest(Guid reqId)
        {
            var response = await Client.GetAsync($"DeviceInSellRequest/{reqId}");

            var pricing = JsonConvert.DeserializeObject<FastPricingToReturnDTO>
                (await response.Content.ReadAsStringAsync());

            response.EnsureSuccessStatusCode();
            return pricing;
        }

        public async Task<bool> EditFastPricing(Guid definitionId, FastPricingDefinitionToCreateDTO dto)
        {
            var data = JsonConvert.SerializeObject(dto);

            var response = await Client.PostAsync($"DefineFastPricing/{definitionId}", new StringContent(data, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync());
            return result;
        }

        public async Task<bool> EditPropertyKeys(List<PropertyKeyDTO> list)
        {
            var data = JsonConvert.SerializeObject(list);

            var response = await Client.PostAsync("EditPropertyKeys", new StringContent(data, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync());
            return result;
        }

        public Task<bool> FAQ(FAQToCreateDTO dto)
        {
            throw new NotImplementedException();
        }

        public async Task<FastPricingDefinitionToReturnDTO> FastPricing(Guid id)
        {
            var response = await Client.GetAsync($"FastPricing/{id}");

            var def = JsonConvert.DeserializeObject<FastPricingDefinitionToReturnDTO>
                (await response.Content.ReadAsStringAsync());

            response.EnsureSuccessStatusCode();
            return def;
        }

        public async Task<PagedList<FastPricingDefinitionToReturnDTO>> FastPricingList(PaginationParams<FastPricingSearch> pagination)
        {
            var response = await Client.GetAsync($"FastPricingList");

            var list = JsonConvert.DeserializeObject<PagedList<FastPricingDefinitionToReturnDTO>>
                (await response.Content.ReadAsStringAsync());

            response.EnsureSuccessStatusCode();
            return list;
        }

        public async Task<List<PropertyKeyDTO>> GetPropertyKeys(Guid catId)
        {
            var response = await Client.GetAsync($"PropertyKeys/{catId}");

            var posts = JsonConvert.DeserializeObject<List<PropertyKeyDTO>>
                (await response.Content.ReadAsStringAsync());

            response.EnsureSuccessStatusCode();
            return posts;
        }

        public async Task<TransactionToReturnDTO> GetTransaction(Guid id)
        {
            var response = await Client.GetAsync($"PropertyKeys/{id}");

            var posts = JsonConvert.DeserializeObject<TransactionToReturnDTO>
                (await response.Content.ReadAsStringAsync());

            response.EnsureSuccessStatusCode();
            return posts;
        }

        public async Task<TransactionItemDTO> GetTransactionItems(Guid transactionId)
        {
            var response = await Client.GetAsync($"TransactionItems/{transactionId}");
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<TransactionItemDTO>(await response.Content.ReadAsStringAsync());
            return result;
        }

        public async Task<bool> IsModelDefined(int id)
        {
            var response = await Client.GetAsync($"DefineFastPricing/IsModelDefined/{id}");
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync());
            return result;
        }

        public Task<bool> RemoveFAQ(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveFastPricingDefinition(Guid id)
        {
            var response = await Client.DeleteAsync($"FastPricingDefinition/{id}");
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync());
            return result;
        }

        public async Task<bool> RemovePropertyKeys(Guid id)
        {
            var response = await Client.DeleteAsync($"PropertyKeys/{id}");
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync());
            return result;
        }

        public Task<bool> SecurityAndPrivacy(AppVariableDTO dto)
        {
            throw new NotImplementedException();
        }

        public async Task<SellRequestToReturnDTO> SellRequest(Guid id)
        {
            var response = await Client.GetAsync($"SellRequest/{id}");
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<SellRequestToReturnDTO>(await response.Content.ReadAsStringAsync());
            return result; throw new NotImplementedException();
        }

        public async Task<PagedList<SellRequestToReturnDTO>> SellRequestList(PaginationParams<SellRequestSearch> pagination)
        {
            var response = await Client.GetAsync($"SellRequest");
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<PagedList<SellRequestToReturnDTO>>(await response.Content.ReadAsStringAsync());
            return result;
        }

        public Task<bool> TermsAndCondition(AppVariableDTO dto)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedList<TransactionToReturnDTO>> TransactionList(PaginationParams<TransactionSearch> pagination)
        {
            var response = await Client.GetAsync($"TransactionList");
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<PagedList<TransactionToReturnDTO>>(await response.Content.ReadAsStringAsync());
            return result;
        }

        public Task<bool> UpdateAboutUs(AppVariableDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
