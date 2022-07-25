using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RealWorldApp.Commons.Entities;

namespace RealWorldApp.DAL
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
            // USER
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

            modelBuilder.Entity<User>()
                .HasMany(u => u.Articles)
                .WithOne(u => u.Author);

            modelBuilder.Entity<User>()
                .HasMany(u => u.FavoriteArticles)
                .WithOne();

            // ARTICLE
            modelBuilder.Entity<Article>()
                .HasIndex(a => a.Title)
                .IsUnique();

            // COMMENTS
            modelBuilder.Entity<Comment>()
                .Property(c => c.Body)
                .IsRequired();

            // TAGS
            modelBuilder.Entity<Tag>()
                .Property(t => t.Name)
                .IsRequired();

            base.OnModelCreating(modelBuilder);
        }
    }
}