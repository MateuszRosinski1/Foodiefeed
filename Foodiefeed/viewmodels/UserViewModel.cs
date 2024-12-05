using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Foodiefeed.services;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace Foodiefeed.viewmodels
{
    public partial class UserViewModel : ObservableObject
    {
        private readonly UserSession _userSession;

        private readonly IFoodiefeedApiService _foodiefeedApiServce;
        private readonly IServiceProvider _serviceProvider;

        public UserViewModel(UserSession userSession,IFoodiefeedApiService foodiefeedApiServce,IServiceProvider serviceProvider)
        {
            ValidateFirstname = true;
            ValidateLastname = true;
            ValidateUsername = true;
            ValidateEmail = true;
            ValidatePasswordRepeat = true;
            ValidatePassword = true;
            _userSession = userSession;
            _foodiefeedApiServce = foodiefeedApiServce;
            _serviceProvider = serviceProvider;
        }

        #region Login Logic

        private class UserCredentials
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        [RelayCommand]
        public async Task LogIn()
        {
            if (LoginCanProceed())
            {
                UserCredentials credentials = new UserCredentials();
                {
                    credentials.Username = Username;
                    credentials.Password = Password;
                };

                var json = JsonConvert.SerializeObject(credentials);


#if WINDOWS
                            var apiBaseUrl = "http://localhost:5000";
#endif
#if ANDROID
                var apiBaseUrl = "http://10.0.2.2:5000";
#endif

                var endpoint = "api/user/login";

                using (var client = new HttpClient())
                {

                    client.BaseAddress = new Uri(apiBaseUrl);

                    try
                    {
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var response = await client.PostAsync(endpoint, content);

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var StringId = await response.Content.ReadAsStringAsync();
                            var id = Convert.ToInt32(StringId);
                            _userSession.InitializeSession(id);
                            await _userSession.SetOnline();
                            await ToBoardPage();
                        }
                        else
                        {
                            //await DisplayAlert("Response", response.Result.StatusCode.ToString(), "OK");
                        }

                    }
                    catch (Exception ex)
                    {
                        //await DisplayAlert("Response", ex.Message, "OK");
                    }
                }
            }
            await Task.Delay(2000);
        }

        private async Task ToBoardPage()
        {

            var boardPage = _serviceProvider.GetRequiredService<BoardPage>();
            App.Current.MainPage = boardPage;
        }

        private bool LoginCanProceed()
        {
            if (inputchanged &&             
                ValidatePassword &&
                ValidateUsername )
            {
                return true;
            }
            return false;
        }

        [RelayCommand]
        public async Task ToSignUpPage()
        {
            App.Current.MainPage = new SignUpView(this);
        }

        #endregion

        #region Register Logic

        #region flags
        private bool inputchanged = false;
        #endregion

        #region properties

        [ObservableProperty]
        string _Firstname;

        [ObservableProperty]
        bool _ValidateFirstname;

        [ObservableProperty]
        string _Lastname;
        [ObservableProperty]
        bool _ValidateLastname;   

        [ObservableProperty]
        string _Email;
        [ObservableProperty]
        bool _ValidateEmail ;

        [ObservableProperty]
        string _PasswordRepeat;

        [ObservableProperty]
        bool _ValidatePasswordRepeat;

        #endregion      

        [RelayCommand]
        public async Task ToLogInPage()
        {
            App.Current.MainPage = new LogInPage(this);
        }

        

        [RelayCommand]
        public async void Register()
        {
            if (CanProceed())
            {
#if WINDOWS
                var apiBaseUrl = "http://localhost:5000";
#endif
#if ANDROID
                var apiBaseUrl = "http://10.0.2.2:5000";
#endif

                var endpoint = "api/user/register";

                using (var client = new HttpClient())
                {

                    client.BaseAddress = new Uri(apiBaseUrl);

                    try
                    {
                        using (var formContent = new MultipartFormDataContent())
                        {
                            formContent.Add(new StringContent(Firstname), "FirstName"); 
                            formContent.Add(new StringContent(Lastname), "LastName"); 
                            formContent.Add(new StringContent(Username), "Username"); 
                            formContent.Add(new StringContent(Email), "Email"); 
                            formContent.Add(new StringContent(Password), "PasswordHash");

                            string basePath = AppDomain.CurrentDomain.BaseDirectory;

                            var files = Directory.GetFiles(basePath, "avatar*.jpg");
                            var filePath = files.FirstOrDefault();

                            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                            {
                                var fileContent = new StreamContent(fileStream);
                                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                                formContent.Add(fileContent, "file", "avatar.jpg");

                                var response = await client.PostAsync(endpoint, formContent);

                                if (!response.IsSuccessStatusCode)
                                {
                                    throw new Exception(await response.Content.ReadAsStringAsync());
                                }
                                await ToLogInPage();
                            }                        
                        }
                    }
                    catch (Exception ex)
                    {
                        //handle ?
                    }
                }
            }

        }

        #region validators
        [RelayCommand]
        public void EmailValidate()
        {
            string emailRegex = @"^([\w-\.]+@([0-9a-zA-Z-.拉着,\u4e00-\u9fff]+)\.[a-zA-Z]{2,5})+$";
            ValidateEmail = Regex.IsMatch(Email, emailRegex);
        }

        [RelayCommand]
        public void ValidatePasswordRepeatFun()
        {
            ValidatePasswordRepeat = PasswordRepeat == Password ? true : false;
        }

        [RelayCommand]
        public void ValidateFirstName()
        {
            ValidateFirstname = Regex.IsMatch(Firstname, "^[A-Z][a-zA-Z]+$");
        }

        [RelayCommand]
        public void ValidateLastName() {
            ValidateLastname = Regex.IsMatch(Lastname, "^[A-Z][a-zA-Z]+$");
            
        }

        #endregion
        private bool CanProceed()
        {
            if( inputchanged && 
                ValidateFirstname && 
                ValidateLastname && 
                ValidateEmail && 
                ValidatePassword && 
                ValidateUsername && 
                ValidatePasswordRepeat)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region shared logic

        [ObservableProperty]
        string _Username;
        [ObservableProperty]
        bool _ValidateUsername;
        [ObservableProperty]
        string _Password;
        [ObservableProperty]
        bool _ValidatePassword;

        [RelayCommand]
        public void UsernameValidate()
        {
            ValidateUsername = Regex.IsMatch(Username, "^(?=.*[a-zA-Z])[a-zA-Z0-9_]+$"); // Letters, digits and underscore allowed. 
            inputchanged = true;
        }

        [RelayCommand]
        public void ValidatePasswordFun()
        {
            ValidatePassword = Regex.IsMatch(Password, "^(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d]{8,}$"); //Minimum eight characters, at least one letter and one number.
        }
        #endregion
    }
}