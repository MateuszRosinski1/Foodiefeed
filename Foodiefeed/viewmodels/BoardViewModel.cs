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
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Maui.Core.Extensions;
using System.Net.Http.Headers;
using CommunityToolkit.Maui.Core;
using System.Net.Http;


namespace Foodiefeed.viewmodels
{
    public partial class BoardViewModel : ObservableObject
    {
        //https://icons8.com/icons/set/microphone icons
        //https://github.com/dotnet/maui/issues/8150  shadow resizing problem
        //https://github.com/CommunityToolkit/Maui/pull/2072 uniformgrid issue
        //Search problems need to be fixed. - fixed
        //internal sever error - https://www.vecteezy.com/vector-art/23833971-500-internal-server-error-concept-illustration-flat-design-vector-eps10-modern-graphic-element-for-landing-page-empty-state-ui-infographic-icon

        private readonly UserSession _userSession;
        private readonly IThemeHandler _themeHandler;
        //private readonly IFoodiefeedApiService _foodiefeedApiServce;
        private readonly IServiceProvider _serviceProvider;
        public ObservableCollection<PostView> Posts { get; set; } = new ObservableCollection<PostView>();

        public ObservableCollection<OnlineFreidnListElementView> OnlineFriends { get; set; } = new ObservableCollection<OnlineFreidnListElementView>();

        public ObservableCollection<UserSearchResultView> SearchResults { get; set; } = new ObservableCollection<UserSearchResultView>();


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

        [ObservableProperty]
        bool noOnlineFriendsVisible;

        [ObservableProperty]
        bool noSavedRecipes;

        [ObservableProperty]
        bool noLikedRecipes;

        [ObservableProperty]
        bool noProfileFriendsVisible;

        [ObservableProperty]
        bool noProfileFollowersVisible;

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

        [ObservableProperty]
        bool loadingScreenVisible;

        [ObservableProperty]
        bool errorScreenVisible;

        [ObservableProperty]
        bool onlineFriendsVisible;

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
        public ObservableCollection<RecipeView> LikedRecipes { get; set; } = new ObservableCollection<RecipeView>();
        public ObservableCollection<RecipeView> SavedRecipes { get; set; } = new ObservableCollection<RecipeView>();

        private Timer onlineFriendsTimer;

        private const string API_BASE_URL = "http://foodiefeedapi-daethrcqgpgnaehs.polandcentral-01.azurewebsites.net";

        public BoardViewModel(UserSession userSession,IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _themeHandler = _serviceProvider.GetService<IThemeHandler>();
            _userSession = userSession;
            try
            {
                InternetAcces = !(Connectivity.NetworkAccess == NetworkAccess.Internet);
                Notifications.CollectionChanged += OnNotificationsChanged;

                FetchNotifications(); // required for RemaningItemThresholdCommand to work.

                NoNotificationNotifierVisible = Notifications.Count == 0 ? true : false;
                OnlineFriends.CollectionChanged += OnOnlineFriendsChanged;
                LikedRecipes.CollectionChanged += OnLikedRecipesChanged;
                SavedRecipes.CollectionChanged += OnSavedRecipesChanged;
                ProfileFriendsList.CollectionChanged += OnFriendsListChanged;
                ProfileFollowersList.CollectionChanged += OnFollowersListChanged;
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

                Connectivity.ConnectivityChanged += ConnectivityChanged;

                ReloadProfileButtonColors(_themeHandler.ThemeFlag);
                ReloadRecipesButtons(_themeHandler.ThemeFlag);
            }
            catch {
                ErrorScreenVisible = true;
            }
        }

        private void OnSavedRecipesChanged(object? sender, NotifyCollectionChangedEventArgs e) => NoSavedRecipes = SavedRecipes.Count == 0 ? true : false;

        private void OnLikedRecipesChanged(object? sender, NotifyCollectionChangedEventArgs e) => NoLikedRecipes = LikedRecipes.Count == 0 ? true : false;

        private void OnNotificationsChanged(object? sender, NotifyCollectionChangedEventArgs e) => NoNotificationNotifierVisible = Notifications.Count == 0 ? true : false;

        private void OnOnlineFriendsChanged(object? sender, NotifyCollectionChangedEventArgs e) => NoOnlineFriendsVisible = OnlineFriends.Count == 0 ? true : false;

        private void OnFriendsListChanged(object? sender, NotifyCollectionChangedEventArgs e) => NoProfileFriendsVisible = ProfileFriendsList.Count == 0? true : false;

        private void OnFollowersListChanged(object? sender, NotifyCollectionChangedEventArgs e) => NoProfileFollowersVisible = ProfileFollowersList.Count == 0? true : false;

        private Timer notificationTimer;
        bool windowloaded;

