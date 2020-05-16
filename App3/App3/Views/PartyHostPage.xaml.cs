using App3.Models;
using System;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace App3
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PartyHostPage : ContentPage
    {
        
        public PartyHostPage()
        { 
            InitializeComponent();
        }
        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            
            try
            {
                if (nameEntry.Text == null)
                {
                    await DisplayAlert("Invalid Title", "Party must have a name", "Ok");
                    return;
                }

                if(locationEntry.Text == null)
                {
                    await DisplayAlert("Invalid Address", "Please type a valid address", "Ok");
                    return;
                }
                var locations = await (new Geocoder()).GetPositionsForAddressAsync(locationEntry.Text);
                var location = locations?.FirstOrDefault();
                if (location == null)
                {
                    
                    await DisplayAlert("Invalid Address", "Please type a valid address", "Ok");
                    return;
                }
                Console.WriteLine(location.Value.Latitude);
                Console.WriteLine(location.Value.Longitude);
                DateTime time = datePicker.Date + timePicker.Time;
                if (time < DateTime.Now)
                {
                    await DisplayAlert("Invalid Time", "Selected time must be in the future", "Ok");
                    return;
                }


                Party newParty = new Party
                {
                    name = nameEntry.Text,
                    address = locationEntry.Text,
                    geoPosition = (Position)location,
                    description = descriptionEntry.Text,
                    time = time,
                };
                await Navigation.PushAsync(new PartyPostPreviewPage(newParty));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception when attempting to geocode address.");
                Debug.WriteLine("Ex: " + ex);
                // Handle exception that may have occurred in geocoding
            }
            
            
        }

    }
}