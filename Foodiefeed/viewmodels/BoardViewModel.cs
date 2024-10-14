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
using System.Diagnostics;
using System.Data;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Foodiefeed.extension;
using System.Collections.Specialized;
using static Foodiefeed.views.windows.contentview.OnlineFreidnListElementView;
using System.Net;
using System.Text.RegularExpressions;
using System.Text;
using Foodiefeed.Resources.Styles;


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

        private const string apiBaseUrl = "http://localhost:5000";

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
            notifications.CollectionChanged += OnNotificationsChanged;
            DisplaySearchResultHistory();
            _userSession = userSession;
            _userSession.Id = 15;

            notifications.Add(new BasicNotofication());
            notifications.Add(new FriendRequestNotification());
            notifications.Add(new BasicNotofication());
            notifications.Add(new BasicNotofication());
            notifications.Add(new BasicNotofication());
            notifications.Add(new FriendRequestNotification());
            notifications.Add(new FriendRequestNotification());
            notifications.Add(new FriendRequestNotification());
            notifications.Add(new BasicNotofication());
            notifications.Add(new FriendRequestNotification());
            notifications.Add(new FriendRequestNotification());

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
        }

        [RelayCommand]
        private async void ClearNotifications()
        {
            for (int i = 0; i <= Notifications.Count - 1; i++)
            {
                var notification = Notifications[i];
                if(notification is not FriendRequestNotification)
                {
                    await notification.HideAnimation(300, 150);
                    Notifications.Remove(notification);
                    i = i - 1;
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

            //if (e.LastVisibleItemIndex == Posts.Count() - 2)
            //{
            //   
            //}
        }


        [RelayCommand]
        public void ToMainView()
        {
            this.PostPageVisible = true;
            this.ProfilePageVisible = false;
            this.SettingsPageVisible = false;
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

        //private string HandlePersonalDataChange(string endpointBase,string propertyValue,string regex)
        //{
        //    if (Regex.IsMatch(propertyValue, regex))
        //    {
        //        return endpointBase + propertyValue;
        //    }
        //    else return string.Empty;
        //}

        private async Task UpdatePersonalData(string endpoint,StringContent contnet)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseUrl);

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

        private enum Buttons
        {
            PostButton,
            FriendsButton,
            FollowersButton
        }

        private async Task SetButtonColors(Buttons button)
        {

            switch (button)
            {
                case Buttons.PostButton:
                    SelfPostButtonColor = Color.FromHex("#ffffff");
                    FriendsButtonColor = Color.FromHex("#c9c9c9"); 
                    FollowersButtonColor = Color.FromHex("#c9c9c9");
                    break;

                case Buttons.FriendsButton:
                    SelfPostButtonColor = Color.FromHex("#c9c9c9");
                    FriendsButtonColor = Color.FromHex("#ffffff");
                    FollowersButtonColor = Color.FromHex("#c9c9c9");
                    break;

                case Buttons.FollowersButton:
                    SelfPostButtonColor = Color.FromHex("#c9c9c9");
                    FriendsButtonColor = Color.FromHex("#c9c9c9");
                    FollowersButtonColor = Color.FromHex("#ffffff");
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
                client.BaseAddress = new Uri(apiBaseUrl);

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
                    //code to show Something went wrong on search panel.
                }
            }

        }

        [RelayCommand]
        public async void AddToFriends(string id)
        {
            var endpoint = $"api/friends/add/{_userSession.Id}/{id}";

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(apiBaseUrl);

                try
                {
                    var response = await httpClient.PostAsync(endpoint, null);
                }
                catch
                {
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
                httpClient.BaseAddress = new Uri(apiBaseUrl);

                try
                {
                    var response = httpClient.DeleteAsync(endpoint);
                }
                catch
                {
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
                httpClient.BaseAddress = new Uri(apiBaseUrl);

                try
                {
                    await httpClient.PostAsync(endpoint,null);
                }
                catch
                {

                }

            }
        }

        [RelayCommand]
        public async void UnfollowUser(string id)
        {
            var endpoint = $"api/followers/unfollow/{_userSession.Id}/{id}";

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(apiBaseUrl);

                try
                {
                    await httpClient.DeleteAsync(endpoint);
                }
                catch
                {

                }

            }
        }

        [RelayCommand]
        public async void CancelFriendRequest(string id)
        {
            var endpoint = $"api/friends/request/cancel/{_userSession.Id}/{id}";

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(apiBaseUrl);

                try
                {
                    var response = await httpClient.DeleteAsync(endpoint);
                }
                catch
                {
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

            }
            else
            {
                mergedDictionaries.Add(new LightTheme());
                SwitchThemeMode = "Light Theme";
            }

        }

        private void DisplaySearchResults(ObservableCollection<UserSearchResult> users)
        {
            SearchResults.Clear();

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

        private void DisplaySearchResultHistory()
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

            var json = File.ReadAllTextAsync(SearchHistoryJsonPath).Result;

            DisplaySearchResults(JsonToObject<ObservableCollection<UserSearchResult>>(json).Result); 
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
                httpclient.BaseAddress = new Uri(apiBaseUrl);
                var endpoint = $"api/user/{id}";

                try
                {
                    var response = await httpclient.GetAsync(endpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        var results = await response.Content.ReadAsStringAsync();
                        if(results is null) { return null; }
                        return JsonConvert.DeserializeObject<UserSearchResult>(results);
                    }
                }catch(Exception ex)
                {
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
                    httpClient.BaseAddress = new Uri(apiBaseUrl);

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
                httpClient.BaseAddress = new Uri(apiBaseUrl);

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

            await Task.Run(() => { ProfilePosts.Clear(); });

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
                httpClient.BaseAddress = new Uri(apiBaseUrl);

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
                client.BaseAddress = new Uri(apiBaseUrl);

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
                        //code of block that displays that Friends are currently unavaiable
                    }
                }catch (Exception e)
                {
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
                httpclient.BaseAddress = new Uri(apiBaseUrl);

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

    }
}
