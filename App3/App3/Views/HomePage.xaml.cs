
using App3.Data;
using App3.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;
namespace App3
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        // Party object visualization template
        public System.Windows.Input.ICommand ToolbarRightCommand { get; private set; }
        public string ToolbarRightSource { get; private set; }
        private bool partyViewIsExpanded;
        string currentUserId;
        public HomePage()
        {
            ToolbarRightSource = "button_refresh";
            ToolbarRightCommand = new Command(async () =>
            {
                await App.GetInvitesAsync();
                update();
            });
            Debug.WriteLine("Loading Home Page");
            InitializeComponent();
            partyViewIsExpanded = false;

            Debug.WriteLine("Updating Home");

            //partyCarousel.ItemTemplate = Templates.PartyObjectUI();
            partyCarousel.CurrentItemChanged += CurrentItemChanged;


            //commentsListView.ItemTemplate = Templates.commentLayout();

            //peopleGoingListView.ItemTemplate = Templates.friendDescriptionLayout();
            //peopleGoingListView.ItemTapped += personItemTapped;

            NavigationPage.SetHasNavigationBar(this, false);

            update();

        }
        Party getCurrentParty()
        {

            return partyCarousel.CurrentItem as Party;
        }
        async public void hostButtonClicked(object sender, EventArgs e)
        {
            NavigationPage nav = new NavigationPage(new PartyHostPage());
            NavigationPage.SetHasNavigationBar(nav, false);
            await Navigation.PushModalAsync(nav);
        }
        async public void OnSettingsButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }

        public async void update()
        {


            if (App.UserInvites.Count > 0)
            {

                foreach (var p in App.UserInvites) p.geoPosition = (await (new Geocoder()).GetPositionsForAddressAsync(p.address)).FirstOrDefault();
                map.generateMap(App.UserInvites);

                partyCarousel.ItemsSource = App.UserInvites;
                partyCarousel.CurrentItem = App.UserInvites[0];
                partyCarousel.IsVisible = true;
                //noPartiesMsg.IsVisible = false;
            }
            else
            {
                partyCarousel.IsVisible = false;
                //noPartiesMsg.IsVisible = true;
            }

        }

        private void CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
        {
            if (!partyViewIsExpanded)
            {
                if (e == null) return;
                Party party = e.CurrentItem as Party;
                map.moveTo(party);

                //goingCount.Text = party.numPeopleGoing.ToString();
                //updateControls();
            }

        }

        private void SearchBar_Focused(object sender, FocusEventArgs e)
        {
            Animation a = new Animation();
            searchBar.IsVisible = true;
            controls.IsEnabled = false;
            a.Add(0, 1, new Animation(v => searchBar.Opacity = v, 0, 1.0));
            a.Add(0, 1, new Animation(v => controls.Opacity = v, 1.0, 0));
            a.Add(0, 1, new Animation(v => keyboardSpacing.Height = v, 0, Application.Current.MainPage.Height / 3.8, Easing.SinInOut));
            //a.Add(0, 1, new Animation(v => partyButtonsView.Opacity = v, 1.0, 0));
            //a.Add(0, 1, new Animation(v => bottomPaddingRow.Height = v, 0, 200));
            a.Commit(owner: searchBar, "showSearch", 50, finished: (x, y) => {
                controls.IsVisible = false;
            });


        }

        private void SearchBar_Unfocused(object sender, FocusEventArgs e)
        {

            //partyButtonsView.IsVisible = true;

            Animation a = new Animation();

            controls.IsVisible = true;
            a.Add(0, 1, new Animation(v => searchBar.Opacity = v, 1.0, 0));

            a.Add(0, 1, new Animation(v => controls.Opacity = v, 0, 1.0));
            a.Add(0, 1, new Animation(v => keyboardSpacing.Height = v, keyboardSpacing.Height.Value, 0, Easing.SinInOut));
            //a.Add(0, 1, new Animation(v => partyButtonsView.Opacity = v, 0, 1.0));
            //a.Add(0, 1, new Animation(v => bottomPaddingRow.Height = v, 200, 0));
            a.Commit(owner: searchBar, "showSearch", 50, finished: (x, y) => { 
                searchBar.IsVisible = false;
                controls.IsEnabled = true;
            });


        }
        double y;
        private void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
        {

            switch (e.StatusType)
            {
                case GestureStatus.Running:
                    partyCarousel.TranslationY = Math.Max(Math.Min(0, y + e.TotalY), -Math.Abs(partyCarousel.Height - Application.Current.MainPage.Height));
                    break;

                case GestureStatus.Completed:
                    // Store the translation applied during the pan
                    y = partyCarousel.TranslationY;
                    break;
            }
            Debug.WriteLine("Scroll: " + y.ToString());
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var current = getCurrentParty();
            Navigation.PushAsync(new PartyDetailsPage(current));
        }

        private void SearchButton_Clicked(object sender, EventArgs e)
        {
            searchBar.Focus();

        }

        private async void searchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as SearchBar;
            var text = entry.Text;
            if (text == null || text.Length == 0)
            {
                partyCarousel.ItemsSource = App.UserInvites;
                return;
            }
            await Task.Run(() => Thread.Sleep(500));
            if (text == entry.Text)
            {
                var invites = App.UserInvites.Where(invite => {
                    return invite.name.ToLower().Contains(text.ToLower());
                    });
                partyCarousel.ItemsSource = null;
                partyCarousel.ItemsSource = invites;
            }
        }
    }
}