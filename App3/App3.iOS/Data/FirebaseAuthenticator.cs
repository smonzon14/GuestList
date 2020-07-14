
using App3.Data;
using App3.iOS.Data;
using Firebase.Auth;
using Foundation;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(FirebaseAuthenticator))]
namespace App3.iOS.Data
{

    public class FirebaseAuthenticator : IFirebaseAuthenticator
    {
        public async Task<string> SignUpUser(string email, string password)
        {

            try
            {
                var user = await Auth.DefaultInstance.CreateUserAsync(email, password);
                return await user.User.GetIdTokenAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error Creating User: " + e.Message);
                return "";
            }
        }
        public async Task<string> LoginWithEmailPassword(string email, string password)
        {

            try
            {
                var user = await Auth.DefaultInstance.SignInWithPasswordAsync(email, password);
                return await user.User.GetIdTokenAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error Logging in: " + e.Message);
                return "";
            }


        }
        public Models.User GetCurrentUser()
        {
            var authUserData = Auth.DefaultInstance.CurrentUser;
            if (authUserData == null) return null;
            return new Models.User
            {
                uid = authUserData.Uid,
                name = authUserData.DisplayName,
                email = authUserData.Email,

            };
        }
        public bool IsSignedIn()
        {

            var user = Auth.DefaultInstance.CurrentUser;
            return user != null;
        }

        public bool SignOut()
        {
            try
            {
                _ = Auth.DefaultInstance.SignOut(out NSError error);
                return error == null;
            }
            catch (Exception)
            {
                Debug.WriteLine("Exception occurred durign user sign out");
                return false;
            }
        }

        public Models.User RefreshCurrentUser()
        {
            return GetCurrentUser();
        }
    }

}