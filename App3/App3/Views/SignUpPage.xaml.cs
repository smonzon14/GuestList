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
            if (name.Text == null || name.Text.Length < 3)
            {
                await DisplayAlert("Name Invalid", "Full name must be at least 3 characters long", "Ok");
                return;
            }

            /*if (await FirebaseHelper.UserWithEmailExists(username.Text))
            {
                await DisplayAlert("Username Exists", "Try a different username", "Ok");
                return;
            }*/
            else
            {
                User user = await App.SignupAsync(username.Text, password.Text, name.Text, GenderPicker.SelectedIndex , DOBPicker.Date);
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