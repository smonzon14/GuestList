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
        private StackLayout partyStackList;
        public Home(TabbedPage1 parent)
        {
            this.parent = parent;
            InitializeComponent();
            
            Debug.WriteLine("Updating Home");

            map = new PartyMap();
            map.HeightRequest = 300;
            map.IsShowingUser = true;
            mainStack.Children.Insert(0, map);

            partyStackList = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children = { }
            };
            var scrollView = new ScrollView
            {
                HorizontalOptions = LayoutOptions.Fill,
                Orientation = ScrollOrientation.Horizontal,

                Content = partyStackList
            };
            
            mainStack.Children.Add(scrollView);
            update();
            
        }
        public void refresh(object sender, EventArgs e)
        {
            Debug.WriteLine("Refreshing...");
            update();
            Debug.WriteLine("Done Refreshing.");
        }
        async public void OnSettingsButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Settings(this.parent));
        }
        private void generateMap(List<Party> pList)
        {
            
            foreach (Party p in pList)
            {
                PartyPin pin = new PartyPin
                {
                    Type = PinType.Place,
                    Position = p.geoPosition,
                    Label = p.description,
                    Address = p.address,
                    Name = p.name,
                    Url = "http://xamarin.com/about/"
                };
                map.Pins.Add(pin);
                map.partyPins.Add(pin);
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
            for (var i = 0; i < 1; i++) {
                string address = "214 Lynn Fells Parkway, Melrose, MA";
                IEnumerable<Position> approxLocation = await geoCoder.GetPositionsForAddressAsync(address);
                Position geoPos = approxLocation.FirstOrDefault();

                Party party = new Party()
                {
                    name = ("Party "+i),
                    description = "BYOB. Ratio DNE. 🔥",
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
            MapSpan span = MapSpan.FromCenterAndRadius(selected.geoPosition, Distance.FromMiles(0.2));
            map.MoveToRegion(span);
            //await Navigation.PushAsync(new PartyDetailsPage(selected));
            
            ((ListView)sender).SelectedItem = null;
            
            
        }
    }
}