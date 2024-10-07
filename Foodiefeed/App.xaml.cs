
using Foodiefeed.viewmodels;

namespace Foodiefeed
{
    public partial class App : Application
    {
        private readonly UserSession _userSession;

        public App(UserViewModel vm,UserSession us)
        {
            InitializeComponent();
            Sharpnado.MaterialFrame.Initializer.Initialize(loggerEnable: false,debugLogEnable: false);
            _userSession = us;
            //MainPage = new LogInPage(vm);
            MainPage = new BoardPage(new BoardViewModel(us));

            //AppDomain.CurrentDomain.UnhandledException
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
