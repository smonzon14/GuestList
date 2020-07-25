using App3.Models;
using SQLite;
using Xamarin.Forms;

namespace App3.Data
{
    public class UserDatabaseController
    {
        static IUserData userData;
        
        public UserDatabaseController()
        {
            userData = DependencyService.Get<IUserData>();
            
        }
        public User GetUser()
        {
            return userData.GetUser();
        }
        public void SetUser(User user)
        {
            userData.SetUser(user);
        }
        public void RemoveUserData()
        {
            userData.RemoveUserData();
        }
        /*
        static object locker = new object();
        SQLiteConnection database;
        
        public UserDatabaseController()
        {

            database = DependencyService.Get<ISQLite>().GetConnection();
            database.CreateTable<UserBase>();
        }
        public UserBase GetUser()
        {
            lock (locker)
            {
                if (database.Table<UserBase>().Count() == 0) return null;
                else return database.Table<UserBase>().First();

            }
        }
        public int SetUser(UserBase user)
        {
            
            lock (locker)
            {
                RemoveUserData();
                
                return database.Insert(user);
            }
        }
        public int RemoveUserData()
        {
            return database.DeleteAll<UserBase>();
        }
        */
        /*public int DeleteUser(int id)
        {
            lock (locker) return database.Delete<User>(id);
            
        }*/
    }
}
