using App3.Data;
using App3.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App3
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabbedPage1 : TabbedPage

    {
        public TabbedPage1()
        {
            InitializeComponent();
            if (refreshUser() == 1) switchToSignIn();
            else loadViews();

        }
        private void loadViews()
        {
            Children.Clear();
            SelectedTabColor = Color.BlueViolet;
            UnselectedTabColor = Color.LightGray;
            

            /*
            // Host Page
            NavigationPage host = new NavigationPage(new Host(this));
            host.Title = "Host";
            host.BarBackgroundColor = Color.Black;
            host.BarTextColor = Color.White;
            Children.Add(host);*/

            //
            NavigationPage friends = new NavigationPage(new Friends(this));
            //friends.Title = "Friends";
            friends.BarBackgroundColor = Color.Black;
            friends.BarTextColor = Color.White;
            friends.IconImageSource = "tab_friends";
            Children.Add(friends);

            // Home Page
            NavigationPage home = new NavigationPage(new Home(this));
            //home.Title = "Parties";
            home.BarBackgroundColor = Color.White;

            home.BarTextColor = Color.Black;
            home.IconImageSource = "tab_home";
            
            Children.Add(home);

            User user = App.UserDatabase.GetUser();
            // Profile Page
            NavigationPage profile = new NavigationPage(new ProfilePage(user, this));
            //profile.Title = "Me";
            profile.BarBackgroundColor = Color.Black;
            profile.BarTextColor = Color.White;
            profile.IconImageSource = "tab_profile";
            Children.Add(profile);

            CurrentPage = Children[1];

        }
        async private void switchToSignIn()
        {
            NavigationPage loginScreen = new NavigationPage(new SignUp());
            loginScreen.BarTextColor = Color.MediumOrchid;
            loginScreen.BarBackgroundColor = Color.Black;
            loginScreen.Disappearing += (sender2, e2) => { loadViews(); };
            await Navigation.PushModalAsync(loginScreen);
        }
        public void OnLogout()
        {
            App.UserDatabase.DeleteUser(0);
            Debug.WriteLine("Deleted User Data");
            DependencyService.Get<IAuthentication>().Logout();
            switchToSignIn();
        }
        // returns 1 if user refresh is unsuccessful (i.e. expired or missing token)
        // returns 0 if user refresh is successful.
        public static int refreshUser()
        {
            User currentUser = App.UserDatabase.GetUser();
            if (currentUser == null) return 1;
            User updatedUser = DependencyService.Get<IAuthentication>().RefreshUserData(currentUser);
            App.UserDatabase.DeleteUser(0);
            if (updatedUser == null) return 1;
            App.UserDatabase.SaveUser(updatedUser);
            updatedUser.printUser();
            return 0;
        }
    }
}