using CommunityToolkit.Maui.Views;
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

    //public static readonly BindableProperty IsFollowedProperty = 
    //    BindableProperty.Create(nameof(IsFollowed),typeof(bool),typeof(UserOptionPopup),default(bool));

    //public static readonly BindableProperty IsFriendProperty =
    //    BindableProperty.Create(nameof(IsFriend), typeof(bool), typeof(UserOptionPopup), default(bool));


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
    }
    //private static void OnFriendChanged(BindableObject bindable, object oldValue, object newValue)
    //{
    //    var view = (UserOptionPopup)bindable;
    //    if (view.IsFriend == true)
    //    {
    //        view.UnfriendBtn.IsVisible = true;
    //        view.AddToFriendBtn.IsVisible = false;
    //    }
    //    else
    //    {
    //        view.UnfriendBtn.IsVisible = false;
    //        view.AddToFriendBtn.IsVisible = true;
    //    }
    //}

    //private static void OnFollowChanged(BindableObject bindable, object oldValue, object newValue)
    //{
    //    var view = (UserOptionPopup)bindable;
    //    if (view.IsFollowed == true)
    //    {
    //        view.UnfollowBtn.IsVisible = true;
    //        view.FollowBtn.IsVisible = false;
    //    }
    //    else
    //    {
    //        view.UnfollowBtn.IsVisible = false;
    //        view.FollowBtn.IsVisible = true;
    //    }
    //}

    //public bool IsFriend
    //{
    //    get => (bool)GetValue(IsFriendProperty);
    //    set => SetValue(IsFriendProperty, value);
    //}

    //public bool IsFollowed
    //{
    //    get => (bool)GetValue(IsFollowedProperty);
    //    set => SetValue(IsFollowedProperty, value);
    //}

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

        var imageBytes = Convert.FromBase64String(newValueString);

        view.avatarImage.Source = Microsoft.Maui.Controls.ImageSource.FromStream(() =>
        {
            var stream = new MemoryStream(imageBytes);
            stream.Position = 0;
            return stream;
        });
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        this.Close();
    }
}