using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace App3
{
    public interface ISQLite
    {
        SQLiteConnection GetConnection();

    }
}
