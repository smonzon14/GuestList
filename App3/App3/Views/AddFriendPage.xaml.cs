using App3.Data;
using App3.Models;
using App3.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App3.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddFriendPage : ContentPage
    {
        CancellationTokenSource tokenSource = new CancellationTokenSource();
        CancellationToken ct;
        Dictionary<User, ProfilePage> profileBuffer = new Dictionary<User, ProfilePage>();
        public AddFriendPage()
        {

            InitializeComponent();
            ct = tokenSource.Token;
            //userList.ItemTemplate = Templates.friendDescriptionLayout();
            userList.ItemTapped += friendItemTapped;
        }
        internal class IntToBoolConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return (int)value != 0;
            }


            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return (bool)value ? 1 : 0;
            }

        }
        private async void friendItemTapped(object sender, ItemTappedEventArgs e)
        {
            var friend = e.Item as User;
            ProfilePage page;
            if (profileBuffer.ContainsKey(friend)) page = profileBuffer[friend];
            else
            {
                Debug.WriteLine("Moving to page: " + friend.name);
                page = new ProfilePage(friend);
                profileBuffer.Add(friend, page);
            }
            await Navigation.PushAsync(page);
            //userList.SelectedItem = null;
        }
        private async void FriendSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            var entry = sender as SearchBar;
            var text = entry.Text;
            if (text == null || text.Length == 0)
            {
                userList.ItemsSource = profileBuffer.Keys.ToList();
                return;
            }
            await Task.Run(() => Thread.Sleep(500));
            if (text == entry.Text)
            {
                Debug.WriteLine("Searching...");
                var users = await FirebaseHelper.FindUsersMatching(entry.Text);
                //foreach (User u in users) u.friendStatus = FirebaseHelper.FriendStatus(App.UserDatabase.GetUser().uid, u.uid).Result;
                
                userList.ItemsSource = users;
            }
        }
        private void btnAdd_Clicked(object sender, EventArgs e)
        {
            var button = (sender as Button);
            addUserAsFriend(button.CommandParameter.ToString());
            button.BackgroundColor = Color.MediumVioletRed;

            return;
        }
        private async Task<string> scanQRCode()
        {
            try
            {
                var scanner = DependencyService.Get<IQrScanningService>();
                var result = await scanner.ScanAsync();
                if (result != null) return result;
            }
            catch { }
            return null;
        }
        private void addUserAsFriend(string uid)
        {
            var currentUser = App.UserDatabase.GetUser();
            if (uid != null && currentUser != null && currentUser.uid != null && !uid.Equals(currentUser.uid))
            {
                if (!FirebaseHelper.AddFriend(currentUser.uid, uid).Result) Debug.WriteLine("Error");
            }


        }
        private async void btnScan_Clicked(object sender, EventArgs e)
        {
            string userid = await scanQRCode();
            if (userid == null) return;
            else
            {
                Console.WriteLine(userid);
                addUserAsFriend(userid);
            }
        }
    }
}