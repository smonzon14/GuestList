using App3.Data;
using App3.Models;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App3.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CurrentUserProfilePage : ContentPage
    {
        public System.Windows.Input.ICommand ToolbarRightCommand { get; private set; } 
        public string ToolbarRightSource { get; private set; } = "button_settings";

        public CurrentUserProfilePage()
        {

            NavigationPage.SetHasNavigationBar(this, false);
            ToolbarRightCommand = new Command(() =>
            {
                Navigation.PushAsync(new SettingsPage());
            });
            InitializeComponent();
            displayUser();
        }

        void refreshView_Refreshing(object sender, EventArgs e)
        {
            //await App.GetInvitesAsync();
            
            
            displayUser();
            postListView.IsRefreshing = false;

        }
        public async void displayUser()
        {
            var user = App.UserDatabase.GetUser();
            if (user == null) return;
            user.updateProfileImageSource();
            BindingContext = user;
            var partiesList = await App.GetPartiesAsync();

            if (partiesList.Count > 0)
            {

                //foreach (var p in partiesList) p.geoPosition = (await (new Xamarin.Forms.Maps.Geocoder()).GetPositionsForAddressAsync(p.address)).FirstOrDefault();

                //noPartiesMsg.IsVisible = false;
                postListView.ItemsSource = partiesList;
            }


        }
        private void Friends_Tapped(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new NavigationPage(new FriendsPage()) { BarBackgroundColor = Color.Black});
        }

        private void Music_Tapped(object sender, EventArgs e)
        {
            Debug.WriteLine("Music Tapped");
        }

        private async void postListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is Party party) await Navigation.PushModalAsync(new PartyDetailsPage(party));
            postListView.SelectedItem = null;
        }
        private async void ChangeProfileImage(object sender, EventArgs e)
        {
            if(sender is Image currentImage)
            {
                await CrossMedia.Current.Initialize();
                try
                {
                    var ImageFile = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                    {
                        PhotoSize = PhotoSize.Small
                    });
                    Animation a1 = new Animation();
                    if (ImageFile == null)
                    {
                        
                    }
                    else
                    {
                        await FirebaseHelper.SetProfileImage(App.UserDatabase.GetUser().uid, ImageFile);
                        currentImage.Source = ImageSource.FromStream(() => { return ImageFile.GetStream(); });
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }  
        }
    }
}