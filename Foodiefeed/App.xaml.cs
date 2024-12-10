using Foodiefeed.viewmodels;
using Microsoft.Maui.Handlers;

namespace Foodiefeed
{
    public partial class App : Application
    {
        private readonly UserSession _userSession;
        private readonly IThemeHandler _themeHandler;

        public App(UserViewModel vm,UserSession us, IThemeHandler themeHandler)
        {
            InitializeComponent();
            _userSession = us;
            _themeHandler = themeHandler;

            MainPage = new LogInPage(vm);
#if WINDOWS
            SwitchHandler.Mapper.AppendToMapping("Custom", (h, v) =>
            {
                // Get rid of On/Off label beside switch, to match other platforms
                h.PlatformView.OffContent = string.Empty;
                h.PlatformView.OnContent = string.Empty;

                h.PlatformView.MinWidth = 0;
            });
#endif

        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window  = base.CreateWindow(activationState);

            window.Destroying += Window_Destroying;

#if WINDOWS
            window.MinimumHeight = 800;
            window.MinimumWidth = 1500;
#endif
            return window;
        }

        private async void Window_Destroying(object sender, EventArgs e)
        {
            await _userSession.SetOffline();
            await _themeHandler.SaveThemeState();
            Application.Current.Quit();          
        }

        protected override void OnStart()
        {
           base.OnStart();
            _themeHandler.LoadTheme();
        }


    }
}
