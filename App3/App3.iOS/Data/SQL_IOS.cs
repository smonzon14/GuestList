using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Foundation;
using SQLite;
using UIKit;
using Xamarin.Forms;
using App3.Models;
[assembly: Dependency(typeof(App3.iOS.Data.SQL_IOS))]

namespace App3.iOS.Data
{
    class SQL_IOS : ISQLite
    {
        public SQLiteConnection GetConnection()
        {
            var documentPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var libraryPath = Path.Combine(documentPath, "..", "Library");
            var path = Path.Combine(libraryPath, "GLUser.db");
            var connection = new SQLiteConnection(path);
            return connection;
        }
    }
}