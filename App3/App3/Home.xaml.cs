using App3.Data;
using App3.Models;
using IdentityModel.OidcClient.Browser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MagicGradients;
using Xamarin.Forms.Maps;
using App3.Maps;
namespace App3
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Home : ContentPage
    {
        // Party object visualization template
        private TabbedPage1 parent;
        private PartyMap map;
        public Home(TabbedPage1 parent)
        {
            this.parent = parent;
            InitializeComponent();
            
            Debug.WriteLine("Updating Home");

            map = new PartyMap();
            map.HeightRequest = 300;
            map.IsShowingUser = true;
            
            //mainStack.Children.Insert(0, map);
            mainStack.Children.Add(map, 0, 1, 0, 3);
            mainStack.LowerChild(map);
            update();
            
        }
        public void refresh(object sender, EventArgs e)
        {
            Debug.WriteLine("Refreshing...");
            update();
            Debug.WriteLine("Done Refreshing.");
        }
        async public void hostButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Host(this.parent));
        }
        async public void OnSettingsButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Settings(this.parent));
        }
        private void generateMap(List<Party> pList)
        {
            int id = 0;
            foreach (Party p in pList)
            {
                int index = -1;
                // Update existing pin
                if((index = map.partyPins.FindIndex(existingPin=>existingPin.partyId==p.id)) > 0)
                {
                    map.partyPins[index].Position = p.geoPosition;
                    map.partyPins[index].Label = p.description;
                    map.partyPins[index].Address = p.address;
                    map.partyPins[index].Name = p.name;
                }
                else // Create new pin
                {
                    PartyPin pin = new PartyPin
                    {
                        partyId = id,
                        Type = PinType.Place,
                        Position = p.geoPosition,
                        Label = p.description,
                        Address = p.address,
                        Name = p.name,
                    };
                    map.Pins.Add(pin);
                    map.partyPins.Add(pin);
                }
                
                id++;
            }
            
            
            
            map.MoveToRegion(MapSpan.FromCenterAndRadius(map.Pins[0].Position, Distance.FromMiles(0.3)));
        }
        public async void update()
        {
            refreshView.IsRefreshing = true;
            if(TabbedPage1.refreshUser() == 1)
            {
                parent.OnLogout();
                return;
            }
            
            // TODO: Implement database retrieval of parties
            List<Party> partiesList = new List<Party>();
            Geocoder geoCoder = new Geocoder();
            List<string> testAddresses = new List<string>
            {
                "214 Lynn Fells Parkway, Melrose, MA",
                "143 Commonwealth Ave, Amherst, MA 01002",
                "153 Commonwealth Ave, Amherst, MA 01002",
                "151 Commonwealth Ave, Amherst, MA 01002"
            };
            for (var i = 0; i < 4; i++) {
                string address = testAddresses[i];
                IEnumerable<Position> approxLocation = await geoCoder.GetPositionsForAddressAsync(address);
                Position geoPos = approxLocation.FirstOrDefault();

                Party party = new Party()
                {
                    name = ("Party "+i),
                    description = "BYOB. 🥳 Ratio DNE. 🔥",
                    maxInvites = 100,
                    going = false,
                    peopleGoing = new List<Person>(),
                    address = address,
                    geoPosition = geoPos
                };
                
                partiesList.Add(party);
                
                //partyStackList.Children.Add(partyView);
            }
            generateMap(partiesList);
            
            partyListView.ItemsSource = partiesList;
            partyListView.ItemTemplate = Templates.PartyObjectUI();
            
            partyListView.ItemTapped -= PartyListView_ItemTappedAsync;
            partyListView.ItemTapped += PartyListView_ItemTappedAsync;
            
            refreshView.IsRefreshing = false;
        }

        private void PartyListView_ItemTappedAsync(object sender, ItemTappedEventArgs e)
        {
            Party selected = (Party)e.Item;
            Debug.WriteLine("Tapped party: " + selected.name);
            Position p = selected.geoPosition;
            
            MapSpan span = MapSpan.FromCenterAndRadius(p, Distance.FromMiles(0.3));
            
            map.MoveToRegion(span);
            //await Navigation.PushAsync(new PartyDetailsPage(selected));
            
            ((ListView)sender).SelectedItem = null;
            
            
        }
    }
}