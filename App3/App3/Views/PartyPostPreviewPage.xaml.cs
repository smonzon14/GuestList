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
    public partial class PartyPostPreviewPage : ContentPage
    {
        private PartyMap map;
        public PartyPostPreviewPage(Party party)
        {
            InitializeComponent();

            var previewPartyList = new List<Party> { party };

            map = new PartyMap();
            map.IsShowingUser = true;

            partyListView.ItemTemplate = Templates.PartyObjectUI();
            partyListView.ItemsSource = previewPartyList;

            mainGrid.Children.Add(map, 0, 1, 0, mainGrid.RowDefinitions.Count);
            mainGrid.LowerChild(map);
            map.generateMap(previewPartyList);
            MapSpan span = MapSpan.FromCenterAndRadius(party.geoPosition, Distance.FromMiles(0.3));
            map.MoveToRegion(span);
        }
        public async void postButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}