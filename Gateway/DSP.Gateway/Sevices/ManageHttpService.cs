
using DSP.Gateway.Data;
using DSP.Gateway.Data.Pricing;
using DSP.Gateway.Utilities;

namespace DSP.Gateway.Sevices
{
    public class ManageHttpService 
    {
        public HttpClient Client { get; }
        public ManageHttpService(HttpClient client)
        {
            client.BaseAddress = new Uri("Manage Base API Adress");
            Client = client;
        }

        public Task<bool> AddToPropertyKeys(ProductKeysDefinitionsDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<List<FAQToReturnDTO>> ArrangeFAQs(List<int> arrangeIds)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ChangeSellRequestStatus(Guid id, SellRequestStatusDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DefineFastPricingKey(FastPricingDefinitionToCreateDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DefinePropertyKes(ProductKeysDefinitionsDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<FastPricingToReturnDTO> DeviceInSellRequest(Guid reqId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EditFastPricing(Guid definitionId, FastPricingDefinitionToCreateDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EditPropertyKeys(List<PropertyKeyDTO> list)
        {
            throw new NotImplementedException();
        }

        public Task<bool> FAQ(FAQToCreateDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<FastPricingDefinitionToReturnDTO> FastPricing(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<FastPricingDefinitionToReturnDTO>> FastPricingList(PaginationParams<FastPricingSearch> pagination)
        {
            throw new NotImplementedException();
        }

        public Task<List<PropertyKeyDTO>> GetPropertyKeys(int catId)
        {
            throw new NotImplementedException();
        }

        public TransactionToReturnDTO GetTransaction(Guid id)
        {
            throw new NotImplementedException();
        }

        public TransactionItemDTO GetTransactionItems(Guid transactionId)
        {
            throw new NotImplementedException();
        }

        public bool IsModelDefined(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveFAQ(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool RemoveFastPricingDefinition(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemovePropertyKeys(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SecurityAndPrivacy(AppVariableDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<SellRequestToReturnDTO> SellRequest(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<SellRequestToReturnDTO>> SellRequestList(PaginationParams<SellRequestSearch> pagination)
        {
            throw new NotImplementedException();
        }

        public Task<bool> TermsAndCondition(AppVariableDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<TransactionToReturnDTO>> TransactionList(PaginationParams<TransactionSearch> pagination)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAboutUs(AppVariableDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
