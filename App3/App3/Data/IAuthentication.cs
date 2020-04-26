using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using App3.Models;
using Auth0.OidcClient;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;

namespace App3.Data
{
    public interface IAuthentication
    {
        Task<User> Login();
        void Logout();
        User RefreshUserData(User user);

    }
}
