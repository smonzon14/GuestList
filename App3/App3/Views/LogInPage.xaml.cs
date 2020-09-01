using System;
using System.Diagnostics;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App3.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogInPage : ContentPage
    {
        public LogInPage()
        {
            InitializeComponent();
        }
        public async void onSubmit(object sender, EventArgs e)
        {
            //if(!await FirebaseHelper.UserWithEmailExists(username.Text)) errorMessage.IsVisible = true;

            if(await App.LoginAsync(username.Text, password.Text) == null)
            {
                errorMessage.Text = "User not found with username/password";
                errorMessage.IsVisible = true;
            }



        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var email = await DisplayPromptAsync("email", "enter your account email for a password reset.", "Send", "Cancel");
            if(await App.ResetPassword(email))
            {
                await DisplayAlert("Check Your Email", "password reset request sent.", "Got it");
            }
        }
    }
}