using App3.Models;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using App3.Data;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using System.Globalization;
using Google.Rpc;

namespace App3
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {
        MasterTabbedPage parent;
        User user { get; set; }
        public System.Windows.Input.ICommand ToolbarRightCommand { get; private set; }
        public string ToolbarRightSource { get; private set; }

        public System.Windows.Input.ICommand ToolbarLeftCommand { get; private set; }
        public string ToolbarLeftSource { get; private set; }
        public ProfilePage(User userToDisplay, MasterTabbedPage parent)
        {
            this.parent = parent;

            NavigationPage.SetHasNavigationBar(this, false);
            user = userToDisplay;

            if (user == null)
            {
                ToolbarRightCommand = new Command(() =>
                {
                    Navigation.PushAsync(new SettingsPage(this.parent));
                });
                ToolbarRightSource = "button_settings";
                user = App.UserDatabase.GetUser();
                if (user == null && parent != null)
                {
                    parent.OnLogout();
                    return;
                }
            }
            else
            {
                ToolbarLeftCommand = new Command(() =>
                {
                    Navigation.PopAsync();
                });
                ToolbarLeftSource = "button_back";
            }

            InitializeComponent();

            refreshView.IsRefreshing = true;
            if (user != null)
            {
                if (user.uid.Equals(App.UserDatabase.GetUser().uid)) friendButton.IsVisible = false;
                displayUser();
            }
            
            refreshView.IsRefreshing = false;

        }

        private void profileActionBtn_Clicked(object sender, EventArgs e)
        {
            
            switch (user.friendStatus)
            {
                case 0:
                    addUserAsFriend(user.uid);

                    App.UserFriends.Add(user);
                    user.friendStatus = 1;
                    break;
                case 1:
                    // Unadd
                    removeFriend(user.uid);

                    App.UserFriends.Remove(user);
                    user.friendStatus = 0;
                    break;
                case 2:
                    addUserAsFriend(user.uid);

                    App.UserFriends.Add(user);
                    user.friendStatus = 3;
                    break;
                case 3:
                    //Unadd
                    removeFriend(user.uid);

                    App.UserFriends.Remove(user);
                    user.friendStatus = 2;
                    break;
                default:
                    break;
            }
            Debug.WriteLine(App.UserFriends.LastOrDefault());
            BindingContext = null;
            BindingContext = user;
        }
        private void addUserAsFriend(string uid)
        {
            var currentUser = App.UserDatabase.GetUser();
            Debug.WriteLine(uid + " " + currentUser.uid);
            if (uid != null && currentUser != null && currentUser.uid != null && !uid.Equals(currentUser.uid))
            {
                if (FirebaseHelper.AddFriend(currentUser.uid, uid).Result) user.friendStatus = 3;
                else user.friendStatus = 1;
            }
        }
        private void removeFriend(string uid)
        {
            var currentUser = App.UserDatabase.GetUser();
            if (uid != null && currentUser != null && currentUser.uid != null && !uid.Equals(currentUser.uid))
            {
                if (!FirebaseHelper.RemoveFriend(currentUser.uid, uid).Result) Debug.WriteLine("Error");
            }
        }
        void refreshView_Refreshing(object sender, EventArgs e)
        {
            displayUser();
            refreshView.IsRefreshing = false;

        }
        public async void displayUser()
        {
            if (user == null) return;

            user.friendStatus = await FirebaseHelper.FriendStatus(App.UserDatabase.GetUser().uid, user.uid);
            BindingContext = user;
            List<Party> partiesList = await FirebaseHelper.GetPartiesThrownByUser(user.uid);
            if (partiesList.Count == 0) noPartiesMsg.IsVisible = true;
            else noPartiesMsg.IsVisible = false;
            partyView.ItemsSource = partiesList;

        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var current = partyView.CurrentItem as Party;
            Navigation.PushAsync(new PartyDetailsPage(current));
        }
    }
}