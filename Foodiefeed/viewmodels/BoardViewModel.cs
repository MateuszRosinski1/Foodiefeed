#if ANDROID
using Android.App;
#endif
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;


namespace Foodiefeed.viewmodels
{
    public partial class BoardViewModel : ObservableObject
    {
        public BoardViewModel()
        {
            Post post = new Post()
            {
                PosterUsername = "Mateusz",
                isFollowed = false,
                PostTextContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                Images = new List<Image>()
                {
                    new Image()
                    {
                        Source = "logobutton.png"
                    }
                },
                Likes = 10231,
                isLiked = false,
                CommentSection = new List<Comment> { 
                    new Comment()
                    {
                        CommentPosterUsername = "Ania",
                        isFollowed=false,
                        CommentTextContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et",
                        Likes = 62,
                        isLiked = false,

                    },
                    new Comment()
                    {
                        CommentPosterUsername = "Ania",
                        isFollowed=true,
                        CommentTextContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et",
                        Likes = 63,
                        isLiked = false,

                    }
                }
            };
            Posts.Add(post);
            Post post2 = new Post()
            {
                PosterUsername = "Mateusz",
                isFollowed = false,
                PostTextContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                Images = new List<Image>()
                {
                    new Image()
                    {
                        Source = "logobutton.png"
                    }
                },
                Likes = 10231,
                isLiked = false,
                CommentSection = new List<Comment> {
                    new Comment()
                    {
                        CommentPosterUsername = "Ania",
                        isFollowed=false,
                        CommentTextContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et",
                        Likes = 62,
                        isLiked = false,

                    },
                    new Comment()
                    {
                        CommentPosterUsername = "Ania",
                        isFollowed=false,
                        CommentTextContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et",
                        Likes = 62,
                        isLiked = false,

                    },
                    new Comment()
                    {
                        CommentPosterUsername = "Ania",
                        isFollowed=false,
                        CommentTextContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et",
                        Likes = 62,
                        isLiked = false,

                    },
                    new Comment()
                    {
                        CommentPosterUsername = "Ania",
                        isFollowed=false,
                        CommentTextContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et",
                        Likes = 62,
                        isLiked = false,

                    },
                    new Comment()
                    {
                        CommentPosterUsername = "Ania",
                        isFollowed=false,
                        CommentTextContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et",
                        Likes = 62,
                        isLiked = false,

                    },
                    new Comment()
                    {
                        CommentPosterUsername = "Ania",
                        isFollowed=true,
                        CommentTextContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et",
                        Likes = 63,
                        isLiked = false,

                    }
                }
            };
            Posts.Add(post2);
            Posts.Add(post2);
            Posts.Add(post2);
            Posts.Add(post2);
            Posts.Add(post2);

        }

        [ObservableProperty]
        ObservableCollection<Post> posts = new();

        [RelayCommand]
        public void Scrolled(ItemsViewScrolledEventArgs e)
        {

            if (e.LastVisibleItemIndex == Posts.Count() - 2)
            {
                for(int i = 0; i <= 2; i++)
                {
                    Post post = new Post()
                    {
                        PosterUsername = "Mateusz",
                        isFollowed = false,
                        PostTextContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                        Images = new List<Image>()
                {
                    new Image()
                    {
                        Source = "logobutton.png"
                    }
                },
                        Likes = 10231,
                        isLiked = false,
                        CommentSection = new List<Comment> {
                    new Comment()
                    {
                        CommentPosterUsername = "Ania",
                        isFollowed=false,
                        CommentTextContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et",
                        Likes = 62,
                        isLiked = false,

                    },
                    new Comment()
                    {
                        CommentPosterUsername = "Ania",
                        isFollowed=false,
                        CommentTextContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et",
                        Likes = 62,
                        isLiked = false,

                    },
                    new Comment()
                    {
                        CommentPosterUsername = "Ania",
                        isFollowed=false,
                        CommentTextContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et",
                        Likes = 62,
                        isLiked = false,

                    },
                    new Comment()
                    {
                        CommentPosterUsername = "Ania",
                        isFollowed=false,
                        CommentTextContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et",
                        Likes = 62,
                        isLiked = false,

                    },
                    new Comment()
                    {
                        CommentPosterUsername = "Ania",
                        isFollowed=false,
                        CommentTextContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et",
                        Likes = 62,
                        isLiked = false,

                    },
                    new Comment()
                    {
                        CommentPosterUsername = "Ania",
                        isFollowed=true,
                        CommentTextContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et",
                        Likes = 63,
                        isLiked = false,

                    }
                }
                    };
                    Posts.Add(post);
                }
            }
            

        }

        
    }
}
