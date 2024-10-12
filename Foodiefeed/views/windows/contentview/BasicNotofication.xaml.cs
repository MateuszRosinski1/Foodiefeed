namespace Foodiefeed.views.windows.contentview;

public partial class BasicNotofication : ContentView , INotification
{
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