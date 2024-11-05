
using CommunityToolkit.Mvvm.ComponentModel;
using Foodiefeed.viewmodels;

namespace Foodiefeed.views.windows.contentview;

public partial class TagView : ContentView
{
	public static readonly BindableProperty IdProperty =
		BindableProperty.Create(nameof(Id),typeof(string),typeof(TagView),default(string));

    public static readonly BindableProperty NameProperty =
        BindableProperty.Create(nameof(Name), typeof(string), typeof(TagView), default(string),propertyChanged: OnTagNameChanged);

    public static readonly BindableProperty FrameBackgroundProperty =
        BindableProperty.Create(nameof(FrameBackground), typeof(Color), typeof(TagView), default(Color), propertyChanged: OnColorChanged);

    private static void OnColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (TagView)bindable;
        view.frame.BackgroundColor = (Color)newValue;
    }

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

	private bool isPicked = false;

    public TagView()
	{
		InitializeComponent();
	}

    private static void OnTagNameChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (TagView)bindable;
		view.TagNameLabel.Text = (string)newValue;
    }

    private async void PointerEntered(object sender, PointerEventArgs e)
    {
		var frame = (Frame)sender;
		await frame.ScaleTo(1.2, 250,Easing.Linear);
    }

    private async void PointerExited(object sender, PointerEventArgs e)
    {
        var frame = (Frame)sender;
        await frame.ScaleTo(1, 250, Easing.Linear);
    }

    private void Tap(object sender, TappedEventArgs e)
    {
        ChangeBackground();
    }

    public void ChangeBackground()
    {
        var element = BoardViewModel.PickedTags.FirstOrDefault(t => t.Id == Id);
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