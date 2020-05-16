
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using App3.Data;
namespace App3.iOS.Data
{
    
    public class FirebaseAuthenticator : IFirebaseAuthenticator
    {
        public FirebaseAuthenticator()
        {
            
        }
        
        public async Task<string> LoginWithEmailPassword(string email, string password)
        {

            //var user = await Firebase.Auth.Auth.DefaultInstance.SignInWithPasswordAsync(email, password);
            return null;//await user.User.GetIdTokenAsync();
            
        }
        public bool IsSignedIn()
        {
            //var user = Firebase.Auth.Auth.DefaultInstance.CurrentUser;
            return false;//user != null;
        }
        public bool SignOut()
        {
            try
            {
                //_ = Firebase.Auth.Auth.DefaultInstance.SignOut(out NSError error);
                return true;//error == null;
            }
            catch (Exception)
            {
                Debug.WriteLine("Exception occurred durign user sign out");
                return false;
            }
        }
    }
    
}