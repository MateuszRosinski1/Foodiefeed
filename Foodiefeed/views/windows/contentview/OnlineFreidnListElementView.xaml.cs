using CommunityToolkit.Maui.ImageSources;

namespace Foodiefeed.views.windows.contentview;

public partial class OnlineFreidnListElementView : ContentView
{
	public static readonly BindableProperty UsernameProperty = 
		BindableProperty.Create(nameof(Username),typeof(string),typeof(OnlineFreidnListElementView),default(string),propertyChanged: OnUsernameChanged);

	public static readonly BindableProperty UserIdProperty =
		BindableProperty.Create(nameof(UserId), typeof(string), typeof(OnlineFreidnListElementView), default(string));

    public static readonly BindableProperty AvatarImageSourceProperty =
       BindableProperty.Create(nameof(AvatarImageSource), typeof(string), typeof(OnlineFreidnListElementView), default(string), propertyChanged: OnImageSourceChanged);

    public static readonly BindableProperty IsOnlineProperty =
               BindableProperty.Create(nameof(IsOnline), typeof(bool), typeof(OnlineFreidnListElementView), default(bool));


    public string Username
	{
		get => (string)GetValue(UsernameProperty);
		set => SetValue(UsernameProperty, value);
	}

    public string AvatarImageSource
    {
        get => (string)GetValue(AvatarImageSourceProperty);
        set => SetValue(AvatarImageSourceProperty, value);
    }

    public string UserId
	{
		get => (string)GetValue(UserIdProperty); 
		set => SetValue(UserIdProperty, value);
	}

    public bool IsOnline 
    { 
        get => (bool)GetValue(IsOnlineProperty);
        set => SetValue(IsOnlineProperty, value);
    }

    public OnlineFreidnListElementView()
    {
        InitializeComponent();
    }

    private static void OnUsernameChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (OnlineFreidnListElementView)bindable;
		view.UsernameLabel.Text = newValue as string;
    }

    private static void OnImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (OnlineFreidnListElementView)bindable;

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

    public enum Status
    {
        Offline,
        Online,
    }
}