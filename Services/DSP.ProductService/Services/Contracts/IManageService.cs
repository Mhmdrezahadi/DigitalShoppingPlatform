
using DSP.ProductService.Data;
using DSP.ProductService.Utilities;

namespace DSP.ProductService.Services
{
    public interface IManageService
    {
        Task<bool> DefinePropertyKes(ProductKeysDefinitionsDTO dto);
        Task<bool> AddToPropertyKeys(ProductKeysDefinitionsDTO dto);
        Task<bool> EditPropertyKeys(List<PropertyKeyDTO> list);
        Task<bool> RemovePropertyKeys(Guid id);
        Task<List<PropertyKeyDTO>> GetPropertyKeys(Guid catId);
        Task<bool> DefineFastPricingKey(FastPricingDefinitionToCreateDTO dto);
        Task<PagedList<FastPricingDefinitionToReturnDTO>> FastPricingList(PaginationParams<FastPricingSearch> pagination);
        Task<FastPricingDefinitionToReturnDTO> FastPricing(Guid id);
        Task<bool> EditFastPricing(Guid definitionId, FastPricingDefinitionToCreateDTO dto);
        Task<PagedList<SellRequestToReturnDTO>> SellRequestList(PaginationParams<SellRequestSearch> pagination);
        Task<SellRequestToReturnDTO> SellRequest(Guid id);
        Task<bool> ChangeSellRequestStatus(Guid id, SellRequestStatusDTO dto);
        Task<bool> FAQ(FAQToCreateDTO dto);
        Task<List<FAQToReturnDTO>> ArrangeFAQs(List<int> arrangeIds);
        Task<bool> RemoveFAQ(Guid id);
        Task<bool> UpdateAboutUs(AppVariableDTO dto);
        Task<bool> SecurityAndPrivacy(AppVariableDTO dto);
        Task<bool> TermsAndCondition(AppVariableDTO dto);
        Task<FastPricingToReturnDTO> DeviceInSellRequest(Guid reqId);
        bool RemoveFastPricingDefinition(Guid id);
        Task<PagedList<TransactionToReturnDTO>> TransactionList(PaginationParams<TransactionSearch> pagination);
        TransactionToReturnDTO GetTransaction(Guid id);
        TransactionItemDTO GetTransactionItems(Guid transactionId);
        bool IsModelDefined(Guid id);
    }
}
