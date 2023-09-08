namespace DSP.ProductService.Utilities
{
    public class UserSearch
    {
        /// <summary>
        /// متن جستجو
        /// </summary>
        public string SearchText { get; set; }
        /// <summary>
        /// مرتب سازی براساس وضعیت
        /// </summary>
        public bool? StatusSearchOrder { get; set; }
        /// <summary>
        /// مرتب سازی
        /// </summary>
        public SearchOrder? UserSearchOrder { get; set; }
    }

    /// <summary>
    /// مرتب سازی
    /// </summary>
    public enum SearchOrder
    {
        /// <summary>
        /// جدید ترین
        /// </summary>
        New,
        /// <summary>
        /// قدیمی ترین
        /// </summary>
        Old
    }
}
