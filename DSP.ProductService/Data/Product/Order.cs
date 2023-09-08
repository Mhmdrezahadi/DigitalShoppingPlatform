using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DSP.ProductService.Data
{
    public class Order : BaseEntity<Guid>
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }
        //public Identity.User User { get; set; }
        public Guid UserId { get; set; }
        public DateTime DT { get; set; }
        public Guid? AddressId { get; set; }
        /// <summary>
        /// شماره سبد خرید 
        /// </summary>
        public string Code { get; set; }
        public decimal Price { get; set; }
        public double Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalPrice { get; set; }
        public string Description { get; set; }
        public OrderStatus OrderStatus { get; set; }
        //public TimeSpan Due { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
        public ICollection<Payment> Payments { get; set; }
    }
    public enum OrderStatus
    {
        /// <summary>
        /// سبد خرید
        /// </summary>
        [Display(Name = "سبد خرید")]
        Basket,

        /// <summary>
        /// درحال بررسی
        /// </summary>
        [Display(Name = "درحال بررسی")]
        Pending,

        /// <summary>
        /// در حال پردازش
        /// </summary>
        [Display(Name = "درحال پردازش")]
        InProcess,

        /// <summary>
        /// تحویل به سفیر تل بال
        /// </summary>
        [Display(Name = "تحویل به سفیر موبیفای")]
        AgentDelivered,

        /// <summary>
        /// تحویل به مشتری
        /// </summary>
        [Display(Name = "تحویل به مشتری")]
        CustomerDelivered,

        /// <summary>
        /// انصراف از خرید
        /// </summary>
        [Display(Name = "انصراف از خرید")]
        Canceled,

        /// <summary>
        /// مرجوعی
        /// </summary>
        [Display(Name = "مرجوع به فروشگاه")]
        Returned
    }
    public class OrderConfiguartion : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(p => p.OrderStatus)
                .HasConversion(
                e => e.ToString(),
                s => Enum.Parse<OrderStatus>(s));

            builder.Property(p => p.Price).HasColumnType("decimal(18,2)");
            builder.Property(p => p.Tax).HasColumnType("decimal(18,2)");
            builder.Property(p => p.TotalPrice).HasColumnType("decimal(18,2)");

            builder.Property(p => p.CreatedAt).HasDefaultValueSql("getdate()");
            builder.Property(p => p.UpdatedAt).HasDefaultValueSql("getdate()");

        }
    }
}
