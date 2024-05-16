using Foodiefeed.viewmodels;
using System.Windows.Input;

namespace Foodiefeed
{
    public partial class LogInPage : ContentPage
    {
        public LogInPage()
        {
            InitializeComponent();
            this.BindingContext = new UserViewModel();
        }

        private async void LogInButtonClickAsync(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BoardView());
        }
    }

}
