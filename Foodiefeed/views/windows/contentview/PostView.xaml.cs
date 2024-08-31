using System.ComponentModel.DataAnnotations;

namespace Foodiefeed.views.windows.contentview;

public partial class PostView : ContentView
{
	[MaxLength(10)]
    public byte[]? Images { get; set; }

	public List<CommentView> Comments { get; set; }

    public PostView()
	{
		Comments = new List<CommentView>();
		Comments.Add(new CommentView());
        Comments.Add(new CommentView());
        Comments.Add(new CommentView());
        Comments.Add(new CommentView());
        Comments.Add(new CommentView());
        Comments.Add(new CommentView());
        Comments.Add(new CommentView());
        Comments.Add(new CommentView());
        Comments.Add(new CommentView());
        Comments.Add(new CommentView());

        InitializeComponent();	

		if(Comments is null) { return; }

		foreach(CommentView comment in Comments)
		{
			CommentSection.Children.Add(comment);
		}
	}

    private async void ExpandCommentSection(object sender, EventArgs e)
    {
       // CommentSectionScroll.MaximumHeightRequest = 600;

        await MainThread.InvokeOnMainThreadAsync(() => {
            var animation = new Animation(v => CommentSectionScroll.MaximumHeightRequest = v, 300, 600);
            animation.Commit(this, "TrackListSideBarAnimation", 16, 500, Easing.CubicIn);
        });

        Button b = (Button)sender;
        b.Clicked -= ExpandCommentSection;
        b.Clicked += CondenseCommentSection;
    }

    private async void CondenseCommentSection(object sender, EventArgs e)
    {
        await MainThread.InvokeOnMainThreadAsync(() => {
            var animation = new Animation(v => CommentSectionScroll.MaximumHeightRequest = v, 600, 300);
            animation.Commit(this, "TrackListSideBarAnimation", 16, 500, Easing.CubicIn);
        });

        Button b = (Button)sender;

        b.Clicked -= CondenseCommentSection;

        b.Clicked += ExpandCommentSection;
    }
}