using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodiefeed
{
    public class UserSession
    {
        private readonly IServiceProvider _serviceProvider;

        public UserSession(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

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
            var endpoint = $"api/user/SetOnline/{Id}";

            using(var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://foodiefeedapi-daethrcqgpgnaehs.polandcentral-01.azurewebsites.net");
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

            var endpoint = $"api/user/SetOffline/{Id}";

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://foodiefeedapi-daethrcqgpgnaehs.polandcentral-01.azurewebsites.net");

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
