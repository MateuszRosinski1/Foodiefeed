using CommunityToolkit.Maui.Views;

namespace Foodiefeed.views.windows.popups;

public partial class LikedCommendPopup : Popup
{
	public LikedCommendPopup(string _UserId,string base64,string username,string content)
	{
        UserId = _UserId;
        Username = username;
        ProfilePictureImageBase64 = base64;
        CommentContent = content;
		InitializeComponent();
	}

    public string ProfilePictureImageBase64 { get; set; }
    public string Username { get; set; }
    public string CommentContent { get; set; }
    public string UserId { get; set; }

    private async void AnimateOptionDots(object sender, PointerEventArgs e)
    {
        await FirstCircle.ScaleTo(1.3, 100, Easing.BounceIn);
        await FirstCircle.ScaleTo(1, 100, Easing.BounceOut);
        await SecondCircle.ScaleTo(1.3, 100, Easing.BounceIn);
        await SecondCircle.ScaleTo(1, 100, Easing.BounceOut);
        await ThirdCircle.ScaleTo(1.3, 100, Easing.BounceIn);
        await ThirdCircle.ScaleTo(1, 100, Easing.BounceOut);
    }
}