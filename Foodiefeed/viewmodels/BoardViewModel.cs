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


namespace Foodiefeed.viewmodels
{
    public partial class BoardViewModel : ObservableObject
    {
        //https://icons8.com/icons/set/microphone icons
        //https://github.com/dotnet/maui/issues/8150  shadow resizing problem
        //https://github.com/CommunityToolkit/Maui/pull/2072 uniformgrid issue
        //Search problems need to be fixed.


        private readonly UserSession _userSession;
        private Thread UpdateOnlineFriendListThread;

        public ObservableCollection<PostView> Posts { get { return posts;} }

        private ObservableCollection<PostView> posts 
            = new ObservableCollection<PostView>();

        public ObservableCollection<OnlineFreidnListElementView> OnlineFriends { get { return onlineFriends; } }

        private ObservableCollection<OnlineFreidnListElementView> onlineFriends 
            = new ObservableCollection<OnlineFreidnListElementView> { };

        //public ObservableCollection<OnListFriendView> ProfilePageFriends { get { return profilePageFriends; } }

        //private ObservableCollection<OnListFriendView> profilePageFriends = 
        //    new ObservableCollection<OnListFriendView>();

        public ObservableCollection<UserSearchResultView> SearchResults { get { return searchResults; } }

        private ObservableCollection<UserSearchResultView > searchResults = 
            new ObservableCollection<UserSearchResultView> { };

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

        private const string API_BASE_URL = "http://localhost:5000";

        [ObservableProperty]
        private string searchParam;

        #region VisibilityFlags

        [ObservableProperty]
        bool searchPanelVisible;

        [ObservableProperty]
        bool profilePageVisible;

        [ObservableProperty]
        bool postPageVisible;

        [ObservableProperty]
        bool settingsPageVisible;

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
        string changedImageProfilePictureBase64;

        #endregion

        public ObservableCollection<INotification> Notifications { get { return notifications; } }
        private ObservableCollection<INotification> notifications = new ObservableCollection<INotification>();


        public BoardViewModel(UserSession userSession)
        {
            InternetAcces = !(Connectivity.NetworkAccess == NetworkAccess.Internet);

            notifications.CollectionChanged += OnNotificationsChanged;
            DisplaySearchResultHistory();
            _userSession = userSession;
            _userSession.Id = 100;
            NoNotificationNotifierVisible = notifications.Count == 0 ? true : false;

            this.ProfilePageVisible = false; //on init false
            this.PostPageVisible = true; //on init true
            this.SettingsPageVisible = false; //on init false
            this.PersonalDataEditorVisible = false; // on init false
            this.SettingsMainHubVisible = true; //on init true
            this.ChangeUsernameEntryVisible = false; //on init false
            this.ChangeEmailEntryVisible = false; //on init false
            this.ChangePasswordEntryVisible = false; //on init false
            this.ProfileFollowersVisible = false; //on init false
            this.ProfilePostsVisible = true; //on init true;
            this.ProfileFriendsVisible = false; //on init false
            this.NoPostOnProfile = false; // on init false
            this.HubPanelVisible = false; // on init false
            this.CanShowSearchPanel = true; //on init true

            //UpdateOnlineFriendListThread = new Thread(UpdateFriendList);
            //UpdateOnlineFriendListThread.Start();

            //Task.Run(UpdateFriendList);
            ChangeTheme();
            Connectivity.ConnectivityChanged += ConnectivityChanged;            
        }

        private void ConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
        {
            var access = Connectivity.NetworkAccess;
            InternetAcces = !(access == NetworkAccess.Internet);
        }

        private Timer notificationTimer;
        bool windowloaded;

