using System.Windows.Input;

namespace Foodiefeed
{
    public partial class LogInPage : ContentPage
    {
        public LogInPage()
        {
            InitializeComponent();
        }

        private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new SignUpView());
        }

        private async void LogInButtonClickAsync(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BoardView());
        }
    }

}
