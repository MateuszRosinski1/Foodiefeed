using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;

namespace Foodiefeed.views.windows.popups;


public partial class FailedActionNotificationPopup : ContentView
{
	public FailedActionNotificationPopup()
	{
		InitializeComponent();
        WeakReferenceMessenger.Default.Register<FailedActionAnimationMessage>(this, (r, message) =>
        {
            if (message.Value == "show")
            {
                Show();
            }
            else if (message.Value == "hide")
            {
                Hide();
            }
        });
    }

    private async void Show()
    {
        await this.TranslateTo(0,250,250,Easing.Linear);
    }

    private async void Hide()
    {
        await this.TranslateTo(0,-0, 250, Easing.Linear);
    }
}