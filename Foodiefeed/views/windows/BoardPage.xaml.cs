using Foodiefeed.viewmodels;

namespace Foodiefeed
{
    public partial class BoardPage : ContentPage
    {
        public BoardPage()
        {
            InitializeComponent();
            this.BindingContext = new BoardViewModel();            
        } 
    }
}
