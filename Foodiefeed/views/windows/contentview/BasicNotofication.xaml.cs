using Foodiefeed.models.dto;

namespace Foodiefeed.views.windows.contentview;

public partial class BasicNotofication : ContentView , INotification
{
    

    public static BindableProperty UserIdProperty =
        BindableProperty.Create(nameof(UserId), typeof(string), typeof(BasicNotofication), default(string));

    //public static BindableProperty ShowPostButtonVisibleProperty =
    //    BindableProperty.Create(nameof(ShowPostButtonVisible), typeof(string), typeof(BasicNotofication), default(string));

    //public bool ShowPostButtonVisible
    //{
    //    get => (bool)GetValue(ShowPostButtonVisibleProperty);
    //    set => SetValue(ShowPostButtonVisibleProperty, value);
    //}

    public string UserId
    {
        get => (string)GetValue(UserIdProperty);
        set => SetValue(UserIdProperty, value);
    }

    public string Message
    {
        get => (string)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    private NotificationType _type;

    public NotificationType Type
    {
        get => _type;
        set => _type = value; 
    }

    public static BindableProperty MessageProperty =
        BindableProperty.Create(nameof(Message),typeof(string),typeof(BasicNotofication),default(string), propertyChanged: OnUsernameChanged);

    private static void OnUsernameChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (BasicNotofication)bindable;
        view.MessageLabel.Text = (string)newValue;
    }

    public BasicNotofication()
	{
		InitializeComponent();
	}

    public async Task HideAnimation(int distance,uint duration)
    {
        double xTranslation = -distance; 

        await this.TranslateTo(xTranslation, 0, duration, Easing.Linear);
    }
}