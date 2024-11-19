#if ANDROID
//using Android.App;
#endif
#if WINDOWS
using Windows.Media.SpeechRecognition;
#endif
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Foodiefeed.views.windows.contentview;
using Foodiefeed.views.windows.popups;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Foodiefeed.models.dto;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Text;
using Foodiefeed.Resources.Styles;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Maui.Core.Extensions;
using System.Net.Http.Headers;
using CommunityToolkit.Maui.Core;


namespace Foodiefeed.viewmodels
{
    public partial class BoardViewModel : ObservableObject
    {
        //https://icons8.com/icons/set/microphone icons
        //https://github.com/dotnet/maui/issues/8150  shadow resizing problem
        //https://github.com/CommunityToolkit/Maui/pull/2072 uniformgrid issue
        //Search problems need to be fixed. - fixed


        private readonly UserSession _userSession;
        private Thread UpdateOnlineFriendListThread;

        public ObservableCollection<PostView> Posts { get; set; } = new ObservableCollection<PostView>();

        //private ObservableCollection<PostView> posts 
        //    = new ObservableCollection<PostView>();

        public ObservableCollection<OnlineFreidnListElementView> OnlineFriends { get; set; } = new ObservableCollection<OnlineFreidnListElementView>();

        //private ObservableCollection<OnlineFreidnListElementView> onlineFriends 
        //    = new ObservableCollection<OnlineFreidnListElementView> { };

        //public ObservableCollection<OnListFriendView> ProfilePageFriends { get { return profilePageFriends; } }

        //private ObservableCollection<OnListFriendView> profilePageFriends = 
        //    new ObservableCollection<OnListFriendView>();

        public ObservableCollection<UserSearchResultView> SearchResults { get; set; } = new ObservableCollection<UserSearchResultView>();

        //private ObservableCollection<UserSearchResultView > searchResults = 
        //    new ObservableCollection<UserSearchResultView> { };

        #region profilePageMemebers
        [ObservableProperty]
        private int profileId;
        [ObservableProperty]
        private string profileName;
        [ObservableProperty]
        private string profileLastName;
        [ObservableProperty]
        private string profileUsername;
        [ObservableProperty]
        private string profileFollowers;
        [ObservableProperty]
        private string profileFriends;
        [ObservableProperty]
        private string avatarBase64;
        #endregion


#if WINDOWS
       private const string API_BASE_URL = "http://localhost:5000";
#endif
#if ANDROID
        private const string API_BASE_URL = "http://10.0.2.2:5000";
#endif

        [ObservableProperty]
        private string searchParam;

        #region VisibilityFlags
        [ObservableProperty]
        bool profileAddFriendAndFollowButtonsVisible;

        [ObservableProperty]
        bool searchPanelVisible;

        [ObservableProperty]
        bool profilePageVisible;

        [ObservableProperty]
        bool postPageVisible;

        [ObservableProperty]
        bool settingsPageVisible;

        [ObservableProperty]
        bool recipePageVisible;

        [ObservableProperty]
        bool personalDataEditorVisible;

        [ObservableProperty]
        bool settingsMainHubVisible;

        [ObservableProperty]
        bool changeUsernameEntryVisible;

        [ObservableProperty]
        bool changeEmailEntryVisible;

        [ObservableProperty]
        bool changePasswordEntryVisible;

        [ObservableProperty]
        bool changeProfilePictureVisible;

        [ObservableProperty]
        bool profileFriendsVisible;

        [ObservableProperty]
        bool profileFollowersVisible;

        [ObservableProperty]
        bool profilePostsVisible;

        [ObservableProperty]
        bool noPostOnProfile;

        [ObservableProperty]
        bool hubPanelVisible;

        [ObservableProperty]
        bool noNotificationNotifierVisible;

        [ObservableProperty]
        bool loadingLabelVisible;

        [ObservableProperty]
        bool addPostFormVisible;

        [ObservableProperty]
        bool likedRecipesVisible;

        [ObservableProperty]
        bool savedRecipesVisible;

        [ObservableProperty]
        ImageSource profilePictureSource;      

        public async Task SetProfilePictureFromBase64(string base64)
        {
            if (string.IsNullOrEmpty(base64))

            if (base64.Contains("base64,"))
                base64 = base64.Substring(base64.IndexOf("base64,") + 7);

            try
            {
                await Task.Run(() =>
                {
                    byte[] imageBytes = Convert.FromBase64String(base64);
                    ProfilePictureSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                });
            }
            catch (FormatException)
            {
                ProfilePictureSource = null;
            }
        }

        #endregion

        [ObservableProperty]
        bool internetAcces;

        #region SettingsVariables
        [ObservableProperty]
        string changedUsername;

        [ObservableProperty]
        string changedEmail;

        [ObservableProperty]
        string changedPassword;

        [ObservableProperty]
        string changedRePassword;

        [ObservableProperty]
        string changedImageProfilePicturePath;

        #endregion

        public ObservableCollection<INotification> Notifications { get; set; } = new ObservableCollection<INotification>();
        //private ObservableCollection<INotification> notifications = new ObservableCollection<INotification>();

        public ObservableCollection<RecipeView> LikedRecipes { get; set; } = new ObservableCollection<RecipeView>();
        public ObservableCollection<RecipeView> SavedRecipes { get; set; } = new ObservableCollection<RecipeView>();


        public BoardViewModel(UserSession userSession)
        {
            
            _userSession = userSession;
            _userSession.Id = 15;
            InternetAcces = !(Connectivity.NetworkAccess == NetworkAccess.Internet);
            Notifications.CollectionChanged += OnNotificationsChanged;

            var notification = new BasicNotofication();
            Notifications.Add(notification);          
            NoNotificationNotifierVisible = Notifications.Count == 0 ? true : false;

            DisplaySearchResultHistory();

            this.ProfilePageVisible = false; //on init false
            this.PostPageVisible = true; //on init true
            this.SettingsPageVisible = false; //on init false
            this.RecipePageVisible = false; // on init false
            this.PersonalDataEditorVisible = false; // on init false
            this.SettingsMainHubVisible = true; //on init true
            this.ChangeUsernameEntryVisible = false; //on init false
            this.ChangeEmailEntryVisible = false; //on init false
            this.ChangeProfilePictureVisible = false; // on init false
            this.ChangePasswordEntryVisible = false; //on init false
            this.ProfileFollowersVisible = false; //on init false
            this.ProfilePostsVisible = true; //on init true;
            this.ProfileFriendsVisible = false; //on init false
            this.NoPostOnProfile = false; // on init false
            this.HubPanelVisible = false; // on init false
            this.CanShowSearchPanel = true; //on init true
            this.AddPostFormVisible = false; //on init false
            this.PostContentEditorVisible = true; //on init true
            this.LikedRecipesVisible = true; //on init true
            this.SavedRecipesVisible = true; //on init false

            var mrgDict = Application.Current.Resources.MergedDictionaries.ElementAt(2);     

            //UpdateOnlineFriendListThread = new Thread(UpdateFriendList);
            //UpdateOnlineFriendListThread.Start();

            //Task.Run(UpdateFriendList);
            //ChangeTheme();          
            Connectivity.ConnectivityChanged += ConnectivityChanged;
        }

        private void ConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
        {
            var access = Connectivity.NetworkAccess;
            InternetAcces = !(access == NetworkAccess.Internet);
        }

        [RelayCommand]
        public async Task Logout()
        {
            //Posts.Clear();
            //OnlineFriends.Clear();
            //SearchResults.Clear();
            //Notifications.Clear();
            //LikedRecipes.Clear();
            //SavedRecipes.Clear();

            _userSession.SetOffline();
            Application.Current.MainPage = new LogInPage(new UserViewModel(_userSession));
        }

        private Timer notificationTimer;
        bool windowloaded;

        [RelayCommand]
        public async Task Appearing()
        {
            if(!windowloaded)   // appearing command invoked 2 times for some reason
            {
                windowloaded = true;
                await FetchNotifications();
                var base64 = await FetchProfilePictureBase64();
                if (base64 is null) return;
                await SetProfilePictureFromBase64(base64);
                await MainWallPostThresholdExceed();
                await UpdateFriendList();
            }
                
            //if(notificationTimer == null)
            //{
            //    notificationTimer = new Timer(async _ => await FetchNotifications(), null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
            //}
        }

        private async Task<string> FetchProfilePictureBase64()
        {
            using(var http = new HttpClient())
            {
                http.BaseAddress = new Uri(API_BASE_URL);

                var endpoint = $"api/user/get-profile-picture-base64/{_userSession.Id}";

                try
                {
                    var response = await http.GetAsync(endpoint);

                    if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        ProfilePictureSource = "avatar.jpg";
                        return null;
                    }
                    return await response.Content.ReadAsStringAsync();

                }catch(Exception ex)
                {
                    NotifiyFailedAction("could not retrive profile picture.");
                    return null;
                }
            }
        }

        private async Task FetchNotifications()
        {
            using(var httpClient = new HttpClient()) {

                httpClient.BaseAddress = new Uri(API_BASE_URL);
                try
                {
                    var endpoint = $"api/notifications/get-all-for-user/{_userSession.Id}";
                    var response = await httpClient.GetAsync(endpoint);

                    if(!response.IsSuccessStatusCode)
                    {
                        throw new Exception();
                    }

                    var results = await response.Content.ReadAsStringAsync();

                    var notifications = await JsonToObject<List<NotificationDto>>(results);

                    HandleNotificationsUpdate(notifications);
                }
                catch
                {
                    NotifiyFailedAction("Something went wrong...");
                }
            }
        }

        List<int> seenPostId = new List<int>();

