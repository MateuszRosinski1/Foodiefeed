using CommunityToolkit.Mvvm.ComponentModel;
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
                        PosterUsername = "Ania",
                        isFollowed=false,
                        CommentTextContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et",
                        Likes = 62,
                        isLiked = false,

                    }
                }
            };
            Posts.Add(post);
        }

        [ObservableProperty]
        ObservableCollection<Post> posts = new();




        //public record Post
        //(
        //    string PosterUsername,
        //    bool IsFollowed,
        //    string PostTextContent,
        //    List<Image> Images,
        //    int Likes,
        //    bool IsLiked,
        //    List<Comment> CommentSection
        //);
       

        //public record Comment
        //(
        //    string PosterUsername,
        //    bool IsFollowed,
        //    string CommentTextContent,
        //    int Likes,
        //    bool IsLiked
        //);

    }
}
