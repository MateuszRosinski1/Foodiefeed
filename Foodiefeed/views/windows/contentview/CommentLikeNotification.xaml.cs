using Foodiefeed.models.dto;

namespace Foodiefeed.views.windows.contentview;

public partial class CommentLikeNotification : ContentView, INotification
{
	public CommentLikeNotification()
	{
		InitializeComponent();
	}

    public static BindableProperty ImageBase64Property =
        BindableProperty.Create(nameof(ImageBase64), typeof(string), typeof(CommentLikeNotification), default(string), propertyChanged: OnImageChanged);

    private static void OnImageChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (CommentLikeNotification)bindable;

        if (newValue is null) return;

        var newValueString = newValue as string;

        if (string.IsNullOrEmpty(newValueString))
        {
            view.image.Source = "avatar.jpg";
            return;
        }

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

    public int NotifcationId { get; set; }

    public async Task HideAnimation(int distance, uint duration)
    {
        double xTranslation = -distance;

        await this.TranslateTo(xTranslation, 0, duration, Easing.Linear);
    }

    public static BindableProperty UserIdProperty =
        BindableProperty.Create(nameof(UserId), typeof(string), typeof(CommentLikeNotification), default(string));

    public static BindableProperty MessageProperty =
        BindableProperty.Create(nameof(Message), typeof(string), typeof(CommentLikeNotification), default(string),propertyChanged: OnMessageChanged);

    private static void OnMessageChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (CommentLikeNotification)bindable;
        view.MessageLabel.Text = newValue as string;
    }

    public static BindableProperty CommentIdProperty =
        BindableProperty.Create(nameof(CommentId), typeof(string), typeof(CommentLikeNotification), default(string));

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

    public string CommentId
    {
        get => (string)GetValue(CommentIdProperty);
        set => SetValue(CommentIdProperty, value);
    }
    public NotificationType Type { 
        get => NotificationType.CommentLike; 
        set => Type = NotificationType.CommentLike; 
    }
}