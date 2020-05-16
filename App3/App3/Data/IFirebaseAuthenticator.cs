using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace App3.Data
{
    public interface IFirebaseAuthenticator
    {
        Task<string> LoginWithEmailPassword(string email, string password);
        bool IsSignedIn();
        bool SignOut();


    }
}
