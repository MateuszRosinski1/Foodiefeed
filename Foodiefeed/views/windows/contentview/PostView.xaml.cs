using CommunityToolkit.Maui.Views;
using Foodiefeed.views.android.popups;
using Foodiefeed.views.windows.popups;
using System.Windows.Input;

namespace Foodiefeed.views.windows.contentview;

public partial class PostView : ContentView
{

    #region privates

    bool isTextContentExpanded = false;
    int currentImageIndex;

    #endregion

    #region BindableProperties

    public static readonly BindableProperty PayloadProperty =
        BindableProperty.Create(nameof(Payload), typeof((string, string)), typeof(PostView), default((string, string)));

    public static readonly BindableProperty NewCommentContentProperty =
        BindableProperty.Create(nameof(NewCommentContent), typeof(string), typeof(PostView), default(string), propertyChanged: OnCommentContentChanged);

    public static readonly BindableProperty PostIdProperty =
        BindableProperty.Create(nameof(PostId), typeof(string), typeof(PostView), default(string));

    public static readonly BindableProperty UsernameProperty =
        BindableProperty.Create(nameof(Username), typeof(string), typeof(PostView), default(string), propertyChanged: OnUsernameTextChanged);

    public static BindableProperty PfpImageBase64Property =
        BindableProperty.Create(nameof(PfpImageBase64), typeof(string), typeof(PostView), default(string), propertyChanged: OnImageChanged);

    public static readonly BindableProperty TimeStampProperty =
        BindableProperty.Create(nameof(TimeStamp), typeof(string), typeof(PostView), default(string), propertyChanged: OnTimeStampChanged);

    public static readonly BindableProperty PostTextContentProperty =
        BindableProperty.Create(nameof(PostTextContent), typeof(string), typeof(PostView), default(string), propertyChanged: OnPostTextContentChanged);

    public static readonly BindableProperty PostLikeCountProperty =
        BindableProperty.Create(nameof(PostLikeCount), typeof(string), typeof(PostView), default(string), propertyChanged: OnPostLikeCountChanged);

    public static readonly BindableProperty ImageSourceProperty =
        BindableProperty.Create(nameof(ImageSource), typeof(string), typeof(PostView), default(string), propertyChanged: OnImageSourceChanged);

    public static readonly BindableProperty DeleteButtonVisibleProperty =
        BindableProperty.Create(nameof(DeleteButtonVisible), typeof(bool), typeof(PostView), default(bool));

    public static readonly BindableProperty PostProductsProperty =
        BindableProperty.Create(nameof(PostProducts), typeof(List<string>), typeof(PostView), default(List<string>), propertyChanged: OnProductsChanged);

    public static readonly BindableProperty PostContentVisibleProperty =
        BindableProperty.Create(nameof(PostContentVisible), typeof(bool), typeof(PostView), default(bool), propertyChanged: OnContentVisiblityChanged);

    public static readonly BindableProperty PostImagesVisibleProperty =
        BindableProperty.Create(nameof(DeleteButtonVisible), typeof(bool), typeof(PostView), default(bool));

    public static readonly BindableProperty LikePostCommandProperty =
        BindableProperty.Create(nameof(LikePostCommand), typeof(ICommand), typeof(PostView), default(ICommand));

    public static readonly BindableProperty UnlikePostCommandProperty =
        BindableProperty.Create(nameof(UnlikePostCommand), typeof(ICommand), typeof(PostView), default(ICommand));

    public static readonly BindableProperty SaveRecipeCommandProperty =
        BindableProperty.Create(nameof(SaveRecipeCommand), typeof(ICommand), typeof(PostView), default(ICommand));

    public static readonly BindableProperty UnsaveRecipeCommandProperty =
        BindableProperty.Create(nameof(UnsaveRecipeCommand), typeof(ICommand), typeof(PostView), default(ICommand));

    public static readonly BindableProperty ImagesBase64Property =
        BindableProperty.Create(nameof(ImagesBase64), typeof(List<string>), typeof(PostView), default(List<string>), propertyChanged: OnImagesBase64Changed);

    public static readonly BindableProperty LikingCommandProperty =
        BindableProperty.Create(nameof(LikingCommand), typeof(ICommand), typeof(PostView), default(ICommand));

    public static readonly BindableProperty SavingCommandProperty =
        BindableProperty.Create(nameof(SavingCommand), typeof(ICommand), typeof(PostView), default(ICommand));

    public static readonly BindableProperty LikeTextProperty =
        BindableProperty.Create(nameof(LikeText), typeof(string), typeof(PostView), default(string));

    public static readonly BindableProperty SavingTextProperty =
        BindableProperty.Create(nameof(SavingText), typeof(string), typeof(PostView), default(string));

    public static readonly BindableProperty SaveIconPathFillProperty =
        BindableProperty.Create(nameof(SaveIconPathFill), typeof(Brush), typeof(PostView), default(Brush));

