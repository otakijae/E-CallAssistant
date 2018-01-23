using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;

namespace ImagineCupProject
{
    /// <summary>
    /// TotalPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TotalPage : Page
    {
        int totalNumber=0;
        int code1 = 0;
        int code2 = 0;
        public TotalPage()
        {
            InitializeComponent();
            this.DataContext = new ChartViewModel();
            printData();
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
                        //Console.WriteLine("{0},{1},{2},{3},{4},{5},{6}", reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6));
                        totalNumber++;
                        if(reader.GetString(6).Equals("fire"))
                        {
                            ;
                            code1++;
                        }
                        else if(reader.GetString(6).Equals("firefire"))
                        {
                            code2++;
                        }
                    }
                    reader.Close();
                    connection.Close();
                }
                operatorReceiveNumText.Text = "TotalNumber = " + totalNumber + "\nCode1 Number = " + code1 + "\nCode2 Number = " + code2;
                operatorReceiveNumText.Text = "   operator JW received " + totalNumber + " call"; 
               // totalText.Text = "Operator JW receive " + totalNumber + " call";

            }
            catch (SqlException er)
            {
                MessageBox.Show(er.ToString());
            }
        }
    }
}
