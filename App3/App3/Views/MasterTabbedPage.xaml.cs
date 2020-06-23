using App3.Data;
using App3.Models;
using App3.Views;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace App3
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterTabbedPage : TabbedPage

    {
        public MasterTabbedPage()
        {
            InitializeComponent();
            if (refreshUser() == 1) switchToSignIn();
            else loadViews();

        }
        private void loadViews()
        {
            
           
            Children.Clear();

            NavigationPage friends = new NavigationPage(new FriendsPage())
            {
                IconImageSource = "tab_friends",
                BarTextColor = Color.White,
                BarBackgroundColor = Color.Black
            };


            // Home Page
            NavigationPage home = new NavigationPage(new HomePage(this))
            {
                IconImageSource = "tab_home",
                BarTextColor = Color.White,
                BarBackgroundColor = Color.Black
            };


            // Profile Page
            NavigationPage profile = new NavigationPage(new ProfilePage(null, this))
            {
                IconImageSource = "tab_profile",
                BarTextColor = Color.White,
                BarBackgroundColor = Color.Black
            };

            NavigationPage feed = new NavigationPage(new FeedPage())
            {
                IconImageSource = "tab_feed",
                BarTextColor = Color.White,
                BarBackgroundColor = Color.Black
            };

            Children.Add(friends);
            Children.Add(home);
            Children.Add(profile);
            Children.Add(feed);

            CurrentPage = home;



        }
        
        async private void switchToSignIn()
        {
            NavigationPage loginScreen = new NavigationPage(new SignUpPage());
            loginScreen.BarTextColor = Color.White;
            loginScreen.BarBackgroundColor = Color.FromHex("#28053d");
            loginScreen.Disappearing += (sender2, e2) => { 
                loadViews(); 
            };
            await Navigation.PushModalAsync(loginScreen);
        }
        public void OnLogout()
        {
            App.UserDatabase.RemoveUserData();
            Debug.WriteLine("Deleted User Data");
            if (!DependencyService.Get<IFirebaseAuthenticator>().SignOut()) DisplayAlert("Error Signing Out", "Oops", "Continue");
            switchToSignIn();
        }
        // returns 1 if user refresh is unsuccessful (i.e. expired or missing token)
        // returns 0 if user refresh is successful.
        public static int refreshUser()
        {
            User currentUser = App.UserDatabase.GetUser();
            if (currentUser == null) return 1;
            User updatedUser = DependencyService.Get<IFirebaseAuthenticator>().RefreshCurrentUser();
            App.UserDatabase.RemoveUserData();
            if (updatedUser == null) return 1;
            var userData = FirebaseHelper.GetUserFromUID(updatedUser.uid).Result;
            if (userData != null)
            {
                userData.uid = updatedUser.uid;
                userData.email = updatedUser.email;
            }
            else userData = updatedUser;
            App.UserDatabase.SetUser(userData);

            userData.printUser();
            return 0;
        }
    }
}