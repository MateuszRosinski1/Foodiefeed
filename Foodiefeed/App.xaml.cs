
namespace Foodiefeed
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //MainPage = new LogInPage();
            MainPage = new BoardPage(new viewmodels.BoardViewModel());

            //AppDomain.CurrentDomain.UnhandledException
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window  = base.CreateWindow(activationState);

#if WINDOWS
            window.MinimumHeight = 800;
            window.MinimumWidth = 1400;
#endif
            return window;
        }
    }
}
