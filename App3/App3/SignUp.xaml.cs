using App3.Data;
using App3.Models;
using Newtonsoft.Json;
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
    public partial class SignUp : ContentPage
    {
        public SignUp()
        {
            InitializeComponent();
        }
        async public void OnSwitchToSignIn(object sender, EventArgs e)
        {

            //await Navigation.PushModalAsync(new SignIn());
            
            Debug.WriteLine("User Sign-in initiated.");
            User authUser = await DependencyService.Get<IAuthentication>().Login();
            //Task.WaitAll(authUser);
            if (authUser != null)
            {
                authUser.printUser();
                Debug.WriteLine("User Authorized.");
                App.UserDatabase.SaveUser(authUser);
                Debug.WriteLine("User Saved.");
                
                await Navigation.PopModalAsync();
            }
        }
        /*
        public void onSubmit(object sender, EventArgs e)
        {
            if (username.Text == "" || password.Text == "")
            {
                errorMessage.Text = "Username or Password cannot be blank.";
                
            }
            User newUser = new User(username.Text, password.Text);
            
        }*/

        public void onSubmit(object sender, EventArgs e)
        {
            var client = new RestClient("https://dev-2huf9bd9.auth0.com/");
            var request = new RestRequest("dbconnections/signup", Method.POST);

            request.AddParameter("client_id", "h0zRTg9paH66Ck2rXwE5WIpVfhERNe1k");
            request.AddParameter("email", username.Text);
            request.AddParameter("password", password.Text);
            request.AddParameter("connection", "GuestList");

            IRestResponse response = client.Execute(request);
            // Once the request is executed we capture the response.
            // If we get a `user_id`, we know that the account has been created
            // and display an appropriate message. If we do not get a `user_id`
            // we know something went wrong, so we ask the user if they already have
            // an account and if not to try again.
            Console.WriteLine(response.Content);
            User user = JsonConvert.DeserializeObject<User>(response.Content);
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

        public class UserSignup
        {
            public string user_id { get; set; }
        }
        
        /*
        async public void onSubmit(object sender, EventArgs e)
        {
            Console.WriteLine("Sign Up request:");
            Console.WriteLine(username);
            Console.WriteLine(password);
            User newUser = new User(username.Text, password.Text);
            if (newUser.CheckInformation())
            {
                
                var res = await App.RestService.Register(newUser);
                Console.WriteLine(res);
                if(res != null)App.UserDatabase.SaveUser(newUser);
                await DisplayAlert("Login", "Login Success", "Ok");
                //Segue to home screen or do data management first
            }
            else
            {
                await DisplayAlert("Login", "Login Incorrect", "Ok");
            }
        }*/
    }
}