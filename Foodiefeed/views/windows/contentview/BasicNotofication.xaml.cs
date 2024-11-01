using Foodiefeed.models.dto;

namespace Foodiefeed.views.windows.contentview;

public partial class BasicNotofication : ContentView , INotification
{
    

    public static BindableProperty UserIdProperty =
        BindableProperty.Create(nameof(UserId), typeof(string), typeof(BasicNotofication), default(string));

    public static BindableProperty ImageBase64Property =
        BindableProperty.Create(nameof(ImageBase64), typeof(string), typeof(BasicNotofication), default(string),propertyChanged: OnImageChanged);

    private static void OnImageChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (BasicNotofication)bindable;

        if (newValue is null) return;

        var newValueString = newValue as string;

        var imageBytes = Convert.FromBase64String(newValueString);

        view.image.Source = Microsoft.Maui.Controls.ImageSource.FromStream(() =>
        {
            var stream = new MemoryStream(imageBytes);
            stream.Position = 0;
            return stream;
        });
    }

    public string ImageBase64
    {
        get => (string)GetValue(ImageBase64Property);
        set => SetValue(ImageBase64Property, value);
    }

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

    public int NotifcationId { get; set; }

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