using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firebase.CloudMessaging;
using Foundation;
using UIKit;
using UserNotifications;

namespace App3.iOS.FCM
{
    public class FCMMessagingDelegate
    {
        [Export("messaging:didReceiveRegistrationToken:")]
        public void DidReceiveRegistrationToken(Messaging messaging, string fcmToken)
        {
            Console.WriteLine($"Firebase registration token: {fcmToken}");

            // TODO: If necessary send token to application server.
            // Note: This callback is fired at each app startup and whenever a new token is generated.
        }
        public IntPtr Handle => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}