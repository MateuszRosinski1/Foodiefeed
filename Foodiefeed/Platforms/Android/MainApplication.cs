using Android.App;
using Android.Content.Res;
using Android.Runtime;

namespace Foodiefeed
{
    [Application(UsesCleartextTraffic = true)]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {

            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
            {
                if (view is Entry)
                {
                    // Remove underline
                    handler.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
                }
            });

            Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping(nameof(Editor), (handler, view) =>
            {
                if (view is Editor)
                {
                    handler.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
                }
            });

            Microsoft.Maui.Handlers.ScrollViewHandler.Mapper.AppendToMapping(nameof(ScrollView), (handler, view) =>
            {
                handler.PlatformView.NestedScrollingEnabled = true;
            });


            Microsoft.Maui.Handlers.ContentViewHandler.Mapper.AppendToMapping(nameof(CollectionView), (handler, view) => 
            {
                handler.PlatformView.NestedScrollingEnabled = true;
            });
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
