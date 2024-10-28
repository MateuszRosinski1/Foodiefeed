
namespace Foodiefeed.views.windows.contentview;

public partial class PostImageView : ContentView
{
	public static BindableProperty ImageSourceProperty =
		BindableProperty.Create(nameof(ImageSource),typeof(string),typeof(PostImageView),default(string),propertyChanged: OnImageSourceChanged);

    private static void OnImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (PostImageView)bindable;
		view.image.Source = newValue as string;
    }

    public string ImageSource
	{
		get => (string)GetValue(ImageSourceProperty);
		set => SetValue(ImageSourceProperty, value);
	}

	public PostImageView()
	{
		InitializeComponent();
	}
}