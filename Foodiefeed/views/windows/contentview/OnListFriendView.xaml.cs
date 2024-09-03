using Microsoft.Maui.Layouts;

namespace Foodiefeed.views.windows.contentview;

public partial class OnListFriendView : ContentView
{
	public OnListFriendView()
	{
		InitializeComponent();
	}

    private async void AnimateOptionDots(object sender, PointerEventArgs e)
    {
		await FirstCircle.ScaleTo(1.3, 100, Easing.BounceIn);
        await FirstCircle.ScaleTo(1, 100, Easing.BounceOut);
        await SecondCircle.ScaleTo(1.3, 100, Easing.BounceIn);
        await SecondCircle.ScaleTo(1, 100, Easing.BounceOut);
        await ThirdCircle.ScaleTo(1.3, 100, Easing.BounceIn);
        await ThirdCircle.ScaleTo(1, 100, Easing.BounceOut);

    }

    private void ShowUserOptionPanel(object sender, TappedEventArgs e)
    {
        //OptionsPanel.IsVisible = true;
        
    }

    private void HideOptionPanel(object sender, PointerEventArgs e)
    {
        //OptionsPanel.IsVisible = false;
    }
}