
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App3
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        MasterTabbedPage parent;
        public SettingsPage(MasterTabbedPage parent)
        {
            this.parent = parent;
            InitializeComponent();
            
        }
        async public void OnLogout(object sender, EventArgs e)
        {
            bool resp = await DisplayAlert("Sign Out", "Are you sure you want to sign out?", "Yes", "No");
            if (!resp) return;
            await Navigation.PopAsync();
            parent.OnLogout();

        }
    }
}