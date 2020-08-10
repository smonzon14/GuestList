using App3.Data;
using App3.Models;
using Plugin.Media;
using System;
using System.Collections.Generic;
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
        public double ToolbarOpacity { get; private set; } = 1.0;
        public ICommand DotsCommand { get; private set; }
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
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            update();

        }

        void update()
        {
            populateFeed();
            populateStories();
        }
        async void populateFeed()
        {
            List<Party> items = new List<Party>();
            
            foreach (var user in App.UserFriends)
            {
                //items.AddRange(await FirebaseHelper.GetPostsForUser(user.uid));
                items.AddRange(await FirebaseHelper.GetPartiesThrownByUser(user));
            }
            //var posts = await FirebaseHelper.GetPostsForUser(uid);
            //items.AddRange(posts);
            items.AddRange(App.UserParties);

            feedListView.ItemsSource = items;
            noInvitesMsg.IsVisible = items.Count == 0;
            

        }
        async void populateStories()
        {
            if (storiesCollectionView == null) return;
            //var stories = new List<Story>();
            var stories = await FirebaseHelper.GetUserStories(App.UserDatabase.GetUser().uid);
            storiesCollectionView.ItemsSource = stories;
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
        private void NewPostButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new NewPostPage());
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
        }

        private void MapButton_Tapped(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new NavigationPage(new HomePage()));
        }

        private void feedListView_Scrolled(object sender, ScrolledEventArgs e)
        {
            if (e.ScrollY <= 0) ToolbarOpacity = 1.0;
            else if (e.ScrollY > 100) ToolbarOpacity = 0.0;
            else ToolbarOpacity = 1.0 - e.ScrollY / 100;
            
        }

    }
}