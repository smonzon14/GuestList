using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using App3.Data;
using Xamarin.Forms;
using App3.iOS.Data;
using App3.Models;

[assembly: Dependency(typeof(UserDefaults))]
namespace App3.iOS.Data
{
    public class UserDefaults : IUserData
    {
        static NSUserDefaults defaults;
        public UserDefaults()
        {
            defaults = NSUserDefaults.StandardUserDefaults;
        }
        public User GetUser()
        {
            var user = new User()
            {
                name = defaults.StringForKey("name"),
                uid = defaults.StringForKey("uid"),
                email = defaults.StringForKey("email")
            };
            user.printUser();
            if (user.uid == null || user.uid.Length == 0) return null;
            return user;
        }

        public void RemoveUserData()
        {
            defaults.SetString("", "name");
            defaults.SetString("", "uid");
            defaults.SetString("", "email");
        }

        public void SetUser(User user)
        {

            defaults.SetString(user.name == null ? "" : user.name, "name");
            defaults.SetString(user.uid == null ? "" : user.uid, "uid");
            defaults.SetString(user.email == null ? "" : user.email, "email");
        }
    }
}