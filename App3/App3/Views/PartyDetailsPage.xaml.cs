using App3.Models;
using Xamarin.Forms;
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
                await Navigation.PopAsync();
            });
            InitializeComponent();
        }

        private void ShowPeople(object sender, System.EventArgs e)
        {

            showCommentsButton.BackgroundColor = Color.Transparent;
            showPeopleButton.BackgroundColor = Color.FromHex("#F50058");
            showMapButton.BackgroundColor = Color.Transparent;
            map.IsVisible = false;
        }
        private void ShowComments(object sender, System.EventArgs e)
        {

            showCommentsButton.BackgroundColor = Color.FromHex("#F50058");
            showPeopleButton.BackgroundColor = Color.Transparent;
            showMapButton.BackgroundColor = Color.Transparent;

            map.IsVisible = false;
        }
        private void ShowMap(object sender, System.EventArgs e)
        {

            showCommentsButton.BackgroundColor = Color.Transparent;
            showPeopleButton.BackgroundColor = Color.Transparent;
            showMapButton.BackgroundColor = Color.FromHex("#F50058");

            map.IsVisible = true;

        }
    }

}