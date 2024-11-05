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

    public static BindableProperty ImageBase64Property =
        BindableProperty.Create(nameof(ImageBase64), typeof(string), typeof(PostLikeNotification), default(string), propertyChanged: OnImageChanged);

    private static void OnImageChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (PostLikeNotification)bindable;

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