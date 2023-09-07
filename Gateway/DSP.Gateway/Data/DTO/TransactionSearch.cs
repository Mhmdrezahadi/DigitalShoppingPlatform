using DSP.Gateway.Utilities;

namespace DSP.Gateway.Data
{
    public class TransactionSearch
    {
        /// <summary>
        /// متن جستجو
        /// </summary>
        public string SearchText { get; set; }
        /// <summary>
        /// مرتب سازی
        /// </summary>
        public SearchOrder? TransactionSearchOrder { get; set; }
        /// <summary>
        /// نوع پرداخت
        /// </summary>
        public PaymentType? PaymentType { get; set; }
        /// <summary>
        /// مرتب سازی بر اساس وضعیت خرید
        /// </summary>
        public bool? StatusSearchOrder { get; set; }
    }
    public enum PaymentType
    {
        /// <summary>
        /// خرید از تل بال
        /// </summary>
        Shopping,

        /// <summary>
        /// واریز به کیف پول
        /// </summary>
        DepositToWallet,

        /// <summary>
        /// برداشت از کیف پول
        /// </summary>
        WithdrawalOfWallet
    }
}
