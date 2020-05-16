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
    public partial class ProfilePage : ContentPage
    {
        MasterTabbedPage parent;
        Person currentUser;
        public ProfilePage(Person user, MasterTabbedPage parent)
        {
            this.parent = parent;
            InitializeComponent();
            if (user != null) displayUser(user);
            
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if(this.currentUser == null)
            {
                User user = App.UserDatabase.GetUser();
                if (user == null) parent.OnLogout();
                else
                {
                    displayUser(user);
                }
            }
        }
        public void displayUser(Person user)
        {
            if (this.currentUser != null) return;
            this.currentUser = user;
            var profileImage = new Frame
            {
                WidthRequest = 200,
                HeightRequest = 200,
                CornerRadius = 100,
                HorizontalOptions = LayoutOptions.Center,
                Padding = 0,
                IsClippedToBounds = true,
                Content = new Image
                {
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Source = "Profile"
                }
            };
            profileStack.Children.Add(profileImage);

            profileStack.Children.Add(new Label
            {
                HorizontalTextAlignment = TextAlignment.Center,
                Text = user.name,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.White,
                FontSize = 30

            });

            var statusString = "Offline";
            switch (user.status)
            {
                case 1:
                    statusString = "Going Out";
                    break;
                case 2:
                    statusString = "Staying In";
                    break;
                default:
                    statusString = "Offline";
                    break;
            }

            profileStack.Children.Add(new Label
            {
                FontSize = 20,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Color.White,
                Text = "Status: " + statusString

            });

            profileStack.Children.Add(new Label
            {
                FontSize = 20,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Color.White,
                Text = "Bio: " + user.bio
            });

            profileStack.Children.Add(new Label
            {
                Padding = 20,
                FontSize = 30,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Color.White,
                Text = "My Parties",
                FontAttributes = FontAttributes.Bold

            });
            var partyListView = new CarouselView
            {
                BackgroundColor = Color.Transparent,
                PeekAreaInsets = 50
            };

            profileStack.Children.Add(partyListView);
            List<Party> partiesList = new List<Party>();
            for (var i = 0; i < 10; i++) partiesList.Add(new Party() { name = "past party", description = "party of mine"});




            partyListView.ItemsSource = partiesList;
            partyListView.ItemTemplate = Templates.PartyObjectUI();

            profileStack.Children.Add(partyListView);
        }
        private void PartyListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            throw new NotImplementedException();
        }

        async void OnSettingsButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage(this.parent));
           
        }
    }
}