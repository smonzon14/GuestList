using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App3.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomMasterTabbedPage : ContentPage
    {
        int CurrentPageIndex;
        public List<ContentPage> Children { get; set; } = new List<ContentPage>();
        public CustomMasterTabbedPage()
        {
            
            var home = new HomePage()
            {
                //Title = "Map",
                IconImageSource = "tab_map"
            };
            var host = new PartyHostPage()
            {
                IconImageSource = "tab_plus"
            };

            // Profile Page
            var profile = new CurrentUserProfilePage()
            {
                //Title = "Profile",
                IconImageSource = "tab_profile"
            };

            var feed = new FeedPage()
            {
                //Title = "Chatter",
                IconImageSource = "tab_feed"
            };

            var music = new MusicPage()
            {
                //Title = "Chatter",
                IconImageSource = "Music"
            };

            Children.Add(feed);
            Children.Add(home);
            Children.Add(host);
            Children.Add(music);
            Children.Add(profile);
            

            InitializeComponent();
            BindableLayout.SetItemsSource(tabBar, Children);
            

            CurrentPageIndex = 0;
            
            CurrentPage.Content = Children[0].Content;
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(sender.ToString());
        }
    }
}