using Data.Entities.Category;
using Data.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<UserFollower> UserFollowers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserFollower>()
                .HasOne(uf => uf.User)
                .WithMany(u => u.Following)
                .HasForeignKey(uf => uf.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserFollower>()
                .HasOne(uf => uf.Follower)
                .WithMany(u => u.Followers)
                .HasForeignKey(uf => uf.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
    
    
}