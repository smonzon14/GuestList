using App3.Data;
using App3.Models;
using Firebase.Database.Query;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace App3
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PartyHostPage : ContentPage
    {
        public int step = 0;
        Party party = new Party();
        MediaFile ImageFile = null;
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
                "(Optional) type a short description",
                "Ready to post?"
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
                    var locations = await new Geocoder().GetPositionsForAddressAsync(locationEntry.Text);
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
                step += d;
                entries[step].IsVisible = true;
                

                promptLabel.Text = prompts[step];
                mapLayout.IsVisible = step == 1 ? true : false; 
                
                a2.Commit(owner: entries[step], "show", 50, easing: Easing.SinInOut);

            });

        }
        private async void NextButton_Clicked(object sender, EventArgs e)
        {
            if (await isStepComplete())
            {
                if (step == 3)
                {
                    await postParty();
                    party.Thrower = App.UserDatabase.GetUser();
                    App.UserParties.Add(party);
                    await Navigation.PopModalAsync();
                    return;
                }
                animateBetweenPrompts(true);
            }
        }
        private async Task<bool> postParty()
        {
            try
            {

                return await FirebaseHelper.CreateParty(party, ImageFile ,0);
            }
            catch
            {
                await DisplayAlert("Error", "Could not post.", "Ok");
                return false;
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

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            try
            {
                ImageFile = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                {
                    PhotoSize = PhotoSize.Medium
                });
                Animation a1 = new Animation();
                if (ImageFile == null)
                {
                    partyImage.Source = null;
                    a1.Add(0, 1, new Animation(v => partyImage.HeightRequest = v, partyImage.Height, 150));

                }
                else
                {
                    partyImage.Source = ImageSource.FromStream(() => { return ImageFile.GetStream(); });
                    a1.Add(0, 1, new Animation(v => partyImage.HeightRequest = v, partyImage.Height, 400));
                }
                a1.Commit(partyImage, "resize", 16, 250, Easing.SinInOut);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void entry_Focused(object sender, FocusEventArgs e)
        {
            
            Animation a1 = new Animation();
            a1.Add(0, 1, new Animation(v => partyImage.HeightRequest = v, partyImage.Height, 150));
            a1.Commit(partyImage, "focusedEntry", 16, 250, Easing.SinInOut);
        }
        private void entry_UnFocused(object sender, FocusEventArgs e)
        {
            Animation a1 = new Animation();
            a1.Add(0, 1, new Animation(v => partyImage.HeightRequest = v, partyImage.Height, partyImage.Source == null ? 150 : 400));
            a1.Commit(partyImage, "focusedEntry", 16, 250, Easing.SinInOut);
        }

        private async void locationEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            var editor = sender as Editor;
            var text = editor.Text;
            if (text == null || text.Length == 0)
            {
                //deal with empty text here
            }
            await Task.Run(() => Thread.Sleep(700));
            if(text == editor.Text)
            {
                var geo = (await new Geocoder().GetPositionsForAddressAsync(text).ConfigureAwait(false)).FirstOrDefault();
                if (geo == null) return;
                MapSpan region = MapSpan.FromCenterAndRadius(geo, Distance.FromMiles(0.1));
                MainThread.BeginInvokeOnMainThread(()=>map.MoveToRegion(region));
            }
            
        }
    }
}