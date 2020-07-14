using App3.Data;
using App3.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace App3
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PartyPostPreviewPage : ContentPage
    {
        private PartyMap map;
        private Party previewParty;
        public PartyPostPreviewPage(Party party)
        {
            InitializeComponent();

            previewParty = party;
            previewParty.pid = "-1";
            var previewPartyList = new List<Party> { previewParty };

            map = new PartyMap();
            map.IsShowingUser = true;

            partyListView.ItemsSource = previewPartyList;

            mainGrid.Children.Add(map, 0, 1, 0, mainGrid.RowDefinitions.Count);
            mainGrid.LowerChild(map);
            map.generateMap(previewPartyList);
            MapSpan span = MapSpan.FromCenterAndRadius(previewParty.geoPosition, Distance.FromMiles(0.3));
            map.MoveToRegion(span);

        }
        private async Task<bool> postParty(Party party)
        {
            try
            {
                return await FirebaseHelper.CreateParty(party, App.UserDatabase.GetUser().uid, 0);
            }
            catch
            {
                await DisplayAlert("Error", "Could not post.", "Ok");
                return false;
            }
        }
        public async void postButtonClicked(object sender, EventArgs e)
        {
            bool success = await postParty(previewParty);
            if (success)
            {
                await Navigation.PopModalAsync();
            }
            else
            {
                await DisplayAlert("Couldn't post", null, "Ok");
            }

        }
    }
}