        [RelayCommand]
        async void Appearing()
        {
            if(!windowloaded)   // appearing command invoked 2 times for some reason
            {
                FetchNotifications();
                windowloaded = true;
            }
                
            //if(notificationTimer == null)
            //{
            //    notificationTimer = new Timer(async _ => await FetchNotifications(), null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
            //}
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
                            NotifcationId = notification.Id
                        });
                        break;
                    case NotificationType.AcceptedFriendRequest: //5
                        newNotifications.Add(new BasicNotofication()
                        {
                            Message = notification.Message,
                            UserId = notification.SenderId.ToString(),
                            Type = NotificationType.AcceptedFriendRequest,
                            NotifcationId = notification.Id
                        });
                        break;
                    case NotificationType.PostLike: //1
                        newNotifications.Add(new PostLikeNotification()
                        {
                            Message = notification.Message,
                            PostId = notification.PostId.ToString(),
                            UserId = notification.SenderId.ToString(),
                            NotifcationId = notification.Id
                        });
                        break;
                    case NotificationType.PostComment: //2
                        newNotifications.Add(new PostCommentNotification()
                        {
                            Message = notification.Message,
                            UserId = notification.SenderId.ToString(),
                            CommentId = notification.CommentId.ToString(),
                            PostId = notification.PostId.ToString(),
                            NotifcationId = notification.Id
                        });
                        break;
                    case NotificationType.CommentLike: //3
                        newNotifications.Add(new CommentLikeNotification()
                        {
                            Message = notification.Message,
                            UserId = notification.SenderId.ToString(),
                            CommentId = notification.CommentId.ToString(),
                            NotifcationId = notification.Id
                        });
                        break;
                    case NotificationType.GainFollower: //4
                        newNotifications.Add(new BasicNotofication()
                        {
                            Message = notification.Message,
                            UserId = notification.SenderId.ToString(),
                            Type = NotificationType.GainFollower,
                            NotifcationId = notification.Id
                            //ShowPostButtonVisible = false
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
            NoNotificationNotifierVisible = notifications.Count == 0 ? true : false;
        }

        [RelayCommand]
        public void Scrolled(ItemsViewScrolledEventArgs e)
        {

            if (e.LastVisibleItemIndex == Posts.Count() - 2)
            {

            }
        }

        [RelayCommand]
        public void ScrolledNotifications(ItemsViewScrolledEventArgs e)
        {
            if (e.LastVisibleItemIndex == Notifications.Count() - 2)
            {
                DisplayNotifications(10, allNotifications);
            }
        }


        [RelayCommand]
        public void ToMainView()
        {
            this.PostPageVisible = true;
            this.ProfilePageVisible = false;
            this.SettingsPageVisible = false;
            NotifiyFailedAction("show");
        }

        [RelayCommand]
        public async void ToProfileView()
        {
            this.PostPageVisible = false;
            this.ProfilePageVisible = true;
            this.SettingsPageVisible = false;
            ProfilePosts.Clear();
            ProfileFollowersList.Clear();
            ProfileFriendsList.Clear();
            ShowUserProfile(_userSession.Id.ToString());
        }

        [RelayCommand]
        public void ToRecipesView()
        {
            NotifiyFailedAction("hide");
        }

        [RelayCommand]
        public void ToSettingView()
        {
            this.PostPageVisible = false;
            this.ProfilePageVisible = false;
            this.SettingsPageVisible = true;
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
        }

        [RelayCommand]
        public void ShowChangeEmialEntry(){
            this.ChangeUsernameEntryVisible = false;
            this.ChangeEmailEntryVisible =true; 
            this.ChangePasswordEntryVisible = false;
        }

        [RelayCommand]
        public void ShowChangePasswordEntry(){
            this.ChangeUsernameEntryVisible = false;
            this.ChangeEmailEntryVisible = false;
            this.ChangePasswordEntryVisible = true;
        }

        [RelayCommand]
        public void ChangeProfilePicture()
        {
            this.ChangeUsernameEntryVisible = false;
            this.ChangeEmailEntryVisible = false;
            this.ChangePasswordEntryVisible = false;
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
            this.ProfileFollowersVisible = false;
            this.ProfilePostsVisible = true;
            this.ProfileFriendsVisible = false;
            try
            {
                if (id != _userSession.Id.ToString())
                {
                    CreateSearchResultHistory(id);
                    this.PostPageVisible = false;
                    this.ProfilePageVisible = true;
                    this.SettingsPageVisible = false;
                    ProfilePosts.Clear();
                    ProfileFollowersList.Clear();
                    ProfileFriendsList.Clear();
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

            this.ProfileFollowersVisible = false;
            this.ProfilePostsVisible = true;
            this.ProfileFriendsVisible = false;
            SetButtonColors(Buttons.PostButton);
            try
            {
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
        public void LikePost(string id)
        {
            int i = 1;
            int j = 2;
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
            }
            else
            {
                mergedDictionaries.Add(new LightTheme());
                SwitchThemeMode = "Light Theme";
                ReloadProfileButtonColors(ThemeFlag);
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

        //private async Task OpenUserProfile(string id)
        //{
        //    var profileJson = await GetUserProfileModel(id);
        //    var profile = await JsonToObject<UserProfileModel>(profileJson);

        //    if (profile is null) { throw new Exception(); }

        //    SetUserProfileModel(profile.Id,
        //                        profile.FriendsCount + " friends",
        //                        profile.FollowsCount + " follows",
        //                        profile.LastName,
        //                        profile.FirstName,
        //                        profile.Username,
        //                        profile.ProfilePictureBase64);          

        //    var json = await GetUserProfilePosts(id);
        //    var posts = await JsonToObject<List<PostDto>>(json);
        //    DisplayProfilePosts(posts);

        //    var json2 = await GetUserProfileFriends(id);
        //    var friends = await JsonToObject<List<ListedFriendDto>>(json2);
        //    DisplayProfileFriends(friends);


        //    var json3 = await GetUserProfileFollowers(id);
        //    var followers = await JsonToObject<List<ListedFriendDto>>(json3); //followers can be displayed using the same Dto and OnListFriewView
        //    DisplayProfileFollowers(followers);
        //}

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

                // Równoległe pobieranie danych
                var postsTask = GetUserProfilePosts(id);
                var friendsTask = GetUserProfileFriends(id);
                var followersTask = GetUserProfileFollowers(id);

                await Task.WhenAll(postsTask, friendsTask, followersTask);

                var posts = await JsonToObject<List<PostDto>>(await postsTask);
                DisplayProfilePosts(posts);

                var friends = await JsonToObject<List<ListedFriendDto>>(await friendsTask);
                DisplayProfileFriends(friends);

                var followers = await JsonToObject<List<ListedFriendDto>>(await followersTask);
                DisplayProfileFollowers(followers);
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"An error occurred: {ex.Message}");
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

        public ObservableCollection<OnListFriendView> ProfileFriendsList { get { return profileFriendsList; } set { profileFriendsList = value; } }
        private ObservableCollection<OnListFriendView> profileFriendsList = new ObservableCollection<OnListFriendView>();

        public ObservableCollection<OnListFriendView> ProfileFollowersList { get { return profileFollowersList; } set { profileFollowersList = value; } }
        private ObservableCollection<OnListFriendView> profileFollowersList = new ObservableCollection<OnListFriendView>();

        public ObservableCollection<PostView> ProfilePosts { get { return profilePosts; } set { profilePosts = value; } }
        private ObservableCollection<PostView> profilePosts = new ObservableCollection<PostView>();

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

        private async Task DisplayProfilePosts(List<PostDto> posts)
        {
            if (posts == null || posts.Count == 0)
            {
                NoPostOnProfile = true;
                ProfilePostsVisible = false;
                return;
            }

            //await Task.Run(() => { Dispatcher.GetForCurrentThread().Dispatch(() => { ProfilePosts.Clear(); }); });
            await Dispatcher.GetForCurrentThread().DispatchAsync(() => { ProfilePosts.Clear(); });
            foreach (var post in posts)
            {
                var commentList = new List<CommentView>();
                foreach (var comment in post.Comments)
                {
                    commentList.Add(new CommentView()
                    {
                        Username = comment.Username,
                        CommentContent = comment.CommentContent,
                        CommentId = comment.CommentId.ToString(),
                        LikeCount = comment.Likes.ToString(),
                        UserId = comment.UserId.ToString()
                    }); ;
                }

                var imageBase64list = new List<string>();
                foreach (var image in post.PostImagesBase64)
                {
                    imageBase64list.Add(image);
                }

                var postview = new PostView()
                {
                    Username = post.Username,
                    TimeStamp = post.TimeStamp,
                    PostLikeCount = post.Likes.ToString(),
                    PostTextContent = post.Description,
                    ImageSource = post.PostImagesBase64[0],
                    Comments = commentList,
                    ImagesBase64 = imageBase64list
                };

      
                ProfilePosts.Add(postview);
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

    }
}
