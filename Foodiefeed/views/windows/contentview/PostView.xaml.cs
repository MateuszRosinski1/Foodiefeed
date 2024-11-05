using System.ComponentModel.DataAnnotations;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;

#if WINDOWS
using Windows.Graphics.Imaging;
using Microsoft.UI.Xaml.Media;
#endif


namespace Foodiefeed.views.windows.contentview;

public partial class PostView : ContentView
{

    #region privates
    bool isTextContentExpanded = false;
    #endregion

    #region BindableProperties

    public static readonly BindableProperty PayloadProperty =
        BindableProperty.Create(nameof(Payload), typeof((string, string)), typeof(PostView), default((string, string)));

    public static readonly BindableProperty NewCommentContentProperty =
        BindableProperty.Create(nameof(NewCommentContent), typeof(string), typeof(PostView), default(string),propertyChanged: OnCommentContentChanged);

    public static readonly BindableProperty PostIdProperty =
        BindableProperty.Create(nameof(PostId), typeof(string), typeof(PostView), default(string));

    public static readonly BindableProperty UsernameProperty =
        BindableProperty.Create(nameof(Username), typeof(string), typeof(PostView), default(string), propertyChanged: OnUsernameTextChanged);

    public static BindableProperty PfpImageBase64Property =
        BindableProperty.Create(nameof(PfpImageBase64), typeof(string), typeof(PostView), default(string), propertyChanged: OnImageChanged);

    public static readonly BindableProperty TimeStampProperty = 
        BindableProperty.Create(nameof(TimeStamp),typeof(string),typeof(PostView),default(string),propertyChanged: OnTimeStampChanged);

    public static readonly BindableProperty PostTextContentProperty =
        BindableProperty.Create(nameof(PostTextContent), typeof(string), typeof(PostView), default(string),propertyChanged: OnPostTextContentChanged);

    public static readonly BindableProperty PostLikeCountProperty =
        BindableProperty.Create(nameof(PostLikeCount), typeof(string), typeof(PostView), default(string), propertyChanged: OnPostLikeCountChanged);

    public static readonly BindableProperty ImageSourceProperty =
        BindableProperty.Create(nameof(ImageSource), typeof(string), typeof(PostView), default(string), propertyChanged: OnImageSourceChanged);

    public static readonly BindableProperty DeleteButtonVisibleProperty =
        BindableProperty.Create(nameof(DeleteButtonVisible), typeof(bool), typeof(CommentView), default(bool));

    public bool DeleteButtonVisible
    {
        get => (bool)GetValue(DeleteButtonVisibleProperty);
        set => SetValue(DeleteButtonVisibleProperty, value);
    }

    #endregion

    int currentImageIndex;

    #region Properties

    public (string,string) Payload
    {
        get => ((string,string))GetValue(PayloadProperty);
        set => SetValue(PayloadProperty, value);
    }

    public string NewCommentContent
    {
        get => (string)GetValue(NewCommentContentProperty);
        set => SetValue(NewCommentContentProperty, value);
    }

    public string PostId
    {
        get => (string)GetValue(PostIdProperty);
        set => SetValue(PostIdProperty, value);
    }

    public string PfpImageBase64
    {
        get => (string)GetValue(PfpImageBase64Property);
        set => SetValue(PfpImageBase64Property, value);
    }

    public static readonly BindableProperty ImagesBase64Property =
    BindableProperty.Create(
        nameof(ImagesBase64),    
        typeof(List<string>),    
        typeof(PostView),        
        new List<string>(),     
        propertyChanged: OnImagesBase64Changed 
    ); 

    public List<string> ImagesBase64
    {
        get => (List<string>)GetValue(ImagesBase64Property);
        set => SetValue(ImagesBase64Property, value);
    }

    public string ImageSource
    {
        get => (string)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

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

    private static void OnCommentContentChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (PostView)bindable;
        view.Payload = (view.PostId, newValue as string);
    }

    private static void OnImagesBase64Changed(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (PostView)bindable;
        var newImagesList = newValue as List<string>;

        if (newImagesList != null)
        {
            Console.WriteLine($"ImagesBase64 has been updated. New count: {newImagesList.Count}");

            if (newImagesList.Count > 0)
            {
                control.ImageSource = newImagesList[0];
            }
        }
    }

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

    private static void OnImageChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (PostView)bindable;

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

    private static void OnImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (PostView)bindable;

        if (newValue is null) return;

        var newValueString = newValue as string;

        var imageBytes = Convert.FromBase64String(newValueString);

        view.postImage.Source = Microsoft.Maui.Controls.ImageSource.FromStream(() =>
        {
            var stream = new MemoryStream(imageBytes);
            stream.Position = 0;
            return stream;
        });
    }

	public List<CommentView> Comments { get; set; }

    public PostView()
    {
        InitializeComponent();
        PostTextContentLabel.SizeChanged += OnPostTextContentLabelSizeChanged;
        currentImageIndex = 0;
        swipeLeftButton.IsVisible = false;
    }

    #region animations

    private async void ExpandCommentSection(object sender, EventArgs e)
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            var animation = new Animation(v => CommentSectionScroll.MaximumHeightRequest = v, 300, 600);
            animation.Commit(this, "1", 16, 500, Easing.CubicIn);
        });

        Button b = (Button)sender;
        b.Clicked -= ExpandCommentSection;
        b.Clicked += CondenseCommentSection;
    }

    private async void CondenseCommentSection(object sender, EventArgs e)
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
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

    private int Clamp(int value, int min, int max)
    {
        return (value < min) ? min : (value > max) ? max : value;
    }

    private void SwipeLeft(object sender, EventArgs e)
    {
        var index = Clamp(currentImageIndex-1, 0, ImagesBase64.Count-1);
        currentImageIndex = index;
        if (currentImageIndex >= 0)
        {
            swipeLeftButton.IsVisible = false;
        }

        if (currentImageIndex < ImagesBase64.Count-1)
        {
            swipeRightButton.IsVisible = true;
        }
        ImageSource = ImagesBase64[currentImageIndex];
    }

    private void SwipeRight(object sender, EventArgs e)
    {
        var index = Clamp(currentImageIndex+1, 0, ImagesBase64.Count-1);
        currentImageIndex = index;
        if(currentImageIndex >= ImagesBase64.Count-1)
        {
            swipeRightButton.IsVisible = false;
        }

        if(currentImageIndex > 0)
        {
            swipeLeftButton.IsVisible = true;
        }
        ImageSource = ImagesBase64[currentImageIndex];
    }

    private async void FontHover(object sender, PointerEventArgs e)
    {
        await AnimateFont(sender, 15, 25, 500, 200);
    }

    private async void FontUnhover(object sender, PointerEventArgs e)
    {
        await AnimateFont(sender, 25, 15, 500, 200);
    }

    private async Task AnimateFont(object sender, int from, int to, uint rate, uint lenght)
    {
        Button button = (Button)sender;
        var animation = new Animation(v => button.FontSize = v, from, to);
        animation.Commit(this, button.Text + "_Font", rate, lenght, Easing.BounceIn);
    }
}