using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace DSP.Gateway.Entities
{
    /// <summary>
    /// User Role
    /// </summary>
    public class UserRole : IdentityUserRole<Guid>
    {
        /// <summary>
        /// User
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Role
        /// </summary>
        public virtual Role Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class UserRoleConfiguartion : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("UserRoles");

            builder.HasKey(e => new { e.UserId, e.RoleId });

            builder.HasOne(e => e.Role)
            .WithMany(f => f.UserRoles)
            .HasForeignKey(k => k.RoleId);

            builder.HasOne(e => e.User)
            .WithMany(f => f.UserRoles)
            .HasForeignKey(k => k.UserId);

            builder.Property(p => p.CreatedAt).HasDefaultValueSql("getdate()");
            builder.Property(p => p.UpdatedAt).HasDefaultValueSql("getdate()");
        }
    }
}