using App3.Maps;
using App3.Models;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace App3
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PartyDetailsPage : ContentPage
    {
        public PartyDetailsPage(Party party)
        {
            BindingContext = party;
            InitializeComponent();
        }

        private void ExitButton_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
    
}