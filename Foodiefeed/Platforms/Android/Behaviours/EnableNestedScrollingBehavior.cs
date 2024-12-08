using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using Microsoft.Maui.Controls;

namespace Foodiefeed.Behaviours
{

    public class EnableNestedScrollingBehavior : Behavior<ScrollView>
    {
        protected override void OnAttachedTo(ScrollView scrollView)
        {
            base.OnAttachedTo(scrollView);

            if (scrollView.Handler?.PlatformView is Android.Views.View view)
            {
                view.Touch += OnTouch;
            }
        }    

        protected override void OnDetachingFrom(ScrollView scrollView)
        {
            base.OnDetachingFrom(scrollView);

            if (scrollView.Handler?.PlatformView is Android.Views.View view)
            {
                view.Touch -= OnTouch;
            }
        }

        private void OnTouch(object? sender, Android.Views.View.TouchEventArgs e)
        {
            (sender as Android.Views.View)?.Parent?.RequestDisallowInterceptTouchEvent(true);
        }
    }
}

