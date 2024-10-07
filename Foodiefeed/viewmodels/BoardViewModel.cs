#if ANDROID
using Android.App;
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


namespace Foodiefeed.viewmodels
{
    public partial class BoardViewModel : ObservableObject
    {
        //https://icons8.com/icons/set/microphone icons
        //https://github.com/dotnet/maui/issues/8150  shadow resizing problem
        //https://github.com/CommunityToolkit/Maui/pull/2072 uniformgrid issue

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

        #endregion


        public ObservableCollection<INotification> Notifications { get { return notifications; } }
        private ObservableCollection<INotification> notifications = new ObservableCollection<INotification>();


        public BoardViewModel(UserSession userSession)
        {
            notifications.CollectionChanged += OnNotificationsChanged;
            DisplaySearchResultHistory();
            UpdateOnlineFriendListThread = new Thread(UpdateOnlineFriendList);
            UpdateOnlineFriendListThread.Start();
            _userSession = userSession;
            _userSession.Id = 15;
            posts.Add(new PostView() { Username = "kiwigamer5" ,TimeStamp = "10 hours",PostLikeCount = 102.ToString() ,
                PostTextContent = "Smak jesieni 🐌☕️\U0001f90e\r\nPuszyste, miękkie i wilgotne cynamonki 🍂\r\n•\r\n•\r\nSkładniki:\r\n\U0001f90eCiasto\r\n•380ml mleka\r\n•100g cukru\r\n•100g masła\r\n•30g świeżych drożdży\r\n•2 jajka\r\n•720g mąki pszennej\r\n•szczypta soli\r\n\U0001f90eNadzienie\r\n•100g masła\r\n•150g cukru (najlepiej trzcinowego)\r\n•4 łyżeczki cynamonu\r\n\U0001f90ePolewa\r\n•100g serka typu philadelphia\r\n•60g śmietanki 36% (dodatkowo 80g śmietanki do wlania między bułeczki)\r\n•160g cukru pudru\r\n\U0001f90eWykonanie\r\n•\r\n•\r\nDo garnka przekładamy mleko, masło i cukier. Podgrzewamy na małym ogniu do momentu całkowitego rozpuszczenia (nie doprowadzamy do wrzenia).\r\nPrzelewamy całość do dużej miski i sprawdzamy temperaturę. Jeśli mleko jest ciepłe (ale nie gorące!) dodajemy drożdże, mieszamy, nakrywamy ściereczką i odstawiamy na 15/20 minut.\r\n•\r\n•\r\nKiedy rozczyn podrośnie dodajemy do niego jajka i mieszamy do połączenia.\r\nDo masy dodajemy mąkę wymieszaną ze szczyptą soli, cały czas wyrabiając ciasto. Gdy będzie gładkie, lekko lepikie nakrywamy ściereczką do wyrośnięcia na ok. 1h.\r\n•\r\n•\r\nW tym czasie przygotowujemy nadzienie. Miękkie masło łączymy z cukrem trzcinowym i cynamonem.\r\n•\r\n•\r\nWyrośnięte ciasto przekładamy na blat i delikatnie zagniatamy.\r\nGładkie, zagniecione ciasto musimy rozwałkować na kształt prostokąta (u mnie ok. 40x50 cm). Smarujemy nadzieniem, następnie zwijamy ciasto, tak aby powstała rolada.\r\nKroimy (żyłką, nitką lub nożem) i układamy na blaszce wyłożonej papierem do pieczenia. Układamy je tak żeby po drugim wyrośnięciu się stykały (tak jak na nagraniu). Blaszkę przykrywamy ściereczką i odstawiamy na 20/30 minut.\r\n•\r\n•\r\nW międzyczasie rozgrzewamy piekarnik do 180° i przygotowujemy polewę mieszając serek, śmietankę oraz cukier puder.\r\n•\r\n•\r\nPo ponownym wyrośnięciu, wlewamy 80g śmietanki pomiędzy bułeczki.\r\n•\r\n•\r\nPieczemy 20 minut, do momentu aż się zarumienią. Po wyjęciu z piekarnika lukrować póki ciepłe, dzięki temu będą bardziej miękkie.\r\n•\r\n•\r\nGotowe!🐌\U0001f90e\r\n•\r\n•",
                Comments =
                [
                    new CommentView()
                    {
                        Username = "martino",
                        CommentContent = "Bardo fajny przepis polecam ",
                        CommentId = 2.ToString(),
                        LikeCount = 234.ToString()
                    },
                    new CommentView(),
                    new CommentView(),
                    new CommentView(),
                    new CommentView(),
                    new CommentView(),
                    new CommentView(),
                    new CommentView(),
                    new CommentView(),
                    new CommentView(),
                ]
                });

            onlineFriends.Add(new OnlineFreidnListElementView() { Username = "mati123",UserId = 16.ToString(), AvatarImageSource = "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" });
            //profilePageFriends.Add(new OnListFriendView() { Username = "mati" });
            //profilePageFriends.Add(new OnListFriendView() { Username = "Adrian Lozycki" });
            //profilePageFriends.Add(new OnListFriendView() { Username = "Kornelio1239045asdasdassd" });


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




            //var usernames = new List<string>
            //{
            //    "Mieki Adrian5", "Sztywny Seba0", "Kasia Zielona", "Mare kKowal", "AniaNowak",
            //    "JanekWojcik", "EwaSierżant", "PiotrMucha", "OlaWolska", "MarcinLis",
            //    "MagdaSienkiewicz", "RafałKrawczyk", "NataliaSkrzypczak", "KrzysztofBiel", "AgnieszkaWójcik",
            //    "TomekPawlak", "KamilaKaczmarek", "GrzegorzStasiak", "JoannaJankowska", "WojtekZalewski"
            //};

            //for (int i = 0; i < usernames.Count; i++)
            //{
            //    searchResults.Add(new UserSearchResultView()
            //    {
            //        Username = usernames[i],
            //        Follows = (i * 10 % 50).ToString(), 
            //        Friends = (i % 10).ToString()        
            //    });
            //}


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
            this.HubPanelVisible = false;
        }

