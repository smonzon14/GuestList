using App3.Data;
using App3.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
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

        private List<Comment> comments;
        public PartyDetailsPage(Party party)
        {
            comments = new List<Comment>();
            
            BindingContext = party;
            ToolbarLeftSource = "button_back";
            ToolbarLeftCommand = new Command(async () =>
            {
                await Navigation.PopModalAsync();
            });
            InitializeComponent();
            
            updateMap();
            _ = populateComments();
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
            var party = await FirebaseHelper.GetParty((BindingContext as Party).pid, (BindingContext as Party).Thrower);
            BindingContext = null;
            BindingContext = party;
            await populateComments();
            partyDetailsListView.IsRefreshing = false;
        }
        private async Task populateComments()
        {
            comments = await FirebaseHelper.GetCommentsForParty((BindingContext as Party).pid);
            partyDetailsListView.ItemsSource = comments;
        }
        private void Button_Clicked(object sender, System.EventArgs e)
        {

        }

        private async void SendCommentButtonClicked(object sender, System.EventArgs e)
        {
            if (message.Text == null || message.Text.Length < 1) return;
            var user = App.UserDatabase.GetUser();
            var comment = new Comment
            {
                message = message.Text,
                uid = user.uid,
                name = user.name,
                likes = 0
            };
            comment = await FirebaseHelper.AddCommentToParty(comment, (BindingContext as Party).pid);
            if (comment == null)
            {
                await DisplayAlert("Could not post.", "", "OK");
            }
            comments.Insert(0, comment);
            partyDetailsListView.ItemsSource = null;
            partyDetailsListView.ItemsSource = comments;


            await progressBar.ProgressTo(1.0, 250, Easing.SinInOut);

            await Task.Delay(200);
            progressBar.Progress = 0;
        }

        private void message_Focused(object sender, FocusEventArgs e)
        {
            partyDetailsListView.ScrollTo(partyDetailsListView.Footer, ScrollToPosition.End, true);
        }

        private void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new ProfilePage((BindingContext as Party).Thrower));
        }
    }

}