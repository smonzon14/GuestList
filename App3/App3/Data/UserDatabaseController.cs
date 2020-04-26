using App3.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Diagnostics;

namespace App3.Data
{
    public class UserDatabaseController
    {
        static object locker = new object();
        SQLiteConnection database;
        public UserDatabaseController()
        {
            database = DependencyService.Get<ISQLite>().GetConnection();
            Debug.WriteLine("Got DB connection.");
            database.CreateTable<User>();
            Debug.WriteLine("created table.");
        }
        public User GetUser()
        {
            lock (locker)
            {
                if(database.Table<User>().Count() == 0)
                {
                    return null;
                }
                else
                {
                    return database.Table<User>().First();
                }
            }
        }
        public int SaveUser(User user)
        {
            Debug.WriteLine("locking");
            lock (locker)
            {

                
                Debug.WriteLine("insert new user");
                return database.Insert(user);
                
            }
        }
        public int DeleteUser(int id)
        {
            lock (locker)
            {
                return database.Delete<User>(id);
            }
        }
    }
}
