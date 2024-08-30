using Foodiefeed.viewmodels;
using Foodiefeed.views.windows.contentview;
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
            PostGrid.Children.Add(new PostView());
            PostGrid.Children.Add(new PostView());
            PostGrid.Children.Add(new PostView());
            PostGrid.Children.Add(new PostView());


        }

        private void OnScrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            
        }
    }
}
