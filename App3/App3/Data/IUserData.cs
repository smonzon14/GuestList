using App3.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace App3.Data
{
    public interface IUserData
    {
        User GetUser();
        void SetUser(User user);
        void RemoveUserData();
    }
}
