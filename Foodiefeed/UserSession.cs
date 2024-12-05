using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodiefeed
{
    public class UserSession
    {
        public int? Id { get; set; }

        public void InitializeSession(int userId)
        {
            Id = userId;
        }

        public void Logout()
        {
            SetOffline();
            UnbindId();
        }

        public void UnbindId()
        {
            Id = null;
        }

        public async Task SetOnline() {

#if WINDOWS
            var apiBaseUrl = "http://localhost:5000";
#endif
#if ANDROID
                var apiBaseUrl = "http://10.0.2.2:5000";
#endif

            var endpoint = "api/user/SetOnline/" + Id.ToString();

            using(var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(apiBaseUrl);

                try
                {
                    var response = await httpClient.PutAsync(endpoint, null);

                    if (!response.IsSuccessStatusCode) { throw new Exception(); }

                }catch (Exception ex)
                {
                    //set session to offline
                }
            }
        }

        public async Task SetOffline() {

#if WINDOWS
            var apiBaseUrl = "http://localhost:5000";
#endif
#if ANDROID
                var apiBaseUrl = "http://10.0.2.2:5000";
#endif

            var endpoint = "api/user/SetOffline/" + Id.ToString();

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(apiBaseUrl);

                try
                {
                    var response = await httpClient.PutAsync(endpoint, null);

                }
                catch (Exception ex)
                {
                    
                }
            }
            UnbindId();
        }
    }
}
