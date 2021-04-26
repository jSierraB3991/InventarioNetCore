using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Domain.Model;

namespace WebApi.Infrastructure.Repository
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options: options)
        {
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Movement> Movements { get; set; }

        public DbSet<Store> Stores { get; set; }

        public DbSet<ProductStore> ProductStores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasIndex(product => product.Name)
                .IsUnique();

            modelBuilder.Entity<Store>()
                .HasIndex(store => store.Name)
                .IsUnique();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            System.Collections.Generic.List<EntityEntry<AuditableEntity>> AddedEntities = ChangeTracker.Entries<AuditableEntity>().Where(E => E.State == EntityState.Added).ToList();
            AddedEntities.ForEach(E =>
            {
                E.Entity.CreateAt = DateTime.Now.ToUniversalTime();
                E.Entity.ModifyDate = DateTime.Now.ToUniversalTime();
            });

            System.Collections.Generic.List<EntityEntry<AuditableEntity>> ModifiedEntities = ChangeTracker.Entries<AuditableEntity>().Where(E => E.State == EntityState.Modified).ToList();
            ModifiedEntities.ForEach(E =>
            {
                E.Entity.ModifyDate = DateTime.Now.ToUniversalTime();
            });
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
