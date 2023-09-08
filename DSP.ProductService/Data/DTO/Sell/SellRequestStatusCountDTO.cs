

using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace DSP.ProductService.Data
{
    public class SellRequestStatusCountDTO
    {
        public SellRequestStatus SellRequestStatus { get; set; }
        public int Count { get; set; }
    }
    public enum SellRequestStatus
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
