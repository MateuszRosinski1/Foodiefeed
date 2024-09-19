using Foodiefeed_api.entities;
using System;

namespace Foodiefeed_api
{
    public class DatabaseSeeder
    {
        public static void SeedData(dbContext context)
        {
            var userMati = context.Users.FirstOrDefault(u => u.Username == "mati");
            var userMariuszek = context.Users.FirstOrDefault(u => u.Username == "Mariuszek29");

            if (userMati == null || userMariuszek == null)
            {
                throw new Exception("Użytkownicy 'mati' i 'Mariuszek29' muszą istnieć w bazie danych przed uruchomieniem seeder'a.");
            }

            if (!context.Posts.Any())
            {
                var posts = new List<Post>
            {
                new Post
                {
                    UserId = userMati.Id,
                    Description = "Przepis na pyszne spaghetti bolognese!",
                    Likes = 45
                },
                new Post
                {
                    UserId = userMariuszek.Id,
                    Description = "Szybkie śniadanie: Omlet z warzywami.",
                    Likes = 32
                },
                new Post
                {
                    UserId = userMati.Id,
                    Description = "Deser idealny: Tiramisu bez jajek.",
                    Likes = 60
                },
                new Post
                {
                    UserId = userMariuszek.Id,
                    Description = "Kurczak w sosie curry, prosto z Azji.",
                    Likes = 55
                }
            };
                context.Posts.AddRange(posts);
                context.SaveChanges();
            }

            if (!context.Comments.Any())
            {
                var comments = new List<Comment>
            {
                new Comment
                {
                    UserId = userMariuszek.Id,
                    CommentContent = "Spaghetti wyszło pyszne! Polecam!",
                    Likes = 12
                },
                new Comment
                {
                    UserId = userMati.Id,
                    CommentContent = "Omlet z warzywami to idealny pomysł na śniadanie.",
                    Likes = 8
                },
                new Comment
                {
                    UserId = userMariuszek.Id,
                    CommentContent = "Tiramisu bez jajek? Brzmi świetnie, muszę spróbować!",
                    Likes = 20
                },
                new Comment
                {
                    UserId = userMati.Id,
                    CommentContent = "Kurczak w sosie curry to klasyka. Super przepis!",
                    Likes = 25
                }
            };
                context.Comments.AddRange(comments);
                context.SaveChanges();
            }

            if (!context.PostCommentMembers.Any())
            {
                var postCommentMembers = new List<PostCommentMember>
            {
                new PostCommentMember
                {
                    PostId = context.Posts.First(p => p.Description.Contains("spaghetti")).PostId,
                    CommentId = context.Comments.First(c => c.CommentContent.Contains("Spaghetti wyszło")).CommentId
                },
                new PostCommentMember
                {
                    PostId = context.Posts.First(p => p.Description.Contains("Omlet")).PostId,
                    CommentId = context.Comments.First(c => c.CommentContent.Contains("Omlet z warzywami")).CommentId
                }
            };
                context.PostCommentMembers.AddRange(postCommentMembers);
                context.SaveChanges();
            }

            if (!context.PostImages.Any())
            {
                var postImages = new List<PostImage>
            {
                new PostImage
                {
                    PostId = context.Posts.First(p => p.Description.Contains("spaghetti")).PostId,
                    ImagePath = "/images/spaghetti.jpg"
                },
                new PostImage
                {
                    PostId = context.Posts.First(p => p.Description.Contains("Omlet")).PostId,
                    ImagePath = "/images/omlet.jpg"
                },
                new PostImage
                {
                    PostId = context.Posts.First(p => p.Description.Contains("Tiramisu")).PostId,
                    ImagePath = "/images/tiramisu.jpg"
                },
                new PostImage
                {
                    PostId = context.Posts.First(p => p.Description.Contains("curry")).PostId,
                    ImagePath = "/images/curry.jpg"
                }
            };
                context.PostImages.AddRange(postImages);
                context.SaveChanges();
            }

            if (!context.PostProducts.Any())
            {
                var postProducts = new List<PostProduct>
            {
                new PostProduct
                {
                    PostId = context.Posts.First(p => p.Description.Contains("spaghetti")).PostId,
                    Product = "Makaron spaghetti"
                },
                new PostProduct
                {
                    PostId = context.Posts.First(p => p.Description.Contains("spaghetti")).PostId,
                    Product = "Mięso mielone"
                },
                new PostProduct
                {
                    PostId = context.Posts.First(p => p.Description.Contains("Omlet")).PostId,
                    Product = "Jajka"
                },
                new PostProduct
                {
                    PostId = context.Posts.First(p => p.Description.Contains("Omlet")).PostId,
                    Product = "Papryka"
                }
            };
                context.PostProducts.AddRange(postProducts);
                context.SaveChanges();
            }

            if (!context.PostTags.Any())
            {
                var postTags = new List<PostTag>
            {
                new PostTag
                {
                    PostId = context.Posts.First(p => p.Description.Contains("spaghetti")).PostId,
                    TagName = "makaron",
                    Description = "Przepisy na makarony"
                },
                new PostTag
                {
                    PostId = context.Posts.First(p => p.Description.Contains("Omlet")).PostId,
                    TagName = "śniadanie",
                    Description = "Pomysły na zdrowe śniadania"
                },
                new PostTag
                {
                    PostId = context.Posts.First(p => p.Description.Contains("Tiramisu")).PostId,
                    TagName = "deser",
                    Description = "Słodkie desery"
                }
            };
                context.PostTags.AddRange(postTags);
                context.SaveChanges();
            }

            if (!context.UserTags.Any())
            {
                var userTags = new List<UserTag>
            {
                new UserTag
                {
                    UserId = userMati.Id,
                    TagName = "desery",
                    Count = 120
                },
                new UserTag
                {
                    UserId = userMariuszek.Id,
                    TagName = "zdrowe jedzenie",
                    Count = 80
                },
                new UserTag
                {
                    UserId = userMati.Id,
                    TagName = "kuchnia włoska",
                    Count = 150
                }
            };
                context.UserTags.AddRange(userTags);
                context.SaveChanges();
            }

            if (!context.Friends.Any())
            {
                var friends = new List<Friend>
            {
                new Friend
                {
                    UserId = userMati.Id,
                    FriendUserId = userMariuszek.Id
                }
            };
                context.Friends.AddRange(friends);
                context.SaveChanges();
            }

            if(!context.Followers.Any()) {
                var followers = new List<Follower>
                {
                    new Follower
                    {
                        UserId = userMariuszek.Id,
                        FollowedUserId = userMati.Id
                    },
                    new Follower
                    {
                        UserId = userMati.Id,
                        FollowedUserId = userMariuszek.Id
                    }
                };
                context.Followers.AddRange(followers);
                context.SaveChanges();
            }
        }
    }
}
