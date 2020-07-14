using App3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.Xaml;

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

        private List<string> prompts = new List<string>()
            {
                "What do you want this event to be named?",
                "Where is it going down?",
                "Which day is it going down?",
                "What time should people be there?",
                "(Optional) type a short description"
            };
        private List<VisualElement> entries;
        public PartyHostPage()
        {
            ToolbarLeftCommand = new Command(() => BackButton_Clicked());
            ToolbarRightCommand = new Command(() => Navigation.PopModalAsync());
            ToolbarRightSource = "x";
            ToolbarLeftSource = "button_back";
            InitializeComponent();
            entries = new List<VisualElement>()
            {
                titleEntry,
                locationEntry,
                datePicker,
                timePicker,
                descriptionEntry
            };

            datePicker.MinimumDate = DateTime.Now;
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
        private void animateBetweenPrompts(bool forward)
        {
            int d = forward ? 1 : -1;

            Animation a1 = new Animation();
            a1.Add(0, 1, new Animation(v => entries[step].Opacity = v, 1.0, 0));
            a1.Add(0, 1, new Animation(v => promptLabel.Opacity = v, 1.0, 0));

            Animation a2 = new Animation();
            a2.Add(0, 1, new Animation(v => entries[step].Opacity = v, 0, 1.0));
            a2.Add(0, 1, new Animation(v => promptLabel.Opacity = v, 0, 1.0));
            //a.Add(0, 1, new Animation(v => bottomPaddingRow.Height = v, 200, 0));
            a1.Commit(owner: entries[step], "hide", 50, easing: Easing.SinInOut, finished: (x, y) =>
            {
                entries[step].IsVisible = false;
                entries[step + d].Opacity = 0;
                entries[step + d].IsVisible = true;
                promptLabel.Text = prompts[step];

                a2.Commit(owner: entries[step += d], "show", 50, easing: Easing.SinInOut);

            });

        }
        private async void NextButton_Clicked(object sender, EventArgs e)
        {
            if (await isStepComplete())
            {
                if (step == 3)
                {
                    await Navigation.PushAsync(new PartyPostPreviewPage(party));
                    step--;
                    return;
                }
                animateBetweenPrompts(true);

            }
        }

        private async void BackButton_Clicked()
        {

            if (step < 1)
            {
                await Navigation.PopModalAsync();
                return;
            }
            animateBetweenPrompts(false);
        }
    }
}