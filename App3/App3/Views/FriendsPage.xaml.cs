using App3.Models;
using App3.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App3.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;
using App3.Views;

namespace App3
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FriendsPage : ContentPage
    {

        public System.Windows.Input.ICommand ToolbarRightCommand { get; private set; }
        public string ToolbarRightSource { get; private set; }
        static Dictionary<User, ProfilePage> profileBuffer = new Dictionary<User, ProfilePage>();
        static AddFriendPage addFriendPage = new AddFriendPage();
        public FriendsPage()
        {
            ToolbarRightCommand = new Command(() =>
            {
                AddFriendButton_Clicked(null, null);
            });
            Appearing += FriendsPage_Appearing;
            ToolbarRightSource = "button_addfriend";
            InitializeComponent();
            //friendsListView.ItemTemplate = Templates.friendDescriptionLayout();
            NavigationPage.SetHasNavigationBar(this, false);
            update();
        }

        private void FriendsPage_Appearing(object sender, EventArgs e)
        {
            Debug.WriteLine("wtf");
            friendsListView.ItemsSource = null;
            friendsListView.ItemsSource = App.UserFriends;
            if (App.UserFriends.Count > 0) noFriendsMsg.IsVisible = false;
            else noFriendsMsg.IsVisible = true;
        }

        private async void friendItemTapped(object sender, ItemTappedEventArgs e)
        {
            
            var friend = e.Item as User;
            ProfilePage page;
            if (profileBuffer.ContainsKey(friend)) page = profileBuffer[friend];
            else
            {
                page = new ProfilePage(friend, null);
                profileBuffer.Add(friend, page);
            }
            await Navigation.PushAsync(page);
            friendsListView.SelectedItem = null;
        }
        
        private void update()
        {
            var friendsList = new List<User>();
            try
            {
                var uid = App.UserDatabase.GetUser().uid;
                friendsList = FirebaseHelper.GetFriendsList(uid).Result;
                friendsListView.ItemsSource = friendsList;//testPartyList();
                App.UserFriends.Clear();
                App.UserFriends.AddRange(friendsList);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error getting friends: " + e);
            }
            finally
            {
                App.UserFriends.Clear();
                App.UserFriends.AddRange(friendsList);
                if (friendsList.Count < 1)
                {
                    //Implement no friends message :(
                    noFriendsMsg.IsVisible = true;
                }
                else
                {
                    noFriendsMsg.IsVisible = false;
                }
            }
            
        }

        private void refreshView_Refreshing(object sender, EventArgs e)
        {
            refreshView.IsRefreshing = true;
            update();
            refreshView.IsRefreshing = false;
        }

        private void AddFriendButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(addFriendPage);
        }
    }
}