        [RelayCommand]
        public async Task Appearing()
        {
            try
            {
                if (!windowloaded)   // appearing command is invoked 2 times for some reason
                {
                    windowloaded = true;
                    LoadingScreenVisible = true;

#if ANDROID
                    var base64 = await FetchProfilePictureBase64();
                    if (base64 is null) throw new Exception();

                    await SetProfilePictureFromBase64(base64);
                    await MainWallPostThresholdExceed();
#endif
#if WINDOWS
                    var base64 = await FetchProfilePictureBase64();
                    if (base64 is null) throw new Exception();

                    var t1 = SetProfilePictureFromBase64(base64);
                    var t2 = MainWallPostThresholdExceed();
                    await Task.WhenAll(t1, t2);

                    if (onlineFriendsTimer == null)
                    {
                        onlineFriendsTimer = new Timer(async _ => await UpdateFriendList(), null, TimeSpan.Zero, TimeSpan.FromMinutes(2));
                    }

                    if (notificationTimer == null)
                    {
                        notificationTimer = new Timer(async _ => await FetchNotifications(true), null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
                    }
#endif


                    LoadingScreenVisible = false;
                }
            }
            catch (Exception)
            {
                LoadingScreenVisible = false;
                ErrorScreenVisible = true;
            }
        }

        private void ConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
        {
            var access = Connectivity.NetworkAccess;
            InternetAcces = !(access == NetworkAccess.Internet);
        }

        [RelayCommand]
        public async Task ShowOnlinefriends()
        {
            this.PostPageVisible = false;
            this.ProfilePageVisible = false;
            this.SettingsPageVisible = false;
            this.RecipePageVisible = false;
#if ANDROID
            NotificationsPageVisible = false;
            OnlineFriendsVisible = true;
#endif
            OnlineFriends.Clear();          
            await UpdateFriendList();
        }

        [ObservableProperty]
        bool notificationsPageVisible;

        [RelayCommand]
        public async Task ShowNotifications()
        {
            notificationsPageNumber = 0;
            this.PostPageVisible = false;
            this.ProfilePageVisible = false;
            this.SettingsPageVisible = false;
            this.RecipePageVisible = false;
#if ANDROID
            NotificationsPageVisible = true;
            OnlineFriendsVisible = false;
#endif
            Notifications.Clear();
            await FetchNotifications();
        }   

        [RelayCommand]
        public async Task Logout()
        {
            _userSession.SetOnline();
            Dispose();
            var loginPage = _serviceProvider.GetRequiredService<LogInPage>();
            await _themeHandler.SaveThemeState();
            Application.Current.MainPage = loginPage;

        }

        private async Task<string> FetchProfilePictureBase64()
        {
            using(var http = new HttpClient())
            {
                http.BaseAddress = new Uri(API_BASE_URL);
                try
                {
                    http.Timeout = TimeSpan.FromSeconds(10);

                    var endpoint = $"api/user/get-profile-picture-base64/{_userSession.Id}";

                
                    var response = await http.GetAsync(endpoint);

                    if(!response.IsSuccessStatusCode)
                    {
                        ProfilePictureSource = "avatar.jpg";
                        return null;
                    }
                    return await response.Content.ReadAsStringAsync();

                }catch(Exception ex)
                {
                    return null;
                }
            }
        }

        bool FetchNotificationsCanExecute = true;
        private int notificationsPageNumber = 0;
        private async Task FetchNotifications(bool IsCalledByitmer = false)
        {
            if (!FetchNotificationsCanExecute) return;

            if (IsCalledByitmer)
            {
                Notifications.Clear();
                notificationsPageNumber = 0;
            }

            using (var httpClient = new HttpClient()) {

                httpClient.BaseAddress = new Uri(API_BASE_URL);
                try
                {
                    var endpoint = $"api/notifications/get-15/{_userSession.Id}?pageNumber={notificationsPageNumber}";
                    var response = await httpClient.GetAsync(endpoint);

                    if(!response.IsSuccessStatusCode)
                    {
                        throw new Exception();
                    }

                    var results = await response.Content.ReadAsStringAsync();

                    var notifications = await JsonToObject<List<NotificationDto>>(results);

                    await HandleNotificationsUpdate(notifications);
                }
                catch
                {
                    await NotifiyFailedAction("Something went wrong...");
                }
            }
            notificationsPageNumber += 1;
        }

        List<int> seenPostId = new List<int>();

        [ObservableProperty]
        bool wallPostLoadingActivityIndicatorVisible;

        [RelayCommand]
        public async Task MainWallPostThresholdExceed()
        {
#if WINDOWS
const int pageSize = 15;
#elif ANDROID
            const int pageSize = 3;
#endif
            WallPostLoadingActivityIndicatorVisible = true;
            using (var http = new HttpClient())
            {
                http.BaseAddress = new Uri(API_BASE_URL);

                try
                {
                    var endpoint = $"api/posts/generate-wall-posts?userId={_userSession.Id}&pageSize={pageSize}";
                    var content = new StringContent(JsonConvert.SerializeObject(seenPostId), Encoding.UTF8, "application/json");
                    http.Timeout = TimeSpan.FromSeconds(180);

                    var response = await http.PostAsync(endpoint,content);

                    if(!response.IsSuccessStatusCode) { throw new Exception(await response.Content.ReadAsStringAsync()); }

                    var results = await response.Content.ReadAsStringAsync();
                    var dtos = await JsonToObject<List<PostDto>>(results);
                    seenPostId.AddRange(dtos.Select(dtos => dtos.PostId));
                    await DisplayPosts(dtos,Posts);
                }
                catch(Exception)
                {
                    await NotifiyFailedAction("Could not load new post at the moment, try again later.");
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


            foreach(var notification in notifications)
            {
                switch(notification.Type)
                {
                    case NotificationType.FriendRequest: //0
                        Notifications.Add(new FriendRequestNotification()
                        {
                            Message = notification.Message,
                            UserId = notification.SenderId.ToString(),
                            NotifcationId = notification.Id,
                            ImageBase64 = notification.Base64
                        });
                        break;
                    case NotificationType.AcceptedFriendRequest: //5
                        Notifications.Add(new BasicNotofication()
                        {
                            Message = notification.Message,
                            UserId = notification.SenderId.ToString(),
                            Type = NotificationType.AcceptedFriendRequest,
                            NotifcationId = notification.Id,
                            ImageBase64 = notification.Base64
                        });
                        break;
                    case NotificationType.PostLike: //1
                        Notifications.Add(new PostLikeNotification()
                        {
                            Message = notification.Message,
                            PostId = notification.PostId.ToString(),
                            UserId = notification.SenderId.ToString(),
                            NotifcationId = notification.Id,
                            ImageBase64 = notification.Base64
                        });
                        break;
                    case NotificationType.PostComment: //2
                        Notifications.Add(new PostCommentNotification()
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
                        Notifications.Add(new CommentLikeNotification()
                        {
                            Message = notification.Message,
                            UserId = notification.SenderId.ToString(),
                            CommentId = notification.CommentId.ToString(),
                            NotifcationId = notification.Id,
                            ImageBase64 = notification.Base64
                        });
                        break;
                    case NotificationType.GainFollower: //4
                        Notifications.Add(new BasicNotofication()
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
       
        }


        public void Dispose()
        {
            notificationTimer?.Dispose();
            onlineFriendsTimer?.Dispose();
        }

        [RelayCommand]
        public async Task ShowCommentedPost((string post,string comment) id)
        {
            var post = await GetPopupPost(id.post,id.comment);

            if(post.PostImagesBase64 is null)
            {
                var popup = new CommentedPostPopup(post.CommentUserId, 
                post.PosterProfilePictureBase64,
                post.CommentProfilePictureImageBase64,
                post.CommentUsername,
                post.CommentContent,
                post.CommentLikes.ToString())
                {
                    Username = post.Username,
                    TimeStamp = post.TimeSpan,
                    PostTextContent = post.Description,
                    PostLikeCount = post.Likes.ToString(),
                    PostProducts = post.ProductsName,
                    PostContentVisible = true
                };

                popup.SetImagesVisiblity(false);
                App.Current.MainPage.ShowPopup(popup);
            }
            else
            {
                App.Current.MainPage.ShowPopup(new CommentedPostPopup(post.CommentUserId,
                post.PosterProfilePictureBase64,
                post.CommentProfilePictureImageBase64,
                post.CommentUsername,
                post.CommentContent,
                post.Likes.ToString())
                {
                    Username = post.Username,
                    TimeStamp = post.TimeSpan,
                    PostTextContent = post.Description,
                    PostLikeCount = post.Likes.ToString(),
                    ImageSource = post.PostImagesBase64[0],
                    ImagesBase64 = post.PostImagesBase64,
                    PostProducts = post.ProductsName,
                    PostContentVisible = true
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
        public async Task OpenCommentEditor(string commentId)
        {
            var popup = new EditCommentPopup(commentId);
            popup.Closed += DisposeEditCommentPopup;
            Application.Current.MainPage.ShowPopup(popup);
        }

        [RelayCommand]
        public async Task DeleteComment(string commentId)
        {
            using(var http = new HttpClient())
            {
                http.BaseAddress = new Uri(API_BASE_URL);

                var endpoint = $"api/comments/delete-comment-{commentId}";

                try
                {
                    var response = await http.DeleteAsync(endpoint);
                    if (!response.IsSuccessStatusCode)
                    {
                        await NotifiyFailedAction("Could not delete comment at the moment, try again later");
                    }

                    await Task.Run(() =>
                    {
                        if (PostPageVisible)
                        {
                            foreach (var post in Posts)
                            {
                                var comment = post.Comments.FirstOrDefault(c => c.CommentId == commentId);
                                if (comment is not null)
                                {
                                    post.Comments.Remove(comment);
                                    return;
                                }
                            }
                        }
                        else
                        {
                            foreach (var post in ProfilePosts)
                            {
                                var comment = post.Comments.FirstOrDefault(c => c.CommentId == commentId);
                                if (comment is not null)
                                {
                                    post.Comments.Remove(comment);
                                    return;
                                }
                            }
                        }
                    });

                }
                catch(Exception)
                {
                    await NotifiyFailedAction("internal server error, try again later.");
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
        public async Task EditComment(string commentId)
        {
            if(EditedCommentContent == string.Empty)
            {
                await NotifiyFailedAction("Comment content cannot be empty");
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
                        await NotifiyFailedAction("Comment edited succesfuly");
                        await Task.Run(() =>
                        {
                            if (PostPageVisible)
                            {
                                foreach (var post in Posts)
                                {
                                    var comment = post.Comments.FirstOrDefault(c => c.CommentId == commentId);
                                    if(comment is not null)
                                    {
                                       comment.CommentContent = EditedCommentContent;
                                       return;
                                    }
                                }
                            }
                            else
                            {
                                foreach (var post in ProfilePosts)
                                {
                                    var comment = post.Comments.FirstOrDefault(c => c.CommentId == commentId);
                                    if (comment is not null)
                                    {
                                        comment.CommentContent = EditedCommentContent;
                                        return;
                                    }
                                }
                            }
                        });
                        return;
                    }
                }
                catch(Exception)
                {
                    await NotifiyFailedAction("Cannot edit your comment at the moment, try again later.");
                }

            }
        }

        [RelayCommand]
        public async Task AddNewComment((string postId,string commentContent) payload)
        {
            var newComment = new NewCommentDto()
            {
                UserId = _userSession.Id.ToString(),
                CommentContent = payload.commentContent
            };
            
            if (string.IsNullOrEmpty(payload.commentContent))
            {
                await NotifiyFailedAction("Comment conetnt cannot be empty.");
                return;
            }

            CommentView comment = new CommentView();
            using (var httpclient = new HttpClient())
            {
                httpclient.BaseAddress = new Uri(API_BASE_URL);

                var endpoint = $"api/comments/add-new-comment-{payload.postId}";

                var content = new StringContent(JsonConvert.SerializeObject(newComment), Encoding.UTF8, "application/json");

                try
                {
                    var response = await httpclient.PostAsync(endpoint, content);

                    if(!response.IsSuccessStatusCode) throw new Exception(await response.Content.ReadAsStringAsync());

                    var json = await response.Content.ReadAsStringAsync();
                    var dto = JsonConvert.DeserializeObject<CommentDto>(json);

                    var temp = dto.UserId == _userSession.Id ? true : false;


                    comment.Username = dto.Username;
                    comment.CommentContent = dto.CommentContent;
                    comment.CommentId = dto.CommentId.ToString();
                    comment.LikeCount = dto.Likes.ToString();
                    comment.UserId = dto.UserId.ToString();
                    comment.PfpImageBase64 = dto.ImageBase64;
                    comment.EditButtonVisible = temp;
                    comment.UnlikeCommentCommand = UnlikeCommentCommand;
                    comment.LikeCommentCommand = LikeCommentCommand;
                    comment.IsLiked = dto.IsLiked;
                }
                catch(Exception e)
                {
                    await NotifiyFailedAction(e.Message);
                    return;
                }
            }

            var post = Posts.FirstOrDefault(p => p.PostId == payload.postId);
            if (post != null)
            {
                post.Comments.Add(comment);
            }
            var profilepost = ProfilePosts.FirstOrDefault(p => p.PostId == payload.postId);
            if(profilepost != null)
            {
                profilepost.Comments.Add(comment);
            }
        }

        [RelayCommand]
        public async Task ShowLikedComment(string commentId)
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
        public async Task ShowLikedPost(string postId)
        {
            var post = await GetPopupLikedPost(postId);
            if (post.PostImagesBase64 is null)
            {
                var popup = new LikedPostPopup()
                {
                    Username = post.Username,
                    TimeStamp = post.TimeSpan,
                    PostTextContent = post.Description,
                    PostLikeCount = post.Likes.ToString(),
                    PostProducts = post.ProductsName,
                    PostContentVisible = true
                };

                popup.SetImagesVisiblity(false);
                App.Current.MainPage.ShowPopup(popup);
            }
            else
            {
                App.Current.MainPage.ShowPopup(new LikedPostPopup()
                {
                    Username = post.Username,
                    TimeStamp = post.TimeSpan,
                    PostTextContent = post.Description,
                    PostLikeCount = post.Likes.ToString(),
                    ImageSource = post.PostImagesBase64[0],
                    ImagesBase64 = post.PostImagesBase64,
                    PostProducts = post.ProductsName,
                    PostContentVisible = true
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
        private async Task ClearNotifications()
        {
            FetchNotificationsCanExecute = false;       
            using(var httpclient = new HttpClient())
            {
                httpclient.BaseAddress = new Uri(API_BASE_URL);

                try
                {
                    List<int> NotificationsId = new List<int>();
                    for (int i = 0; i <= Notifications.Count() - 1; i++)
                    {
                        if (Notifications[i] is not FriendRequestNotification)
                        {
                            NotificationsId.Add(Notifications[i].NotifcationId);
                        }
                    }

                    var endpoint = $"/api/notifications/remove-range-notifications/{_userSession.Id}";

                    var jsonContent = new StringContent(JsonConvert.SerializeObject(NotificationsId), Encoding.UTF8, "application/json");

                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Delete,
                        RequestUri = new Uri(API_BASE_URL + endpoint),
                        Content = jsonContent
                    };

                    var response = await httpclient.SendAsync(request);

                    if (!response.IsSuccessStatusCode) throw new Exception();

                    for (int i = 0; i <= Notifications.Count() - 1; i++)
                    {
                        if (Notifications[i] is not FriendRequestNotification)
                        {
                            await Notifications[i].HideAnimation(300, 150);
                            Notifications.Remove(Notifications[i]);
                            i = i - 1;
                        }
                    }

                    notificationsPageNumber = 0;
                    FetchNotificationsCanExecute = true;
                    await FetchNotifications();
                }
                catch(Exception)
                {
                    await NotifiyFailedAction("Could not clear notifications at the moment, try again later.");
                }
            } 
            
            FetchNotificationsCanExecute = true;
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

        public ObservableCollection<PostImageView> AddPostImages { get; set; } = new ObservableCollection<PostImageView>();

        [RelayCommand]
        public async Task UploadPostImages()
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
        public async Task DeletePost(string postId)
        {
            using(var http = new HttpClient())
            {
                http.BaseAddress = new Uri(API_BASE_URL);
                var endpoint = $"api/posts/delete?postId={postId}&userId={_userSession.Id}";
                try
                {
                    var response = await http.DeleteAsync(endpoint);

                    if (!response.IsSuccessStatusCode) { throw new Exception(await response.Content.ReadAsStringAsync()); } 

                }catch(Exception e)
                {
                    await NotifiyFailedAction(e.Message);
                    return;
                }
            }

            var post = Posts.FirstOrDefault(p => p.PostId == postId);
            if(post is not null) Posts.Remove(post);

            var postProfile = ProfilePosts.FirstOrDefault(p => p.PostId == postId);
            if (postProfile is not null) Posts.Remove(postProfile);
        }

        [RelayCommand]
        public async Task AddPost()
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
                        catch (Exception)
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

        public ObservableCollection<TagView> Tags { get; set; } = new ObservableCollection<TagView>();

        [ObservableProperty]
        bool addPostFormActivityIndicatorVisible;

        [RelayCommand]
        public async Task ChooseTags()
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
                    await NotifiyFailedAction("Could not retrive tags from the server, try again later.");
                }
                finally
                {
                    TagPickerVisible = true;
                    PostContentEditorVisible = false;
                    AddPostFormActivityIndicatorVisible = false;
                }
            }
        }

        [MaxLength(4)]
        public static ObservableCollection<TagView> PickedTags { get; set; } = new ObservableCollection<TagView>();

        [ObservableProperty]
        string filterParam;

        [RelayCommand]
        public async Task PickTag(string id)
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
                    await NotifiyFailedAction("internal server error, try again later");
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
        public async Task NotificationsThresholdExceed(ItemsViewScrolledEventArgs e)
        {
            await FetchNotifications();
        }

        [RelayCommand]
        public void ToMainView()
        {
            this.PostPageVisible = true;
            this.ProfilePageVisible = false;
            this.SettingsPageVisible = false;
            this.RecipePageVisible = false;
#if ANDROID
            NotificationsPageVisible = false;
            OnlineFriendsVisible = false;
#endif
        }

        [RelayCommand]
        public async Task ToProfileView()
        {
            this.PostPageVisible = false;
            this.ProfilePageVisible = true;
            this.SettingsPageVisible = false;
            this.RecipePageVisible = false;

#if ANDROID
            NotificationsPageVisible = false;
            OnlineFriendsVisible = false;
#endif

            ProfilePosts.Clear();
            ProfileFollowersList.Clear();
            ProfileFriendsList.Clear();
            await ShowUserProfile(_userSession.Id.ToString());
        }

        [RelayCommand]
        public async Task ToRecipesView()
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

#if ANDROID
            NotificationsPageVisible = false;
            OnlineFriendsVisible = false;
#endif
        }

        [RelayCommand]
        public async Task ShowPopup(string id)
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
        public async Task OpenPersonalDataEditor()
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
        public async Task ChooseNewProfilePicture()
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
        public async Task SaveChangedPersonalData()
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
            else if (this.ChangeProfilePictureVisible)
            {
                await UploadNewProfilePicture();
                return;
            }

            if(endpoint != string.Empty && content is not null)
             await UpdatePersonalData(endpoint,content);
        }

        [RelayCommand]
        public async Task RemoveUserProfilePicture()
        {
            using(var http = new HttpClient())
            {
                var endpoint = $"api/user/delete-profile-picture/{_userSession.Id}";
                try
                {
                    var filePath = Path.Combine(FileSystem.Current.AppDataDirectory, "avatar.jpg");

                    if (!File.Exists(filePath))
                    {
                        using var resource = await FileSystem.OpenAppPackageFileAsync("avatar.jpg");
                        using var fileStream = File.Create(filePath);
                        await resource.CopyToAsync(fileStream);
                    }

                    using (var formContent = new MultipartFormDataContent())
                    {
                        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            var fileContent = new StreamContent(fileStream);
                            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                            formContent.Add(fileContent, "file", "avatar.jpg");
                        }

                        var request = new HttpRequestMessage
                        {
                            Method = HttpMethod.Delete,
                            RequestUri = new Uri(API_BASE_URL + endpoint),
                            Content = formContent
                        };

                        var response = await http.SendAsync(request);

                        if (!response.IsSuccessStatusCode) throw new Exception("Could not remove profile picture at the moment");

                        ProfilePictureSource = filePath;
                    }          
                }catch(Exception e)
                {
                    await NotifiyFailedAction(e.Message);
                }
            }
        }

        private async Task UploadNewProfilePicture()
        {
            using (var http = new HttpClient())
            {
                http.BaseAddress = new Uri(API_BASE_URL);

                var endpoint = $"api/user/upload-new-profile-picture/{_userSession.Id}";

                try
                {
                    using (var formContent = new MultipartFormDataContent())
                    {
                        var file = ChangedImageProfilePicturePath;
                        if (file != null && File.Exists(file))
                        {
                            using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                            {
                                var fileContent = new StreamContent(fileStream);
                                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                                formContent.Add(fileContent, "file", Path.GetFileName(file));

                                var response = await http.PostAsync(endpoint, formContent);

                                if(!response.IsSuccessStatusCode)  throw new Exception(await response.Content.ReadAsStringAsync());

                                var base64 = await FetchProfilePictureBase64();
                                if (base64 is null) throw new Exception();
                                await SetProfilePictureFromBase64(base64);
                            }
                        }
                        else
                        {
                            await NotifiyFailedAction("Provied file is not found or it is empty.");
                        }
                    }
                }catch(Exception e)
                {
                    await NotifiyFailedAction(e.Message);
                }
            }
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
                catch(Exception)
                {
                    await NotifiyFailedAction("cannot update personal data at the moment");
                }
               
            }
        }

        public bool CanShowSearchPanel { get; set; }
        [RelayCommand]
        public async Task ShowSearchPanel()
        {
#if WINDOWS
            if (CanShowSearchPanel)
            {
                this.SearchPanelVisible = true;
                await DisplaySearchResultHistory();
            }
#endif
#if ANDROID
            this.SearchPanelVisible = true;
            await DisplaySearchResultHistory();
#endif

        }

        [RelayCommand]
        public async Task HideSearchPanel()
        {
            await Task.Delay(200);
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
        public async Task ShowUserProfile(string id)
        {
            pageNumber = 0;
            ProfileAddFriendAndFollowButtonsVisible = false;
            this.ProfileFollowersVisible = false;
            this.ProfilePostsVisible = true;
            this.ProfileFriendsVisible = false;
            await SetButtonColors(Buttons.PostButton);
            try
            {
                if (id != _userSession.Id.ToString())
                {
                    await CreateSearchResultHistory(id);
                    this.PostPageVisible = false;
                    this.ProfilePageVisible = true;
                    this.SettingsPageVisible = false;
                    this.RecipePageVisible = false;
                    ProfilePosts.Clear();
                    ProfileFollowersList.Clear();
                    ProfileFriendsList.Clear();
                    ProfileAddFriendAndFollowButtonsVisible = true;
                }

                await OpenUserProfile(id);
            }catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }

        [RelayCommand]
        public async Task ShowUserProfilePopup(string id)
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
            await SetButtonColors(Buttons.PostButton);
            try
            {
                ProfileAddFriendAndFollowButtonsVisible = true;
                await OpenUserProfile(id);
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

        private async Task ReloadProfileButtonColors(bool isDarkTheme)
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
            if (CurrentClickedButtonColor is null && CurrentUnclickedButtonColor is null) await ReloadProfileButtonColors(themeFlag);

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
        public async Task ShowProfilePosts()
        {
            pageNumber = 0;
            ProfilePosts.Clear();
            ProfileFollowersList.Clear();
            ProfileFriendsList.Clear();
            this.ProfileFollowersVisible = false;
            this.ProfilePostsVisible = true;
            this.ProfileFriendsVisible = false;
            await SetButtonColors(Buttons.PostButton);

            var posts = await GetUserProfilePosts(ProfileId.ToString());

            var postsDto = await JsonToObject<List<PostDto>>(posts);
            await DisplayPosts(postsDto, ProfilePosts);
        }

        [RelayCommand]
        public async Task ShowProfileFriends()
        {
            ProfilePosts.Clear();
            ProfileFollowersList.Clear();
            ProfileFriendsList.Clear();
            this.ProfileFollowersVisible = false;
            this.ProfilePostsVisible = false;
            this.ProfileFriendsVisible = true;
            await SetButtonColors(Buttons.FriendsButton);

            var friends = await GetUserProfileFriends(ProfileId.ToString());
            var dtos = await JsonToObject<List<ListedFriendDto>>(friends);
            await DisplayProfileFriends(dtos);
        }

        [RelayCommand]
        public async Task ShowProfileFollowers()
        {
            ProfilePosts.Clear();
            ProfileFollowersList.Clear();
            ProfileFriendsList.Clear();
            this.ProfileFollowersVisible = true;
            this.ProfilePostsVisible = false;
            this.ProfileFriendsVisible = false;
            await SetButtonColors(Buttons.FollowersButton);

            var followers = await GetUserProfileFollowers(ProfileId.ToString());

            var dtos = await JsonToObject<List<ListedFriendDto>>(followers);
            await DisplayProfileFollowers(dtos);
        }

        [RelayCommand]
        public async Task LikeComment(string commentId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(API_BASE_URL);

                try
                {
                    var endpoint = $"api/comments/like-comment/{_userSession.Id}?commentId={commentId}";

                    var response = await client.PostAsync(endpoint, null);

                    if (!response.IsSuccessStatusCode) throw new Exception(await response.Content.ReadAsStringAsync());

                    var idStr = await response.Content.ReadAsStringAsync();


                    var post = Posts.FirstOrDefault(p => p.PostId == idStr);

                    if (post is not null)
                    {
                        var comment = post.Comments.First(c => c.CommentId == commentId);
                        var likes = Convert.ToInt32(comment.LikeCount);
                        likes += 1;
                        comment.LikeCount = likes.ToString();
                        comment.IsLiked = true;
                    }

                    var post2 = ProfilePosts.FirstOrDefault(p => p.PostId == idStr);

                    if (post2 is not null)
                    {
                        var comment = post2.Comments.First(c => c.CommentId == commentId);
                        var likes = Convert.ToInt32(comment.LikeCount);
                        likes += 1;
                        comment.LikeCount = likes.ToString();
                        comment.IsLiked = true;
                    }
                }
                catch (Exception ex)
                {
                    await NotifiyFailedAction(ex.Message);
                    return;
                }
                
            }
        }

        [RelayCommand]
        public async Task UnlikeComment(string commentId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(API_BASE_URL);

                try
                {
                    var endpoint = $"api/comments/unlike-comment/{_userSession.Id}?commentId={commentId}";

                    var response = await client.DeleteAsync(endpoint);

                    if (!response.IsSuccessStatusCode) throw new Exception(await response.Content.ReadAsStringAsync());

                    var idStr = await response.Content.ReadAsStringAsync();


                    var post = Posts.FirstOrDefault(p => p.PostId == idStr);

                    if (post is not null)
                    {
                        var comment = post.Comments.First(c => c.CommentId == commentId);
                        var likes = Convert.ToInt32(comment.LikeCount);
                        likes -= 1;
                        comment.LikeCount = likes.ToString();
                        comment.IsLiked = false;
                    }

                    var post2 = ProfilePosts.FirstOrDefault(p => p.PostId == idStr);

                    if (post2 is not null)
                    {
                        var comment = post2.Comments.First(c => c.CommentId == commentId);
                        var likes = Convert.ToInt32(comment.LikeCount);
                        likes -= 1;
                        comment.LikeCount = likes.ToString();
                        comment.IsLiked = false;
                    }
                }
                catch (Exception ex)
                {
                    await NotifiyFailedAction(ex.Message);
                    return;
                }

            }
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
                    await NotifiyFailedAction(e.Message);
                    return;
                }

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
                    await NotifiyFailedAction(e.Message);
                    return;
                }
                
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

        [RelayCommand]
        public async Task SaveRecipe(string postId)
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
                        await NotifiyFailedAction(message);
                        return;
                    }

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
                catch
                {
                    await NotifiyFailedAction("internal server error, try again later");
                }
            }
        }

        [RelayCommand]
        public async Task Search()
        {
            SearchResults.Clear();

            if (SearchParam == string.Empty) { await DisplaySearchResultHistory(); return; }

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
                        var searchResults = await JsonToObject<ObservableCollection<UserSearchResult>>(json);
                        await DisplaySearchResults(searchResults
                            .Reverse()
                            .ToObservableCollection());

                    }
                }
                catch(Exception)
                {
                    await NotifiyFailedAction("Could not load search results. Try again later");
                }
            }

        }

        [RelayCommand]
        public async Task DeclineFriendRequest((string senderId, int notificationId) ids)
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
                    await NotifiyFailedAction("Cant send friend request");
                }
            }
        }

