using App3.Models;
using App3.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App3
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FriendsPage : ContentPage
    {
        MasterTabbedPage parent;
        public FriendsPage(MasterTabbedPage parent)
        {
            this.parent = parent;
            InitializeComponent();
            friendsListView.ItemTemplate = Templates.friendDescriptionLayout();
            friendsListView.ItemTapped += friendItemTapped;
            NavigationPage.SetHasNavigationBar(this, false);
            update();
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
        private async void btnScan_Clicked(object sender, EventArgs e)
        {
            string userid = await scanQRCode();
            if (userid == null) await DisplayAlert("User Not Found", "", "Ok");
            else
            {
                Console.WriteLine(userid);
            }
        }
        private async void friendItemTapped(object sender, ItemTappedEventArgs e)
        {
            var friend = e.Item as Friend;
            await Navigation.PushAsync(new ProfilePage(friend, this.parent));
            friendsListView.SelectedItem = null;
        }
        private List<Friend> testPartyList()
        {
            List<Friend> list = new List<Friend>();
            List<string> testNames = new List<string>
            {
                "Friend 0",
                "First Last",
                "Hello World",
                "John Doe"
            };
            for (var i = 0; i < 4; i++)
            {
                Image img = new Image
                {
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Source = "Profile"
                };
                Friend friend = new Friend
                {
                    id = i,
                    bio = "what about me?",
                    status = (new Random()).Next(2),
                    name = testNames[i],
                    image = img
                    
                };
                list.Add(friend);
            }
            return list;
        }
        private void update()
        {
            friendsListView.ItemsSource = testPartyList();
        }

        private void refreshView_Refreshing(object sender, EventArgs e)
        {
            refreshView.IsRefreshing = true;
            update();
            refreshView.IsRefreshing = false;
        }
    }
}