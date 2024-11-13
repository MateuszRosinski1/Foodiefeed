
using Foodiefeed.Resources.Styles;
using Foodiefeed.viewmodels;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace Foodiefeed
{
    public partial class App : Application
    {
        private readonly UserSession _userSession;

        public App(UserViewModel vm,UserSession us)
        {
            InitializeComponent();
            _userSession = us;
            //MainPage = new LogInPage(vm);
            MainPage = new BoardPage(new BoardViewModel(us));

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

            //window.Destroying += Window_Destroying;

#if WINDOWS
            window.MinimumHeight = 800;
            window.MinimumWidth = 1500;
#endif
            return window;
        }

        private void Window_Destroying(object sender, EventArgs e)
        {

            if (_userSession.IsLoggedIn)
            {
                _userSession.SetOffline();
                _userSession.UnbindId();
            }
            

            Application.Current.Quit();            
        }


    }
}
