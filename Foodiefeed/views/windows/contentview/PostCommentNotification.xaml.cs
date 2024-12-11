
using Foodiefeed.models.dto;
using System.ComponentModel.Design;

namespace Foodiefeed.views.windows.contentview;

public partial class PostCommentNotification : ContentView, INotification
{
	public PostCommentNotification()
	{
		InitializeComponent();
	}

    public int NotifcationId { get; set; }

    public static BindableProperty UserIdProperty =
        BindableProperty.Create(nameof(UserId), typeof(string), typeof(PostCommentNotification), default(string));

    public static BindableProperty MessageProperty =
        BindableProperty.Create(nameof(Message), typeof(string), typeof(PostCommentNotification), default(string), propertyChanged: OnMessageChanged);

    public static BindableProperty CommentIdProperty =
        BindableProperty.Create(nameof(CommentId), typeof(string), typeof(PostCommentNotification), default(string));

    public static BindableProperty PostIdProperty =
        BindableProperty.Create(nameof(PostId), typeof(string), typeof(PostCommentNotification), default(string));

    public static BindableProperty ImageBase64Property =
        BindableProperty.Create(nameof(ImageBase64), typeof(string), typeof(PostCommentNotification), default(string), propertyChanged: OnImageChanged);

    private static void OnImageChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (PostCommentNotification)bindable;

        if (newValue is null) return;

        var newValueString = newValue as string;

        view.image.Source = newValueString;

        //if (string.IsNullOrEmpty(newValueString))
        //{
        //    view.image.Source = "avatar.jpg";
        //    return;
        //}

        //var imageBytes = Convert.FromBase64String(newValueString);

        //view.image.Source = Microsoft.Maui.Controls.ImageSource.FromStream(() =>
        //{
        //    var stream = new MemoryStream(imageBytes);
        //    stream.Position = 0;
        //    return stream;
        //});
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

    public (string post, string comment) Ids
    {
        get { return (PostId,CommentId); }
    }

    public string Message
    {
        get => (string)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public string PostId
    {
        get => (string)GetValue(PostIdProperty);
        set => SetValue(PostIdProperty, value);
    }

    public string CommentId
    {
        get => (string)GetValue(CommentIdProperty);
        set => SetValue(CommentIdProperty, value);
    }
    public NotificationType Type { get => NotificationType.PostComment; set => Type = NotificationType.PostComment; }

    public async Task HideAnimation(int distance, uint duration)
    {
        double xTranslation = -distance;

        await this.TranslateTo(xTranslation, 0, duration, Easing.Linear);
    }

    private static void OnMessageChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (PostCommentNotification)bindable;
        view.MessageLabel.Text = newValue as string;
    }
}