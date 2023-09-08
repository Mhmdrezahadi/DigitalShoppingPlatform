using Microsoft.EntityFrameworkCore;


namespace DSP.ProductService.Data
{
    public class ProductServiceDbContext : DbContext
    {
        public ProductServiceDbContext(DbContextOptions<ProductServiceDbContext> options)
        : base(options)
        {
        }
        public virtual DbSet<Order> Orders { get; set; }

        public virtual DbSet<OrderDetail> OrderDetails { get; set; }

        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<PropertyKey> PropertyKeys { get; set; }

        public virtual DbSet<PropertyValue> PropertyValues { get; set; }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<Like> Likes { get; set; }

        public virtual DbSet<Image> Images { get; set; }

        public virtual DbSet<FastPricingKey> FastPricingKeys { get; set; }

        public virtual DbSet<FastPricingDD> FastPricingDDs { get; set; }

        public virtual DbSet<FastPricingValue> FastPricingValues { get; set; }

        public virtual DbSet<PriceLog> PriceLogs { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<FastPricingDefinition> FastPricingDefinitions { get; set; }
        public virtual DbSet<Color> Colors { get; set; }
        public virtual DbSet<SellRequest> SellRequests { get; set; }
        //public virtual DbSet<FAQ> FAQs { get; set; }
        //public virtual DbSet<AppVariable> AppVariables { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Basket> Baskets { get; set; }
        public virtual DbSet<BasketDetail> BasketDetails { get; set; }
        public virtual DbSet<ProductDetail> ProductDetails { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