        [ObservableProperty]
        bool wallPostLoadingActivityIndicatorVisible;

        [RelayCommand]
        public async Task MainWallPostThresholdExceed()
        {
            WallPostLoadingActivityIndicatorVisible = true;
            using (var http = new HttpClient())
            {              
                try
                {
                    var endpoint = $"/api/posts/generate-wall-posts?userId={_userSession.Id}";
                    var content = new StringContent(JsonConvert.SerializeObject(seenPostId), Encoding.UTF8, "application/json");

                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        RequestUri = new Uri(API_BASE_URL + endpoint),
                        Content = content
                    };

                    var response = await http.SendAsync(request);

                    if(!response.IsSuccessStatusCode) { throw new Exception(); }

                    var results = await response.Content.ReadAsStringAsync();
                    var dtos = await JsonToObject<List<PostDto>>(results);
                    seenPostId.AddRange(dtos.Select(dtos => dtos.PostId));
                    await DisplayPosts(dtos,Posts);
                }
                catch
                {
                    NotifiyFailedAction("Could not load new post at the moment, try again later.");
                }
                finally
                {
                    WallPostLoadingActivityIndicatorVisible = false;
                }
            }
        }


        private async Task HandleNotificationsUpdate(List<NotificationDto> notifications)
        {
            notifications.Sort((x,y) => y.CreatedAt.CompareTo(x.CreatedAt));

            ObservableCollection<INotification> newNotifications = new ObservableCollection<INotification>();

            foreach(var notification in notifications)
            {
                switch(notification.Type)
                {
                    case NotificationType.FriendRequest: //0
                        newNotifications.Add(new FriendRequestNotification()
                        {
                            Message = notification.Message,
                            UserId = notification.SenderId.ToString(),
                            NotifcationId = notification.Id,
                            ImageBase64 = notification.Base64
                        });
                        break;
                    case NotificationType.AcceptedFriendRequest: //5
                        newNotifications.Add(new BasicNotofication()
                        {
                            Message = notification.Message,
                            UserId = notification.SenderId.ToString(),
                            Type = NotificationType.AcceptedFriendRequest,
                            NotifcationId = notification.Id,
                            ImageBase64 = notification.Base64
                        });
                        break;
                    case NotificationType.PostLike: //1
                        newNotifications.Add(new PostLikeNotification()
                        {
                            Message = notification.Message,
                            PostId = notification.PostId.ToString(),
                            UserId = notification.SenderId.ToString(),
                            NotifcationId = notification.Id,
                            ImageBase64 = notification.Base64
                        });
                        break;
                    case NotificationType.PostComment: //2
                        newNotifications.Add(new PostCommentNotification()
                        {
                            Message = notification.Message,
                            UserId = notification.SenderId.ToString(),
                            CommentId = notification.CommentId.ToString(),
                            PostId = notification.PostId.ToString(),
                            NotifcationId = notification.Id,
                            ImageBase64 = notification.Base64
                        });
                        break;
                    case NotificationType.CommentLike: //3
                        newNotifications.Add(new CommentLikeNotification()
                        {
                            Message = notification.Message,
                            UserId = notification.SenderId.ToString(),
                            CommentId = notification.CommentId.ToString(),
                            NotifcationId = notification.Id,
                            ImageBase64 = notification.Base64
                        });
                        break;
                    case NotificationType.GainFollower: //4
                        newNotifications.Add(new BasicNotofication()
                        {
                            Message = notification.Message,
                            UserId = notification.SenderId.ToString(),
                            Type = NotificationType.GainFollower,
                            NotifcationId = notification.Id,
                            ImageBase64 = notification.Base64
                        });
                        break;
                }
            }
            Notifications.Clear();
            allNotifications.Clear();
            allNotifications = newNotifications;
            DisplayNotifications(20, newNotifications);        
        }

        private ObservableCollection<INotification> allNotifications = new ObservableCollection<INotification>();
        private int currentNotificationsDisplayCount = 0;

        private async void DisplayNotifications(int displayCount,ObservableCollection<INotification> notifications)
        {
            var temp = currentNotificationsDisplayCount;
            for (int i = currentNotificationsDisplayCount; i <= temp+displayCount; i++)
            {
                try
                {
                    Notifications.Add(notifications[i]);
                    currentNotificationsDisplayCount++;

                }catch(ArgumentOutOfRangeException e)
                {                   
                    break;
                }
            }
        }

        public void Dispose()
        {
            notificationTimer?.Dispose();
        }

        [RelayCommand]
        public async void ShowCommentedPost((string post,string comment) id)
        {
            var post = await GetPopupPost(id.post,id.comment);

            if(post.PostImagesBase64 is null)
            {
                var popup = new CommentedPostPopup(post.CommentUserId,
                post.CommentProfilePictureImageBase64,
                post.CommentUsername,
                post.CommentContent,
                post.CommentLikes.ToString())
                {
                    Username = post.Username,
                    TimeStamp = "10 hours ago",
                    PostTextContent = post.Description,
                    PostLikeCount = post.Likes.ToString(),
                };

                popup.SetImagesVisiblity(false);
                App.Current.MainPage.ShowPopup(popup);
            }
            else
            {
                App.Current.MainPage.ShowPopup(new CommentedPostPopup(post.CommentUserId,
                post.CommentProfilePictureImageBase64,
                post.CommentUsername,
                post.CommentContent,
                post.Likes.ToString())
                {
                    Username = post.Username,
                    TimeStamp = "10 hours ago",
                    PostTextContent = post.Description,
                    PostLikeCount = post.Likes.ToString(),
                    ImageSource = post.PostImagesBase64[0],
                    ImagesBase64 = post.PostImagesBase64
                });
            }
            
        }

        private async Task<PopupPostDto> GetPopupPost(string postId,string commentId)
        {
            using (var httpclient = new HttpClient())
            {
                httpclient.BaseAddress = new Uri(API_BASE_URL);
                var endpoint = $"api/posts/popup-post/{postId}/{commentId}";
                try
                {
                    var respose  = await httpclient.GetAsync(endpoint);

                    if(!respose.IsSuccessStatusCode)
                    {

                    }

                    var json = await respose.Content.ReadAsStringAsync();

                    var obj = await JsonToObject<PopupPostDto>(json);

                    return obj;
                }
                catch
                {
                    return null;
                }
            }
           
        }

        [RelayCommand]
        public async void OpenCommentEditor(string commentId)
        {
            var popup = new EditCommentPopup(commentId);
            popup.Closed += DisposeEditCommentPopup;
            Application.Current.MainPage.ShowPopup(popup);
        }

        [RelayCommand]
        public async void DeleteComment(string commentId)
        {
            using(var http = new HttpClient())
            {
                var endpoint = $"api/comments/delete-comment-{commentId}";
                http.BaseAddress = new Uri(API_BASE_URL);

                try
                {
                    var response = await http.DeleteAsync(endpoint);
                    if (!response.IsSuccessStatusCode)
                    {
                        NotifiyFailedAction("Could not delete comment at the moment, try again later");
                    }
                }
                catch
                {
                    NotifiyFailedAction("internal server error, try again later.");
                }
            }
        }

        private void DisposeEditCommentPopup(object? sender, PopupClosedEventArgs e)
        {
            EditedCommentContent = string.Empty;
        }

        [ObservableProperty]
        string editedCommentContent;

        [RelayCommand]
        public async void EditComment(string commentId)
        {
            if(EditedCommentContent == string.Empty)
            {
                NotifiyFailedAction("Comment content cannot be empty");
                return;
            }

            using (var http = new HttpClient()) {

                http.BaseAddress = new Uri(API_BASE_URL);

                var content = new StringContent(JsonConvert.SerializeObject(EditedCommentContent), Encoding.UTF8, "application/json");
                var endpoint = $"api/comments/edit-comment-{commentId}";

                try
                {
                    var response = await http.PutAsync(endpoint, content);
                    if(response.IsSuccessStatusCode)
                    {
                        NotifiyFailedAction("Comment edited succesfuly");
                        return;
                    }
                }
                catch
                {

                }

            }
        }

        [RelayCommand]
        public async void AddNewComment((string postId,string commentContent) payload)
        {
            var newComment = new NewCommentDto()
            {
                UserId = _userSession.Id.ToString(),
                CommentContent = payload.commentContent
            };
            
            if (string.IsNullOrEmpty(payload.commentContent))
            {
                NotifiyFailedAction("Comment conetnt cannot be empty.");
                return;
            }

            using (var httpclient = new HttpClient())
            {
                httpclient.BaseAddress = new Uri(API_BASE_URL);

                var endpoint = $"api/comments/add-new-comment-{payload.postId}";

                var content = new StringContent(JsonConvert.SerializeObject(newComment), Encoding.UTF8, "application/json");

                try
                {
                    var response = await httpclient.PostAsync(endpoint, content);
                }
                catch
                {

                }
            }

            var post = Posts.FirstOrDefault(p => p.PostId == payload.postId);
            if (post != null)
            {
                //post.Comments.Add(new CommentView() { });
                //UPDATE CURRENT UI/ HTTP GET FOR SINGLE COMMENT REQUIRED
            }
            var profilepost = ProfilePosts.FirstOrDefault(p => p.PostId == payload.postId);
            if(profilepost != null)
            {
            }
        }

        [RelayCommand]
        public async void ShowLikedComment(string commentId)
        {
            var comment = await GetCommentById(commentId);
            App.Current.MainPage.ShowPopup(new LikedCommendPopup(
                comment.UserId.ToString(),
                comment.ImageBase64,
                comment.Username,
                comment.CommentContent,
                comment.Likes.ToString()));
        }

        private async Task<CommentDto> GetCommentById(string id)
        {
            using(var httpclient = new HttpClient())
            {
                httpclient.BaseAddress = new Uri(API_BASE_URL);

                var endpoint = $"api/comments/get-comment-{id}";

                try
                {
                    var response = await httpclient.GetAsync(endpoint);

                    var results = await response.Content.ReadAsStringAsync();

                    var comment = await JsonToObject<CommentDto>(results);
                    return comment;
                }
                catch
                {
                    return null;
                }
            }

        }

        [RelayCommand]
        public async void ShowLikedPost(string postId)
        {
            var post = await GetPopupLikedPost(postId);
            if (post.PostImagesBase64 is null)
            {
                var popup = new LikedPostPopup()
                {
                    Username = post.Username,
                    TimeStamp = "10 hours ago",
                    PostTextContent = post.Description,
                    PostLikeCount = post.Likes.ToString(),
                };

                popup.SetImagesVisiblity(false);
                App.Current.MainPage.ShowPopup(popup);
            }
            else
            {
                App.Current.MainPage.ShowPopup(new LikedPostPopup()
                {
                    Username = post.Username,
                    TimeStamp = "10 hours ago",
                    PostTextContent = post.Description,
                    PostLikeCount = post.Likes.ToString(),
                    ImageSource = post.PostImagesBase64[0],
                    ImagesBase64 = post.PostImagesBase64
                });
            }
        }

        private async Task<PopupPostDto> GetPopupLikedPost(string id)
        {
            using (var httpclient = new HttpClient())
            {
                httpclient.BaseAddress = new Uri(API_BASE_URL);
                var endpoint = $"api/posts/popup-liked-post/{id}";
                try
                {
                    var respose = await httpclient.GetAsync(endpoint);

                    if (!respose.IsSuccessStatusCode)
                    {

                    }

                    var json = await respose.Content.ReadAsStringAsync();

                    var obj = await JsonToObject<PopupPostDto>(json);

                    return obj;
                }
                catch
                {
                    return null;
                }
            }
        }

        [RelayCommand]
        private async void ClearNotifications()
        {
            List<int> NotificationsId = new List<int>();
            for (int i = 0; i <= currentNotificationsDisplayCount - 1; i++)
            {
                var notification = Notifications[0];
                if (notification is not FriendRequestNotification)
                {
                    //await notification.HideAnimation(300, 150);
                    NotificationsId.Add(notification.NotifcationId);
                    allNotifications.Remove(notification);
                    Notifications.Remove(notification);
                    //i = i - 1;
                }
            }

            using(var httpclient = new HttpClient())
            {
                //httpclient.BaseAddress = new Uri(API_BASE_URL);

                try
                {
                    var endpoint = $"/api/notifications/remove-range-notifications/{_userSession.Id}";

                    var jsonContent = new StringContent(JsonConvert.SerializeObject(NotificationsId), Encoding.UTF8, "application/json");

                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Delete,
                        RequestUri = new Uri(API_BASE_URL + endpoint),
                        Content = jsonContent
                    };

                    var response = await httpclient.SendAsync(request);


                    currentNotificationsDisplayCount = 0;
                    DisplayNotifications(10, allNotifications);
                }
                catch
                {

                }
            }


            
            //code to clear notification in database
        }

        private void OnNotificationsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            NoNotificationNotifierVisible = Notifications.Count == 0 ? true : false;
        }

        [RelayCommand]
        public void ShowAddPostForm()
        {
            this.AddPostFormVisible = true;
            PostContentEditorVisible = true;
        }

        [RelayCommand]
        public void HideAddPostForm()
        {
            Tags.Clear();
            alltags.Clear();
            PickedTags.Clear();

            allproducts.Clear();
            Products.Clear();
            PickedProducts.Clear();

            this.AddPostFormVisible = false;
            PostContent = string.Empty;
            FilterParam = string.Empty;
            this.TagPickerVisible = false;
            ProductPickerVisible = false;
        }

        [RelayCommand]
        public void ApplyTags()
        {
            TagPickerVisible = false;
            PostContentEditorVisible = true;
            Tags.Clear();
            alltags.Clear();
        }

        //ObservableCollection<PostImageView> addPostImages = new ObservableCollection<PostImageView>();
        public ObservableCollection<PostImageView> AddPostImages { get; set; } = new ObservableCollection<PostImageView>();

        [RelayCommand]
        public async void UploadPostImages()
        {
            var fileResult = await FilePicker.Default.PickMultipleAsync(new PickOptions
                {
                    PickerTitle = "Choose maximum 10 images.",
                    FileTypes = FilePickerFileType.Images               
                });

            if (fileResult is null) return;

            var selectedFiles = fileResult.Take(10).ToList();
            foreach ( var file in selectedFiles )
            {
                if(AddPostImages.Count < 10)
                {
                    AddPostImages.Add(new PostImageView() { ImageSource = file.FullPath });
                }
            }
        }

        [RelayCommand]
        public void UnloadImage(string path)
        {
            foreach(var image in AddPostImages)
            {
                if(image.ImageSource == path)
                {
                    AddPostImages.Remove(image);
                    return;
                }
            }
        }

        [ObservableProperty]
        Color editorBorderColor = Brush.Gray.Color;

        [ObservableProperty]
        string postContent;

        [ObservableProperty]
        bool tagPickerVisible;

        [ObservableProperty]
        bool postContentEditorVisible;

        [ObservableProperty]
        bool productPickerVisible;

        [ObservableProperty]
        bool postModelvalidationFailedNotifierVisible;

        [ObservableProperty]
        string errMsg;

        [RelayCommand]
        public async void DeletePost(string postId)
        {
            //var endpoint = $""
            int i = 0;
        }

        [RelayCommand]
        public async void AddPost()
        {
            if(string.IsNullOrEmpty(PostContent))
            {
                EditorBorderColor = Color.FromHex("#e32441");
                await Task.Delay(3000);
                EditorBorderColor = Brush.Gray.Color;
                return;
            }else if (PickedTags.Count != 4)
            {
                ErrMsg = "You must pick 4 tags to post.";
                PostModelvalidationFailedNotifierVisible = true;
                await Task.Delay(3000);
                ErrMsg = string.Empty;
                PostModelvalidationFailedNotifierVisible = false;
                return;
            }else if (!PickedProducts.Any())
            {
                ErrMsg = "You must pick atleat one recipe product";
                PostModelvalidationFailedNotifierVisible = true;
                await Task.Delay(3000);
                ErrMsg = string.Empty;
                PostModelvalidationFailedNotifierVisible = false;
                return;
            }
            
            using (var content = new MultipartFormDataContent())
            {
                content.Add(new StringContent(_userSession.Id.ToString()), "UserId");
                content.Add(new StringContent(PostContent), "Description");

                foreach (var tag in PickedTags)
                {
                    content.Add(new StringContent(tag.Id), "TagsId");
                }

                foreach(var product in PickedProducts)
                {
                    content.Add(new StringContent(product.Id), "ProductsId");
                }

                var imageStreams = new List<Stream>();

                try
                {
                    foreach (var image in AddPostImages)
                    {
                        var fileStream = new FileStream(image.ImageSource, FileMode.Open, FileAccess.Read);
                        imageStreams.Add(fileStream);

                        var streamContent = new StreamContent(fileStream);
                        var fileExtension = Path.GetExtension(image.ImageSource).ToLower();
                        string mimeType = fileExtension switch
                        {
                            ".jpg" or ".jpeg" => "image/jpeg",
                            ".png" => "image/png",
                            _ => "application/octet-stream" 
                        };
                        streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(mimeType);
                        content.Add(streamContent, "Images", Path.GetFileName(image.ImageSource));
                    }

                    using (var http = new HttpClient())
                    {
                        http.BaseAddress = new Uri(API_BASE_URL);
                        var endpoint = "api/posts/create";

                        try
                        {
                            var response = await http.PostAsync(endpoint, content);

                            if (response.IsSuccessStatusCode)
                            {

                            }
                        }
                        catch (Exception ex)
                        {
                            //Console.WriteLine($"Error in sending request: {ex.Message}");
                        }
                     }
                }
                finally
                {
                    foreach (var stream in imageStreams)
                    {
                        stream.Dispose();
                    }
                }


            }

        }


        //ObservableCollection<TagView> tags = new ObservableCollection<TagView>();

        public ObservableCollection<TagView> Tags { get; set; } = new ObservableCollection<TagView>();

        [ObservableProperty]
        bool addPostFormActivityIndicatorVisible;

        [RelayCommand]
        public async void ChooseTags()
        {
            AddPostFormActivityIndicatorVisible = true;
            Tags.Clear();
            using(var http = new HttpClient())
            {
                http.BaseAddress = new Uri(API_BASE_URL);
                var endpoint = "/get-all-tags";

                try
                {
                    var response = await http.GetAsync(endpoint);
                    var results = await response.Content.ReadAsStringAsync();
                    var tagslist = await JsonToObject<List<TagView>>(results);
                    foreach(var tag in tagslist)
                    {
                        if (PickedTags.FirstOrDefault(t => t.Id == tag.Id) is not null)
                        {
                            tag.FrameBackground = Brush.Green.Color;
                        }
                        else
                        {
                            tag.FrameBackground = (Color)Application.Current.Resources["TagViewFrameBackground"];
                        }
                        Tags.Add(tag);
                        alltags.Add(tag);
                    }
                }
                catch
                {
                    AddPostFormVisible = false;
                    NotifiyFailedAction("Could not retrive tags from the server, try again later.");
                }
                finally
                {
                    TagPickerVisible = true;
                    PostContentEditorVisible = false;
                    AddPostFormActivityIndicatorVisible = false;
                }
            }
        }

        //static ObservableCollection<TagView> pickedtags = new ObservableCollection<TagView>();

        [MaxLength(4)]
        public static ObservableCollection<TagView> PickedTags { get; set; } = new ObservableCollection<TagView>();

        [ObservableProperty]
        string filterParam;

        [RelayCommand]
        public async void PickTag(string id)
        {
            var tag = PickedTags.FirstOrDefault(t => t.Id == id);
            if (tag is not null)
            {
                PickedTags.Remove(tag);
                return;
            }

            if (PickedTags.Count < 4)
            {              
                PickedTags.Add(Tags.First(t => t.Id == id));
            }
        }

        ObservableCollection<TagView> alltags = new ObservableCollection<TagView>();
        [ObservableProperty]
        bool tagsActivityIndicatorVisible;

        [RelayCommand]
        public async Task FilterTags()
        {
            if (!AddPostFormVisible) return;

            try
            {
                TagsActivityIndicatorVisible = true;
                if (FilterParam == string.Empty)
                {
                    Tags.Clear();
                    foreach (var tag in alltags)
                    {                      
                        Tags.Add(tag);
                    }
                    return;
                }
                var newtags = alltags.Where(t => t.Name.Contains(FilterParam))
                           .ToObservableCollection();
                Tags.Clear();
                foreach (var tag in newtags)
                {      
                    Tags.Add(tag);                 
                }
            }
            catch
            {

            }
            finally
            {
                foreach (var tag in Tags)
                {
                    if (PickedTags.Contains(tag))
                    {
                        tag.FrameBackground = Brush.Green.Color;
                    }
                }
                TagsActivityIndicatorVisible = false;
            }
        }


        ObservableCollection<ProductView> allproducts = new ObservableCollection<ProductView>();
        public ObservableCollection<ProductView> Products { get; set; } = new ObservableCollection<ProductView>();
        public static ObservableCollection<ProductView> PickedProducts { get; set; } = new ObservableCollection<ProductView>();

        [RelayCommand]
        public async Task LoadProducts()
        {
            AddPostFormActivityIndicatorVisible = true;
            var endpoint = "/get-all-products";

            using(var http = new HttpClient())
            {
                http.BaseAddress = new Uri(API_BASE_URL);

                try
                {
                    var response = await http.GetAsync(endpoint);

                    var resutls = await response.Content.ReadAsStringAsync();

                    var products = await JsonToObject<ObservableCollection<ProductView>>(resutls);
                    allproducts = new ObservableCollection<ProductView>(products);

                    foreach(var product in allproducts)
                    {
                        if (PickedProducts.FirstOrDefault(t => t.Id == product.Id) is not null)
                        {
                            product.FrameBackground = Brush.Green.Color;
                        }
                        else
                        {
                            product.FrameBackground = (Color)Application.Current.Resources["TagViewFrameBackground"];
                        }
                    }

                    for(int i = 0;i <= 100;i++)
                    {
                        Products.Add(new ProductView() { 
                            Id = allproducts[i].Id, 
                            Name = allproducts[i].Name, 
                            FrameBackground = allproducts[i].FrameBackground 
                        });
                    }

                }
                catch
                {
                    NotifiyFailedAction("internal server error, try again later");
                }
                finally 
                {
                    PostContentEditorVisible = false;
                    ProductPickerVisible = true;
                    AddPostFormActivityIndicatorVisible = false;
                }
            }
        }

        [RelayCommand]
        public async Task PickProduct(string id) {

            var product  = PickedProducts.FirstOrDefault(p => p.Id == id);
            if (product is not null)
            {
                PickedProducts.Remove(product);
                return;
            }

            PickedProducts.Add(Products.First(p => p.Id == id));          
        }

        [ObservableProperty]
        bool productsActivityIndicatorVisible;

        private bool _canTresholdEventBeFired = true;

        [RelayCommand]
        public async Task FilterProducts() {

            if (!AddPostFormVisible) return;

            _canTresholdEventBeFired = false;
            try
            {
                ProductsActivityIndicatorVisible = true;
                if (string.IsNullOrEmpty(FilterParam))
                {
                    _canTresholdEventBeFired = true;
                    Products.Clear();
                    for (int i = 0; i <= 100; i++)
                    {
                        Products.Add(new ProductView()
                        {
                            Id = allproducts[i].Id,
                            Name = allproducts[i].Name,
                            FrameBackground = allproducts[i].FrameBackground
                        });
                    }
                    return;
                }
                var newproducts = allproducts.Where(t => t.Name.Contains(FilterParam))
                           .ToObservableCollection();

                Products.Clear();
                foreach (var product in newproducts)
                {

                    Products.Add(product);
                }
            }
            catch
            {

            }
            finally
            {
                foreach (var product in Products)
                {
                    if (PickedProducts.Contains(product))
                    {
                        product.FrameBackground = Brush.Green.Color;
                    }
                }
                ProductsActivityIndicatorVisible = false;
            }
        }

        [RelayCommand]
        public async Task ApplyProducts()
        {
            ProductPickerVisible = false;
            PostContentEditorVisible = true;
            Products.Clear();
            allproducts.Clear();
        }

        private bool _isLoadingMore = false;
        private const double LoadThreshold = 0.8;
        private const int ItemsToLoad = 100;

        [RelayCommand]
        public async Task ProductsScroll(ScrollView scrollView)
        {
            if (!_canTresholdEventBeFired) return;

            if (_isLoadingMore || ProductsActivityIndicatorVisible)
                return;

            if (scrollView == null || scrollView.Height <= 0)
                return;

            double totalScrollableHeight = scrollView.ContentSize.Height - scrollView.Height;
            double scrolledRatio = scrollView.ScrollY / totalScrollableHeight;

            if (scrolledRatio >= LoadThreshold)
            {
                _isLoadingMore = true;
                ProductsActivityIndicatorVisible = true;

                await LoadMoreProducts();

                ProductsActivityIndicatorVisible = false;
                _isLoadingMore = false;
            }
        }

        private async Task LoadMoreProducts()
        {
            int currentCount = Products.Count();
            int maxCount = allproducts.Count();

            for (int i = currentCount; i < Math.Min(currentCount + ItemsToLoad, maxCount); i++)
            {
                Products.Add(new ProductView
                {
                    Id = allproducts[i].Id,
                    Name = allproducts[i].Name,
                    FrameBackground = allproducts[i].FrameBackground
                });
            }
        }

        [RelayCommand]
        public void NotificationsThresholdExceed(ItemsViewScrolledEventArgs e)
        {
           DisplayNotifications(10, allNotifications);
        }

        [RelayCommand]
        public void ToMainView()
        {
            this.PostPageVisible = true;
            this.ProfilePageVisible = false;
            this.SettingsPageVisible = false;
            this.RecipePageVisible = false;
        }

        [RelayCommand]
        public async void ToProfileView()
        {
            this.PostPageVisible = false;
            this.ProfilePageVisible = true;
            this.SettingsPageVisible = false;
            this.RecipePageVisible = false;

            ProfilePosts.Clear();
            ProfileFollowersList.Clear();
            ProfileFriendsList.Clear();
            ShowUserProfile(_userSession.Id.ToString());
        }

        [RelayCommand]
        public void ToRecipesView()
        {
            this.PostPageVisible = false;
            this.ProfilePageVisible = false;
            this.SettingsPageVisible = false;
            this.RecipePageVisible = true;
            ShowLikedRecipes();
        }

        [RelayCommand]
        public void ToSettingView()
        {
            this.PostPageVisible = false;
            this.ProfilePageVisible = false;
            this.SettingsPageVisible = true;
            this.RecipePageVisible = false;
        }

        [RelayCommand]
        public async void ShowPopup(string id)
        {
            var profileJson = await GetUserProfileModel(id);
            var user = await JsonToObject<UserProfileModel>(profileJson);

            var popup = new UserOptionPopup(user.IsFollowed,user.IsFriend,user.HasPendingFriendRequest)
            {
                UserId = user.Id.ToString(), 
                Username = user.Username, 
                AvatarImageSource = user.ProfilePictureBase64,
            };

            App.Current.MainPage.ShowPopup(popup);
        }


        [RelayCommand]
        public async void OpenPersonalDataEditor()
        {
            await Task.Delay(100);
            this.PersonalDataEditorVisible = true;
            this.SettingsMainHubVisible = false;
            ShowChangeUsernameEntry();
        }

        [RelayCommand]
        public void OpenSettingsLandingPage()
        {
            this.PersonalDataEditorVisible = false;
            this.SettingsMainHubVisible = true;
        }

        [RelayCommand]
        public void BackToSettingHub()
        {
            this.PersonalDataEditorVisible = false;
            this.SettingsMainHubVisible = true;
        }

        [RelayCommand]
        public void ShowChangeUsernameEntry(){
            this.ChangeUsernameEntryVisible = true; 
            this.ChangeEmailEntryVisible = false; 
            this.ChangePasswordEntryVisible = false;
            this.ChangeProfilePictureVisible = false;

            ClearDataChangeEntries();
        }

        [RelayCommand]
        public void ShowChangeEmialEntry(){
            this.ChangeUsernameEntryVisible = false;
            this.ChangeEmailEntryVisible =true; 
            this.ChangePasswordEntryVisible = false;
            this.ChangeProfilePictureVisible = false;

            ClearDataChangeEntries();
        }

        [RelayCommand]
        public void ShowChangePasswordEntry(){
            this.ChangeUsernameEntryVisible = false;
            this.ChangeEmailEntryVisible = false;
            this.ChangePasswordEntryVisible = true;
            this.ChangeProfilePictureVisible = false;

            ClearDataChangeEntries();
        }

        [RelayCommand]
        public void ChangeProfilePicture()
        {
            this.ChangeUsernameEntryVisible = false;
            this.ChangeEmailEntryVisible = false;
            this.ChangePasswordEntryVisible = false;
            this.ChangeProfilePictureVisible = true;

            ClearDataChangeEntries();
        }

        private void ClearDataChangeEntries()
        {
            this.ChangedUsername = string.Empty;
            this.ChangedEmail = string.Empty;
            this.ChangedPassword = string.Empty;
            this.ChangedRePassword = string.Empty;
            this.ChangedImageProfilePicturePath = string.Empty;
        }

        [RelayCommand]
        public async void ChooseNewProfilePicture()
        {
            var fileResult = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Pick your new profile picture wisely :)",
                FileTypes = FilePickerFileType.Images,
            });

            if(fileResult is null)
            {
                return;
            }

            this.ChangedImageProfilePicturePath = fileResult.FullPath;
        }

        [RelayCommand]
        public async void SaveChangedPersonalData()
        {
            string endpoint = "";
            StringContent content = null;
            if (this.ChangeUsernameEntryVisible)
            {
                endpoint = Regex.IsMatch(ChangedUsername, "^(?=.*[a-zA-Z])[a-zA-Z0-9_]+$") ? $"api/user/change-username/{_userSession.Id}" : string.Empty;
                var json = JsonConvert.SerializeObject(ChangedUsername);
                content = new StringContent(json,Encoding.UTF8,"application/json");
            }
            else if (this.ChangeEmailEntryVisible)
            {
                endpoint = Regex.IsMatch(ChangedEmail, @"^([\w-\.]+@([0-9a-zA-Z-.拉着,\u4e00-\u9fff]+)\.[a-zA-Z]{2,5})+$") ? $"api/user/change-email/{_userSession.Id}" : string.Empty;
                var json = JsonConvert.SerializeObject(ChangedEmail);
                content = new StringContent(json, Encoding.UTF8, "application/json");
            }
            else if (this.ChangePasswordEntryVisible)
            {
                if (ChangedPassword != ChangedRePassword) HandleMissmatchPassword();
                endpoint = Regex.IsMatch(ChangedPassword, "^(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d]{8,}$") ? $"api/user/change-password/{_userSession.Id}" : string.Empty;
                var json = JsonConvert.SerializeObject(ChangedPassword);
                content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            if(endpoint != string.Empty && content is not null)
             await UpdatePersonalData(endpoint,content);
        }

        private void HandleMissmatchPassword()
        {
            //
            //sadasdasd
            //
        }

        private async Task UpdatePersonalData(string endpoint,StringContent contnet)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(API_BASE_URL);

                try
                {
                    var response = await client.PutAsync(endpoint, contnet);
                }
                catch(Exception ex)
                {

                }
               
            }
        }

        public bool CanShowSearchPanel { get; set; }
        [RelayCommand]
        public void ShowSearchPanel()
        {
            if (CanShowSearchPanel)
            {
                this.SearchPanelVisible = true;
                DisplaySearchResultHistory();
            }
        }

        [RelayCommand]
        public async Task HideSearchPanel()
        {
            Task.Delay(200).Wait();
            this.SearchPanelVisible = false;
            CanShowSearchPanel = false;
        }

        [RelayCommand]
        public async Task SetSearchPanelVisibilityToTrue()
        {
            CanShowSearchPanel = true;
        }

        [RelayCommand]
        public async Task SetSearchPanelVisibilityToFalse()
        {
            if(!SearchPanelVisible) {
                CanShowSearchPanel = false;
            }
        }

        [RelayCommand]
        public void ShowHubPanel()
        {
            this.HubPanelVisible = true;
        }

        [RelayCommand]
        public void HideHubPanel()
        {
            this.HubPanelVisible = false;
        }

        [RelayCommand]
        public async void ShowUserProfile(string id)
        {
            ProfileAddFriendAndFollowButtonsVisible = false;
            this.ProfileFollowersVisible = false;
            this.ProfilePostsVisible = true;
            this.ProfileFriendsVisible = false;
            SetButtonColors(Buttons.PostButton);
            try
            {
                if (id != _userSession.Id.ToString())
                {
                    CreateSearchResultHistory(id);
                    this.PostPageVisible = false;
                    this.ProfilePageVisible = true;
                    this.SettingsPageVisible = false;
                    this.RecipePageVisible = false;
                    ProfilePosts.Clear();
                    ProfileFollowersList.Clear();
                    ProfileFriendsList.Clear();
                    ProfileAddFriendAndFollowButtonsVisible = true;
                }

                OpenUserProfile(id);
            }catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }

        [RelayCommand]
        public void ShowUserProfilePopup(string id)
        {
            ProfilePosts.Clear();
            ProfileFollowersList.Clear();
            ProfileFriendsList.Clear();

            this.PostPageVisible = false;
            this.ProfilePageVisible = true;
            this.SettingsPageVisible = false;
            this.RecipePageVisible = false;

            this.ProfileFollowersVisible = false;
            this.ProfilePostsVisible = true;
            this.ProfileFriendsVisible = false;
            SetButtonColors(Buttons.PostButton);
            try
            {
                ProfileAddFriendAndFollowButtonsVisible = true;
                OpenUserProfile(id);
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }

        [ObservableProperty]
        private Color _selfPostButtonColor = Color.FromHex("#ffffff");

        [ObservableProperty]
        private Color _friendsButtonColor = Color.FromHex("#c9c9c9");

        [ObservableProperty]
        private Color _followersButtonColor = Color.FromHex("#c9c9c9");

        private Color lightThemeClickedButton  = Color.FromHex("#ffffff");
        private Color lightThemeUnClickedButton = Color.FromHex("#c9c9c9");

        private Color darkThemeClickedButton = Color.FromHex("#212121");
        private Color darkThemeUnClickedButton = Color.FromHex("#333333");

        [ObservableProperty]
        private Color _currentClickedButtonColor;
        [ObservableProperty]
        private Color _currentUnclickedButtonColor;

        private enum Buttons
        {
            PostButton,
            FriendsButton,
            FollowersButton
        }

        private async void ReloadProfileButtonColors(bool isDarkTheme)
        {
            if(isDarkTheme)
            {
                CurrentClickedButtonColor = darkThemeClickedButton;
                CurrentUnclickedButtonColor = darkThemeUnClickedButton;
            }
            else
            {
                CurrentClickedButtonColor = lightThemeClickedButton;
                CurrentUnclickedButtonColor = lightThemeUnClickedButton;
            }

            if (ProfilePostsVisible){
                await SetButtonColors(Buttons.PostButton);
            }
            else if(ProfileFriendsVisible) {
                await SetButtonColors(Buttons.FriendsButton);
            }
            else if(ProfileFollowersVisible) {
                await SetButtonColors(Buttons.FollowersButton);
            }

        }

        private async Task SetButtonColors(Buttons button)
        {

            switch (button)
            {
                case Buttons.PostButton:
                    SelfPostButtonColor = CurrentClickedButtonColor;
                    FriendsButtonColor = CurrentUnclickedButtonColor; 
                    FollowersButtonColor = CurrentUnclickedButtonColor;
                    break;

                case Buttons.FriendsButton:
                    SelfPostButtonColor = CurrentUnclickedButtonColor;
                    FriendsButtonColor = CurrentClickedButtonColor;
                    FollowersButtonColor = CurrentUnclickedButtonColor;
                    break;

                case Buttons.FollowersButton:
                    SelfPostButtonColor = CurrentUnclickedButtonColor;
                    FriendsButtonColor = CurrentUnclickedButtonColor;
                    FollowersButtonColor = CurrentClickedButtonColor;
                    break;
            }
        }

        [RelayCommand]
        public async void ShowProfilePosts()
        {
            this.ProfileFollowersVisible = false;
            this.ProfilePostsVisible = true;
            this.ProfileFriendsVisible = false;
            await SetButtonColors(Buttons.PostButton);

        }

        [RelayCommand]
        public async void ShowProfileFriends()
        {
            this.ProfileFollowersVisible = false;
            this.ProfilePostsVisible = false;
            this.ProfileFriendsVisible = true;
            await SetButtonColors(Buttons.FriendsButton);

        }

        [RelayCommand]
        public async void ShowProfileFollowers()
        {
            this.ProfileFollowersVisible = true;
            this.ProfilePostsVisible = false;
            this.ProfileFriendsVisible = false;
            await SetButtonColors(Buttons.FollowersButton);

        }

        [RelayCommand]
        public async Task LikePost(string id)
        {
            var endpoint = $"api/posts/like-post/{_userSession.Id}?postId={id}";

            using (var http = new HttpClient())
            {
                http.BaseAddress = new Uri(API_BASE_URL);

                try
                {
                    var response = await http.PostAsync(endpoint, null);

                    if (!response.IsSuccessStatusCode) throw new Exception(await response.Content.ReadAsStringAsync());

                }
                catch (Exception e)
                {
                    NotifiyFailedAction(e.Message);
                }
                finally
                {
                    var post = Posts.FirstOrDefault(p => p.PostId == id);

                    if (post is not null)
                    {
                        var postLikes = Convert.ToInt32(post.PostLikeCount);
                        postLikes += 1;
                        post.PostLikeCount = postLikes.ToString();
                        post.IsLiked = true;
                    }

                    var post2 = ProfilePosts.FirstOrDefault(p => p.PostId == id);

                    if (post2 is not null)
                    {
                        var postLikes = Convert.ToInt32(post2.PostLikeCount);
                        postLikes += 1;
                        post2.PostLikeCount = postLikes.ToString();
                        post2.IsLiked = true;
                    }
                }
            }
        }

        [RelayCommand]
        public async Task UnlikePost(string id)
        {

            var endpoint = $"api/posts/unlike-post/{_userSession.Id}?postId={id}";

            using (var http = new HttpClient())
            {
                http.BaseAddress = new Uri(API_BASE_URL);

                try
                {
                    var response = await http.DeleteAsync(endpoint);

                    if (!response.IsSuccessStatusCode) throw new Exception(await response.Content.ReadAsStringAsync());

                }
                catch (Exception e)
                {
                    NotifiyFailedAction(e.Message);
                }
                finally
                {
                    var post = Posts.FirstOrDefault(p => p.PostId == id);

                    if (post is not null)
                    {
                        var postLikes = Convert.ToInt32(post.PostLikeCount);
                        postLikes -= 1;
                        post.PostLikeCount = postLikes.ToString();
                        post.IsLiked = false;
                    }

                    var post2 = ProfilePosts.FirstOrDefault(p => p.PostId == id);

                    if (post2 is not null)
                    {
                        var postLikes = Convert.ToInt32(post2.PostLikeCount);
                        postLikes -= 1;
                        post2.PostLikeCount = postLikes.ToString();
                        post2.IsLiked = false;
                    }
                }
            }

        }

        [RelayCommand]
        public async void SaveRecipe(string postId)
        {
            var endpoint = $"api/recipes/save/{_userSession.Id}/{postId}";

            using (var http = new HttpClient())
            {
                http.BaseAddress = new Uri(API_BASE_URL);

                try
                {
                    var response = await http.PostAsync(endpoint, null);

                    if (!response.IsSuccessStatusCode)
                    {
                        var message = await response.Content.ReadAsStringAsync();
                        NotifiyFailedAction(message);
                        return;
                    }
                    NotifiyFailedAction("recipe saved succesfuly");
                }
                catch
                {
                    NotifiyFailedAction("internal server error, try again later");
                }
                finally
                {
                    var post = Posts.FirstOrDefault(p => p.PostId == postId);

                    if (post is not null)
                    {
                        post.IsSaved = true;
                    }

                    var post2 = ProfilePosts.FirstOrDefault(p => p.PostId == postId);

                    if (post2 is not null)
                    {
                        post2.IsSaved = true;
                    }
                }
            }
        }

        [RelayCommand]
        public async void Search()
        {

            SearchResults.Clear();

            if (SearchParam == string.Empty) { DisplaySearchResultHistory(); return; }

            var endpoint = $"api/user/search-users/{SearchParam}/{_userSession.Id}";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(API_BASE_URL);

                try
                {
                    var response = await client.GetAsync(endpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        DisplaySearchResults(await JsonToObject<ObservableCollection<UserSearchResult>>(json));

                    }
                }
                catch(Exception ex)
                {
                    NotifiyFailedAction("Could not load search results due to service maintance. Try again later");
                    //code to show Something went wrong on search panel.
                }
            }

        }

        [RelayCommand]
        public async void DeclineFriendRequest((string senderId, int notificationId) ids)
        {
            var endpoint = $"api/friends/request/delete/{ids.senderId}/{_userSession.Id}";

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(API_BASE_URL);

                try
                {
                    var response = await httpClient.DeleteAsync(endpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        var notification = Notifications.FirstOrDefault(n => n.NotifcationId == ids.notificationId);
                        Notifications.Remove(notification);
                    }
                }
                catch
                {
                    NotifiyFailedAction("Cant send friend request");
                    // code to handle unsuccsesful friend request.
                }
            }
        }

        [RelayCommand]
        public async void AcceptFriendRequest((string senderId,int notificationId) ids)
        {
            var endpoint = $"api/friends/request/accept/{ids.senderId}/{_userSession.Id}";

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(API_BASE_URL);

                try
                {
                    var response = await httpClient.PostAsync(endpoint, null);

                    if (response.IsSuccessStatusCode)
                    {
                        var notification = Notifications.FirstOrDefault(n => n.NotifcationId == ids.notificationId);
                        Notifications.Remove(notification);
                    }
                }
                catch
                {
                    NotifiyFailedAction("Cant send friend request");
                    // code to handle unsuccsesful friend request.
                }
            }
        }

        [RelayCommand]
        public async void AddToFriends(string id)
        {
            var endpoint = $"api/friends/add/{_userSession.Id}/{id}";

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(API_BASE_URL);

                try
                {
                    var response = await httpClient.PostAsync(endpoint, null);

                    if (response.IsSuccessStatusCode)
                    {

                    }
                }
                catch
                {
                    NotifiyFailedAction("Cant send friend request");
                    // code to handle unsuccsesful friend request.
                }
            }

            WeakReferenceMessenger.Default.Send<string, string>("close", "popup");
        }

        [RelayCommand]
        public async void UnfriendUser(string id)
        {
            var endpoint = $"api/friends/unfriend/{id}/{_userSession.Id}";

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(API_BASE_URL);

                try
                {
                    var response = httpClient.DeleteAsync(endpoint);
                }
                catch
                {
                    NotifiyFailedAction("Something went wrong...");
                    // code to handle unsuccsesful unfriend action.
                }
            }

            WeakReferenceMessenger.Default.Send<string, string>("close", "popup");

        }

        [RelayCommand]
        public async void FollowUser(string id)
        {
            var endpoint = $"api/followers/follow/{_userSession.Id}/{id}";

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(API_BASE_URL);   

                try
                {
                    await httpClient.PostAsync(endpoint,null);
                }
                catch
                {
                    NotifiyFailedAction("Could not finish following action due to inner issues.");
                }
            }
            WeakReferenceMessenger.Default.Send<string, string>("close", "popup");
        }

        [RelayCommand]
        public async void UnfollowUser(string id)
        {
            var endpoint = $"api/followers/unfollow/{_userSession.Id}/{id}";

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(API_BASE_URL);

                try
                {
                    await httpClient.DeleteAsync(endpoint);
                }
                catch
                {
                    NotifiyFailedAction("Could not finish unfollowing action due to inner issues.");

                }
            }

            WeakReferenceMessenger.Default.Send<string, string>("close", "popup");
        }

        [RelayCommand]
        public async void CancelFriendRequest(string id)
        {
            var endpoint = $"api/friends/request/cancel/{_userSession.Id}/{id}";

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(API_BASE_URL);

                try
                {
                    var response = await httpClient.DeleteAsync(endpoint);
                }
                catch
                {
                    NotifiyFailedAction("Request was already canceled by sender.");
                    // code to handle unsuccsesful request cancel.
                }
            }
        }

        [ObservableProperty]
        bool themeFlag; //true - darktheme | true - lighttheme
        [ObservableProperty]
        string switchThemeMode = "Light Theme";

        [RelayCommand]
        public void ChangeTheme()
        {
            ThemeFlag = !ThemeFlag;

            var mergedDictionaries = Application.Current.Resources.MergedDictionaries;

            if (mergedDictionaries.Count > 2)
            {
                mergedDictionaries.Remove(mergedDictionaries.ElementAt(2));
            }

            if (ThemeFlag)
            {
                mergedDictionaries.Add(new DarkTheme());
                SwitchThemeMode = "Dark Theme";
                ReloadProfileButtonColors(ThemeFlag);
                ReloadRecipesButtons(ThemeFlag);
            }
            else
            {
                mergedDictionaries.Add(new LightTheme());
                SwitchThemeMode = "Light Theme";
                ReloadProfileButtonColors(ThemeFlag);
                ReloadRecipesButtons(ThemeFlag);
            }
        }

        private void DisplaySearchResults(ObservableCollection<UserSearchResult> users)
        {
            SearchResults.Clear();

            if(users is null)
            {
                //hanlde no hisotry results
                return;
            }

            foreach (UserSearchResult user in users)
            {
                SearchResults.Add(new UserSearchResultView()
                {
                    UserId = user.Id.ToString(),
                    Username = user.Username,
                    Follows = user.FollowersCount.ToString(),
                    Friends = user.FriendsCount.ToString()
                });
            }
        }

        private async void DisplaySearchResultHistory()
        {

            string ApplicationDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Foodiefeed");
            string SearchHistoryJsonPath = Path.Combine(ApplicationDirectory, "searchHistory.json");

            if (!Directory.Exists(ApplicationDirectory))
            {
                Directory.CreateDirectory(ApplicationDirectory);
            }

            if (!File.Exists(SearchHistoryJsonPath))
            {
                File.Create(SearchHistoryJsonPath);
            }

            var json = await File.ReadAllTextAsync(SearchHistoryJsonPath);

            DisplaySearchResults(await JsonToObject<ObservableCollection<UserSearchResult>>(json)); 
            //add a block of code that displays that there are not search results.

        }

        private async void CreateSearchResultHistory(string userId)
        {
            const int MAX_HISTORY_SIZE = 6;

            string ApplicationDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Foodiefeed");
            string SearchHistoryJsonPath = Path.Combine(ApplicationDirectory, "searchHistory.json");

            if (!Directory.Exists(ApplicationDirectory))
            {

                Directory.CreateDirectory(ApplicationDirectory);
            }
            if (!File.Exists(SearchHistoryJsonPath))
            {
                File.Create(SearchHistoryJsonPath);
            }

            UserSearchResult usr = await GetUserSearchResult(userId);

            if (usr is null) return;

            var json = File.ReadAllText(SearchHistoryJsonPath);
            var SearchHistory = await JsonToObject<ObservableCollection<UserSearchResult>>(json);

            

            var existingUser = SearchHistory.FirstOrDefault(x => x.Id == usr.Id);

            if (existingUser != null)
            {
                SearchHistory.Remove(existingUser);

                SearchHistory.Insert(0, existingUser);

            }
            else
            {
                if (SearchHistory.Count >= MAX_HISTORY_SIZE)
                {
                    SearchHistory.RemoveAt(SearchHistory.Count - 1);
                }
                SearchHistory.Insert(0, usr);
            }

            var saveJson = JsonConvert.SerializeObject(SearchHistory);

            await File.WriteAllTextAsync(SearchHistoryJsonPath, saveJson);
        }

        private async Task<UserSearchResult> GetUserSearchResult(string id){

            using (var httpclient = new HttpClient())
            {
                httpclient.BaseAddress = new Uri(API_BASE_URL);
                var endpoint = $"api/user/{id}";

                try
                {
                    var response = await httpclient.GetAsync(endpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        var results = await response.Content.ReadAsStringAsync();
                        if(results is null && results == string.Empty) { return null; }
                        return JsonConvert.DeserializeObject<UserSearchResult>(results);
                    }
                }catch(Exception ex)
                {
                    NotifiyFailedAction("Something went wrong...");

                    //block of code handling execption
                }
                return null;
            }
        }

        public async Task UpdateFriendList()
        {
            while (true)
            {
                OnlineFriends.Clear();
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(API_BASE_URL);

                    try
                    {
                        var endpoint = $"api/friends/online/{_userSession.Id}";

                        var response = httpClient.GetAsync(endpoint).Result;
                        Console.WriteLine("2");
                        var json = response.Content.ReadAsStringAsync().Result;

                        var onlineFriends = JsonToObject<List<ListedFriendDto>>(json).Result;

                        foreach(var friend in onlineFriends)
                        {
                            OnlineFriends.Add(new OnlineFreidnListElementView()
                            {
                                UserId = friend.Id.ToString(),
                                Username = friend.Username,
                                AvatarImageSource = friend.ProfilePictureBase64,
                                IsOnline = true
                            });
                        }
                    }
                    catch(Exception ex)
                    {
                        NotifiyFailedAction("Something went wrong...");
                    }

                    try
                    {
                        var endpoint = $"api/friends/offline/{_userSession.Id}";

                        var response = httpClient.GetAsync(endpoint).Result;

                        var json = response.Content.ReadAsStringAsync().Result;
                        var offlineFriends = JsonToObject<List<ListedFriendDto>>(json).Result;

                        foreach (var friend in offlineFriends)
                        {
                            OnlineFriends.Add(new OnlineFreidnListElementView()
                            {
                                UserId = friend.Id.ToString(),
                                Username = friend.Username,
                                AvatarImageSource = friend.ProfilePictureBase64,
                                IsOnline = false
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        NotifiyFailedAction("Something went wrong...");
                    }
                }
                await Task.Delay(60000);
            }
        }

        private async Task OpenUserProfile(string id)
        {
            try
            {
                LoadingLabelVisible = true;

                var profileJson = await GetUserProfileModel(id);
                var profile = await JsonToObject<UserProfileModel>(profileJson);

                if (profile is null) { throw new Exception("Profile not found."); }

                SetUserProfileModel(profile.Id,
                                    profile.FriendsCount + " friends",
                                    profile.FollowsCount + " follows",
                                    profile.LastName,
                                    profile.FirstName,
                                    profile.Username,
                                    profile.ProfilePictureBase64);

                var postsTask = GetUserProfilePosts(id);
                var friendsTask = GetUserProfileFriends(id);
                var followersTask = GetUserProfileFollowers(id);

                await Task.WhenAll(postsTask, friendsTask, followersTask);

                var posts = await JsonToObject<List<PostDto>>(await postsTask);
                DisplayPosts(posts,ProfilePosts);

                var friends = await JsonToObject<List<ListedFriendDto>>(await friendsTask);
                DisplayProfileFriends(friends);

                var followers = await JsonToObject<List<ListedFriendDto>>(await followersTask);
                DisplayProfileFollowers(followers);
            }
            catch (Exception ex)
            {
                NotifiyFailedAction("Could not load user profile.");
            }
            finally
            {
                LoadingLabelVisible = false;
            }
        }

        private async void DisplayProfileFollowers(List<ListedFriendDto> followers)
        {
            //ProfileFollowersList.Clear();
            foreach (var follower in followers)
            {
                ProfileFollowersList.Add(new OnListFriendView()
                {
                    Username = follower.Username,
                    UserId = follower.Id.ToString(),
                    AvatarImageSource = follower.ProfilePictureBase64
                });
            }
        }

        private async void DisplayProfileFriends(List<ListedFriendDto> friends) 
        {
            foreach (var friend in friends)
            {
                ProfileFriendsList.Add(new OnListFriendView()
                {
                    Username = friend.Username,
                    UserId = friend.Id.ToString(),
                    AvatarImageSource = friend.ProfilePictureBase64
                });
            }
        }

        private async Task<T> JsonToObject<T>(string json) where T : class
        {
            var obj = JsonConvert.DeserializeObject<T>(json);

            return obj;
        }

        public ObservableCollection<OnListFriendView> ProfileFriendsList { get; set; } = new ObservableCollection<OnListFriendView>();
        //private ObservableCollection<OnListFriendView> profileFriendsList = new ObservableCollection<OnListFriendView>();

        public ObservableCollection<OnListFriendView> ProfileFollowersList { get; set; } = new ObservableCollection<OnListFriendView>();
        //private ObservableCollection<OnListFriendView> profileFollowersList = new ObservableCollection<OnListFriendView>();

        public ObservableCollection<PostView> ProfilePosts { get; set; } = new ObservableCollection<PostView>();
        //private ObservableCollection<PostView> profilePosts = new ObservableCollection<PostView>();

        private async Task<string> GetUserProfilePosts(string id)
        {
            var endpoint = $"api/posts/profile-posts/{id}";

            using(var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(API_BASE_URL);
                    
                try
                {
                    var response = await httpClient.GetAsync(endpoint);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception();
                    }

                    return await response.Content.ReadAsStringAsync();                   
                }
                catch
                {
                    NotifiyFailedAction("Something went wrong...");
                    //code block to handle exeption 
                }
            }
            return string.Empty;
        }

        private async Task DisplayPosts(List<PostDto> posts,ObservableCollection<PostView> collection)
        {
            if (ProfilePageVisible && (posts == null || posts.Count == 0))
            {
                NoPostOnProfile = true;
                ProfilePostsVisible = false;
                return;
            }


            if(ProfilePageVisible) await Dispatcher.GetForCurrentThread().DispatchAsync(() => { collection.Clear(); });


            foreach (var post in posts)
            {
                var commentList = new List<CommentView>();
                foreach (var comment in post.Comments)
                {
                    var temp = comment.UserId == _userSession.Id ? true : false;
                    var newcomment = new CommentView()
                    {
                        Username = comment.Username,
                        CommentContent = comment.CommentContent,
                        CommentId = comment.CommentId.ToString(),
                        LikeCount = comment.Likes.ToString(),
                        UserId = comment.UserId.ToString(),
                        PfpImageBase64 = comment.ImageBase64,
                        EditButtonVisible = temp
                    };
                    commentList.Add(newcomment);
                }

                var imageBase64list = new List<string>();
                foreach (var image in post.PostImagesBase64)
                {
                    imageBase64list.Add(image);
                }

                try
                {
                    if (imageBase64list.Count() == 0)
                    {
                        var postview = new PostView()
                        {
                            Username = post.Username,
                            TimeStamp = post.TimeStamp,
                            PostLikeCount = post.Likes.ToString(),
                            PostTextContent = post.Description,
                            Comments = commentList,
                            PfpImageBase64 = post.ProfilePictureBase64,
                            PostId = post.PostId.ToString(),
                            PostProducts = post.ProductsName,
                            DeleteButtonVisible = post.UserId == _userSession.Id ? true : false,
                            PostImagesVisible = false,
                            LikePostCommand = LikePostCommand,
                            UnlikePostCommand = UnlikePostCommand,
                            SaveRecipeCommand = SaveRecipeCommand,
                            UnsaveRecipeCommand = DeleteSavedRecipeCommand,
                            IsLiked = post.IsLiked,
                            IsSaved = post.IsSaved,

                        };
                        collection.Add(postview);
                    }
                    else
                    {
                        var postview = new PostView()
                        {
                            Username = post.Username,
                            TimeStamp = post.TimeStamp,
                            PostLikeCount = post.Likes.ToString(),
                            PostTextContent = post.Description,
                            ImageSource = post.PostImagesBase64[0],
                            Comments = commentList,
                            ImagesBase64 = imageBase64list,
                            PfpImageBase64 = post.ProfilePictureBase64,
                            PostId = post.PostId.ToString(),
                            PostProducts = post.ProductsName,
                            DeleteButtonVisible = post.UserId == _userSession.Id ? true : false,
                            PostImagesVisible = true,
                            LikePostCommand = LikePostCommand,
                            UnlikePostCommand = UnlikePostCommand,
                            SaveRecipeCommand = SaveRecipeCommand,
                            UnsaveRecipeCommand = DeleteSavedRecipeCommand,
                            IsLiked = post.IsLiked,
                            IsSaved = post.IsSaved,
                        };
                        collection.Add(postview);
                    }
                }
                catch (Exception e)
                {
                    int i = 0;
                }
                
                await Task.Delay(100);
            }           
        }

        private async Task<string> GetUserProfileFollowers(string id)
        {
            var endpoint = $"api/followers/get-user-followers/{id}";

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(API_BASE_URL);

                try
                {
                    var response = await httpClient.GetAsync(endpoint);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception();
                    }

                    return await response.Content.ReadAsStringAsync();
                }
                catch
                {
                    NotifiyFailedAction("Something went wrong...");
                    //code block to handle exeption 
                }
            }
            return string.Empty;
        }

        private async Task<string> GetUserProfileFriends(string id)
        {
            var endpoint = $"api/friends/profile-friends/{id}";

            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(API_BASE_URL);

                try
                {
                    var response = await client.GetAsync(endpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    else if(response is null)
                    {
                        throw new Exception();
                    }
                    else if (!response.IsSuccessStatusCode)
                    {
                        NotifiyFailedAction("Something went wrong...");
                        //code of block that displays that Friends are currently unavaiable
                    }
                }
                catch (Exception e)
                {
                    NotifiyFailedAction("Something went wrong...");
                    //code block that handle exception
                }
            }
            return string.Empty;
        }

        private async Task<string> GetUserProfileModel(string id)
        {
            var endpoint = $"api/user/user-profile/{id}/{_userSession.Id}";
            
            using (var httpclient = new HttpClient())
            {
                httpclient.BaseAddress = new Uri(API_BASE_URL);

                try
                {
                    var response = await httpclient.GetAsync(endpoint);

                    if(response is null) { throw new Exception(); }

                    var json = await response.Content.ReadAsStringAsync();

                    return json;

                }
                catch(Exception ex)
                {

                }
            }
            return string.Empty;
        }

        private void SetUserProfileModel(int Id,string FriendsCount,string FollowsCount,string LastName,string FirstName,string Username,string imageBase64)
        {
            ProfileId = Id;
            ProfileFriends = FriendsCount + " friends";
            ProfileFollowers = FollowsCount + " follows";
            ProfileLastName = LastName;
            ProfileName = FirstName;
            ProfileUsername = Username;
            AvatarBase64 = imageBase64;          
        }

        [ObservableProperty]
        string failedActionMessage;

        private async void NotifiyFailedAction(string message)
        {
            FailedActionMessage = message;
            WeakReferenceMessenger.Default.Send(new FailedActionAnimationMessage("show"));
            await Task.Delay(3000);
            WeakReferenceMessenger.Default.Send(new FailedActionAnimationMessage("hide"));
        }

        [ObservableProperty]
        Color likedRecipesButtonBackround;

        [ObservableProperty]
        Color savedRecipesButtonBackround;

        private void ReloadRecipesButtons(bool isDarkTheme)
        {
            if (isDarkTheme)
            {
                LikedRecipesButtonBackround = darkThemeClickedButton;
                SavedRecipesButtonBackround = darkThemeUnClickedButton;
            }
            else
            {
                LikedRecipesButtonBackround = lightThemeClickedButton;
                SavedRecipesButtonBackround = lightThemeUnClickedButton;
            }
        }

        int lastRecipeId;
        [RelayCommand]
        public async Task ShowLikedRecipes()
        {
            var resources = Application.Current.Resources.MergedDictionaries.ElementAt(2);

            resources.TryGetValue("MainBackgroundColor", out var clickedButtonColor);
            resources.TryGetValue("MainBackgroundColorContrast", out var unclickedButtonColor);

            LikedRecipesButtonBackround = (Color)clickedButtonColor;
            SavedRecipesButtonBackround = (Color)unclickedButtonColor;

            await DisposeRecipesPage(SavedRecipes);

            var dtos = await FetchRecipes($"api/recipes/get-liked/{_userSession.Id}/{lastRecipeId}");
            if (dtos is null) return;

            await AppendToRecipeCollection(LikedRecipes, dtos, RecipeType.Liked);

            SavedRecipesVisible = false;
            LikedRecipesVisible = true;
        }

        [RelayCommand]
        public async Task ShowSavedRecipes()
        {
            var resources = Application.Current.Resources.MergedDictionaries.ElementAt(2);

            resources.TryGetValue("MainBackgroundColor", out var clickedButtonColor);
            resources.TryGetValue("MainBackgroundColorContrast", out var unclickedButtonColor);

            LikedRecipesButtonBackround = (Color)unclickedButtonColor;
            SavedRecipesButtonBackround = (Color)clickedButtonColor;

            await DisposeRecipesPage(LikedRecipes);

            var dtos = await FetchRecipes($"api/recipes/get-saved/{_userSession.Id}/{lastRecipeId}");
            if (dtos is null) return;

            await AppendToRecipeCollection(SavedRecipes,dtos,RecipeType.Saved);

            LikedRecipesVisible = false;
            SavedRecipesVisible = true;
        }

        /// <summary>
        /// Fetches a list of <c>RecipeDto</c> based on the provided endpoint
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns>List of <c>RecipeDto</c> or <c>null</c> when exception or server error</returns>
        private async Task<List<RecipeDto>> FetchRecipes(string endpoint)
        {
            using(var http = new HttpClient())
            {
                http.BaseAddress = new Uri(API_BASE_URL);

                try
                {
                    var response = await http.GetAsync(endpoint);

                    if(!response.IsSuccessStatusCode)
                    {
                        NotifiyFailedAction(await response.Content.ReadAsStringAsync());
                        return null;
                    }
                    var results = await response.Content.ReadAsStringAsync();
                    var recipes = await JsonToObject<List<RecipeDto>>(results);
                    return recipes;
                }
                catch
                {
                    NotifiyFailedAction("Cannot retrive recipes, try again later.");
                    return null;
                }
            }

        }

        /// <summary>
        /// Clears <see cref="RecipeView"/> collection and sets <see cref="lastRecipeId"/> to 0
        /// 
        /// </summary>
        /// <param name="collection">ObservableCollection to be disposed (cleared)</param>
        /// <param name="Value">page visibility value</param>
        private async Task DisposeRecipesPage(ObservableCollection<RecipeView> collection)
        {
            collection.Clear();
            lastRecipeId = 0;
        }

        [RelayCommand]
        public async Task DeleteSavedRecipe(string id)
        {
            using (var http = new HttpClient())
            {
                http.BaseAddress = new Uri(API_BASE_URL);
                var endpoint = $"api/recipes/delete-saved/{id}/{_userSession.Id}";
                try
                {
                    var response = await http.DeleteAsync(endpoint);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception("Recipe could not be deleted at the moment");
                    }

                    var recipe = SavedRecipes.First(x => x.Id == id);

                    SavedRecipes.Remove(recipe);
                }
                catch (Exception ex)
                {
                    NotifiyFailedAction(ex.Message);
                }
                finally
                {
                    var post = Posts.FirstOrDefault(p => p.PostId == id);

                    if (post is not null)
                    {
                        post.IsSaved = false;
                    }

                    var post2 = ProfilePosts.FirstOrDefault(p => p.PostId == id);

                    if (post2 is not null)
                    {
                        post2.IsSaved = false;
                    }
                }
            }
        }

        [RelayCommand]
        public async Task DeleteLikedRecipe(string id)
        {
            using (var http = new HttpClient())
            {
                http.BaseAddress = new Uri(API_BASE_URL);
                var endpoint = $"api/posts/delete-like/{id}/{_userSession.Id}";
                try
                {
                    var response = await http.DeleteAsync(endpoint);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception("Recipe could not be deleted at the moment");
                    }

                    var recipe = LikedRecipes.First(x => x.Id == id);

                    LikedRecipes.Remove(recipe);
                }
                catch (Exception ex)
                {
                    NotifiyFailedAction(ex.Message);
                }
            }          
        }

        [RelayCommand]
        public async Task RecipesItemsThresholdExceed(RecipeType type)
        {
            var dtos = type switch
            {
                RecipeType.Liked => await FetchRecipes($"api/recipes/get-liked/{_userSession.Id}/{lastRecipeId}"),
                RecipeType.Saved => await FetchRecipes($"api/recipes/get-saved/{_userSession.Id}/{lastRecipeId}"),
                _ => null
            };

            if (dtos is null)
            {
                return;
            }
            else if (type is RecipeType.Liked)
            {
                await AppendToRecipeCollection(LikedRecipes, dtos, RecipeType.Liked);
            }
            else if (type is RecipeType.Saved)
            {
                await AppendToRecipeCollection(SavedRecipes, dtos, RecipeType.Saved);
            }
        }

        private RecipeView CreateLikedRecipeView(string id,string username,string content,string imageBase64,List<string> products)
        {
            return new RecipeView() { 
                Id = id, 
                Username = username, 
                RecipeContent = content, 
                Image = imageBase64, 
                Products = products,
                DeleteCommand = DeleteLikedRecipeCommand 
            };
        }

        private RecipeView CreateSavedRecipeView(string id, string username, string content, string imageBase64, List<string> products)
        {
            return new RecipeView()
            {
                Id = id,
                Username = username,
                RecipeContent = content,
                Image = imageBase64,
                Products = products,
                DeleteCommand = DeleteSavedRecipeCommand
            };
        }

        private async Task AppendToRecipeCollection(ObservableCollection<RecipeView> collection,List<RecipeDto> dtos,RecipeType type)
        {
            Func<RecipeDto, RecipeView> createViewFunc = type switch
            {
                RecipeType.Liked => dto => CreateLikedRecipeView(dto.Id.ToString(), dto.Username, dto.Content, dto.ImageBase64, dto.Products),
                RecipeType.Saved => dto => CreateSavedRecipeView(dto.Id.ToString(), dto.Username, dto.Content, dto.ImageBase64, dto.Products),
                _ => throw new ArgumentException("Recipe type not supported")
            };

            foreach (var dto in dtos)
            {
                collection.Add(createViewFunc(dto));
                lastRecipeId = dto.Id;
            }
        }   
    }

    public enum RecipeType
    {
        Liked,
        Saved
    }
}
