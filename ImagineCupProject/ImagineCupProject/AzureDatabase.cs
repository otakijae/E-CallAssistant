using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows;

namespace ImagineCupProject
{
    class AzureDatabase
    {
        public void insertData(string operatorName,string time, string location, string phoneNumber, string callerName, string problem, string code)
        {
            try
            {
                var cb = new SqlConnectionStringBuilder();
                cb.DataSource = "jangwonserver.database.windows.net";
                cb.UserID = "jangwon";
                cb.Password = "wkddnjs2!!";
                cb.InitialCatalog = "emergencycallDatabase";

                string sql;
                using (var connection = new SqlConnection(cb.ConnectionString))
                {
                    connection.Open();
                    sql = "insert into emergencyCall values('" + operatorName + "', '" + time + "', '" + location + "', '" + phoneNumber + "', '" + callerName + "', '" + problem + "', '" + code + "');";
                    SqlCommand cmd = new SqlCommand(sql, connection);
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        //MessageBox.Show("Enroll!");
                    }
                    else
                    {
                        MessageBox.Show("Database Error!");
                    }
                }
            }
            catch (SqlException er)
            {
                MessageBox.Show(er.ToString());
            }
        }

        public void sendDataTo112(string operatorName, string time, string location, string phoneNumber, string callerName, string problem, string code)
        {
            try
            {
                var cb = new SqlConnectionStringBuilder();
                cb.DataSource = "jangwonserver.database.windows.net";
                cb.UserID = "jangwon";
                cb.Password = "wkddnjs2!!";
                cb.InitialCatalog = "emergencycallDatabase";

                string sql;
                using (var connection = new SqlConnection(cb.ConnectionString))
                {
                    connection.Open();
                    sql = "insert into sendTo112 values('" + operatorName + "', '" + time + "', '" + location + "', '" + phoneNumber + "', '" + callerName + "', '" + problem + "', '" + code + "');";
                    SqlCommand cmd = new SqlCommand(sql, connection);
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        //MessageBox.Show("Enroll!");
                    }
                    else
                    {
                        MessageBox.Show("Database Error!");
                    }
                }
            }
            catch (SqlException er)
            {
                MessageBox.Show(er.ToString());
            }
        }

        public void sendDataTo110(string operatorName, string time, string location, string phoneNumber, string callerName, string problem, string code)
        {
            try
            {
                var cb = new SqlConnectionStringBuilder();
                cb.DataSource = "jangwonserver.database.windows.net";
                cb.UserID = "jangwon";
                cb.Password = "wkddnjs2!!";
                cb.InitialCatalog = "emergencycallDatabase";

                string sql;
                using (var connection = new SqlConnection(cb.ConnectionString))
                {
                    connection.Open();
                    sql = "insert into sendTo110 values('" + operatorName + "', '" + time + "', '" + location + "', '" + phoneNumber + "', '" + callerName + "', '" + problem + "', '" + code + "');";
                    SqlCommand cmd = new SqlCommand(sql, connection);
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        //MessageBox.Show("Enroll!");
                    }
                    else
                    {
                        MessageBox.Show("Database Error!");
                    }
                }
            }
            catch (SqlException er)
            {
                MessageBox.Show(er.ToString());
            }
        }

        public void printData()
        {
            try
            {
                var cb = new SqlConnectionStringBuilder();
                cb.DataSource = "jangwonserver.database.windows.net";
                cb.UserID = "jangwon";
                cb.Password = "wkddnjs2!!";
                cb.InitialCatalog = "emergencycallDatabase";

                string sql;
                using (var connection = new SqlConnection(cb.ConnectionString))
                {
                    connection.Open();
                    sql = "select * from emergencyCall";
                    SqlCommand cmd = new SqlCommand(sql, connection);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine("{0},{1},{2},{3},{4},{5},{6}", reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6));
                    }
                    reader.Close();
                    connection.Close();
                }
            }
            catch (SqlException er)
            {
                MessageBox.Show(er.ToString());
            }
        }
        
    }
}
