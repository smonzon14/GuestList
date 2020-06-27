using App3.Data;
using App3.Models;
using Firebase.Storage;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App3.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FeedPage : ContentPage
    {
        public ICommand DotsCommand { get; private set; }
        public FeedPage()
        {
            DotsCommand = new Command(async (object item)=>
            {
                if(item is Post || item is Party)
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


        List<Story> getTestStories()
        {

            var list = new List<Story>();
            var post = new Story
            {
                uid = "123",
                name = "Sebastian Monzon"
            };

            for (int i = 0; i < 20; i++) list.Add(post);

            return list;
        }
        void update()
        {
            populateFeed();
            populateStories();
        }
        async void populateFeed()
        {
            string uid = App.UserDatabase.GetUser().uid;
            List<object> items = new List<object>();
            var posts = await FirebaseHelper.GetPostsForUser(uid);
            items.AddRange(posts);
            foreach (var user in App.UserFriends)
            {
                items.AddRange(await FirebaseHelper.GetPostsForUser(user.uid));
                items.AddRange(await FirebaseHelper.GetPartiesThrownByUser(user.uid));
            }
            var parties = App.UserParties;
            items.AddRange(parties);
            feedListView.ItemsSource = items;
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
            System.Diagnostics.Debug.WriteLine(e.Item.GetType());
            if (e.Item is Post post) await Navigation.PushAsync(new PostViewPage(post));
            else if (e.Item is Party party) await Navigation.PushAsync(new PartyDetailsPage(party));
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
                FirebaseHelper.PostStory(new Story {
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

    }
}