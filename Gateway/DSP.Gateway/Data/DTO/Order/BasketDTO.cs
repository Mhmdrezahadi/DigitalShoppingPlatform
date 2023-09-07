using DSP.Gateway.Entities;
using System.Drawing;

namespace DSP.Gateway.Data
{
    public class BasketDTO
    {
        //public Identity.User User { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
        public DateTime DT { get; set; }
        //todo remove address?
        public Address Address { get; set; }
        public Guid? AddressId { get; set; }
        ///// <summary>
        ///// کد پیگیری 
        ///// </summary>
        //public string Code { get; set; }
        public decimal Price { get; set; }
        public double Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalPrice { get; set; }
        public string Description { get; set; }
        public TimeSpan Due { get; set; }
        public Guid? CouponId { get; set; }
        public ICollection<BasketDetailDTO> BasketDetails { get; set; }
    }
    public class BasketDetailDTO
    {
        public Guid BasketId { get; set; }
        public Guid ProductId { get; set; }
        public Color Color { get; set; }
        public Guid ColorId { get; set; }
        public int Count { get; set; }
        public decimal Amount { get; set; }
        public double Discount { get; set; }
    }
}
