using App3.Models;
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
    public partial class Friends : ContentPage
    {
        public Friends()
        {
            InitializeComponent();
            friendsListView.ItemTemplate = Templates.friendDescriptionLayout();
            update();
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