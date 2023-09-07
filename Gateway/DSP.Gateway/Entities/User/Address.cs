using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace DSP.Gateway.Entities
{
    public class Address : BaseEntity<Guid>
    {
        public string Label { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string DetailedAddress { get; set; }
        public string PostalCode { get; set; }
        public string ContactNumber { get; set; }
        public string ContactName { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
        public bool IsVerified { get; set; } = true;
    }
    public class AddressConfiguartion : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.Property(p => p.CreatedAt).HasDefaultValueSql("getdate()");
            builder.Property(p => p.UpdatedAt).HasDefaultValueSql("getdate()");

            builder.Property(p => p.IsVerified).HasDefaultValue(true);
        }
    }
}
