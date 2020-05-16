using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App3.Models;
using App3.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;

namespace App3.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogInPage : ContentPage
    {
        public LogInPage()
        {
            InitializeComponent();
        }
        public void OnSwitchToSignUp(object sender, EventArgs e)
        {
            
        }
        public async void onSubmit(object sender, EventArgs e)
        {
            string tokenId = "";//await DependencyService.Get<IFirebaseAuthenticator>().LoginWithEmailPassword(username.Text, password.Text);
            Debug.WriteLine(tokenId);
        }
    }
}