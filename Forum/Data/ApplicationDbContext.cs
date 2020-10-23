using System;
using System.Collections.Generic;
using System.Text;
using Forum.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Forum.ViewModels;

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

            modelBuilder.Entity<Topic>()
                .HasMany(t => t.Posts)
                .WithOne(p => p.Topic)
                .HasForeignKey(prop => prop.TopicId);

            base.OnModelCreating(modelBuilder);
        }

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    builder.Entity<Section>(entity =>
        //    {
        //        entity.Property(e => e.Name)
        //            .IsRequired();
        //    });

        //    base.OnModelCreating(builder);
        //}

        public DbSet<Image> Images { get; set; }
        public new DbSet<User> Users { get; set; }
        public DbSet<Moderator> Moderators { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Section> Sections { get; set; }
    }
}
