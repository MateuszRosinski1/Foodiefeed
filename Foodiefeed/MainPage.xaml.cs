using Foodiefeed.viewmodels;
using System.Windows.Input;

namespace Foodiefeed
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = new UserViewModel();
        }
    }

}
