using App3.Data;
using App3.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using static App3.Data.UserDatabaseController;

namespace App3
{
    public partial class App : Application
    {
        //static TokenDatabaseController tokenDatabase;
        static UserDatabaseController userDatabase;
        //static RestService restService;

        static IFirebaseAuthenticator authenticator = DependencyService.Get<IFirebaseAuthenticator>();
        public static UserDatabaseController UserDatabase
        {
            get
            {
                if (userDatabase == null) userDatabase = new UserDatabaseController();
                return userDatabase;
            }
        }

        static List<Party> userParties;
        public static List<Party> UserParties
        {
            get
            {
                if (userParties == null) userParties = new List<Party>();//FirebaseHelper.GetPartiesThrownByUser(UserDatabase.GetUser().uid).Result;
                return userParties;
            }
        }

        static List<Party> userInvites;
        public static List<Party> UserInvites
        {
            get
            {
                if (userInvites == null) userInvites = new List<Party>();//FirebaseHelper.GetPartiesThrownByUser(UserDatabase.GetUser().uid).Result;
                return userInvites;
            }
        }

        static List<User> userFriends;
        public static List<User> UserFriends
        {
            get
            {
                if (userFriends == null) userFriends = new List<User>(); //FirebaseHelper.GetFriendsList(UserDatabase.GetUser().uid).Result;
                return userFriends;
            }
        }
        public App()
        {
            InitializeComponent();
            //HttpsService.httpsValidation.Initialize();
            //MainPage = new NavigationPage(new Home());
            //userFriends = FirebaseHelper.GetFriendsList(UserDatabase.GetUser().uid).Result;
            //userParties = FirebaseHelper.GetPartiesThrownByUser(UserDatabase.GetUser().uid).Result;
            if (!RefreshUser()) Logout();
            else
            {

                userFriends = FirebaseHelper.GetFriendsList(UserDatabase.GetUser().uid).Result;
                Debug.WriteLine("HELLO@#");
                if (userFriends != null) userInvites = FirebaseHelper.GetInvitedParties(userFriends).Result;
                MainPage = createMainPage();
                    
            }
        }
        private static Page createMainPage()
        {
            return new MasterTabbedPage();
        }
        private static bool RefreshUser()
        {
            var currentUser = UserDatabase.GetUser();
            if (currentUser == null) return false;
            User updatedUser = DependencyService.Get<IFirebaseAuthenticator>().RefreshCurrentUser();
            UserDatabase.RemoveUserData();
            if (updatedUser == null) return false;
            User userData = FirebaseHelper.GetUserFromUID(updatedUser.uid).Result;
            if (userData != null)
            {
                userData.uid = updatedUser.uid;
                userData.email = updatedUser.email;
            }
            else userData = updatedUser;
            UserDatabase.SetUser(userData);

            userData.printUser();
            return true;
        }
        public static void Logout()
        {

            UserDatabase.RemoveUserData();
            if (!DependencyService.Get<IFirebaseAuthenticator>().SignOut()) Debug.WriteLine("Error Signing out");
            NavigationPage loginScreen = new NavigationPage(new SignUpPage());
            loginScreen.BarTextColor = Color.White;
            loginScreen.BarBackgroundColor = Color.FromHex("#28053d");
            Current.MainPage = loginScreen;
        }
        public static async Task<User> SignupAsync(string email, string password, string name)
        {
            string tokenId = await authenticator.SignUpUser(email, password);
            if (tokenId == "") return null;
            Debug.WriteLine("tokenId = " + tokenId);
            User user = authenticator.GetCurrentUser();
            user.name = name;
            if (!await FirebaseHelper.AddUser(user))
            {
                Debug.WriteLine("Could not add user to database");
                return null;
            }
            user.printUser();
            UserDatabase.SetUser(user);
            await GetUserMediaAndDisplay();
            return user;
        }
        public static async Task<User> LoginAsync(string email, string password)
        {


            string tokenId = await authenticator.LoginWithEmailPassword(email, password);
            if (tokenId == "") return null;
            Debug.WriteLine(tokenId);
            var user = authenticator.GetCurrentUser(); //await FirebaseHelper.GetUserFromUID(uid);
            user.printUser();
            if (user == null)
            {
                Debug.WriteLine("User not found");
                return null;
            }
            UserDatabase.SetUser(user);
            Debug.WriteLine("OK");
            await GetUserMediaAndDisplay();
            return user;

        }
        public static async Task GetUserMediaAndDisplay()
        {
            await GetFriendsAsync();
            await GetInvitesAsync();
            Current.MainPage = createMainPage();
        }
        public static async Task<List<User>> GetFriendsAsync()
        {
            
            userFriends = await FirebaseHelper.GetFriendsList(UserDatabase.GetUser().uid);
            Debug.WriteLine("Got Friends");
            return userFriends;
        }
        public static async Task<List<Party>> GetInvitesAsync()
        {

            if (userFriends == null) return userInvites;
            userInvites = await FirebaseHelper.GetInvitedParties(userFriends);
            return userInvites;
        }
        public static async Task<List<Party>> GetPartiesAsync()
        {
            userParties = await FirebaseHelper.GetPartiesThrownByUser(UserDatabase.GetUser());
            return userParties;
        }
    }
}
