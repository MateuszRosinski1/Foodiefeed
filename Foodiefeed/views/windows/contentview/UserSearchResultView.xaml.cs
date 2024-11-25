

namespace Foodiefeed.views.windows.contentview;

public partial class UserSearchResultView : ContentView
{
    public string UserId { get; set; }

    public static readonly BindableProperty UsernameProperty =
        BindableProperty.Create(nameof(Username),typeof(string),typeof(UserSearchResultView),default(string),propertyChanged: OnUsernameChanged);

    public static readonly BindableProperty FollowsProperty =
        BindableProperty.Create(nameof(Follows), typeof(string), typeof(UserSearchResultView), default(string), propertyChanged: OnFollowsChanged);

    public static readonly BindableProperty FriendsProperty =
        BindableProperty.Create(nameof(Friends), typeof(string), typeof(UserSearchResultView), default(string), propertyChanged: OnFriendsChanged);

    public static readonly BindableProperty PfpImageBase64Property =
        BindableProperty.Create(nameof(PfpImageBase64), typeof(string), typeof(UserSearchResultView), default(string), propertyChanged: OnImageChanged);

    private static void OnImageChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (UserSearchResultView)bindable;

        if (newValue is null) return;

        var newValueString = newValue as string;

        if (string.IsNullOrEmpty(newValueString))
        {
            view.pfpImage.Source = "avatar.jpg";
            return;
        }

        var imageBytes = Convert.FromBase64String(newValueString);

        view.pfpImage.Source = Microsoft.Maui.Controls.ImageSource.FromStream(() =>
        {
            var stream = new MemoryStream(imageBytes);
            stream.Position = 0;
            return stream;
        });
    }

    public UserSearchResultView()
	{
		InitializeComponent();
	}

    public string PfpImageBase64
    {
        get => (string)GetValue(PfpImageBase64Property);
        set => SetValue(PfpImageBase64Property, value);
    }

    public string Username
    {
        get => (string)GetValue(UsernameProperty);
        set => SetValue(UsernameProperty, value);
    }

    public string Follows
    {
        get =>(string)GetValue(FollowsProperty);
        set => SetValue(FollowsProperty, value);
    }

    public string Friends
    {
        get => (string)GetValue(FriendsProperty);
        set => SetValue(FriendsProperty, value);
    }

    private static void OnUsernameChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (UserSearchResultView)bindable;
        view.UsernameLabel.Text = newValue as string;
    }

    private static void OnFollowsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (UserSearchResultView)bindable;
        view.FollowsLabel.Text = newValue as string +" Follows";
    }

    private static void OnFriendsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (UserSearchResultView)bindable;
        view.FriendsLabel.Text = newValue as string+" Friends";
    }

    #region buttonAnimation

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
    #endregion
}