        [RelayCommand]
        public async Task AcceptFriendRequest((string senderId,int notificationId) ids)
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
                    await NotifiyFailedAction("Cant send friend request");
                }
            }
        }

        [RelayCommand]
        public async Task AddToFriends(string id)
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
                    await NotifiyFailedAction("Cant send friend request");
                    // code to handle unsuccsesful friend request.
                }
            }
        }

        [RelayCommand]
        public async Task UnfriendUser(string id)
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
                    await NotifiyFailedAction("Something went wrong...");
                }
            }
        }

        [RelayCommand]
        public async Task FollowUser(string id)
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
                    await NotifiyFailedAction("Could not finish following action due to inner issues.");
                }
            }
        }

        [RelayCommand]
        public async Task UnfollowUser(string id)
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
                    await NotifiyFailedAction("Could not finish unfollowing action due to inner issues.");
                }
            }
        }

        [RelayCommand]
        public async Task CancelFriendRequest(string id)
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
                    await NotifiyFailedAction("Request was already canceled by sender.");
                }
            }
        }

        [ObservableProperty]
        bool themeFlag;  //true - darktheme | true - lighttheme
        [ObservableProperty]
        string switchThemeMode = "Light Theme";

        [RelayCommand]
        public async Task ChangeTheme()
        {
            _themeHandler.ThemeFlag = !_themeHandler.ThemeFlag;

            if (_themeHandler.ThemeFlag)
            {             
                SwitchThemeMode = "Dark Theme";
            }
            else
            {
                SwitchThemeMode = "Light Theme";
            }

            await _themeHandler.ChangeTheme();
            await ReloadProfileButtonColors(_themeHandler.ThemeFlag);
            ReloadRecipesButtons(_themeHandler.ThemeFlag);
        }


        private async Task DisplaySearchResults(ObservableCollection<UserSearchResult> users)
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
                    Friends = user.FriendsCount.ToString(),
                    PfpImageBase64 = user.ProfilePictureBase64
                });
            }
        }

        private async Task DisplaySearchResultHistory()
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

            await DisplaySearchResults(await JsonToObject<ObservableCollection<UserSearchResult>>(json)); 
            //add a block of code that displays that there are not search results.

        }

        private async Task CreateSearchResultHistory(string userId)
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
                }catch(Exception)
                {
                    await NotifiyFailedAction("Something went wrong...");
                }
                return null;
            }
        }

        public async Task UpdateFriendList() 
        {
            OnlineFriends.Clear();
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(API_BASE_URL);

                try
                {
                    var endpoint = $"api/friends/online/{_userSession.Id}";

                    var response = await httpClient.GetAsync(endpoint);

                    var json = await response.Content.ReadAsStringAsync();

                    var onlineFriends = await JsonToObject<List<ListedFriendDto>>(json);

                    foreach(var friend in onlineFriends)
                    {
                        var view = new OnlineFreidnListElementView()
                        {
                            UserId = friend.Id.ToString(),
                            Username = friend.Username,
                            AvatarImageSource = friend.ProfilePictureBase64,
                            IsOnline = true
                        };

                        await Application.Current.Dispatcher.DispatchAsync(() => { OnlineFriends.Add(view); });
                    }
                }
                catch(Exception)
                {
                    await NotifiyFailedAction("Something went wrong...");
                }

                try
                {
                    var endpoint = $"api/friends/offline/{_userSession.Id}";

                    var response = await httpClient.GetAsync(endpoint);

                    var json = await response.Content.ReadAsStringAsync();
                    var offlineFriends = await JsonToObject<List<ListedFriendDto>>(json);

                    foreach (var friend in offlineFriends)
                    {
                        var view = new OnlineFreidnListElementView()
                        {
                            UserId = friend.Id.ToString(),
                            Username = friend.Username,
                            AvatarImageSource = friend.ProfilePictureBase64,
                            IsOnline = false
                        };

                        await Application.Current.Dispatcher.DispatchAsync(() => { OnlineFriends.Add(view); });
                    }
                }
                catch (Exception)
                {
                    await NotifiyFailedAction("Something went wrong...");
                }
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

                var postsTask = await GetUserProfilePosts(id);
                //var friendsTask = GetUserProfileFriends(id);
                //var followersTask = GetUserProfileFollowers(id);

                //await Task.WhenAll(postsTask, friendsTask, followersTask);

                var posts = await JsonToObject<List<PostDto>>(postsTask);
                await DisplayPosts(posts,ProfilePosts);

                //var friends = await JsonToObject<List<ListedFriendDto>>(await friendsTask);
                //await DisplayProfileFriends(friends);

                //var followers = await JsonToObject<List<ListedFriendDto>>(await followersTask);
                //await DisplayProfileFollowers(followers);
            }
            catch (Exception)
            {
                await NotifiyFailedAction("Could not load user profile.");
            }
            finally
            {
                LoadingLabelVisible = false;
            }
        }

        [RelayCommand]
        public async Task ProfilePostThresholdReached() //android exclusive command
        {
            var postsJson = await GetUserProfilePosts(ProfileId.ToString());
            await DisplayPosts(await JsonToObject<List<PostDto>>(postsJson), ProfilePosts); 
        }

        private async Task DisplayProfileFollowers(List<ListedFriendDto> followers)
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

        private async Task DisplayProfileFriends(List<ListedFriendDto> friends) 
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

        public ObservableCollection<OnListFriendView> ProfileFollowersList { get; set; } = new ObservableCollection<OnListFriendView>();

        public ObservableCollection<PostView> ProfilePosts { get; set; } = new ObservableCollection<PostView>();


        private int pageNumber = 0;
        private async Task<string> GetUserProfilePosts(string id)
        {
            var endpoint = $"api/posts/profile-posts/{id}?pageNumber={pageNumber}";

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
                    pageNumber += 1;
                    return await response.Content.ReadAsStringAsync();                   
                }
                catch
                {
                    await NotifiyFailedAction("Something went wrong...");
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
                return;
            }

            //if(ProfilePageVisible) await Dispatcher.GetForCurrentThread().DispatchAsync(() => { collection.Clear(); });

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
                        EditButtonVisible = temp,
                        UnlikeCommentCommand = UnlikeCommentCommand,
                        LikeCommentCommand = LikeCommentCommand,
                        IsLiked = comment.IsLiked,
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
                            TimeStamp = post.TimeSpan,
                            PostLikeCount = post.Likes.ToString(),
                            PostTextContent = post.Description,
                            Comments = new List<CommentView>(commentList),
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
                            TimeStamp = post.TimeSpan,
                            PostLikeCount = post.Likes.ToString(),
                            PostTextContent = post.Description,
                            ImageSource = post.PostImagesBase64[0],
                            Comments = new List<CommentView>(commentList),
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
                catch (Exception)
                {
                    await NotifiyFailedAction("Cannot display post at the moment");
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
                    await NotifiyFailedAction("Something went wrong...");
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
                        await NotifiyFailedAction("Something went wrong...");
                    }
                }
                catch (Exception)
                {
                    await NotifiyFailedAction("Something went wrong...");
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
                catch(Exception)
                {
                    await NotifiyFailedAction("Cannot load the profile at the moment, try again later.");
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

        private async Task NotifiyFailedAction(string message)
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
                        await NotifiyFailedAction(await response.Content.ReadAsStringAsync());
                        return null;
                    }
                    var results = await response.Content.ReadAsStringAsync();
                    var recipes = await JsonToObject<List<RecipeDto>>(results);
                    return recipes;
                }
                catch
                {
                    await NotifiyFailedAction("Cannot retrive recipes, try again later.");
                    return null;
                }
            }

        }

        /// <summary>
        /// Clears <see cref="RecipeView"/> collection and sets <see cref="lastRecipeId"/> to 0
        /// 
        /// </summary>
        /// <param name="collection">ObservableCollection to be disposed (cleared)</param>
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
                catch (Exception ex)
                {
                    await NotifiyFailedAction(ex.Message);
                }

            }
        }

        [RelayCommand]
        public async Task DeleteLikedRecipe(string id)
        {
            using (var http = new HttpClient())
            {
                http.BaseAddress = new Uri(API_BASE_URL);

                var endpoint = $"api/posts/unlike-post/{_userSession.Id}?postId={id}";
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
                    await NotifiyFailedAction(ex.Message);
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
