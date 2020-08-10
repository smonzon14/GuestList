using App3.Models;
using Google.Api;
using SQLite;
using Xamarin.Forms;

namespace App3.Data
{
    public class UserDatabaseController
    {
        static IUserData userData;
        User user;
        public UserDatabaseController()
        {
            userData = DependencyService.Get<IUserData>();
            user = userData.GetUser();
            user.updateProfileImageSource();
        }
        public User GetUser()
        {
            if(user == null)
            {
                user = userData.GetUser();
                if (user != null) user.updateProfileImageSource();
            }
            return user;
        }
        public void SetUser(User user)
        {
            userData.SetUser(user);
            user = userData.GetUser();
            user.updateProfileImageSource();
        }
        public void RemoveUserData()
        {
            userData.RemoveUserData();
            user = null;
        }
    }
}
