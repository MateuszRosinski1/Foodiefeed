using Android.Content;
using Android.Views;
using Foodiefeed.Platforms.Android.CustomRenderers;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;


[assembly: ExportRenderer(typeof(ScrollView), typeof(TouchEventScrollViewRenderer))]
namespace Foodiefeed.Platforms.Android.CustomRenderers
{
    public class TouchEventScrollViewRenderer : ScrollViewRenderer
    {
        public TouchEventScrollViewRenderer(Context context) : base(context)
        {
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            switch (ev.Action & MotionEventActions.Mask)
            {
                case MotionEventActions.Down: 
                    this.NestedScrollingEnabled = false;
                    break;
                case MotionEventActions.Move: 
                    this.NestedScrollingEnabled = true; 
                    break;
                default:
                    break;
            }
            return base.OnInterceptTouchEvent(ev);
        }
    }
}
