using App3.Models;
using SQLite;
using Xamarin.Forms;

namespace App3.Data
{
    public class UserDatabaseController
    {
        static object locker = new object();
        SQLiteConnection database;
        public UserDatabaseController()
        {
            database = DependencyService.Get<ISQLite>().GetConnection();
            database.CreateTable<User>();
        }
        public User GetUser()
        {
            lock (locker)
            {
                if (database.Table<User>().Count() == 0) return null;
                else return database.Table<User>().First();

            }
        }
        public int SetUser(User user)
        {
            lock (locker)
            {
                RemoveUserData();
                return database.Insert(user);
            }
        }
        public int RemoveUserData()
        {
            return database.DeleteAll<User>();
        }

        /*public int DeleteUser(int id)
        {
            lock (locker) return database.Delete<User>(id);
            
        }*/
    }
}
