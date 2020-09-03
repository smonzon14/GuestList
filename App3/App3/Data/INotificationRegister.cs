using System;
using System.Collections.Generic;
using System.Text;

namespace App3.Data
{
    public interface INotificationRegister
    {
        void RegisterForNotifications();
        void DeregisterNotifications();
    }
}
