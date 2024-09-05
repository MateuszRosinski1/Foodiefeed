
namespace Foodiefeed
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //MainPage = new LogInPage();
            MainPage = new BoardPage(new viewmodels.BoardViewModel());
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window  = base.CreateWindow(activationState);

            window.MinimumHeight = 800;
            window.MinimumWidth = 1400;

            return window;
        }
    }
}
