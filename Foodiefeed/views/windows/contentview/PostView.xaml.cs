using System.ComponentModel.DataAnnotations;

namespace Foodiefeed.views.windows.contentview;

public partial class PostView : ContentView
{

    #region privates
    bool isTextContentExpanded = false;
    #endregion

    #region BindableProperties

    public static readonly BindableProperty UsernameProperty =
        BindableProperty.Create(nameof(Username), typeof(string), typeof(PostView), default(string), propertyChanged: OnUsernameTextChanged);

    public static readonly BindableProperty TimeStampProperty = 
        BindableProperty.Create(nameof(TimeStamp),typeof(string),typeof(PostView),default(string),propertyChanged: OnTimeStampChanged);

    public static readonly BindableProperty PostTextContentProperty =
        BindableProperty.Create(nameof(PostTextContent), typeof(string), typeof(PostView), default(string),propertyChanged: OnPostTextContentChanged);

    public static readonly BindableProperty PostLikeCountProperty =
        BindableProperty.Create(nameof(PostLikeCount), typeof(string), typeof(PostView), default(string), propertyChanged: OnPostLikeCountChanged);

    #endregion

    #region Properties

    public string PostLikeCount
    {
        get => (string)GetValue(PostLikeCountProperty);
        set => SetValue(PostLikeCountProperty, value);
    }

    public string PostTextContent
    {
        get => (string)GetValue(PostTextContentProperty);
        set => SetValue(PostTextContentProperty, value);
    }

    public string Username
    {
        get => (string)GetValue(UsernameProperty);
        set => SetValue(UsernameProperty, value);
    }

    public string TimeStamp
    {
        get => (string)(GetValue(TimeStampProperty));
        set => SetValue(TimeStampProperty, value);
    }

    #endregion

    private static void OnPostTextContentChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (PostView)bindable;
        view.PostTextContentLabel.Text = newValue as string;
    }

    private static void OnTimeStampChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (PostView)bindable;
        view.PostTimeStampLabel.Text = newValue as string + " ago.";
    }

    private static void OnUsernameTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (PostView)bindable;
        view.UsernameLabel.Text = newValue as string;
    }

    private static void OnPostLikeCountChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (PostView)bindable;
        view.PostLikeCountLabel.Text = newValue as string;
    }

    [MaxLength(10)]
    public byte[]? Images { get; set; }

	public List<CommentView> Comments { get; set; }

    public PostView()
	{
        InitializeComponent();
        PostTextContentLabel.SizeChanged += OnPostTextContentLabelSizeChanged;     
	}

    #region animations

    private async void ExpandCommentSection(object sender, EventArgs e)
    {

        await MainThread.InvokeOnMainThreadAsync(() => {
            var animation = new Animation(v => CommentSectionScroll.MaximumHeightRequest = v, 300, 600);
            animation.Commit(this, "1", 16, 500, Easing.CubicIn);
        });

        Button b = (Button)sender;
        b.Clicked -= ExpandCommentSection;
        b.Clicked += CondenseCommentSection;
    }

    private async void CondenseCommentSection(object sender, EventArgs e)
    {
        await MainThread.InvokeOnMainThreadAsync(() => {
            var animation = new Animation(v => CommentSectionScroll.MaximumHeightRequest = v, 600, 300);
            animation.Commit(this, "TrackListSideBarAnimation", 16, 500, Easing.CubicIn);
        });

        Button b = (Button)sender;

        b.Clicked -= CondenseCommentSection;

        b.Clicked += ExpandCommentSection;
    }

    private void ExpandPostContentText(object sender, TappedEventArgs e)
    {
        if (isTextContentExpanded)
        {
            PostTextContentScroll.MaximumHeightRequest = 150;
            isTextContentExpanded = false;
            ExpandOrCollapseLabel.Text = "Collapse...";
            ExpandOrCollapseLabel.Text = "Expand...";
        }
        else
        {
            PostTextContentScroll.MaximumHeightRequest = 400;
            isTextContentExpanded = true;
            ExpandOrCollapseLabel.Text = "Collapse...";
        }
    }

    private void UnderlineText(object sender, PointerEventArgs e)
    {
        var label = sender as Label;
        label.TextDecorations = TextDecorations.Underline;

    }

    private void UnUnderLineText(object sender, PointerEventArgs e)
    {
        var label = sender as Label;
        label.TextDecorations = TextDecorations.None;
    }

    #endregion

    private void OnPostTextContentLabelSizeChanged(object sender, EventArgs e)
    {
        if (PostTextContentLabel.Height > 0 && PostTextContentLabel.Height <= 400)
        {
            ExpandOrCollapseLabel.IsVisible = false;
        }
    }
}