        [RelayCommand]
        private async void ClearNotifications()
        {
            for (int i = 0; i <= Notifications.Count - 1; i++)
            {
                var notification = Notifications[i];
                await notification.HideAnimation(300, 150);
                Notifications.Remove(notification);
                //await RemoveAsync(notification,Notifications);
                i = i - 1;
            }
            //code to clear notification in database
        }

        //private Task RemoveAsync(INotification notification,ObservableCollection<INotification> collection)
        //{
        //    collection.Remove(notification);
        //    return Task.CompletedTask;
        //}

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
        public void ToProfileView()
        {
            this.PostPageVisible = false;
            this.ProfilePageVisible = true;
            this.SettingsPageVisible = false;
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

            var popup = new UserOptionPopup()
            {
                UserId = user.Id.ToString(), 
                Username = user.Username, 
                AvatarImageSource = user.ProfilePictureBase64,
                IsFollowed = user.IsFollowed,
                IsFriend = user.IsFriend
                
            };

            App.Current.MainPage.ShowPopup(popup);
        }


        [RelayCommand]
        public void OpenPersonalDataEditor()
        {
            this.PersonalDataEditorVisible = true;
            this.SettingsMainHubVisible = false;
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
        public void ShowSearchPanel()
        {
            this.SearchPanelVisible = true;
            DisplaySearchResultHistory();
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
        public void ShowUserProfile(string id)
        {
            ProfilePosts.Clear();
            ProfileFollowersList.Clear();
            ProfileFriendsList.Clear();
            this.ProfileFollowersVisible = false;
            this.ProfilePostsVisible = true;
            this.ProfileFriendsVisible = false;
            try
            {
                if (id != _userSession.Id.ToString())
                {
                    CreateSearchResultHistory(id);
                    ToProfileView();
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
            if (SearchParam == string.Empty) { DisplaySearchResultHistory(); return; }

            var endpoint = "api/user/search-users/" + SearchParam;

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
            var endpoint = $"api/friend-request/send/{id}/{_userSession.Id}";

        }

        [RelayCommand]
        public async void UnfriendUser(string id)
        {
            var endpoint = $"api/friends/unfriend/{id}/{_userSession.Id}";
        }

        [RelayCommand]
        public async void FollowUser(string id)
        {
            var endpoint = $"api/followers/follow//{id}/{_userSession.Id}";
        }

        [RelayCommand]
        public async void UnfollowUser(string id)
        {
            var endpoint = $"api/followers/unfollow//{id}/{_userSession.Id}";
        }

        [RelayCommand]
        public async void SearchByVoice()
        {
            
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
                    Friends = user.FollowersCount.ToString()
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

        private void UpdateOnlineFriendList()
        {
            while (true)
            {
                Thread.Sleep(60000);
            }
        }

        private async Task OpenUserProfile(string id)
        {

            //ProfilePosts.Clear();
            //ProfileFollowersList.Clear();
            //ProfileFriendsList.Clear();

            var profileJson = await GetUserProfileModel(id);
            var profile = await JsonToObject<UserProfileModel>(profileJson);

            if (profile is null) { throw new Exception(); }

            SetUserProfileModel(profile.Id,
                                profile.FriendsCount + " friends",
                                profile.FollowsCount + " follows",
                                profile.LastName,
                                profile.FirstName,
                                profile.Username,
                                profile.ProfilePictureBase64);

            var json = await GetUserProfilePosts(id);
            var posts = await JsonToObject<List<PostDto>>(json);
            DisplayProfilePosts(posts);

            var json2 = await GetUserProfileFriends(id);
            var friends = await JsonToObject<List<ListedFriendDto>>(json2);
            DisplayProfileFriends(friends);
            
            
            var json3 = await GetUserProfileFollowers(id);
            var followers = await JsonToObject<List<ListedFriendDto>>(json3); //followers can be displayed using the same Dto and OnListFriewView
            DisplayProfileFollowers(followers);
 
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
            //ProfileFriendsList.Clear();
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

        private void DisplayProfilePosts(List<PostDto> posts)
        {
            if (posts == null || posts.Count == 0)
            {
                NoPostOnProfile = true;
                ProfilePostsVisible = false;
                return;
            }
            //ProfilePosts.Clear();
            foreach(var post in posts)
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
