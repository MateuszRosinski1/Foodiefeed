#if WINDOWS
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Maui.Platform;
#endif

namespace Foodiefeed.views.windows.contentview;

public partial class OnListFriendView : ContentView
{
    public static readonly BindableProperty UsernameProperty =
        BindableProperty.Create(nameof(Username), typeof(string), typeof(OnListFriendView), default(string), propertyChanged: OnUsernameChanged);


    public static readonly BindableProperty AvatarImageSourceProperty =
        BindableProperty.Create(nameof(AvatarImageSource), typeof(string), typeof(PostView), default(string), propertyChanged: OnImageSourceChanged);

    public static readonly BindableProperty UserIdProperty =
    BindableProperty.Create(nameof(UserId), typeof(string), typeof(PostView), default(string));


    public string UserId 
    {
        get => (string)GetValue(UserIdProperty);
        set => SetValue(UserIdProperty, value);
    }

    public string AvatarImageSource
    {
        get => (string)GetValue(AvatarImageSourceProperty);
        set => SetValue(AvatarImageSourceProperty, value);
    }

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

    private static void OnImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (OnListFriendView)bindable;

        if (newValue is null) return;

        var newValueString = newValue as string;

        view.avatarImage.Source = newValueString;


        //if (string.IsNullOrEmpty(newValueString))
        //{
        //    view.avatarImage.Source = "avatar.jpg";
        //    return;
        //}

        //var imageBytes = Convert.FromBase64String(newValueString);

        //view.avatarImage.Source = Microsoft.Maui.Controls.ImageSource.FromStream(() =>
        //{
        //    var stream = new MemoryStream(imageBytes);
        //    stream.Position = 0;
        //    return stream;
        //});
    }
}