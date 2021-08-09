using MySql.Data.MySqlClient;

namespace BankForm
{
    class Database
    {
        string server;
        string database;
        string uid;
        string password;

        MySqlConnection connection;

        public Database(string server, string database, string uid, string password)
        {
            this.server = server;
            this.database = database;
            this.uid = uid;
            this.password = password;
        }
        public void Connect()
        {
            string connectionString = $"SERVER={server}; DATABASE={database}; UID={uid}; PASSWORD={password}";

            connection = new MySqlConnection(connectionString);
        }

        public MySqlConnection GetConnection() => connection;
    }
}
