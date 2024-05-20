using Foodiefeed.viewmodels;
using Microsoft.Maui.Controls;
using System.Windows.Input;

namespace Foodiefeed
{
    public partial class BoardPage : ContentPage
    {
        public BoardPage(BoardViewModel vm)
        {
            InitializeComponent();
            this.BindingContext = vm;
        }

        private void OnScrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            
        }
    }
}