    public static readonly BindableProperty IsLikedProperty =
        BindableProperty.Create(nameof(IsLiked), typeof(bool), typeof(PostView), default(bool));

    public static readonly BindableProperty IsSavedProperty =
        BindableProperty.Create(nameof(IsSaved), typeof(bool), typeof(PostView), default(bool));

    public static readonly BindableProperty CommentsProperty =
        BindableProperty.Create(nameof(Comments), typeof(List<CommentView>), typeof(PostView), default(List<CommentView>), propertyChanged: OnCommentsChanged);

    private static void OnCommentsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (PostView)bindable;
        view.CommentsVariable = (List<CommentView>)newValue;
    }

    public List<CommentView> Comments
    {
        get => (List<CommentView>)GetValue(CommentsProperty);
        set => SetValue(CommentsProperty, value);
    }

    public List<CommentView> CommentsVariable
    {
        get { return commentsVariable; }
        set { commentsVariable = value; }
    }
    private List<CommentView> commentsVariable;

    #endregion

    #region Properties

    public List<string> PostProducts
    {
        get => (List<string>)GetValue(PostProductsProperty);
        set => SetValue(PostProductsProperty, value);
    }

    public (string, string) Payload
    {
        get => ((string, string))GetValue(PayloadProperty);
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

    public List<string> ImagesBase64
    {
        get => (List<string>)GetValue(ImagesBase64Property);
        set 
        {
            SetValue(ImagesBase64Property, value);
            if (value.Count == 1) swipeRightButton.IsVisible = false;
        }
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

    public bool PostImagesVisible
    {
        get => (bool)GetValue(PostImagesVisibleProperty);
        set => SetValue(PostImagesVisibleProperty, value);
    }

    public bool DeleteButtonVisible
    {
        get => (bool)GetValue(DeleteButtonVisibleProperty);
        set => SetValue(DeleteButtonVisibleProperty, value);
    }

    public bool PostContentVisible
    {
        get => (bool)GetValue(PostContentVisibleProperty);
        set => SetValue(PostContentVisibleProperty, value);
    }

    public ICommand LikePostCommand
    {
        get => (ICommand)GetValue(LikePostCommandProperty);
        set => SetValue(LikePostCommandProperty, value);
    }

    public ICommand UnlikePostCommand
    {
        get => (ICommand)GetValue(UnlikePostCommandProperty);
        set => SetValue(UnlikePostCommandProperty, value);
    }

    public ICommand SaveRecipeCommand
    {
        get => (ICommand)GetValue(SaveRecipeCommandProperty);
        set => SetValue(SaveRecipeCommandProperty, value);
    }

    public ICommand UnsaveRecipeCommand
    {
        get => (ICommand)GetValue(UnsaveRecipeCommandProperty);
        set => SetValue(UnsaveRecipeCommandProperty, value);
    }
    //
    public ICommand LikingCommand
    {
        get => (ICommand)GetValue(LikingCommandProperty);
        set => SetValue(LikingCommandProperty, value);

    }

    public ICommand SavingCommand
    {
        get => (ICommand)GetValue(SavingCommandProperty);
        set => SetValue(SavingCommandProperty, value);
    }

    public string LikeText
    {
        get => (string)GetValue(LikeTextProperty);
        set => SetValue(LikeTextProperty, value);
    }
    public string SavingText
    {
        get => (string)GetValue(SavingTextProperty);
        set => SetValue(SavingTextProperty, value);
    }

    public Brush SaveIconPathFill
    {
        get => (Brush)GetValue(SaveIconPathFillProperty);
        set => SetValue(SaveIconPathFillProperty, value);
    }

    public bool IsLiked {
        get
        {
            return (bool)GetValue(IsLikedProperty);
        }
        set
        {
            SetValue(IsLikedProperty, value);
            if (value is false)
            {
                LikeText = "Like It!";
                LikingCommand = LikePostCommand;
            }
            else if (value is true)
            {
                LikeText = "Unlike it ;(";
                LikingCommand = UnlikePostCommand;
            }
        }
    }


    public bool IsSaved {
        get
        {
            return (bool)GetValue(IsSavedProperty);
        }
        set
        {
            SetValue(IsSavedProperty, value);
            if (value is false)
            {
                SavingText = "Save";
                SavingCommand = SaveRecipeCommand;
                SaveIconPathFill = Brush.Gray;
            }
            else if (value is true)
            {
                SavingText = "Unsave";
                SavingCommand = UnsaveRecipeCommand;
                SaveIconPathFill = Brush.Yellow;
            }
        }
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
        view.PostTimeStampLabel.Text = newValue as string;
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

        if (string.IsNullOrEmpty(newValueString))
        {
            view.pfpImage.Source = "avatar.jpg";
            return;
        }

        view.pfpImage.Source = newValueString;
    }

    private static void OnImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (PostView)bindable;

        if (newValue is null) return;

        var newValueString = newValue as string;

        view.postImage.Source = newValueString;;
    }

    private static void OnContentVisiblityChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (PostView)bindable;
        view.PostContentGrid.IsVisible = (bool)newValue;
        view.PostProductsGrid.IsVisible = !(bool)newValue;
    }

    private static void OnProductsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (PostView)bindable;

        if (newValue is null) return;

        List<string> Products = newValue as List<string>;

        string productString = view.ProductsToString(products: Products);

        view.PostProductsContentLabel.Text = productString;
    }

    //public List<CommentView> Comments { get; set; }

    public PostView()
    {
        InitializeComponent();
        PostTextContentLabel.SizeChanged += OnPostTextContentLabelSizeChanged;
        currentImageIndex = 0;
        swipeLeftButton.IsVisible = false;
        PostContentVisible = true;

        
#if ANDROID
ExpandOrCollapseLabel.IsVisible = false;
#endif
#if WINDOWS
        ShowMoreLabel.IsVisible = false;
        PointerGestureRecognizer pgr = new PointerGestureRecognizer();
        pgr.PointerEntered += ScaleButton;
        pgr.PointerExited += UnscaleButton;
        CommentButton.GestureRecognizers.Add(pgr);
#endif
    }

    #region animations

    private async void ExpandCommentSection(object sender, EventArgs e)
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            var animation = new Animation(v =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    CommentSectionScroll.HeightRequest = v;
                });
            }, 300, 600);
            animation.Commit(this, "1", 16, 500, Easing.CubicIn);
        });

        Button b = (Button)sender;
        b.Clicked -= ExpandCommentSection;
        b.Clicked += CondenseCommentSection;
        CommentSectionScroll.MaximumHeightRequest = 600;
    }

    private async void CondenseCommentSection(object sender, EventArgs e)
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            var animation = new Animation(v =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    CommentSectionScroll.HeightRequest = v;
                });
            }, 600, 300);
            animation.Commit(this, "2", 16, 500, Easing.CubicIn);
        });

        Button b = (Button)sender;

        b.Clicked -= CondenseCommentSection;

        b.Clicked += ExpandCommentSection;
        CommentSectionScroll.HeightRequest = 300;
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
#if WINDOWS
        const int HEIGHT_LIMIT_VALUE = 400;
