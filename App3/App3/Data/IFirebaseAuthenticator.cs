using App3.Models;
using System.Threading.Tasks;

namespace App3.Data
{
    public interface IFirebaseAuthenticator
    {
        Task<string> SignUpUser(string email, string password);
        Task<string> LoginWithEmailPassword(string email, string password);
        User GetCurrentUser();
        bool IsSignedIn();
        bool SignOut();
        User RefreshCurrentUser();

        Task<bool> ResetPassword(string email);

    }
}
