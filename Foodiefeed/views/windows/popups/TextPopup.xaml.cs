using CommunityToolkit.Maui.Views;

namespace Foodiefeed.views.windows.popups;

public partial class TextPopup : Popup
{
	public static readonly BindableProperty TextContentProperty = BindableProperty.Create(nameof(TextContent),typeof(string),typeof(TextPopup),default(string),propertyChanged: OnTextContentChanged);

    public string TextContent 
    { 
        get => (string)GetValue(TextContentProperty);
        set => SetValue(TextContentProperty, value);
    }

    private static void OnTextContentChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (TextPopup)bindable;
        view.ContentLabel.Text = (string)newValue;
    }

    public TextPopup()
	{
		InitializeComponent();
	}
}