using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DSP.Gateway.Entities
{
    public class UserDbContext : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
                : base(options)
        {
        }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Province> Provinces { get; set; }
        public virtual DbSet<City> Cities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasAnnotation(
                "Relational:Collation",
                "Persian_100_CI_AS");

            var entitiesAssembly = typeof(IEntity).Assembly;

            modelBuilder.RegisterAllEntities<IEntity>(entitiesAssembly);
            modelBuilder.RegisterEntityTypeConfiguration(entitiesAssembly);
            modelBuilder.AddRestrictDeleteBehaviorConvention();
            modelBuilder.AddSequentialGuidForIdConvention();

            modelBuilder.Entity<User>(x => x.Property(p => p.Id).HasDefaultValueSql("NEWID()"));

            modelBuilder.Entity<Role>(x => x.Property(p => p.Id).HasDefaultValueSql("NEWID()"));
        }
    }
}
