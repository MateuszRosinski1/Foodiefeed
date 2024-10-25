using CommunityToolkit.Maui.Views;

namespace Foodiefeed.views.windows.popups;

public partial class LikedPostPopup : Popup
{
    #region privates
    bool isTextContentExpanded = false;
    #endregion

    #region BindableProperties

    public static readonly BindableProperty UsernameProperty =
        BindableProperty.Create(nameof(Username), typeof(string), typeof(LikedPostPopup), default(string), propertyChanged: OnUsernameTextChanged);

    public static readonly BindableProperty TimeStampProperty =
        BindableProperty.Create(nameof(TimeStamp), typeof(string), typeof(LikedPostPopup), default(string), propertyChanged: OnTimeStampChanged);

    public static readonly BindableProperty PostTextContentProperty =
        BindableProperty.Create(nameof(PostTextContent), typeof(string), typeof(LikedPostPopup), default(string), propertyChanged: OnPostTextContentChanged);

    public static readonly BindableProperty PostLikeCountProperty =
        BindableProperty.Create(nameof(PostLikeCount), typeof(string), typeof(LikedPostPopup), default(string), propertyChanged: OnPostLikeCountChanged);

    public static readonly BindableProperty ImageSourceProperty =
        BindableProperty.Create(nameof(ImageSource), typeof(string), typeof(LikedPostPopup), default(string), propertyChanged: OnImageSourceChanged);

    #endregion

    int currentImageIndex;

    #region Properties

    public static readonly BindableProperty ImagesBase64Property =
    BindableProperty.Create(
        nameof(ImagesBase64),
        typeof(List<string>),
        typeof(LikedPostPopup),
        new List<string>(),
        propertyChanged: OnImagesBase64Changed
    );

    public void SetImagesVisiblity(bool visiblity)
    {
        this.ImagesGrid.IsVisible = visiblity;
    }

    private static void OnImagesBase64Changed(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CommentedPostPopup)bindable;
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

    private static void OnPostTextContentChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (LikedPostPopup)bindable;
        view.PostTextContentLabel.Text = newValue as string;
    }

    private static void OnTimeStampChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (LikedPostPopup)bindable;
        view.PostTimeStampLabel.Text = newValue as string + " ago.";
    }

    private static void OnUsernameTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (LikedPostPopup)bindable;
        view.UsernameLabel.Text = newValue as string;
    }

    private static void OnPostLikeCountChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (LikedPostPopup)bindable;
        view.PostLikeCountLabel.Text = newValue as string;
    }

    private static void OnImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (LikedPostPopup)bindable;

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


    public LikedPostPopup()
    {
        InitializeComponent();
        currentImageIndex = 0;
        swipeLeftButton.IsVisible = false;
    }

    private int Clamp(int value, int min, int max)
    {
        return (value < min) ? min : (value > max) ? max : value;
    }

    private void SwipeLeft(object sender, EventArgs e)
    {
        var index = Clamp(currentImageIndex - 1, 0, ImagesBase64.Count - 1);
        currentImageIndex = index;
        if (currentImageIndex >= 0)
        {
            swipeLeftButton.IsVisible = false;
        }

        if (currentImageIndex < ImagesBase64.Count - 1)
        {
            swipeRightButton.IsVisible = true;
        }
        ImageSource = ImagesBase64[currentImageIndex];
    }

    private void SwipeRight(object sender, EventArgs e)
    {
        var index = Clamp(currentImageIndex + 1, 0, ImagesBase64.Count - 1);
        currentImageIndex = index;
        if (currentImageIndex >= ImagesBase64.Count - 1)
        {
            swipeRightButton.IsVisible = false;
        }

        if (currentImageIndex > 0)
        {
            swipeLeftButton.IsVisible = true;
        }
        ImageSource = ImagesBase64[currentImageIndex];
    }
}