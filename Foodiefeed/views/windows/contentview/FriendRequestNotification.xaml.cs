using Foodiefeed.models.dto;

namespace Foodiefeed.views.windows.contentview;

public partial class FriendRequestNotification : ContentView, INotification
{
	public FriendRequestNotification()
	{
		InitializeComponent();
	}

    public static BindableProperty UserIdProperty =
        BindableProperty.Create(nameof(UserId), typeof(string), typeof(FriendRequestNotification), default(string));

    public static BindableProperty MessageProperty =
        BindableProperty.Create(nameof(Message), typeof(string), typeof(FriendRequestNotification), default(string), propertyChanged: OnUsernameChanged);

    public static BindableProperty ImageBase64Property =
        BindableProperty.Create(nameof(ImageBase64), typeof(string), typeof(FriendRequestNotification), default(string), propertyChanged: OnImageChanged);

    private static void OnImageChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (FriendRequestNotification)bindable;

        if (newValue is null) return;

        var newValueString = newValue as string;

        var imageBytes = Convert.FromBase64String(newValueString);

        view.image.Source = Microsoft.Maui.Controls.ImageSource.FromStream(() =>
        {
            var stream = new MemoryStream(imageBytes);
            stream.Position = 0;
            return stream;
        });
    }

    public string ImageBase64
    {
        get => (string)GetValue(ImageBase64Property);
        set => SetValue(ImageBase64Property, value);
    }

    public string UserId
    {
        get => (string)GetValue(UserIdProperty);
        set => SetValue(UserIdProperty, value);
    }

    public string Message
    {
        get => (string)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public (string UserId,int notificationId) Ids
    {
        get => (UserId, NotifcationId);
    }

    public NotificationType Type { get => NotificationType.FriendRequest; set => Type = NotificationType.FriendRequest; }

    public int NotifcationId { get; set; }

    private static void OnUsernameChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = bindable as FriendRequestNotification;
        view.MessageLabel.Text = (string)newValue;
    }

    public async Task HideAnimation(int distance, uint duration)
    {
        double xTranslation = -distance;

        await this.TranslateTo(xTranslation, 0, duration, Easing.Linear);
    }
}