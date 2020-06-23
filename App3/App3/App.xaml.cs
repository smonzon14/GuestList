using App3.Data;
using App3.Models;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.Generic;
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

        static List<Party> userParties;
        public static List<Party> UserParties { get
            {
                if (userParties == null) userParties = new List<Party>();//FirebaseHelper.GetPartiesThrownByUser(UserDatabase.GetUser().uid).Result;
                return userParties;
            }
        }
        static List<User> userFriends;
        public static List<User> UserFriends
        {
            get
            {
                if (userFriends == null) userFriends = new List<User>(); //FirebaseHelper.GetFriendsList(UserDatabase.GetUser().uid).Result;
                return userFriends;
            }
        }
        public App()
        {
            InitializeComponent();
            //HttpsService.httpsValidation.Initialize();
            //MainPage = new NavigationPage(new Home());
            //userFriends = FirebaseHelper.GetFriendsList(UserDatabase.GetUser().uid).Result;
            //userParties = FirebaseHelper.GetPartiesThrownByUser(UserDatabase.GetUser().uid).Result;
            MainPage = new MasterTabbedPage();
            
            
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
