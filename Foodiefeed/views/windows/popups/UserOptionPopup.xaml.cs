using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace Foodiefeed.views.windows.popups;

public partial class UserOptionPopup : Popup
{

    public UserOptionPopup()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty AvatarImageSourceProperty =
       BindableProperty.Create(nameof(AvatarImageSource), typeof(string), typeof(UserOptionPopup), default(string), propertyChanged: OnImageSourceChanged);

    public static readonly BindableProperty UserIdProperty =
        BindableProperty.Create(nameof(UserId), typeof(string), typeof(UserOptionPopup), default(string));

    public static readonly BindableProperty UsernameProperty =
        BindableProperty.Create(nameof(Username), typeof(string), typeof(UserOptionPopup), default(string), propertyChanged: OnUsernameChanged);

    public UserOptionPopup(bool isFollowed,bool isFriend,bool HasPendingFriendRequest)
    {
        InitializeComponent();

        if(HasPendingFriendRequest)
        {
            UnfriendBtn.IsVisible = false;
            AddToFriendBtn.IsVisible = false;
            CancelFriendRequestBtn.IsVisible = true;
        }
        else
        {
            if (isFriend)
            {
                UnfriendBtn.IsVisible = true;
                AddToFriendBtn.IsVisible = false;
                CancelFriendRequestBtn.IsVisible = false;
            }
            else
            {
                UnfriendBtn.IsVisible = false;
                AddToFriendBtn.IsVisible = true;
                CancelFriendRequestBtn.IsVisible = false;

            }
        }

        if (isFollowed)
        {
            UnfollowBtn.IsVisible = true;
            FollowBtn.IsVisible = false;
        }
        else
        {
            UnfollowBtn.IsVisible = false;
            FollowBtn.IsVisible = true;
        }

        WeakReferenceMessenger.Default.Register<string, string>(this, "popup", (r, d) =>
        {
            if (d == "close")
            {
                WeakReferenceMessenger.Default.Unregister<string>(this);
                MainThread.BeginInvokeOnMainThread(() => { this.Close(); });
            }
        });

    }

    public string UserId
    {
        get => (string)GetValue(UserIdProperty);
        set => SetValue(UserIdProperty, value);
    }

    public string AvatarImageSource
    {
        get => (string)GetValue(AvatarImageSourceProperty);
        set => SetValue(AvatarImageSourceProperty, value);
    }

    public string Username
    {
        get => (string)GetValue(UsernameProperty);
        set => SetValue(UsernameProperty, value);
    }

    private static void OnUsernameChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (UserOptionPopup)bindable;
        view.UsernameLabel.Text = newValue as string;
    }

    private static void OnImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (UserOptionPopup)bindable;

        if (newValue is null) return;

        var newValueString = newValue as string;

        view.avatarImage.Source = newValueString;
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        this.Close();
    }
}