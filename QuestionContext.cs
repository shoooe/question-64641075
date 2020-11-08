using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;

namespace Question
{
    public class QuestionContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Post> Posts { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=question;Username=app;Password=app")
                .LogTo(Console.WriteLine, LogLevel.Information);
            // optionsBuilder.UseOrderByAll();
            // optionsBuilder.ReplaceService<IQueryTranslationPreprocessorFactory, CustomQueryTranslationPreprocessorFactory>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<User>().HasKey(x => x.Id);
            modelBuilder.Entity<User>().Property(x => x.Id).HasColumnName("id");
            modelBuilder.Entity<User>().Property(x => x.FullName).HasColumnName("full_name");
            modelBuilder.Entity<User>().HasMany(x => x.Posts).WithOne(x => x.User)
                .HasForeignKey(x => x.AuthorId);

            modelBuilder.Entity<Post>().ToTable("posts");
            modelBuilder.Entity<Post>().HasKey(x => x.Id);
            modelBuilder.Entity<Post>().Property(x => x.Id).HasColumnName("id");
            modelBuilder.Entity<Post>().Property(x => x.Title).HasColumnName("title");
            modelBuilder.Entity<Post>().Property(x => x.Body).HasColumnName("body");
            modelBuilder.Entity<Post>().Property(x => x.Score).HasColumnName("score");
            modelBuilder.Entity<Post>().Property(x => x.AuthorId).HasColumnName("author_id");
        }
    }
}