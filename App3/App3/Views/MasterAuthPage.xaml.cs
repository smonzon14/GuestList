using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App3.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterAuthPage : ContentPage
    {
        public MasterAuthPage()
        {
            InitializeComponent();
        }

        private void SignUp_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SignUpPage());
        }

        private void LogIn_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new LogInPage());
        }
    }
}