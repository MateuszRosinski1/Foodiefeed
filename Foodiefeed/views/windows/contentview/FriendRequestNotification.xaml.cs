namespace Foodiefeed.views.windows.contentview;

public partial class FriendRequestNotification : ContentView, INotification
{
	public FriendRequestNotification()
	{
		InitializeComponent();
	}

    public async Task HideAnimation(int distance, uint duration)
    {
        double xTranslation = -distance;

        await this.TranslateTo(xTranslation, 0, duration, Easing.Linear);
    }
}