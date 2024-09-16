namespace Foodiefeed.views.windows.contentview;

public partial class UserSearchResultView : ContentView
{
	public UserSearchResultView()
	{
		InitializeComponent();
	}

    private async void PointerGestureRecognizer_PointerEntered(object sender, PointerEventArgs e)
    {
        await AnimateFont(sender, 20, 25, 500, 200);
        await AnimateShadow(sender, 0, 0.5, 50, 300);
    }

    private async void PointerGestureRecognizer_PointerExited(object sender, PointerEventArgs e)
    {
        await AnimateFont(sender, 25, 20, 500, 200);
        await AnimateShadow(sender, 0.5, 0, 50, 300);
    }

    private async void CondenseButtonSizeAnimation(object sender, PointerEventArgs e)
    {
        await AnimateSize(sender, 1);
    }
    private async void ScaleButtonSizeAnimation(object sender, PointerEventArgs e)
    {
        await AnimateSize(sender, 1.1);
    }

    private async Task AnimateSize(object sender, double scale)
    {
        Button button = (Button)sender;
        await button.ScaleTo(scale, 100, Easing.BounceOut);
    }

    private async Task AnimateFont(object sender, int from, int to, uint rate, uint lenght)
    {
        Button button = (Button)sender;
        var animation = new Animation(v => button.FontSize = v, from, to);
        animation.Commit(this, button.Text + "_Font", rate, lenght, Easing.BounceIn);
    }

    private async Task AnimateShadow(object sender, double from, double to, uint rate, uint lenght)
    {
        Button button = (Button)sender;
        var animation = new Animation(v => button.Shadow.Opacity = (float)v, from, to);
        animation.Commit(this, button.Text + "_Shadow", rate, lenght, Easing.Linear);
    }
}