using System;
using System.Data;
using System.Data.SqlClient;

namespace MyAppDB
{
    public static class DatabaseConnection
    {
        static SqlConnection sqlConnection;
        public static string ConnectionString { get; set; } = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ExpandableWebAppDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public static SqlDataReader Query(string SQLStatment)
        {
            if (sqlConnection?.State == ConnectionState.Open)
            {
                sqlConnection.Close();
            }
            sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();
            SqlCommand command = new SqlCommand(SQLStatment, sqlConnection);
            SqlDataReader result = command.ExecuteReader();
            return result;
        }

        public static int NonQuery(string SQLStatment)
        {
            if (sqlConnection?.State == ConnectionState.Open)
            {
                sqlConnection.Close();
            }
            sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();
            SqlCommand command = new SqlCommand(SQLStatment, sqlConnection);
            int result = command.ExecuteNonQuery();
            return result;
        }

        public static string Scalar(string SQLStatment)
        {
            if (sqlConnection?.State == ConnectionState.Open)
            {
                sqlConnection.Close();
            }
            sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();
            SqlCommand command = new SqlCommand(SQLStatment, sqlConnection);
            string result = command.ExecuteScalar().ToString();
            return result;
        }
    }
}
