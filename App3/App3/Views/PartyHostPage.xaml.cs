using App3.Models;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App3
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PartyHostPage : ContentPage
    {
        public int step = 0;
        Party party = new Party();
        public System.Windows.Input.ICommand ToolbarRightCommand { get; private set; }
        public string ToolbarRightSource { get; private set; }

        public System.Windows.Input.ICommand ToolbarLeftCommand { get; private set; }
        public string ToolbarLeftSource { get; private set; }
        public PartyHostPage()
        { 
            ToolbarLeftCommand = new Command(() => BackButton_Clicked());
            ToolbarRightCommand = new Command(() => Navigation.PopModalAsync());
            ToolbarRightSource = "x";
            ToolbarLeftSource = "button_back";
            InitializeComponent();
            datePicker.MinimumDate = DateTime.Now;
            update();
        }
        
        private async Task<bool> isStepComplete()
        {
            switch (step)
            {
                case 0: // Title entry
                    if (titleEntry.Text == null)
                    {
                        await DisplayAlert("Invalid Title", "Party must have a name", "Ok").ConfigureAwait(false);
                        return false;
                    }
                    party.name = titleEntry.Text;
                    break;
                case 1: // Location
                    
                    if (locationEntry.Text == null)
                    {
                        await DisplayAlert("Invalid Address", "Please type a valid address", "Ok").ConfigureAwait(false);
                        return false;
                    }
                    var locations = await (new Geocoder()).GetPositionsForAddressAsync(locationEntry.Text);
                    var location = locations?.FirstOrDefault();
                    if (location == null)
                    {

                        await DisplayAlert("Invalid Address", "Please type a valid address", "Ok").ConfigureAwait(false);
                        return false;
                    }
                    party.address = locationEntry.Text;
                    party.geoPosition = (Position)location;
                    break;

                case 2: // Date
                    return true;
                case 3: // Time
                    DateTime time = datePicker.Date + timePicker.Time;
                    if (time < DateTime.Now)
                    {
                        await DisplayAlert("Invalid Time", "Selected time must be in the future", "Ok").ConfigureAwait(false);
                        return false;
                    }
                    party.time = time;
                    break;

                case 4: // Description
                    party.description = descriptionEntry.Text;
                    return true;
                default:
                    return false;
            }
            return true;

            
            
        }

        private async void NextButton_Clicked(object sender, EventArgs e)
        {
            if ( await isStepComplete())
            {
                step++;
                update();
            }
        }
        private void BackButton_Clicked()
        {
            if (step == 0) return;
            step--;
            update();
        }
        private async void update()
        {
            if (step < 0) await Navigation.PopAsync();
            if (step > 4) step = 4;
            
            if (step == 4)
            {
                
                await Navigation.PushAsync(new PartyPostPreviewPage(party));
                return;
            }
            var prompts = new List<string>()
            {
                "What do you want this event to be named?",
                "Where is it going down?",
                "Which day is it going down?",
                "What time should people be there?",
                "(Optional) type a short description"
            };
            var entries = new List<VisualElement>()
            {
                titleEntry,
                locationEntry,
                datePicker,
                timePicker,
                descriptionEntry
            };
            promptLabel.Text = prompts[step];
            for (int i = 0; i < 5; i++)
            {
                if (i == step) entries[i].IsVisible = true;
                else entries[i].IsVisible = false;
            }
            
        }
    }
}