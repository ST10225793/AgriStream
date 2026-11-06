using AgriStream.Models;
using Microsoft.EntityFrameworkCore;

namespace AgriStream.Data
    {
        public class ApplicationDbContext : DbContext
        {
            public ApplicationDbContext(
                DbContextOptions<ApplicationDbContext> options)
                : base(options)
            {
            }

            public DbSet<User> Users => Set<User>();

            public DbSet<Role> Roles => Set<Role>();

            protected override void OnModelCreating(
                ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<User>()
                    .HasIndex(u => u.Username)
                    .IsUnique();

                modelBuilder.Entity<Role>()
                    .HasIndex(r => r.RoleName)
                    .IsUnique();

                modelBuilder.Entity<User>()
                    .HasOne(u => u.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(u => u.RoleID);
            }
        }
    }


   
