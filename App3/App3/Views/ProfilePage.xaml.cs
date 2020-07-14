using App3.Data;
using App3.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.GoogleMaps;

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

            if (user == null)
            {
                ToolbarRightCommand = new Command(() =>
                {
                    Navigation.PushAsync(new SettingsPage());
                });
                ToolbarRightSource = "button_settings";
                user = App.UserDatabase.GetUser();
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

            if (user != null)
            {
                if (user.uid.Equals(App.UserDatabase.GetUser().uid)) friendButton.IsVisible = false;
                displayUser();
            }


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
            postListView.IsRefreshing = false;

        }
        public async void displayUser()
        {
            if (user == null) return;

            user.friendStatus = await FirebaseHelper.FriendStatus(App.UserDatabase.GetUser().uid, user.uid);
            BindingContext = user;
            partiesList = await FirebaseHelper.GetPartiesThrownByUser(user.uid);
            postsList = await FirebaseHelper.GetPostsForUser(user.uid);

            if (partiesList.Count > 0)
            {

                foreach (var p in partiesList) p.geoPosition = (await (new Geocoder()).GetPositionsForAddressAsync(p.address)).FirstOrDefault();
                map.generateMap(partiesList);

                partyCarousel.ItemsSource = partiesList;
                partyCarousel.CurrentItem = partiesList[0];
                partyCarousel.IsVisible = true;
                //noPartiesMsg.IsVisible = false;
            }
            else
            {
                partiesView.IsVisible = false;
                //noPartiesMsg.IsVisible = true;
            }

            if (postsList.Count > 0)
            {
                postListView.ItemsSource = postsList;
            }

        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var current = partyCarousel.CurrentItem as Party;
            Navigation.PushAsync(new PartyDetailsPage(current));
        }

        private void ShowPosts(object sender, EventArgs e)
        {
            Debug.WriteLine("Showing posts");
            showPostsButton.BackgroundColor = Color.FromHex("#F50058");
            showPartiesButton.BackgroundColor = Color.Transparent;
            partiesView.IsVisible = false;
            postListView.ItemsSource = postsList;

        }

        private void ShowParties(object sender, EventArgs e)
        {
            Debug.WriteLine("Showing parties");

            showPostsButton.BackgroundColor = Color.Transparent;
            showPartiesButton.BackgroundColor = Color.FromHex("#F50058");
            partiesView.IsVisible = true;
            postListView.ItemsSource = null;
        }
        private void CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
        {

            if (e == null) return;
            Party party = e.CurrentItem as Party;
            map.moveTo(party);

            //goingCount.Text = party.numPeopleGoing.ToString();
            //updateControls();


        }
    }
}