using System;
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
        public async void OnSwitchToSignUp(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
        public async void onSubmit(object sender, EventArgs e)
        {
            //if(!await FirebaseHelper.UserWithEmailExists(username.Text)) errorMessage.IsVisible = true;

            await App.LoginAsync(username.Text, password.Text);



        }
    }
}