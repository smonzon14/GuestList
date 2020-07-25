using SQLite;
using System;
using System.IO;
using Xamarin.Forms;
[assembly: Dependency(typeof(App3.iOS.Data.SQL_IOS))]

namespace App3.iOS.Data
{
    class SQL_IOS : ISQLite
    {
        public SQLiteConnection GetConnection()
        {
            var documentPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var libraryPath = Path.Combine(documentPath, "..", "Library");
            var path = Path.Combine(libraryPath, "GLUserDefault4.db");
            var connection = new SQLiteConnection(path);
            return connection;
        }
    }
}