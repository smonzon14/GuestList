using App3.Models;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace App3
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PartyDetailsPage : ContentPage
    {
        public System.Windows.Input.ICommand ToolbarLeftCommand { get; private set; }
        public string ToolbarLeftSource { get; private set; }
        
        public PartyDetailsPage(Party party)
        {
            BindingContext = party;
            ToolbarLeftSource = "button_back";
            ToolbarLeftCommand = new Command(async () =>
            {
                await Navigation.PopModalAsync();
            });
            InitializeComponent();
            updateMap();
        }

        async void updateMap()
        {
            var party = (BindingContext as Party);
            var locations = await new Geocoder().GetPositionsForAddressAsync(party.address);
            party.geoPosition = (Position)locations?.FirstOrDefault();
            
            map.moveTo(party);
        }

        private async void partyDetailsListView_Refreshing(object sender, System.EventArgs e)
        {
            var party = BindingContext as Party;
            BindingContext = null;
            BindingContext = await Data.FirebaseHelper.GetParty(party.pid, party.Thrower);
            partyDetailsListView.IsRefreshing = false;
        }
        private List<Post> getPostsForParty()
        {
            var list = new List<Post>();
            list.Add(new Post
            {
                message = "This is a test Post. Also testing multiline text wrap.",
                name = "Diego El Fuego",
                likes = 100
            });
            list.Add(new Post
            {
                message = "This is a another test Post.",
                name = "Father John",
                likes = 10
            });
            return list;
        }
        private void populatePosts()
        {

            partyDetailsListView.ItemsSource = getPostsForParty();
        }
        private void Button_Clicked(object sender, System.EventArgs e)
        {

        }
    }

}