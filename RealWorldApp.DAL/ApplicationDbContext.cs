using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RealWorldApp.DAL.Entities;

namespace RealWebAppAPI.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options, operationalStoreOptions)
        {
        }

        public DbSet<Article> articles { get; set; }
        public DbSet<Comment> comments { get; set; }
        public DbSet<Tag> tags { get; set; }
        public DbSet<User> users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .IsRequired();
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Id)
                .IsUnique();
            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired();
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            modelBuilder.Entity<User>()
                .Property(u => u.UserName)
                .IsRequired();
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}