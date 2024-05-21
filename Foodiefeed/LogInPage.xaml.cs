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

        private async void ClickAnimation(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            await btn.ScaleTo(1.05, 150, Easing.Linear);
            await btn.ScaleTo(1, 150, Easing.Linear);
        }

        private async void ImageClickAnimation(object sender, EventArgs e)
        {
            ImageButton btn = (ImageButton)sender;
            await btn.ScaleTo(0.5, 150, Easing.Linear);
            await btn.ScaleTo(0.4, 150, Easing.Linear);
        }
    }

}
