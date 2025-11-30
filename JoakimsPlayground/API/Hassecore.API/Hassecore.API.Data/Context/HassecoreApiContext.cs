using Hassecore.API.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hassecore.API.Data.Context
{
    public class HassecoreApiContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public HassecoreApiContext(DbContextOptions<HassecoreApiContext> options) : base(options)
        {
                
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ExternalId).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();
                entity.Property(e => e.LastOnline).IsRequired();
            });
        }
    }
}
