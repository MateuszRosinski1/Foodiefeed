using CommunityToolkit.Maui.Views;
using Foodiefeed.views.windows.contentview;
using System.ComponentModel;

namespace Foodiefeed.views.android.popups;

public partial class CommentSectionPopup : Popup
{

	public static readonly BindableProperty CommentsProperty =
		BindableProperty.Create(nameof(Comments), typeof(List<CommentView>), typeof(CommentSectionPopup), default(List<CommentView>));

    public List<CommentView> Comments 
	{ 
		get => (List<CommentView>)GetValue(CommentsProperty);
		set => SetValue(CommentsProperty, value);
	}

    public static readonly BindableProperty NewCommentContentProperty =
    BindableProperty.Create(
        nameof(NewCommentContent),
        typeof(string),
        typeof(CommentSectionPopup),
        default(string),
        propertyChanged: OnContentChanged);

    private static void OnContentChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (CommentSectionPopup)bindable;
        view.Content = view.CommentEditor.Text;
        view.OnPropertyChanged("Payload");
    }

    public string NewCommentContent
    {
        get => (string)GetValue(NewCommentContentProperty);
        set => SetValue(NewCommentContentProperty, value);
    }

    public string PostId { get; set; }
    public string Content { get; set; }

    public (string,string) Payload => (PostId,Content);

    public CommentSectionPopup()
	{
		InitializeComponent();
	}
}