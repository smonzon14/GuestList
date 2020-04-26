using App3.Data;
using App3.Models;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App3
{
    public partial class App : Application
    {
        //static TokenDatabaseController tokenDatabase;
        static UserDatabaseController userDatabase;
        //static RestService restService;
        public static UserDatabaseController UserDatabase
        {
            get
            {
                if (userDatabase == null) userDatabase = new UserDatabaseController();
                return userDatabase;
            }
        }
        
        public App()
        {
            InitializeComponent();
            //HttpsService.httpsValidation.Initialize();
            //MainPage = new NavigationPage(new Home());
            MainPage = new TabbedPage1();
            
            
        }
        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
     
        
        /*
        public static TokenDatabaseController TokenDatabase
        {
            get
            {
                if (tokenDatabase == null) tokenDatabase = new TokenDatabaseController();
                return tokenDatabase;
            }
        }
        public static RestService RestService
        {
            get
            {
                if(restService == null) restService = new RestService();
                return restService;
            }
        }*/
    }
}
