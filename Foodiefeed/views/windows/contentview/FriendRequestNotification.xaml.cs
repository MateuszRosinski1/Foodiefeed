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