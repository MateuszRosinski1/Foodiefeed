#if ANDROID
using Android.App;
#endif
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Foodiefeed.views.windows.contentview;
using Foodiefeed.views.windows.popups;
using System.Collections.ObjectModel;


namespace Foodiefeed.viewmodels
{
    public partial class BoardViewModel : ObservableObject
    {
        public ObservableCollection<PostView> Posts { get { return posts;} }

        private ObservableCollection<PostView> posts 
            = new ObservableCollection<PostView>();

        public ObservableCollection<OnlineFreidnListElementView> OnlineFriends { get { return onlineFriends; } }

        private ObservableCollection<OnlineFreidnListElementView> onlineFriends 
            = new ObservableCollection<OnlineFreidnListElementView> { };

        public ObservableCollection<OnListFriendView> ProfilePageFriends { get { return profilePageFriends; } }

        private ObservableCollection<OnListFriendView> profilePageFriends = 
            new ObservableCollection<OnListFriendView>();

        public struct CurrentProfilePageMember
        {
            public int Id;
            public string Name;
            public string LastName;
            public string Username;
            public int followers;
        }

        #region VisibilityFlags

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

        #endregion

        public BoardViewModel()
        {

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
            profilePageFriends.Add(new OnListFriendView() { Username = "mati" });
            profilePageFriends.Add(new OnListFriendView() { Username = "Adrian Lozycki" });
            profilePageFriends.Add(new OnListFriendView() { Username = "Kornelio1239045asdasdassd" });


            this.ProfilePageVisible = true; //on init false
            this.PostPageVisible = false; //on init true
            this.SettingsPageVisible = false; //on init false
            this.PersonalDataEditorVisible = false; // on init false
            this.SettingsMainHubVisible = true; //on init true
            this.ChangeUsernameEntryVisible = false; //on init false
            this.ChangeEmailEntryVisible = false; //on init false
            this.ChangePasswordEntryVisible = false; //on init false
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
        public void LikePost(string id)
        {
            int i = 1;
            int j = 2;
        }
    }
}
