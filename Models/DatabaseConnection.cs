using System;
using MySql.Data.MySqlClient;

namespace curr.Models
{
    public class DatabaseConnection
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string user;
        private string password;
        private string port;
        private string connectionString;
        private string sslM;

        public void LoginToDatabase()
        {
            server = "localhost";
            database = "CURRENCY";
            user = "root";
            password = "root";
            port = "3306";
            sslM = "none";

            connectionString = string.Format("server={0};port={1};user id={2}; password={3}; database={4}; SslMode={5}", server, port, user, password, database, sslM);

            connection = new MySqlConnection(connectionString);
            connection.Open();
            System.Console.WriteLine( "Server Version: " + connection.ServerVersion + "\nDatabase: " + connection.Database );
        }
        // public void Get()
        // {
        //     var sqlTransaction = connection.BeginTransaction();
        // }
        public void Insert(PopularCurrency popularCurrency)
        {
            // Start a local transaction.
            MySqlCommand command = connection.CreateCommand();
            MySqlTransaction transaction;
            transaction = connection.BeginTransaction(System.Data.IsolationLevel.Serializable);
            try
            {
                command.CommandText =
                    "insert into currency.popular_buy_and_sell(id, date, currency_code, currency_name, buy_rate, sell_rate, conversion) values "+
                    "(\"" + popularCurrency.Id+"\""+ ", "+
                    "\'" + popularCurrency.Date.Year+"-"+popularCurrency.Date.Month+"-"+popularCurrency.Date.Day+"-"+"\'"+ ", "+
                    "\"" + popularCurrency.CurrencyCode+"\""+ ", "+
                    "\"" + popularCurrency.CurrencyName+"\""+ ", "+
                    + popularCurrency.BuyRate+", "+
                    + popularCurrency.SellRate+", "+
                    + popularCurrency.Conversion+
                    ")";
                command.ExecuteNonQuery();

                try
                {
                    transaction.Commit();
                }
                catch (Exception commitException)
                {
                    Helper.TraceMessage("Commit Exception Type: " + commitException.GetType());
                    Helper.TraceMessage("  Message: "+ commitException.Message);
                }
                
                Helper.TraceMessage("Records was written to database.");
            }
            catch (Exception ex)
            {
                Helper.TraceMessage("Commit Exception Type: " + ex.GetType());
                Helper.TraceMessage("  Message: "+ ex.Message);
                
                try
                {
                    transaction.Rollback();
                }
                catch (Exception transactionException)
                {
                    Helper.TraceMessage("Commit Exception Type: " + transactionException.GetType());
                    Helper.TraceMessage("  Message: "+ transactionException.Message);
                }
            }
        }
        public void CloseConnectionToDatabase()
        {
            connection.Close();
        }
    }
}