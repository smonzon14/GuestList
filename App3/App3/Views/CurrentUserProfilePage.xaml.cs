using App3.Data;
using App3.Models;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App3.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CurrentUserProfilePage : ContentPage
    {
        public System.Windows.Input.ICommand ToolbarRightCommand { get; private set; } 
        public string ToolbarRightSource { get; private set; } = "button_settings";

        NavigationPage FriendsPage = new NavigationPage(new FriendsPage()) { BarTextColor = Color.White };
        NavigationPage MusicPage = new NavigationPage(new MusicPage()) { BarTextColor = Color.White };
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
            friendsCount.SetBinding(Label.TextProperty, "Count");
            friendsCount.BindingContext = App.UserFriends;
            partiesCount.SetBinding(Label.TextProperty, "Count");
            partiesCount.BindingContext = App.UserParties;
            var user = App.UserDatabase.GetUser();
            if (user == null) return;
            user.updateProfileImageSource();
            BindingContext = user;
            var partiesList = await App.GetPartiesAsync();

            if (partiesList.Count > 0) postListView.ItemsSource = partiesList;

            

        }
        private void Friends_Tapped(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(FriendsPage);
        }

        private void Music_Tapped(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(MusicPage);
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
                        await FirebaseHelper.SetProfileImage(ImageFile);
                        currentImage.Source = ImageSource.FromStream(() => { return ImageFile.GetStream(); });
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }  
        }

        private void EditProfileButton_Clicked(object sender, EventArgs e)
        {

        }
        private double toolbarOpacity = 1.0;
        public double ToolbarOpacity { get { return toolbarOpacity; } private set { toolbarOpacity = value; } }

        private void ListView_Scrolled(object sender, ScrolledEventArgs e)
        {
            if (e.ScrollY <= 0) toolbarOpacity = 0.0;
            else if (e.ScrollY > 60) toolbarOpacity = 1.0;
            else toolbarOpacity = e.ScrollY / 60;
            OnPropertyChanged("ToolbarOpacity");
        }
    }
}