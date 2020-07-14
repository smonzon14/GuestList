using App3.Models;
using App3.Views;
using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App3
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUpPage : ContentPage
    {
        LogInPage logInPage;
        public SignUpPage()
        {
            InitializeComponent();
            logInPage = new LogInPage();


        }
        async public void OnSwitchToSignIn(object sender, EventArgs e)
        {
            await Navigation.PushAsync(logInPage);
            /*
            Debug.WriteLine("User Sign-in initiated.");
            User user;
            do
            {
                user = await DependencyService.Get<IAuthentication>().Login();
                
            } while (user != null || ! await FirebaseHelper.UserWithEmailExists(user.email));
            user.printUser();
            var firebaseUser = await FirebaseHelper.GetUser(user.email);
            Debug.WriteLine("User Authorized.");
            App.UserDatabase.SetUser(firebaseUser);
            Debug.WriteLine("User Saved.");

            await Navigation.PopModalAsync();*/
        }

        /*
        public async void onSubmit(object sender, EventArgs e)
        {
            if (await FirebaseHelper.AddUser(username.Text, password.Text))
            {
                var user = await FirebaseHelper.GetUser(username.Text);
                App.UserDatabase.SaveUser(user);
                await Navigation.PopModalAsync();
            }
            else
            {
                errorMessage.IsVisible = true;
            }
            
        }
        */

        public async void onSubmit(object sender, EventArgs e)
        {
            Debug.WriteLine(username.Text);
            if (password.Text != retypePassword.Text)
            {
                await DisplayAlert("Password Mismatch", "Please retype password correctly", "Ok");
                return;
            }
            if (username.Text == null || username.Text.Length < 5)
            {
                await DisplayAlert("Username Invalid", "Username must be at least 5 characters long", "Ok");
                return;
            }
            /*if (await FirebaseHelper.UserWithEmailExists(username.Text))
            {
                await DisplayAlert("Username Exists", "Try a different username", "Ok");
                return;
            }*/
            else
            {
                User user = await App.SignupAsync(username.Text, password.Text);
                if (user == null)
                {
                    errorMessage.IsVisible = true;
                }
                else
                {

                    user.printUser();
                }
            }

        }

    }
}