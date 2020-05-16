using App3.Maps;
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
        private MasterTabbedPage parent;
        private PartyMap map;
        private bool partyViewIsExpanded;
        private Person currentUser;
        public HomePage(Person user, MasterTabbedPage parent)
        {
            InitializeComponent();
            this.parent = parent;
            this.currentUser = user;
            partyViewIsExpanded = false;
            
            Appearing += appearing;
            Debug.WriteLine("Updating Home");

            map = new PartyMap();
            map.IsShowingUser = true;


            partyListView.ItemTemplate = Templates.PartyObjectUI();
            partyListView.CurrentItemChanged += CurrentItemChanged;
            
            //mainStack.Children.Insert(0, map);
            
            mainGrid.Children.Add(map, 0, mainGrid.ColumnDefinitions.Count, 0, mainGrid.RowDefinitions.Count);
            mainGrid.LowerChild(map);

            commentsListView.ItemTemplate = Templates.commentLayout();

            NavigationPage.SetHasNavigationBar(this, false);
            
            update();

        }
        private void appearing(object sender, EventArgs e)
        {
            update();
        }
        Party getCurrentParty()
        {
            return partyListView.CurrentItem as Party;
        }
        void OnUpvoteButtonClicked(object sender, EventArgs e)
        {
            Party currentParty = getCurrentParty();

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
            

            peopleGoing.IsVisible = true;
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
            commentsListView.ItemsSource = currentParty.comments;
            
            //TODO: load comments
        }
        public void closeCommentsVew(object sender, EventArgs e)
        {
            commentsView.IsVisible = false;
        }
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
            ContentPage page = new PartyHostPage();
            page.ToolbarItems.Add(new ToolbarItem
             {
                 Text = "Cancel",
                 Command = new Command(() => Navigation.PopModalAsync())
             });
            NavigationPage nav = new NavigationPage(page);
            nav.BarBackgroundColor = Color.BlueViolet;
            await Navigation.PushModalAsync(nav);
        }
        async public void OnSettingsButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage(this.parent));
        }
        
        private async Task<List<Party>> testPartyList()
        {
            List<Party> list = new List<Party>();
            Geocoder geoCoder = new Geocoder();
            List<string> testAddresses = new List<string>
            {
                "214 Lynn Fells Parkway, Melrose, MA",
                "143 Commonwealth Ave, Amherst, MA 01002",
                "153 Commonwealth Ave, Amherst, MA 01002",
                "151 Commonwealth Ave, Amherst, MA 01002"
            };
            List<Comment> comments = new List<Comment>();
            comments.Add(new Comment
            {
                profileImage = new Image { Source = "Profile" },
                username = "Username",
                comment = "this party blows",
                likes = 20,
                datePosted = DateTime.Now
            });
            for (var i = 0; i < 4; i++)
            {
                string address = testAddresses[i];
                IEnumerable<Position> approxLocation = await geoCoder.GetPositionsForAddressAsync(address);
                Position geoPos = approxLocation.FirstOrDefault();

                Party party = new Party()
                {
                    id = i,
                    name = ("Party " + i),
                    description = "BYOB. 🥳 Ratio DNE. 🔥",
                    going = false,
                    peopleGoing = new List<Person>(),
                    numPeopleGoing = 0,
                    address = address,
                    geoPosition = geoPos,
                    comments = comments
                };
                list.Add(party);
            }
            return list;
        }
        public async void update()
        {

            if (MasterTabbedPage.refreshUser() == 1)
            {
                parent.OnLogout();
                return;
            }

            // TODO: Implement database retrieval of parties

            List<Party> partiesList = await testPartyList();

            map.generateMap(partiesList);

            partyListView.ItemsSource = partiesList;
            partyListView.CurrentItem = partiesList[0];
        }
        private void PartyTapped(object s, EventArgs e)
        {
            e.ToString();
        }
        private void CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
        {

            if (!partyViewIsExpanded)
            {
                Party party = e.CurrentItem as Party;
                MapSpan span = MapSpan.FromCenterAndRadius(party.geoPosition, Distance.FromMiles(0.3));
                map.MoveToRegion(span);
                map.RaiseCallToNativeMethod(map.partyPins.Find(x => x.partyId == party.id));
            }

        }
    }
}