#endif
#if ANDROID
        const int HEIGHT_LIMIT_VALUE = 150;
#endif
        if (PostTextContentLabel.Height > 0 && PostTextContentLabel.Height <= HEIGHT_LIMIT_VALUE)
        {
            ExpandOrCollapseLabel.IsVisible = false;
            ShowMoreLabel.IsVisible = false;
        }
    }

    private int Clamp(int value, int min, int max)
    {
        return (value < min) ? min : (value > max) ? max : value;
    }

    private void SwipeLeft(object sender, EventArgs e)
    {
        var index = Clamp(currentImageIndex - 1, 0, ImagesBase64.Count - 1);
        currentImageIndex = index;
        if (currentImageIndex <= 0)
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


    private async void UnscaleButton(object sender, PointerEventArgs e)
    {
        var btn = (Button)sender;
        await btn.ScaleTo(1, 250, Easing.Linear);
    }

    private async void ScaleButton(object sender, PointerEventArgs e)
    {
        var btn = (Button)sender;
        await btn.ScaleTo(1.2, 250, Easing.Linear);
    }

    private async void UnscaleHSL(object sender, PointerEventArgs e)
    {
        var hsl = (HorizontalStackLayout)sender;
        await hsl.ScaleTo(1, 250, Easing.Linear);
    }

    private async void ScaleHSL(object sender, PointerEventArgs e)
    {
        var hsl = (HorizontalStackLayout)sender;
        await hsl.ScaleTo(1.2, 250, Easing.Linear);
    }

    private void ShowProducts(object sender, EventArgs e)
    {
        PostContentVisible = !PostContentVisible;
    }

    private async void ScaleButtonn(object sender, PointerEventArgs e)
    {
        var hsl = (Button)sender;
        await hsl.ScaleTo(1.2, 250, Easing.Linear);
    }

    private void FocusEditor(object sender, EventArgs e)
    {
#if WINDOWS
        CommentEditor.Focus();
#endif
#if ANDROID
        var popup = new CommentSectionPopup();
        popup.Comments = this.Comments;
        popup.PostId = PostId;
        App.Current.MainPage.ShowPopup(popup);
#endif
    }

    private void ShowMoreContentClicked(object sender, TappedEventArgs e)
    {
        var popup = new TextPopup();
        if (PostContentVisible)
        {
            popup.TextContent = PostTextContent;
        }
        else
        {
            popup.TextContent = ProductsToString(PostProducts);
        }
        Application.Current.MainPage.ShowPopup(popup);
    }

    private string ProductsToString(List<string> products)
    {
        if (products is null ) return string.Empty;

        string str = string.Empty;
        foreach (var product in products)
        {
            str += product + '\n';
        }
        return str;
    }
}