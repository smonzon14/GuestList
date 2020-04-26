using App3.Maps;
using App3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace App3
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PartyDetailsPage : ContentPage
    {

        public PartyDetailsPage(Party party)
        {
            InitializeComponent();
            PartyMap mapView = new PartyMap()
            {
                MapType = MapType.Street
            };
            Content = mapView;
            PartyPin pin = new PartyPin
            {
                Type = PinType.Place,
                Position = new Position(37.79752, -122.40183),
                Label = "Xamarin San Francisco Office",
                Address = "394 Pacific Ave, San Francisco CA",
                Name = "Xamarin",
                Url = "http://xamarin.com/about/"
            };
            mapView.partyPins = new List<PartyPin> { pin };
            mapView.Pins.Add(pin);
            mapView.MoveToRegion(MapSpan.FromCenterAndRadius(pin.Position, Distance.FromMiles(0.3)));
            mapView.HasScrollEnabled = false;
        }
    }
    
}