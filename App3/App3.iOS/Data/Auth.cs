
using Auth0.OidcClient;
using System.Diagnostics;
using IdentityModel.OidcClient;
using System.Threading.Tasks;
using App3.Data;
using App3.Models;
using RestSharp;
using Newtonsoft.Json;
using Xamarin.Forms;

[assembly: Dependency(typeof(App3.iOS.Auth))]
namespace App3.iOS
{
    public class Auth : IAuthentication
    {
        static private string domain = "dev-2huf9bd9.auth0.com";
        static private string clientid = "h0zRTg9paH66Ck2rXwE5WIpVfhERNe1k";
        static private Auth0Client client = new Auth0Client(new Auth0ClientOptions
        {

            Domain = domain,
            ClientId = clientid,

        });

        
        public User RefreshUserData(User user)
        {
            var client = new RestClient("https://dev-2huf9bd9.auth0.com/");
            var request = new RestRequest("userinfo", Method.GET);
            request.AddParameter("access_token", user.access_token);
            request.AddParameter("scope", "openid offline_access");

            // We execute the request and capture the response
            // in a variable called `response`
            IRestResponse response = client.Execute(request);

            // Using the Newtonsoft.Json library we deserialaize the string into an object,
            // we have created a LoginToken class that will capture the keys we need
            Debug.WriteLine(response.Content);
            try
            {
                User updated = JsonConvert.DeserializeObject<User>(response.Content);
                updated.access_token = user.access_token;
                updated.id_token = user.id_token;
                return updated;
            }
            catch
            {
                return null;
            }
            

            // We check to see if we received an `id_token` and if we did make a secondary call
            // to get the user data. If we did not receive an `id_token` we can safely assume
            // that the authentication failed so we display an error message telling the user
            // to try again.
            
            
        }
        async public Task<User> Login()
        {
            Debug.WriteLine("Prompting Login...");
            var loginResult = await client.LoginAsync(new LoginRequest());
            if (loginResult.IsError)
            {
                Debug.WriteLine($"An error occurred during login: {loginResult.Error}");
                return null;
            }

            
            User user = new User();
            

            user.name = loginResult.User.FindFirst(c => c.Type == "name")?.Value;
            user.email = loginResult.User.FindFirst(c => c.Type == "email")?.Value;
            user.id_token = loginResult.IdentityToken;
            user.access_token = loginResult.AccessToken;

            
            //user.token = new Token(loginResult.IdentityToken, loginResult.AccessToken);
            return user;
            
        }
        public void Logout()
        {
            var client = new RestClient("https://dev-2huf9bd9.auth0.com/");
            var request = new RestRequest("/v2/logout", Method.GET);
            //request.AddParameter("client_id", clientid);

            IRestResponse response = client.Execute(request);
            
            Debug.WriteLine(response.Content);
            // Using the Newtonsoft.Json library we deserialaize the string into an object,
            // we have created a LoginToken class that will capture the keys we need
            //Token token = JsonConvert.DeserializeObject<Token>(response.Content);
            //BrowserResultType browserResult = await client.LogoutAsync();
            
        }
        
    }
}