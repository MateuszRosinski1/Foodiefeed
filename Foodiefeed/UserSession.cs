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

        public bool IsLoggedIn { get; set; } =  false;

        public void InitializeSession(int userId)
        {
            Id = userId;
            IsLoggedIn = true;
        }

        public void UnbindId()
        {
            Id = null;
            IsLoggedIn = false;
        }

        public void SetOnline() {

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
                    var response = httpClient.PutAsync(endpoint, null);

                }catch (Exception ex)
                {
                    //set session to offline
                }
            }
        }

        public void SetOffline() {

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
                    var response = httpClient.PutAsync(endpoint, null);

                }
                catch (Exception ex)
                {
                    
                }
            }

        }
    }
}
