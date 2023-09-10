namespace DSP.ProductService.Utilities
{
    public enum BadRequest
    {
        /// <summary>
        /// بازه ی درصدی اشتباه
        /// </summary>
        PricingRateError,

        /// <summary>
        /// فرمت اشتباه در مقدار بازه
        /// </summary>
        PricingRateInCorrectFormat,

        /// <summary>
        /// فرمت اشتباه در متن خطا و توضیح خطا
        /// </summary>
        PricingErrorInCorrectFormat,


        /// <summary>
        /// فرمت اشتباه در خطا و مقدار بازه
        /// </summary>
        PricingBothRateAndErrorInCorrectFormant,

        /// <summary>
        /// (خطا در تعداد شرط ها(بیشتر یا کمتر از 2 تا
        /// </summary>
        PricingConditionsError,
    }
}
