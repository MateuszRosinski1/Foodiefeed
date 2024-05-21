using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Foodiefeed.models.dto;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;

namespace Foodiefeed.viewmodels
{
    public partial class UserViewModel : ObservableObject
    {
        #region Login Logic

        private class UserCredentials
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        [RelayCommand]
        public async Task LogIn()
        {
            //            if (LoginCanProceed())
            //            {
            //                UserCredentials credentials = new UserCredentials();
            //                {
            //                    credentials.Username = Username;
            //                    credentials.Password = Password;
            //                };

            //                var json = JsonConvert.SerializeObject(credentials);


            //#if WINDOWS
            //                var apiBaseUrl = "http://localhost:5000";
            //#endif
            //#if ANDROID
            //                           var apiBaseUrl = "http://10.0.2.2:5000";
            //#endif

            //                var endpoint = "api/user/login";

            //                using (var client = new HttpClient())
            //                {

            //                    client.BaseAddress = new Uri(apiBaseUrl);

            //                    try
            //                    {
            //                        var content = new StringContent(json, Encoding.UTF8, "application/json");

            //                        var response = client.PostAsync(endpoint, content);

            //                        if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            //                        {
            //                            await ToBoardPage();
            //                        }
            //                        else
            //                        {
            //                            //await DisplayAlert("Response", response.Result.StatusCode.ToString(), "OK");
            //                        }

            //                    }
            //                    catch (Exception ex)
            //                    {
            //                        //await DisplayAlert("Response", ex.Message, "OK");
            //                    }
            //                }
            //            }
            await Task.Delay(2000);
            await ToBoardPage();
        }

        private async Task ToBoardPage()
        {
            App.Current.MainPage = new AppShell();
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
            App.Current.MainPage = new SignUpView();
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
        public UserViewModel()
        {
            ValidateFirstname = true;
            ValidateLastname = true;
            ValidateUsername = true;
            ValidateEmail = true;
            ValidatePasswordRepeat = true;
            ValidatePassword = true;
        }

        [RelayCommand]
        public async Task ToLogInPage()
        {
            App.Current.MainPage = new LogInPage();
        }

        

        [RelayCommand]
        public async void Register()
        {
            if (CanProceed())
            {

                CreateUserDto dto = new CreateUserDto();
                {
                    dto.FirstName = Firstname;
                    dto.LastName = Lastname;
                    dto.Username = Username;
                    dto.Email = Email;
                    dto.PasswordHash = Password;
                }


                var json = JsonConvert.SerializeObject(dto);

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
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var response = client.PostAsync(endpoint, content);

                        if (response.Result.IsSuccessStatusCode)
                        {
                            var responseContent = await response.Result.Content.ReadAsStringAsync();

                            await ToLogInPage();
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
