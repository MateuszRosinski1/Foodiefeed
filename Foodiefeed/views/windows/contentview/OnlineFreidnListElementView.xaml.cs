namespace Foodiefeed.views.windows.contentview;

public partial class OnlineFreidnListElementView : ContentView
{
	public static readonly BindableProperty UsernameProperty = 
		BindableProperty.Create(nameof(Username),typeof(string),typeof(OnlineFreidnListElementView),default(string),propertyChanged: OnUsernameChanged);

    public string Username
	{
		get => (string)GetValue(UsernameProperty);
		set => SetValue(UsernameProperty, value);
	}

	public OnlineFreidnListElementView()
	{
		InitializeComponent();
	}

    private static void OnUsernameChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (OnlineFreidnListElementView)bindable;
		view.UsernameLabel.Text = newValue as string;
    }
}