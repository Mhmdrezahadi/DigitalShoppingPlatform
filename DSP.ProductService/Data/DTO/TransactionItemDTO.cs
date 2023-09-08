
namespace DSP.ProductService.Data
{
    public class TransactionItemDTO
    {
        public PaymentType TransactionType { get; set; }
        // if transaction belong to product
        public List<TransactionProductDTO> Products { get; set; }
        // if transaction belong to a wallet
        //public WalletModel Wallet { get; set; }
    }
}
