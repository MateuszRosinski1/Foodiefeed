
using Foodiefeed.models.dto;
using System.ComponentModel.Design;

namespace Foodiefeed.views.windows.contentview;

public partial class PostCommentNotification : ContentView, INotification
{
	public PostCommentNotification()
	{
		InitializeComponent();
	}

    public static BindableProperty UserIdProperty =
        BindableProperty.Create(nameof(UserId), typeof(string), typeof(PostCommentNotification), default(string));

    public static BindableProperty MessageProperty =
        BindableProperty.Create(nameof(Message), typeof(string), typeof(PostCommentNotification), default(string), propertyChanged: OnMessageChanged);

    public static BindableProperty CommentIdProperty =
        BindableProperty.Create(nameof(CommentId), typeof(string), typeof(PostCommentNotification), default(string));

    public static BindableProperty PostIdProperty =
        BindableProperty.Create(nameof(PostId), typeof(string), typeof(PostCommentNotification), default(string));

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