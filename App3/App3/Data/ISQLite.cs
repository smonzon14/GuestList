using SQLite;

namespace App3
{
    public interface ISQLite
    {
        SQLiteConnection GetConnection();

    }
}
