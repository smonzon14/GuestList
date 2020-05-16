using App3.Data;
using App3.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App3
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUpPage : ContentPage
    {
        public SignUpPage()
        {
            InitializeComponent();
        }
        async public void OnSwitchToSignIn(object sender, EventArgs e)
        {
            User authUser = await DependencyService.Get<IAuthentication>().Login();
            if (authUser != null)
            {
                authUser.printUser();
                Debug.WriteLine("User Authorized.");
                App.UserDatabase.SaveUser(authUser);
                await Navigation.PopModalAsync();
            }
        }
        public void onSubmit(object sender, EventArgs e)
        {
            var client = new RestClient("https://dev-2huf9bd9.auth0.com/");
            var request = new RestRequest("dbconnections/signup", Method.POST);

            request.AddParameter("client_id", "h0zRTg9paH66Ck2rXwE5WIpVfhERNe1k");
            request.AddParameter("email", username.Text);
            request.AddParameter("password", password.Text);
            request.AddParameter("connection", "GuestList");

            IRestResponse response = client.Execute(request);

            //User user = JsonConvert.DeserializeObject<User>(response.Content);
            User user = null;
            if (user != null)
            {
                user.printUser();
                DisplayAlert("Account Created", "Head back to the hompage and login with your new account", "Ok");
            }
            else
            {
                DisplayAlert("Oh No!", "Account could not be created. Do you already have an account? Please try again.", "Ok");
            }
        }

    }
}