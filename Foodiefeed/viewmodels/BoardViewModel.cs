#if ANDROID
using Android.App;
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


namespace Foodiefeed.viewmodels
{
    public partial class BoardViewModel : ObservableObject
    {

        //https://github.com/dotnet/maui/issues/8150  shadow resizing problem

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

        #endregion

        public BoardViewModel(UserSession userSession)
        {
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

            onlineFriends.Add(new OnlineFreidnListElementView() { Username = "mati123" });
            //profilePageFriends.Add(new OnListFriendView() { Username = "mati" });
            //profilePageFriends.Add(new OnListFriendView() { Username = "Adrian Lozycki" });
            //profilePageFriends.Add(new OnListFriendView() { Username = "Kornelio1239045asdasdassd" });



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
        public async void ShowPopup()
        {
            var popup = new UserOptionPopup();
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
        public async void ShowUserProfile(string id)
        {
            if(id != _userSession.Id.ToString()) { 
                CreateSearchResultHistory(id); 
                ToProfileView();
            }

            await OpenUserProfile(id);

        }

        [RelayCommand]
        public void ShowProfilePosts()
        {
            this.ProfileFollowersVisible = false;
            this.ProfilePostsVisible = true;
            this.ProfileFriendsVisible = false;
        }

        [RelayCommand]
        public void ShowProfileFriends()
        {
            this.ProfileFollowersVisible = false;
            this.ProfilePostsVisible = false;
            this.ProfileFriendsVisible = true;
        }

        [RelayCommand]
        public void ShowProfileFollowers()
        {
            this.ProfileFollowersVisible = true;
            this.ProfilePostsVisible = false;
            this.ProfileFriendsVisible = false;
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
                        DisplaySearchResults(JsonToSearchResults(json));

                    }
                }
                catch(Exception ex)
                {
                    //code to show Something went wrong on search panel.
                }
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
                    Friends = user.FollowersCount.ToString()
                });
            }
        }

        private ObservableCollection<UserSearchResult> JsonToSearchResults(string json)
        {
            var users = JsonConvert.DeserializeObject<ObservableCollection<UserSearchResult>>(json);

            if (users is null) return new();

            return users;
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

            var json = File.ReadAllText(SearchHistoryJsonPath);

            DisplaySearchResults(JsonToSearchResults(json)); 
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
            var SearchHistory = JsonToSearchResults(json);

            

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
            ProfilePosts.Clear();
            ProfileFollowersList.Clear();
            ProfileFriendsList.Clear();

            GetUserProfileModel(id);
            GetUserProfileFriends(id);

            var json = await GetUserProfilePosts(id);
            var posts = JsonToPostDto(json);
            DisplayProfilePosts(posts);
            

            GetUserProfileFollowers(id);
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

        private List<PostDto> JsonToPostDto(string json)
        {
            var posts = JsonConvert.DeserializeObject<List<PostDto>>(json);

            return posts;
        }

        private void DisplayProfilePosts(List<PostDto> posts)
        {
            if (posts.Count == 0)
            {
                NoPostOnProfile = true;
                ProfilePostsVisible = false;
                return;
            }
            ProfilePosts.Clear();

            foreach(var post in posts)
            {
                ProfilePosts.Add(new PostView()
                {
                    Username = post.Username,
                    TimeStamp = post.TimeStamp,
                    PostLikeCount = post.Likes.ToString(),
                    PostTextContent = post.Description
                });
            }
            
        }


        private async void GetUserProfileFollowers(string id)
        {

        }

        private async void GetUserProfileFriends(string id)
        {

        }

        private async void GetUserProfileModel(string id)
        {
            var endpoint = $"api/user/user-profile/{id}";

            using (var httpclient = new HttpClient())
            {
                httpclient.BaseAddress = new Uri(apiBaseUrl);

                try
                {
                    var response = await httpclient.GetAsync(endpoint);

                    if(response is null) { throw new Exception(); }

                    var json = await response.Content.ReadAsStringAsync();

                    var profile = await JsonToUserProfileModel(json);

                    if(profile is null) { throw new Exception(); }

                    //ProfileId = profile.Id;
                    //ProfileFriends = profile.FriendsCount + " friends";
                    //ProfileFollowers = profile.FollowsCount +" follows";
                    //ProfileLastName = profile.LastName;
                    //ProfileName = profile.FirstName;
                    //ProfileUsername = profile.Username;
                    SetUserProfileModel(profile.Id, 
                                        profile.FriendsCount + " friends", 
                                        profile.FollowsCount + " follows", 
                                        profile.LastName, 
                                        profile.FirstName,
                                        profile.Username);

                }
                catch(Exception ex)
                {

                }
            }
        }

        private async Task<UserProfileModel> JsonToUserProfileModel(string json)
        {
            UserProfileModel model = JsonConvert.DeserializeObject<UserProfileModel>(json);

            return model;
        }

        private void SetUserProfileModel(int Id,string FriendsCount,string FollowsCount,string LastName,string FirstName,string Username)
        {
            ProfileId = Id;
            ProfileFriends = FriendsCount + " friends";
            ProfileFollowers = FollowsCount + " follows";
            ProfileLastName = LastName;
            ProfileName = FirstName;
            ProfileUsername = Username;
        }
    }
}
