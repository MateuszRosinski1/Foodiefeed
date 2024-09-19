using Foodiefeed.viewmodels;
using Foodiefeed.views.windows.contentview;
using Microsoft.Maui.Controls;
using System.Windows.Input;
using Foodiefeed.extension;
using Microsoft.Maui.Controls.Shapes;


namespace Foodiefeed
{
    public partial class BoardPage : ContentPage
    {
        public BoardPage(BoardViewModel vm)
        {
            InitializeComponent();
            this.BindingContext = vm;
            //PostGrid.Children.Add(new PostView());
            //PostGrid.Children.Add(new PostView());
            //PostGrid.Children.Add(new PostView());
            //PostGrid.Children.Add(new PostView());

            ////ProfileSection.Children.Add(new PostView());
            ////ProfileSection.Children.Add(new PostView());
            ////ProfileSection.Children.Add(new PostView());
            //OnlineFriendList.Children.Add(new OnlineFreidnListElementView());
            //OnlineFriendList.Children.Add(new OnlineFreidnListElementView());
            //OnlineFriendList.Children.Add(new OnlineFreidnListElementView());
            //OnlineFriendList.Children.Add(new OnlineFreidnListElementView());
            //OnlineFriendList.Children.Add(new OnlineFreidnListElementView());
            //OnlineFriendList.Children.Add(new OnlineFreidnListElementView());
            //OnlineFriendList.Children.Add(new OnlineFreidnListElementView());
        }

        private void OnScrolled(object sender, ItemsViewScrolledEventArgs e)
        {

        }

        private async void PointerGestureRecognizer_PointerEntered(object sender, PointerEventArgs e)
        {
            await AnimateFont(sender, 20, 25, 500, 200);
            await AnimateShadow(sender, 0, 0.5, 50, 300);
        }

        private async void PointerGestureRecognizer_PointerExited(object sender, PointerEventArgs e)
        {
            await AnimateFont(sender,25,20,500,200);
            await AnimateShadow(sender, 0.5, 0, 50, 300);
        }

        private async void CondenseButtonSizeAnimation(object sender, PointerEventArgs e)
        {
            await AnimateSize(sender,1);
        }
        private async void ScaleButtonSizeAnimation(object sender, PointerEventArgs e)
        {
            await AnimateSize(sender, 1.1);
        }

        private async Task AnimateSize(object sender,double scale)
        {
            Button button = (Button)sender;
            await button.ScaleTo(scale,100,Easing.BounceOut);
        }

        private async Task AnimateFont(object sender,int from,int to,uint rate,uint lenght)
        {
            Button button = (Button)sender;
            var animation = new Animation(v => button.FontSize = v, from, to);
            animation.Commit(this, button.Text+"_Font", rate, lenght, Easing.BounceIn);
        }

        private async Task AnimateShadow(object sender, double from, double to, uint rate, uint lenght)
        {
            Button button = (Button)sender;
            var animation = new Animation(v => button.Shadow.Opacity = (float)v, from, to);
            animation.Commit(this, button.Text+"_Shadow", rate, lenght, Easing.Linear);
        }

        //private async void FriendsButton_Clicked(object sender, EventArgs e)
        //{
        //    await SetButtonColors(FriendsButton);
        //}

        //private async void FollowersButton_Clicked(object sender, EventArgs e)
        //{
        //    await SetButtonColors(FollowersButton);
        //}

        //private async void SelfPostButton_Clicked(object sender, EventArgs e)
        //{
        //    await SetButtonColors(SelfPostButton);
        //}

        //private async Task SetButtonColors(Button activeButton)
        //{
        //    var buttons = new List<Button> { SelfPostButton, FriendsButton, FollowersButton };

        //    foreach (var button in buttons)
        //    {
        //        if (button == activeButton)
        //        {
        //            await button.ColorTo(Color.FromHex("#c9c9c9"), Colors.White, c => button.BackgroundColor = c, 100);
        //        }
        //        else
        //        {
        //            await button.ColorTo(Color.FromHex("#c9c9c9"), Color.FromHex("#c9c9c9"), c => button.BackgroundColor = c, 100);
        //        }
        //    }
        //}

        private void NotificationBellAnimation(object sender, PointerEventArgs e)
        {
            var path = sender as Microsoft.Maui.Controls.Shapes.Path;

            if (path is null) return;

            NotificationBellTransform.CenterX = path.Width / 2;

            var bellRotationFirstCycle = new Animation(v => NotificationBellTransform.Angle = v, 0, 15, Easing.Linear);
            var bellRotationSecondCycle = new Animation(v => NotificationBellTransform.Angle = v, 15, -15, Easing.Linear);
            var bellRotationThirdCycle = new Animation(v => NotificationBellTransform.Angle = v, -15, 0, Easing.Linear);

            var sequentialAnimation = new Animation
            {
                { 0, 0.25, bellRotationFirstCycle },
                { 0.25, 0.75, bellRotationSecondCycle },
                { 0.75, 1, bellRotationThirdCycle }
            };

            sequentialAnimation.Commit(this, "BellRotationSequential", 16, 600, Easing.Linear);
        }

        private async void OptionButtonScaleAnimation(object sender, PointerEventArgs e)
        {
            var button = sender as Button;
            if (button is null) return;
            await button.ScaleTo(1.1, 100, Easing.Linear);

        }

        private async void OptionButtonDescendAnimation(object sender, PointerEventArgs e)
        {
            var button = sender as Button;
            if (button is null) return;
            await button.ScaleTo(1, 100,Easing.Linear);
        }
    }
}
