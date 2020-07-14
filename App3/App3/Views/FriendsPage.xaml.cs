using App3.Data;
using App3.Models;
using App3.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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
        }

        private void FriendsPage_Appearing(object sender, EventArgs e)
        {
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
                page = new ProfilePage(friend);
                profileBuffer.Add(friend, page);
            }
            await Navigation.PushAsync(page);
            friendsListView.SelectedItem = null;
        }

        private async Task update()
        {
            List<User> list;
            try { 
                list = await App.GetFriendsAsync();
                if (list.Count < 1)
                {
                    noFriendsMsg.IsVisible = true;
                    friendsListView.ItemsSource = null;
                }
                else
                {
                    noFriendsMsg.IsVisible = false;
                    friendsListView.ItemsSource = null;
                    friendsListView.ItemsSource = list;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error getting friends: " + e);
            }
            
        }

        private async void refreshView_Refreshing(object sender, EventArgs e)
        {
            refreshView.IsRefreshing = true;
            await update();
            refreshView.IsRefreshing = false;
        }

        private void AddFriendButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(addFriendPage);
        }
    }
}