using Foodiefeed.models.dto;
using Foodiefeed.viewmodels;
using Newtonsoft.Json;
using System.Text;

namespace Foodiefeed
{
    public partial class SignUpView : ContentPage
    {
        public SignUpView()
        {
            InitializeComponent();
            this.BindingContext = new UserViewModel();
        }
       
    }
}
