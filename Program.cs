using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Question
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public virtual IEnumerable<Post> Posts { get; set; } 
            = new HashSet<Post>();
    }

    public class Post
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string Title { get; set; } = null!;
        public string Body { get; set; } = null!;
        public int Score { get; set; }

        public virtual User User { get; set; } = null!;
    }

    public class QuestionContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Post> Posts { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=question;Username=app;Password=app")
                .LogTo(Console.WriteLine, LogLevel.Information);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<User>().Property(x => x.Id).HasColumnName("id");
            modelBuilder.Entity<User>().Property(x => x.FullName).HasColumnName("full_name");
            modelBuilder.Entity<User>().HasMany(x => x.Posts).WithOne(x => x.User)
                .HasForeignKey(x => x.AuthorId);

            modelBuilder.Entity<Post>().ToTable("posts");
            modelBuilder.Entity<Post>().Property(x => x.Id).HasColumnName("id");
            modelBuilder.Entity<Post>().Property(x => x.Title).HasColumnName("title");
            modelBuilder.Entity<Post>().Property(x => x.Body).HasColumnName("body");
            modelBuilder.Entity<Post>().Property(x => x.Score).HasColumnName("score");
            modelBuilder.Entity<Post>().Property(x => x.AuthorId).HasColumnName("author_id");
        }
    }

    enum PostOrder
    {
        TitleAsc,
        TitleDesc,
        ScoreAsc,
        ScoreDesc,
    }

    static class IQueryableExtensions
    {
        public static IOrderedQueryable<Post> OrderByCommon(this IQueryable<Post> queryable, PostOrder orderBy)
            => orderBy switch
            {
                PostOrder.TitleAsc => queryable.OrderBy(x => x.Title),
                PostOrder.TitleDesc => queryable.OrderByDescending(x => x.Title),
                PostOrder.ScoreAsc => queryable.OrderBy(x => x.Score).ThenBy(x => x.Title),
                PostOrder.ScoreDesc => queryable.OrderByDescending(x => x.Score).ThenBy(x => x.Title),
                _ => throw new NotSupportedException(),
            };
    }

    class Program
    {
        static void WorkingExample()
        {
            var dbContext = new QuestionContext();
            var users = dbContext.Users
                .Select(x => new
                {
                    User = x,
                    Top3Posts = x.Posts.AsQueryable()
                        .OrderByDescending(x => x.Score)
                        .ThenBy(x => x.Title)
                        .Take(3)
                        .ToList()
                }).ToList();
            foreach (var user in users)
            {
                Console.WriteLine($"User {user.User.FullName}");
                Console.WriteLine($"Best posts:");
                foreach (var post in user.Top3Posts)
                {
                    Console.WriteLine($"Post {post.Title} with score {post.Score}");
                }
            }
        }

        static void NonWorkingExample()
        {
            var input = PostOrder.ScoreDesc;
            var dbContext = new QuestionContext();
            var users = dbContext.Users
                .Select(x => new
                {
                    User = x,
                    Top3Posts = x.Posts.AsQueryable()
                        .OrderByCommon(input)
                        .Take(3)
                        .ToList()
                }).ToList();
            foreach (var user in users)
            {
                Console.WriteLine($"User {user.User.FullName}");
                Console.WriteLine($"Best posts:");
                foreach (var post in user.Top3Posts)
                {
                    Console.WriteLine($"Post {post.Title} with score {post.Score}");
                }
            }
        }

        static void Main(string[] args)
        {
            // WorkingExample();
            NonWorkingExample();
        }
    }
}
