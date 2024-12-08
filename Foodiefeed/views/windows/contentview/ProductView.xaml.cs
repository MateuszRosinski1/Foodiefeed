using Foodiefeed.viewmodels;

namespace Foodiefeed.views.windows.contentview;

public partial class ProductView : ContentView
{
    public static readonly BindableProperty IdProperty =
        BindableProperty.Create(nameof(Id), typeof(string), typeof(TagView), default(string));

    public static readonly BindableProperty NameProperty =
        BindableProperty.Create(nameof(Name), typeof(string), typeof(TagView), default(string), propertyChanged: OnProductNameChanged);

    public static readonly BindableProperty FrameBackgroundProperty =
        BindableProperty.Create(nameof(FrameBackground), typeof(Color), typeof(TagView), default(Color), propertyChanged: OnColorChanged);

    public Color FrameBackground
    {
        get => (Color)GetValue(FrameBackgroundProperty);
        set => SetValue(FrameBackgroundProperty, value);
    }

    public string Id
    {
        get => (string)GetValue(IdProperty);
        set => SetValue(IdProperty, value);
    }

    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    public ProductView()
	{
		InitializeComponent();
        this.FrameBackground = (Color)Application.Current.Resources["TagViewFrameBackground"];
    }

    private async void PointerEntered(object sender, PointerEventArgs e)
    {
        var frame = (Frame)sender;
        await frame.ScaleTo(1.2, 250, Easing.Linear);
    }

    private async void PointerExited(object sender, PointerEventArgs e)
    {
        var frame = (Frame)sender;
        await frame.ScaleTo(1, 250, Easing.Linear);
    }

    private static void OnColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (ProductView)bindable;
        view.frame.BackgroundColor = (Color)newValue;
    }

    private static void OnProductNameChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (ProductView)bindable;
        view.ProductNameLabel.Text = (string)newValue;
    }

    private void Tap(object sender, TappedEventArgs e)
    {
        ChangeBackground();
    }

    public void ChangeBackground()
    {
        var element = BoardViewModel.PickedProducts.FirstOrDefault(t => t.Id == Id);
        if (element is not null)
        {
            FrameBackground = Brush.Green.Color;
        }
        else
        {
            FrameBackground = (Color)Application.Current.Resources["TagViewFrameBackground"];
        }
    }
}