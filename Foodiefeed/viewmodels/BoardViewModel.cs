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
        [ObservableProperty]
        ObservableCollection<PostView> posts = new ObservableCollection<PostView>();
   
        [ObservableProperty]
        bool profilePageVisible;

        [ObservableProperty]
        bool postPageVisible;

        public BoardViewModel()
        {
            this.ProfilePageVisible = true; //on init false
            this.postPageVisible = false; //on init true
            
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

        }

        [RelayCommand]
        public void ToProfileView()
        {
            this.PostPageVisible = false;
            this.ProfilePageVisible = true;
            
        }

        [RelayCommand]
        public void ToRecipesView()
        {

        }

        [RelayCommand]
        public void ToSettingView()
        {

        }

        [RelayCommand]
        public async void ShowPopup()
        {
            var popup = new UserOptionPopup();
            App.Current.MainPage.ShowPopup(popup);
        }

        
    }

}
