
using App3.Data;
using App3.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private MasterTabbedPage parent;
        private bool partyViewIsExpanded;
        string currentUserId;
        public HomePage(MasterTabbedPage parent)
        {
            ToolbarRightSource = "button_refresh";
            ToolbarRightCommand = new Command(() =>
            {
                HamburgerButton_Clicked(null, null);
            });
            Debug.WriteLine("Loading Home Page");
            InitializeComponent();
            this.parent = parent;
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
        /*
        private async void personItemTapped(object sender, ItemTappedEventArgs e)
        {
            var friend = e.Item as User;
            await Navigation.PushAsync(new ProfilePage(friend, this.parent));
            peopleGoingListView.SelectedItem = null;
        }
        private void appearing(object sender, EventArgs e)
        {
            //update();
        }*/
        /*
        void OnUpvoteButtonClicked(object sender, EventArgs e)
        {
            Party currentParty = getCurrentParty();

            if (currentParty == null) return;
            if (currentParty.going)
            {
                //currentParty.peopleGoing.Remove(currentUser);
                currentParty.going = false;
                currentParty.numPeopleGoing--;
            }
            else
            {
                //currentParty.peopleGoing.Add(currentUser);
                currentParty.going = true;
                currentParty.numPeopleGoing++;
            }
            updateControls();

        }
        void updateControls()
        {
            Party currentParty = getCurrentParty();

            if (currentParty == null) return;
            if (currentParty.going)
            {
                upButton.BackgroundColor = Color.White;
                goingCount.TextColor = Color.White;
            }
            else
            {
                upButton.BackgroundColor = Color.Magenta;
                goingCount.TextColor = Color.Magenta;
            }

            goingCount.Text = currentParty.numPeopleGoing.ToString();
        }
        async void OnShareButtonClickedAsync(object sender, EventArgs e)
        {
            List<string> options = new List<string> { "Invite", "Message", "Snapchat" };
            string action = await DisplayActionSheet("Share", "Cancel", null, options[0], options[1], options[2]);
            if (action.Equals(options[0]))
            {
                
            }
            else if (action.Equals(options[1]))
            {
                
            }
            else if (action.Equals(options[2]))
            {
                
            }
        }
        async void OnGoButtonClickedAsync(object sender, EventArgs e)
        {
            List<string> options = new List<string> { "Uber", "Lyft", "Maps" };
            string action = await DisplayActionSheet("Directions: Method.", "Cancel", null, options[0],options[1],options[2]);
            if (action.Equals(options[0]))
            {
                //Open address in uber
            }else if (action.Equals(options[1]))
            {
                //open address in lyft
            }else if (action.Equals(options[2]))
            {
                //open address in maps
            }
        }
        void openPeopleGoingView(object sender, EventArgs e)
        {
            Party currentParty = getCurrentParty();

            peopleGoing.IsVisible = true;
            if(currentParty != null)
            {
                peopleGoingListView.ItemsSource = currentParty.peopleGoing;
            }
            
            
            //TODO: load people
        }
        void closePeopleGoingVew(object sender, EventArgs e)
        {
            peopleGoing.IsVisible = false;
        }
        void openCommentsView(object sender, EventArgs e)
        {
            commentsView.IsVisible = true;
            Party currentParty = getCurrentParty();
            Console.WriteLine(currentParty);
            //commentsListView.ItemsSource = currentParty.comments;
            
            //TODO: load comments
        }
        public void closeCommentsVew(object sender, EventArgs e)
        {
            commentsView.IsVisible = false;
        }
        */
        public void refresh(object sender, EventArgs e)
        {
            Debug.WriteLine("Refreshing...");
            //refreshView.IsRefreshing = true;
            update();
            //refreshView.IsRefreshing = false;
            Debug.WriteLine("Done Refreshing.");
        }
        async public void hostButtonClicked(object sender, EventArgs e)
        {
            NavigationPage nav = new NavigationPage(new PartyHostPage());
            NavigationPage.SetHasNavigationBar(nav, false);
            await Navigation.PushModalAsync(nav);
        }
        async public void OnSettingsButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage(this.parent));
        }
        
        public async void update()
        {

            if (MasterTabbedPage.refreshUser() == 1)
            {
                parent.OnLogout();
                return;
            }
            currentUserId = App.UserDatabase.GetUser().uid;
            // TODO: Implement database retrieval of parties

            List<Party> partiesList = new List<Party>(); //await FirebaseHelper.GetInvitedParties(currentUserId);
            foreach(var user in App.UserFriends)
            {
                partiesList.AddRange(await FirebaseHelper.GetPartiesThrownByUser(user.uid));
            }
            partiesList.AddRange(await FirebaseHelper.GetPartiesThrownByUser(currentUserId));

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

        private void HamburgerButton_Clicked(object sender, EventArgs e)
        {
            update();
        }

        private void SearchBar_Focused(object sender, FocusEventArgs e)
        {
            Animation a = new Animation();
            searchBar.IsVisible = true;
            a.Add(0, 1, new Animation(v => searchBar.Opacity = v, 0, 1.0));
            //a.Add(0, 1, new Animation(v => partyButtonsView.Opacity = v, 1.0, 0));
            //a.Add(0, 1, new Animation(v => bottomPaddingRow.Height = v, 0, 200));
            a.Commit(owner: searchBar, "showSearch", 50, finished: (x, y) => { });
            
            
        }

        private void SearchBar_Unfocused(object sender, FocusEventArgs e)
        {

            //partyButtonsView.IsVisible = true;
            
            Animation a = new Animation();
            a.Add(0, 1, new Animation(v => searchBar.Opacity = v, 1.0, 0));
            //a.Add(0, 1, new Animation(v => partyButtonsView.Opacity = v, 0, 1.0));
            //a.Add(0, 1, new Animation(v => bottomPaddingRow.Height = v, 200, 0));
            a.Commit(owner: searchBar, "showSearch", 50, finished: (x, y) => { searchBar.IsVisible = false; });
            
            
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
    }
}