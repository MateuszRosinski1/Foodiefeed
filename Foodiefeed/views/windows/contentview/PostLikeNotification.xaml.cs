using Foodiefeed.models.dto;

namespace Foodiefeed.views.windows.contentview;

public partial class PostLikeNotification : ContentView, INotification
{
	public PostLikeNotification()
	{
		InitializeComponent();
	}

    public int NotifcationId { get; set; }

    public async Task HideAnimation(int distance, uint duration)
    {
        double xTranslation = -distance;

        await this.TranslateTo(xTranslation, 0, duration, Easing.Linear);
    }

    public NotificationType Type { get => NotificationType.PostLike; set => Type = NotificationType.PostLike; }


    public static BindableProperty UserIdProperty =
        BindableProperty.Create(nameof(UserId), typeof(string), typeof(PostLikeNotification), default(string));

    public static BindableProperty MessageProperty =
        BindableProperty.Create(nameof(Message), typeof(string), typeof(PostLikeNotification), default(string), propertyChanged: OnMessageChanged);

    public static BindableProperty PostIdProperty =
        BindableProperty.Create(nameof(PostId), typeof(string), typeof(PostLikeNotification), default(string));


    public string PostId
    {
        get => (string)GetValue(PostIdProperty);
        set => SetValue(PostIdProperty, value);
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

    

    private static void OnMessageChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (PostLikeNotification)bindable;
        view.MessageLabel.Text = newValue as string;
    }
}