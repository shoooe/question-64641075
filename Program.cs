using System;
using System.Linq;

namespace Question
{
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

        static void OrderByAllNonWorkingExample()
        {
            var input = PostOrder.ScoreDesc;
            var sorting = input switch
            {
                PostOrder.ScoreAsc => new[]
                {
                    new Sorting<Post>(x => x.Score, SortingDirection.Asc),
                    new Sorting<Post>(x => x.Title, SortingDirection.Asc)
                },
                PostOrder.ScoreDesc => new[]
                {
                    new Sorting<Post>(x => x.Score, SortingDirection.Desc),
                    new Sorting<Post>(x => x.Title, SortingDirection.Asc)
                },
                PostOrder.TitleAsc => new[]
                {
                    new Sorting<Post>(x => x.Title, SortingDirection.Asc)
                },
                PostOrder.TitleDesc => new[]
                {
                    new Sorting<Post>(x => x.Title, SortingDirection.Desc)
                },
                _ => throw new NotSupportedException(),
            };
            var dbContext = new QuestionContext();
            var users = dbContext.Users
                .Select(x => new
                {
                    User = x,
                    Top3Posts = x.Posts.AsQueryable()
                        .OrderByAll(sorting)
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

        static void AlternativeExample1()
        {
            var input = PostOrder.ScoreDesc;
            var dbContext = new QuestionContext();
            var query = dbContext.Posts
                .OrderByCommon(input);
            var users = dbContext.Users
                .Select(x => new
                {
                    Key = x.Id,
                    List = query.Where(y => y.AuthorId == x.Id)
                        .Take(3)
                        .ToList()
                })
                .ToList();
            foreach (var user in users)
            {
                Console.WriteLine($"User {user.Key}");
                Console.WriteLine($"Best posts:");
                foreach (var post in user.List)
                {
                    Console.WriteLine($"Post {post.Title} with score {post.Score}");
                }
            }
        }

        static void AlternativeExample2()
        {
            var input = PostOrder.ScoreDesc;
            var dbContext = new QuestionContext();
            var query = dbContext.Posts
                .OrderByCommon(input);
            var users = query
                .Select(x => x.AuthorId)
                .Distinct()
                // .Select(x => new { Key = x })
                // .ToList();
                .Select(x => new
                {
                    Key = x,
                    List = query.Where(y => y.AuthorId == x)
                        .Take(3)
                        .ToList()
                })
                .ToList();
        }

        static void AlternativeExample3()
        {
            var input = PostOrder.ScoreDesc;
            var dbContext = new QuestionContext();
            var sub = dbContext.Posts
                .OrderByCommon(input);
            var posts = sub
                .Select(a => a.AuthorId)
                .Distinct()
                .SelectMany(a => sub.Where(b => b.AuthorId == a).Take(3), (a, b) => b)
                .ToList();
            foreach (var post in posts)
            {
                Console.WriteLine($"Author {post.AuthorId} Post {post.Title} with score {post.Score}");
            }
        }

        static void FinalExample()
        {
            var input = PostOrder.ScoreDesc;
            var dbContext = new QuestionContext();
            var users = dbContext.Posts
                .PartitionBy(x => x.AuthorId, x => x.OrderByCommon(input), take: 3, skip: 0)
                .ToLookup(x => x.AuthorId);
            foreach (var user in users)
            {
                Console.WriteLine($"Author {user.Key}");
                foreach (var post in user) 
                    Console.WriteLine($"Post {post.Title} with score {post.Score}");
            }
        }

        static void Main(string[] args)
        {
            // WorkingExample();
            // NonWorkingExample();
            // AlternativeExample1();
            // AlternativeExample2();
            // AlternativeExample3();
            FinalExample();
        }
    }
}
