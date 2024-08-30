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
    }
}
