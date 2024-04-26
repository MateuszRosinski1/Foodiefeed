using Microsoft.Maui.Handlers;

namespace Foodiefeed.Platforms.Windows
{
    public class MyEntryHandler : EntryHandler
    {
        protected override void ConnectHandler(Microsoft.UI.Xaml.Controls.TextBox nativeView)
        {
            nativeView.Background = null;
            base.ConnectHandler(nativeView);
        }
    }
}
