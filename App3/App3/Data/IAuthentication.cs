
using System.Threading.Tasks;
using App3.Models;


namespace App3.Data
{
    public interface IAuthentication
    {
        Task<User> Login();
        void Logout();
        User RefreshUserData(User user);

    }
}
