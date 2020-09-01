using App3.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App3
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterTabbedPage : TabbedPage

    {
        private Page _lastPage;
        public MasterTabbedPage()
        {
            InitializeComponent();
            loadViews();

        }
        internal class FakePage : ContentPage { public FakePage() { BackgroundColor = Color.Black; } }
        private void loadViews()
        {


            Children.Clear();


            // Home Page
            NavigationPage home = new NavigationPage(new HomePage())
            {
                //Title = "Map",
                IconImageSource = "tab_map",
                BarTextColor = Color.White,
                BarBackgroundColor = Color.Black
            };
            Page host = new FakePage()
            {
                IconImageSource = "tab_plus"
            };
            
            // Profile Page
            NavigationPage profile = new NavigationPage(new CurrentUserProfilePage())
            {
                //Title = "Profile",
                IconImageSource = "tab_profile",
                BarTextColor = Color.White,
                BarBackgroundColor = Color.Black
            };
            
            NavigationPage feed = new NavigationPage(new FeedPage())
            {
                //Title = "Chatter",
                IconImageSource = "tab_feed",
                BarTextColor = Color.White,
                BarBackgroundColor = Color.Black
            };

            NavigationPage music = new NavigationPage(new MusicPage())
            {
                //Title = "Chatter",
                IconImageSource = "Music",
                BarTextColor = Color.White,
                BarBackgroundColor = Color.Black
            };

            //Children.Add(friends);
            //Children.Add(home);

            Children.Add(feed);
            Children.Add(home);
            Children.Add(host);
            Children.Add(music);
            Children.Add(profile);

            CurrentPage = home;
            _lastPage = home;


        }
        protected override async void OnCurrentPageChanged()
        {
            if (CurrentPage is FakePage)
            {
                var createPage = new NavigationPage(new PartyHostPage()) { BarTextColor = Color.White, BarBackgroundColor = Color.Black };
                CurrentPage = _lastPage;
                await Navigation.PushModalAsync(createPage);
            }
            else _lastPage = CurrentPage;

            base.OnCurrentPageChanged();
        }


        // returns 1 if user refresh is unsuccessful (i.e. expired or missing token)
        // returns 0 if user refresh is successful.

    }
}