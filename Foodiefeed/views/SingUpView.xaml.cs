using Foodiefeed_api.models.user;
using Newtonsoft.Json;
using System.Text;

namespace Foodiefeed
{
    public partial class SignUpView : ContentPage
    {
        public SignUpView()
        {
            InitializeComponent();
        }

        private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new MainPage());
        }

        private async void AddUser(object sender, EventArgs e)
        {

            CreateUserDto dto = new CreateUserDto()
            {
                FirstName = FirstNameEntry.Text,
                LastName = LastNameEntry.Text,
                Email = emailEntry.Text,
                PasswordHash = passwordEntry.Text,
                ProfilePicturePath = "deafult"
            };

            var json = JsonConvert.SerializeObject(dto);

            var apiBaseUrl = "http://localhost:5000";
            var endpoint = "api/user";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                //client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                //client.DefaultRequestHeaders.Add("Content-Type", "application/json");
               // client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                try
                {
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = client.PostAsync(endpoint, content);

                    if (response.Result.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Result.Content.ReadAsStringAsync();

                        await DisplayAlert("Response",responseContent,"OK");
                    }
                    else
                    {
                        await DisplayAlert("Response", response.Result.StatusCode.ToString(), "OK");
                    }

                }
                catch(Exception ex)
                {
                    await DisplayAlert("Response", ex.Message, "OK");
                }
            }
        }
    }
}
