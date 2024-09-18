using Microsoft.Maui.Layouts;

namespace Foodiefeed.views.windows.contentview;

public partial class OnListFriendView : ContentView
{
    public static readonly BindableProperty UsernameProperty =
        BindableProperty.Create(nameof(Username), typeof(string), typeof(OnListFriendView), default(string), propertyChanged: OnUsernameChanged);

    public string UserId { get; set; }

    public string Username
    {
        get => (string)GetValue(UsernameProperty);
        set => SetValue(UsernameProperty, value);
    }

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

    private static void OnUsernameChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (OnListFriendView )bindable;
        view.UsernameLabel.Text = newValue as string;
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