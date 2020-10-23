using Forum.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Forum.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IOptions<UserSettings> userSettings)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Section>()
                .HasMany(s => s.Topics)
                .WithOne(t => t.Section)
                .HasForeignKey(prop => prop.SectionId);

            modelBuilder.Entity<Section>()
                .HasMany(s => s.Moderators);

            modelBuilder.Entity<Post>()
                .HasOne(p => p.Editor);

            modelBuilder.Entity<Post>()
                .HasOne(p => p.Author)
                .WithMany(u => u.Posts)
                .HasForeignKey(prop => prop.AuthorId);

            modelBuilder.Entity<Moderator>()
                .HasMany(m => m.ModeratableSections);

            modelBuilder.Entity<Topic>()
                .HasMany(t => t.Posts)
                .WithOne(p => p.Topic)
                .HasForeignKey(prop => prop.TopicId);

            modelBuilder.Entity<Topic>()
                .HasOne(t => t.Section)
                .WithMany(s => s.Topics)
                .HasForeignKey(prop => prop.SectionId);

            modelBuilder.Entity<Topic>()
                .HasOne(t => t.Editor);

            modelBuilder.Entity<Topic>()
                .HasOne(t => t.Author)
                .WithMany(u => u.Topics)
                .HasForeignKey(prop => prop.AuthorId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Posts)
                .WithOne(u => u.Author);

            modelBuilder.Entity<User>()
               .HasMany(u => u.Topics)
               .WithOne(t => t.Author);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Image> Images { get; set; }
        public new DbSet<User> Users { get; set; }
        public DbSet<Moderator> Moderators { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Section> Sections { get; set; }
    }
}
