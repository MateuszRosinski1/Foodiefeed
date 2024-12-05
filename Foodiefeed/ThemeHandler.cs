using Foodiefeed.Resources.Styles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodiefeed
{
    public interface IThemeHandler
    {
        Task LoadTheme();
        Task ChangeTheme();
        Task SaveThemeState();
        bool ThemeFlag { get; set; }
    }

    class ThemeHandler : IThemeHandler
    {
        public bool ThemeFlag { get; set; }

        public async Task ChangeTheme()
        {
            var mergedDictionaries = Application.Current.Resources.MergedDictionaries;

            if (mergedDictionaries.Count > 2)
            {
                mergedDictionaries.Remove(mergedDictionaries.ElementAt(2));
            }

            if (ThemeFlag)
            {
                mergedDictionaries.Add(new DarkTheme());
            }
            else
            {
                mergedDictionaries.Add(new LightTheme());
            }
        }

        public async Task LoadTheme()
        {
            ThemeFlag = Preferences.Get("ThemeFlag",default(bool));

            await ChangeTheme();          
        }

        public async Task SaveThemeState()
        {
            Preferences.Set("ThemeFlag",ThemeFlag);
            await Task.CompletedTask;
        }
    }
}
