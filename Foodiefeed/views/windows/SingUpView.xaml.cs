using Foodiefeed.models.dto;
using Foodiefeed.viewmodels;
using Newtonsoft.Json;
using System.Text;

namespace Foodiefeed
{
    public partial class SignUpView : ContentPage
    {
        public SignUpView(UserViewModel vm)
        {
            InitializeComponent();
            this.BindingContext = vm;
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
