using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace DSP.ProductService.Data
{
    public class Payment : BaseEntity<Guid>
    {
        public Order Order { get; set; }
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public DateTime DT { get; set; }
        public string Authority { get; set; }
        public String MerchantID { get; set; }
        public bool IsSuccess { get; set; }
        public decimal Amount { get; set; }
        public string GateWayName { get; set; }

        /// <summary>
        ///  شماره تراکنش
        /// </summary>
        public string RefID { get; set; }
        /// <summary>
        /// کد پیگیری
        /// </summary>
        public string TrackingCode { get; set; }
        public int Status { get; set; }

        /// <summary>
        /// (پن شماره کارت واریز کننده(چندرقم وسط مخفی 
        /// </summary>
        public string CardPan { get; set; }

        /// <summary>
        /// هش شماره کارت واریز کننده
        /// </summary>
        public string CardHash { get; set; }

        /// <summary>
        /// مقدار کارمزد
        /// </summary>
        public int Fee { get; set; }

        /// <summary>
        /// نوع کارمزد
        /// </summary>
        public string FeeType { get; set; }

        /// <summary>
        /// نوع تراکنش
        /// </summary>
        public PaymentType PaymentType { get; set; }
    }

    public class PaymentConfiguartion : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.Property(p => p.Amount).HasColumnType("decimal(18,2)");

            builder.Property(p => p.CreatedAt).HasDefaultValueSql("getdate()");
            builder.Property(p => p.UpdatedAt).HasDefaultValueSql("getdate()");

            builder.Property(p => p.PaymentType)
                .HasConversion(
                e => e.ToString(),
                s => Enum.Parse<PaymentType>(s));
        }
    }
}
