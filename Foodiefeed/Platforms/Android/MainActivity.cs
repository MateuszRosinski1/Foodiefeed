﻿using Android.App;
using Android.Content.PM;
using Android.OS;

namespace Foodiefeed
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnStop()
        {
            base.OnStop();
        }


        protected override void OnResume()
        {
            base.OnResume();
        }
    }
}
