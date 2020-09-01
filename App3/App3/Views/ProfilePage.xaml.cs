using App3.Data;
using App3.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App3
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {
        User user { get; set; }
        public System.Windows.Input.ICommand ToolbarRightCommand { get; private set; }
        public string ToolbarRightSource { get; private set; }

        public System.Windows.Input.ICommand ToolbarLeftCommand { get; private set; }
        public string ToolbarLeftSource { get; private set; }
        public List<Party> partiesList;
        public List<Post> postsList;
        public ProfilePage(User userToDisplay)
        {

            NavigationPage.SetHasNavigationBar(this, false);
            user = userToDisplay;

            
            ToolbarLeftCommand = new Command(() =>
            {
                Navigation.PopAsync();
            });
            ToolbarLeftSource = "button_back";
            InitializeComponent();



            displayUser();


        }

        private void profileActionBtn_Clicked(object sender, EventArgs e)
        {

            switch (user.FriendStatus)
            {
                case 0:
                    addUserAsFriend(user.uid);

                    App.UserFriends.Add(user);
                    user.FriendStatus = 1;
                    break;
                case 1:
                    // Unadd
                    removeFriend(user.uid);

                    App.UserFriends.Remove(user);
                    user.FriendStatus = 0;
                    break;
                case 2:
                    addUserAsFriend(user.uid);

                    App.UserFriends.Add(user);
                    user.FriendStatus = 3;
                    break;
                case 3:
                    //Unadd
                    removeFriend(user.uid);

                    App.UserFriends.Remove(user);
                    user.FriendStatus = 2;
                    break;
                default:
                    break;
            }
            Debug.WriteLine(App.UserFriends.LastOrDefault());
        }
        private void addUserAsFriend(string uid)
        {
            if (uid != null)
            {
                if (FirebaseHelper.AddFriend(uid).Result) user.FriendStatus = 3;
                else user.FriendStatus = 1;
            }
        }
        private void removeFriend(string uid)
        {
            var currentUser = App.UserDatabase.GetUser();
            if (uid != null && currentUser != null && currentUser.uid != null && !uid.Equals(currentUser.uid))
            {
                if (!FirebaseHelper.RemoveFriend(uid).Result) Debug.WriteLine("Error");
            }
        }
        void refreshView_Refreshing(object sender, EventArgs e)
        {
            displayUser();
            postListView.IsRefreshing = false;

        }
        public async void displayUser()
        {
            if (user == null) return;
            user.updateProfileImageSource();
            BindingContext = user;
            var partiesList = await FirebaseHelper.GetPartiesThrownByUser(user);

            if (partiesList.Count > 0)
            {

                postListView.ItemsSource = partiesList;
            }


        }
        private async void postListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is Party party) await Navigation.PushModalAsync(new PartyDetailsPage(party));
            postListView.SelectedItem = null;
        }
    }
}