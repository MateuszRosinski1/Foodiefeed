using CommunityToolkit.Maui.Views;

namespace Foodiefeed.views.windows.popups;

public partial class UserOptionPopup : Popup
{

    public UserOptionPopup()
    {
        InitializeComponent();
    }

    //public static readonly BindableProperty AvatarImageSourceProperty =
    //   BindableProperty.Create(nameof(AvatarImageSource), typeof(string), typeof(UserOptionPopup), default(string), propertyChanged: OnImageSourceChanged);

    //public static readonly BindableProperty UserIdProperty =
    //    BindableProperty.Create(nameof(UserId), typeof(string), typeof(UserOptionPopup), default(string));

    //public static readonly BindableProperty UsernameProperty =
    //    BindableProperty.Create(nameof(Username), typeof(string), typeof(UserOptionPopup), default(string), propertyChanged: OnUsernameChanged);

    //public string UserId
    //{
    //    get => (string)GetValue(UserIdProperty);
    //    set => SetValue(UserIdProperty, value);
    //}

    //public string AvatarImageSource
    //{
    //    get => (string)GetValue(AvatarImageSourceProperty);
    //    set => SetValue(AvatarImageSourceProperty, value);
    //}

    //public string Username
    //{
    //    get => (string)GetValue(UsernameProperty);
    //    set => SetValue(UsernameProperty, value);
    //}

    //private static void OnUsernameChanged(BindableObject bindable, object oldValue, object newValue)
    //{
    //    var view = (UserOptionPopup)bindable;
    //    view.UsernameLabel.Text = newValue as string;
    //}

    //private static void OnImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
    //{
    //    var view = (UserOptionPopup)bindable;

    //    if (newValue is null) return;

    //    var newValueString = newValue as string;

    //    var imageBytes = Convert.FromBase64String(newValueString);

    //    view.avatarImage.Source = Microsoft.Maui.Controls.ImageSource.FromStream(() =>
    //    {
    //        var stream = new MemoryStream(imageBytes);
    //        stream.Position = 0;
    //        return stream;
    //    });
    //}
}