using CommunityToolkit.Maui.Views;

namespace Foodiefeed.views.windows.popups;

public partial class EditCommentPopup :  Popup
{
    public string CommentId { get; set; }

    public EditCommentPopup(string commentId)
	{
		this.CommentId = commentId;
		InitializeComponent();
	}
}