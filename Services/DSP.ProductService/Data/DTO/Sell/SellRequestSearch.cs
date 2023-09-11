using DSP.ProductService.Utilities;
using System.ComponentModel.DataAnnotations;

namespace DSP.ProductService.Data
{
    public class SellRequestSearch
    {
        /// <summary>
        /// متن جستجو
        /// </summary>
        public string SearchText { get; set; }
        /// <summary>
        /// مرتب سازی
        /// </summary>
        public SearchOrder? SellRequestSearchOrder { get; set; }
        /// <summary>
        /// وضعیت درخواست
        /// </summary>
        public SellRequestSearchStatus? SellRequestStatus { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? BrandId { get; set; }
        public Guid? ModelId { get; set; }
    }
    public enum SellRequestSearchStatus
    {
        /// <summary>
        /// در حال بررسی
        /// </summary>
        [Display(Name = "درحال بررسی")]
        InProcess,

        /// <summary>
        /// مراجعه سفیر موبیفای
        /// </summary>
        [Display(Name = "مراجعه سفیر موبیفای")]
        AgentVisit,

        /// <summary>
        /// پرداخت شده
        /// </summary>
        [Display(Name = "پرداخت شده")]
        Paid,

        /// <summary>
        /// عدم موافقت تل بال
        /// </summary>
        [Display(Name = "عدم توافق‌")]
        Rejected
    }
}
