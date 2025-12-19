using Hassecore.API.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hassecore.API.Data.Context
{
    public class HassecoreApiContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserPair> UserPairs { get; set; }
        public DbSet<UserPairingRequest> UserPairingRequests { get; set; }

        public HassecoreApiContext(DbContextOptions<HassecoreApiContext> options) : base(options)
        {
                
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder options)
        //=> options.UseSqlServer($"Server=.\\SQLEXPRESS;Database=HassecoreTestDb1;User ID=Hassecore;Password=LfS812L?:ndF8iu?_DSA897asfHfas;Persist Security Info=True;Pooling=False;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=True");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureUser(modelBuilder);

            ConfigureUserPair(modelBuilder);

            ConfigureUserPairingRequest(modelBuilder);
        }

        private void ConfigureUser(ModelBuilder modelBuilder)
        {
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

        private void ConfigureUserPair(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserPair>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.User1Id).IsRequired();
                entity.Property(e => e.User2Id).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.HasIndex(x => x.User1Id).IsUnique();
                entity.HasIndex(x => x.User2Id).IsUnique();

                entity.HasOne(x => x.User1)
                      .WithMany()
                      .HasForeignKey(e => e.User1Id)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.User2)
                      .WithMany()
                      .HasForeignKey(e => e.User2Id)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureUserPairingRequest(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserPairingRequest>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SenderId).IsRequired();
                entity.Property(e => e.ReceiverId).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.HasOne(x => x.Sender)
                      .WithMany()
                      .HasForeignKey(e => e.SenderId)
                      .OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x => x.Receiver)
                      .WithMany()
                      .HasForeignKey(e => e.ReceiverId)
                      .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
