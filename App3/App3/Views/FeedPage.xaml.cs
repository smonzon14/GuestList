using App3.Data;
using App3.Models;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App3.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FeedPage : ContentPage
    {
        
        public ICommand DotsCommand { get; private set; }
        public ICommand ToolbarRightCommand { get; private set; }
        public string ToolbarRightSource { get; private set; } = "button_settings";
        ObservableCollection<Party> invites = new ObservableCollection<Party>();
        
        public FeedPage()
        {
            DotsCommand = new Command(async (object item) =>
            {
                if (item is Post || item is Party)
                {
                    string[] options = { "Share", "Go to Profile", "Go to Comments" };
                    var resp = await DisplayActionSheet(null, "Cancel", "Report", options);

                    if (resp.Equals("Cancel")) return;
                    if (resp.Equals("Report"))
                    {
                        //Report
                    }

                    int i;
                    for (i = 0; i < options.Length; i++) if (options[i].Equals(resp)) break;
                    switch (i)
                    {
                        case 0:
                            //Share
                            break;
                        case 1:
                            //Go to profile
                            break;
                        case 2:
                            //Go to comments
                            break;
                    }
                }
            });
            NavigationPage.SetHasNavigationBar(this, false);
            ToolbarRightCommand = new Command(() =>
            {
                Navigation.PushAsync(new SettingsPage());
            });
            InitializeComponent();
            update();

        }

        void update()
        {

            feedListView.ItemsSource = invites;
            populateFeed();
            //populateStories();
        }
        private void mergeInvites(List<Party> parties)
        {

            //0: 4/15
            //1: 4/16
            if (parties.Count == 0) return;
            if(invites.Count > 0)
            {
                
                
                for (int x = invites.Count-1; x > 0; x--)
                {
                    if (parties[0].posted.CompareTo(invites[x].posted) > 0)
                    {
                        invites.Insert(x, parties[0]);
                        parties.RemoveAt(0);
                        x++;
                        if (parties.Count == 0) break;
                    }
                }
                
            }
            else
            {
                while (parties.Count != 0)
                {
                    invites.Add(parties[parties.Count - 1]);
                    parties.RemoveAt(parties.Count - 1);
                }
            }
            
            

        }
        async void populateFeed()
        {
            invites.Clear();

            mergeInvites(await App.GetPartiesAsync());
            mergeInvites(await App.GetInvitesAsync());

            noInvitesMsg.IsVisible = invites.Count == 0;
            
        }
        

        private void RefreshView_Refreshing(object sender, EventArgs e)
        {
            update();
            feedListView.IsRefreshing = false;
        }

        private async void feedListView_ItemSelected(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is Post post) await Navigation.PushAsync(new PostViewPage(post));
            else if (e.Item is Party party) await Navigation.PushModalAsync(new PartyDetailsPage(party));
            feedListView.SelectedItem = null;
        }


        /*async void populateStories()
        {
            if (storiesCollectionView == null) return;
            //var stories = new List<Story>();
            var stories = await FirebaseHelper.GetUserStories(App.UserDatabase.GetUser().uid);
            storiesCollectionView.ItemsSource = stories;
        }
        private void storiesCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine("Hello");
            var story = (e.CurrentSelection.FirstOrDefault() as Story);

        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            try
            {
                var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
                });
                if (file == null)
                    return;
                userStory.Source = ImageSource.FromStream(() => { return file.GetStream(); });
                FirebaseHelper.PostStory(new Story
                {
                    media = file.GetStream(),
                    location = Xamarin.Essentials.Geolocation.GetLastKnownLocationAsync().ToString(),
                    uid = App.UserDatabase.GetUser().uid
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }*/

        private void MapButton_Tapped(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new NavigationPage(new HomePage()) { BarTextColor = Color.White });
        }

        private double toolbarOpacity = 1.0;
        public double ToolbarOpacity { get { return toolbarOpacity; } private set { toolbarOpacity = value; } }

        private void feedListView_Scrolled(object sender, ScrolledEventArgs e)
        {
            if (e.ScrollY <= 0) toolbarOpacity = 0.0;
            else if (e.ScrollY > 60) toolbarOpacity = 1.0;
            else toolbarOpacity = e.ScrollY / 60;
            OnPropertyChanged("ToolbarOpacity");
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new PartyHostPage());
        }
    }
}