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

            //await btn.ScaleTo(1.05, 150, Easing.BounceIn);
            //await btn.ScaleTo(1, 150, Easing.BounceOut);


        }
    }

}
