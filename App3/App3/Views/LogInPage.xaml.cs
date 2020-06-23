using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;
using App3.Data;
using App3.Models;

namespace App3.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogInPage : ContentPage
    {
        public LogInPage()
        {
            InitializeComponent();
        }
        public async void OnSwitchToSignUp(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
        public async void onSubmit(object sender, EventArgs e)
        {
            //if(!await FirebaseHelper.UserWithEmailExists(username.Text)) errorMessage.IsVisible = true;

            IFirebaseAuthenticator authenticator = DependencyService.Get<IFirebaseAuthenticator>();


            string tokenId = await authenticator.LoginWithEmailPassword(username.Text, password.Text);
            if (tokenId == "")
            {
                errorMessage.IsVisible = true;
            }
            else
            {
                Debug.WriteLine(tokenId);

                var user = authenticator.GetCurrentUser(); //await FirebaseHelper.GetUserFromUID(uid);
                
                if (user == null)
                {
                    Debug.WriteLine("User not found");
                    await DisplayAlert("User not found", "Invalid email / password", "Ok");
                    return;
                }
                //var user = await FirebaseHelper.GetUser(username.Text);
                App.UserDatabase.SetUser(user);
                Debug.WriteLine("User from database:");
                await Navigation.PopModalAsync();
            }
            
            
            
        }
    }
}