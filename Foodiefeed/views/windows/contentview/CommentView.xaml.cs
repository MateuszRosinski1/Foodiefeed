


namespace Foodiefeed.views.windows.contentview;

public partial class CommentView : ContentView
{
    public string CommentId { get; set; }
	public string UserId { get; set; }

    public static readonly BindableProperty UsernameProperty =
		BindableProperty.Create(nameof(Username), typeof(string), typeof(CommentView), default(string), propertyChanged: OnUsernameTextChanged);

	public static readonly BindableProperty CommentContentProperty =
		BindableProperty.Create(nameof(CommentContent), typeof(string), typeof(CommentView), default(string), propertyChanged: OnCommentContentTextChanged);

	public static readonly BindableProperty LikeCountProperty =
		BindableProperty.Create(nameof(LikeCount), typeof(string), typeof(CommentView), default(string), propertyChanged: LikeCountTextChanged);

    public static readonly BindableProperty PfpImageBase64Property =
        BindableProperty.Create(nameof(PfpImageBase64), typeof(string), typeof(CommentView), default(string), propertyChanged: OnImageChanged);

    public static readonly BindableProperty EditButtonVisibleProperty =
        BindableProperty.Create(nameof(EditButtonVisible), typeof(bool), typeof(CommentView), default(bool));

    public bool EditButtonVisible
    {
        get =>  (bool)GetValue(EditButtonVisibleProperty);
        set => SetValue(EditButtonVisibleProperty, value);
    }

    public string CommentContent
	{
		get => (string)GetValue(CommentContentProperty);
		set => SetValue(CommentContentProperty, value);
	}

    public string Username
	{
		get => (string)GetValue(UsernameProperty);
		set => SetValue(UsernameProperty, value);
	}

	public string LikeCount
	{
		get => (string)GetValue(LikeCountProperty);
		set => SetValue(LikeCountProperty, value);
	}

    public string PfpImageBase64
    {
        get => (string)GetValue(PfpImageBase64Property);
        set => SetValue(PfpImageBase64Property, value);
    }


    public CommentView()
	{
		InitializeComponent();
	}

    private static void OnImageChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (CommentView)bindable;

        if (newValue is null) return;

        var newValueString = newValue as string;

        var imageBytes = Convert.FromBase64String(newValueString);

        view.pfpImage.Source = Microsoft.Maui.Controls.ImageSource.FromStream(() =>
        {
            var stream = new MemoryStream(imageBytes);
            stream.Position = 0;
            return stream;
        });
    }

    private static void OnUsernameTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
		var view = (CommentView)bindable;
		view.UsernameLabel.Text = newValue as string;
    }

    private static void OnCommentContentTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (CommentView)bindable;
        view.CommentText.Text = newValue as string;
    }

    private static void LikeCountTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
		var view = (CommentView)bindable;
		view.LikeCountLabel.Text = newValue as string;
    }

    private async void AnimateOptionDots(object sender, PointerEventArgs e)
    {
        await FirstCircle.ScaleTo(1.3, 100, Easing.BounceIn);
        await FirstCircle.ScaleTo(1, 100, Easing.BounceOut);
        await SecondCircle.ScaleTo(1.3, 100, Easing.BounceIn);
        await SecondCircle.ScaleTo(1, 100, Easing.BounceOut);
        await ThirdCircle.ScaleTo(1.3, 100, Easing.BounceIn);
        await ThirdCircle.ScaleTo(1, 100, Easing.BounceOut);
    }
}