using System.Data.SqlClient;
using System.Windows;

namespace ImagineCupProject
{
    class AzureDatabase
    {
        int[] timeCount = new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        int[] codeCount = new int[6] { 0, 0, 0, 0, 0, 0 };
        int maxTimeName;
        int maxCodeName;
        int max;
        string returnName;


        public void InsertData(EventVO currentEvent)
        {
            try
            {
                if (currentEvent.EventSecondANSWER == null)
                {
                    currentEvent.EventSecondANSWER = "empty";
                }
                if (currentEvent.EventEighthANSWER == null)
                {
                    currentEvent.EventEighthANSWER = "empty";
                }
                var cb = new SqlConnectionStringBuilder();
                /*
                cb.DataSource = "jangwonserver.database.windows.net";
                cb.UserID = "jangwon";
                cb.Password = "wkddnjs2!!";
                cb.InitialCatalog = "emergencycallDatabase";
                */
                cb.DataSource = "mynewserver-20171214.database.windows.net";
                cb.UserID = "ServerAdmin";
                cb.Password = "JaeShin12!";
                cb.InitialCatalog = "mySampleDatabase";
                string sql;
                using (var connection = new SqlConnection(cb.ConnectionString))
                {
                    connection.Open();

                    /*
                    sql = "insert into ECALL(EventOperator,eventStartTime,eventEndTime, eventLocation,eventPhoneNumber,eventCallerName,eventProblem,eventCode," +
                        "eventFirstAnswer,eventSecondAnswer,eventThirdAnswer,eventFourthAnswer,eventFifthAnswer,eventSixthAnswer,eventSeventhAnswer,eventEightAnswer) " +
                        "values(@EventOPERATOR,@EventSTARTTIME,@EventENDTIME,@EventLOCATION,@EventPHONENUMBER,@EventCALLERNAME,@EventPROBLEM,@EventCODE," +
                        "@FirstAnswer,@SecondAnswer,@ThirdAnswer,@FourthAnswer,@FifthAnswer,@SixthAnswer,@SeventhAnswer,@EightAnswer)";
*/
                    
                   sql = "insert into ECALL(EventOperator,eventStartTime,eventEndTime, eventLocation,eventPhoneNumber,eventCallerName,eventProblem,eventCode) " +
                      "values(@EventOPERATOR,@EventSTARTTIME,@EventENDTIME,@EventLOCATION,@EventPHONENUMBER,@EventCALLERNAME,@EventPROBLEM,@EventCODE)";
                      
                    SqlCommand cmd = new SqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@EventOPERATOR", currentEvent.EventOPERATOR);
                    cmd.Parameters.AddWithValue("@EventSTARTTIME", currentEvent.EventSTARTTIME);
                    cmd.Parameters.AddWithValue("@EventENDTIME", currentEvent.EventENDTIME);
                    cmd.Parameters.AddWithValue("@EventLOCATION", currentEvent.EventLOCATION);
                    cmd.Parameters.AddWithValue("@EventPHONENUMBER", currentEvent.EventPHONENUMBER);
                    cmd.Parameters.AddWithValue("@EventCALLERNAME", currentEvent.EventCALLERNAME);
                    cmd.Parameters.AddWithValue("@EventPROBLEM", currentEvent.EventPROBLEM);
                    cmd.Parameters.AddWithValue("@EventCODE", currentEvent.EventCODE);

                    /*
                    cmd.Parameters.AddWithValue("@FirstAnswer", currentEvent.EventFirstANSWER);
                    cmd.Parameters.AddWithValue("@SecondAnswer", currentEvent.EventSecondANSWER);
                    cmd.Parameters.AddWithValue("@ThirdAnswer", currentEvent.EventThirdANSWER);
                    cmd.Parameters.AddWithValue("@FourthAnswer", currentEvent.EventFourthANSWER);
                    cmd.Parameters.AddWithValue("@FifthAnswer", currentEvent.EventFifthANSWER);
                    cmd.Parameters.AddWithValue("@SixthAnswer", currentEvent.EventSixthANSWER);
                    cmd.Parameters.AddWithValue("@SeventhAnswer", currentEvent.EventSeventhANSWER);
                    cmd.Parameters.AddWithValue("@EightAnswer", currentEvent.EventEighthANSWER);
                    */

                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        //MessageBox.Show("Enroll!");
                    }
                    else
                    {
                        //MessageBox.Show("Database Error!");
                    }
                }

            }
            catch (SqlException er)
            {
                //MessageBox.Show(er.ToString());
            }
        }

        public void SendDataTo112(string operatorName, string time, string location, string phoneNumber, string callerName, string problem, string code)
        {
            try
            {
                var cb = new SqlConnectionStringBuilder();
                cb.DataSource = "mynewserver-20171214.database.windows.net";
                cb.UserID = "ServerAdmin";
                cb.Password = "JaeShin12!";
                cb.InitialCatalog = "mySampleDatabase";
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
                        //MessageBox.Show("Database Error!");
                    }
                }
            }
            catch (SqlException er)
            {
                //MessageBox.Show(er.ToString());
            }
        }

        public void SendDataTo110(string operatorName, string time, string location, string phoneNumber, string callerName, string problem, string code)
        {
            try
            {
                var cb = new SqlConnectionStringBuilder();
                cb.DataSource = "mynewserver-20171214.database.windows.net";
                cb.UserID = "ServerAdmin";
                cb.Password = "JaeShin12!";
                cb.InitialCatalog = "mySampleDatabase";

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
                        //MessageBox.Show("Database Error!");
                    }
                }
            }
            catch (SqlException er)
            {
                //MessageBox.Show(er.ToString());
            }
        }

        public int PrintTotalNumber()
        {
            int countCall = 0;
            try
            {
                var cb = new SqlConnectionStringBuilder();
                cb.DataSource = "mynewserver-20171214.database.windows.net";
                cb.UserID = "ServerAdmin";
                cb.Password = "JaeShin12!";
                cb.InitialCatalog = "mySampleDatabase";
                string sql;
                using (var connection = new SqlConnection(cb.ConnectionString))
                {
                    connection.Open();
                    sql = "select * from ECALL";
                    SqlCommand cmd = new SqlCommand(sql, connection);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        countCall = reader.GetInt32(0);

                    }
                    reader.Close();
                    connection.Close();

                }
            }
            catch (SqlException er)
            {
                //MessageBox.Show(er.ToString());
            }

            return countCall;
        }

        //Azure DB에서 데이터출력
        public string PrintData()
        {
            try
            {
                var cb = new SqlConnectionStringBuilder();
                cb.DataSource = "mynewserver-20171214.database.windows.net";
                cb.UserID = "ServerAdmin";
                cb.Password = "JaeShin12!";
                cb.InitialCatalog = "mySampleDatabase";

                string sql;
                using (var connection = new SqlConnection(cb.ConnectionString))
                {
                    connection.Open();
                    sql = "select * from ECALL";
                    SqlCommand cmd = new SqlCommand(sql, connection);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {

                        if (reader.GetString(2).ToString().Substring(12, 2).Equals("00") | reader.GetString(2).ToString().Substring(12, 2).Equals("01"))
                        {
                            timeCount[0]++;
                        }
                        else if (reader.GetString(2).ToString().Substring(12, 2).Equals("02") | reader.GetString(2).ToString().Substring(12, 2).Equals("03"))
                        {
                            timeCount[1]++;
                        }
                        else if (reader.GetString(2).ToString().Substring(12, 2).Equals("04") | reader.GetString(2).ToString().Substring(12, 2).Equals("05"))
                        {
                            timeCount[2]++;
                        }
                        else if (reader.GetString(2).ToString().Substring(12, 2).Equals("06") | reader.GetString(2).ToString().Substring(12, 2).Equals("07"))
                        {
                            timeCount[3]++;
                        }
                        else if (reader.GetString(2).ToString().Substring(12, 2).Equals("08") | reader.GetString(2).ToString().Substring(12, 2).Equals("09"))
                        {
                            timeCount[4]++;
                        }
                        else if (reader.GetString(2).ToString().Substring(12, 2).Equals("10") | reader.GetString(2).ToString().Substring(12, 2).Equals("11"))
                        {
                            timeCount[5]++;
                        }
                        else if (reader.GetString(2).ToString().Substring(12, 2).Equals("12") | reader.GetString(2).ToString().Substring(12, 2).Equals("13"))
                        {
                            timeCount[6]++;
                        }
                        else if (reader.GetString(2).ToString().Substring(12, 2).Equals("14") | reader.GetString(2).ToString().Substring(12, 2).Equals("15"))
                        {
                            timeCount[7]++;
                        }
                        else if (reader.GetString(2).ToString().Substring(12, 2).Equals("16") | reader.GetString(2).ToString().Substring(12, 2).Equals("17"))
                        {
                            timeCount[8]++;
                        }
                        else if (reader.GetString(2).ToString().Substring(12, 2).Equals("18") | reader.GetString(2).ToString().Substring(12, 2).Equals("19"))
                        {
                            timeCount[9]++;
                        }
                        else if (reader.GetString(2).ToString().Substring(12, 2).Equals("20") | reader.GetString(2).ToString().Substring(12, 2).Equals("21"))
                        {
                            timeCount[10]++;
                        }
                        else if (reader.GetString(2).ToString().Substring(12, 2).Equals("22") | reader.GetString(2).ToString().Substring(12, 2).Equals("23"))
                        {
                            timeCount[11]++;
                        }

                        if (reader.GetString(8).ToLower().Contains("terror") | reader.GetString(8).ToLower().Contains("gun"))
                        {
                            codeCount[0]++;
                        }
                        else if (reader.GetString(8).ToLower().Equals("disaster"))
                        {
                            codeCount[1]++;
                        }
                        else if (reader.GetString(8).ToLower().Equals("fire"))
                        {
                            codeCount[2]++;
                        }
                        else if (reader.GetString(8).ToLower().Equals("violence"))
                        {
                            codeCount[3]++;
                        }
                        else if (reader.GetString(8).ToLower().Equals("motor") | reader.GetString(8).ToLower().Equals("vehicle") | reader.GetString(8).ToLower().Equals("accidents"))
                        {
                            codeCount[4]++;
                        }
                        else
                        {
                            codeCount[5]++;
                        }
                    }
                    reader.Close();
                    connection.Close();
                }
                //maxTime값 구하기
                max = timeCount[0];
                maxTimeName = 0;
                for (int i = 1; i < 12; i++)
                {
                    if (timeCount[i] > max)
                    {
                        max = timeCount[i];
                        maxTimeName = i;
                    }

                }

                //maxCode값 구하기
                max = codeCount[0];
                maxCodeName = 0;
                for (int i = 1; i < 6; i++)
                {
                    if (codeCount[i] > max)
                    {
                        max = codeCount[i];
                        maxCodeName = i;
                    }

                }

                returnName = maxTimeName.ToString() + ' ' + maxCodeName.ToString();
            }
            catch (SqlException er)
            {
                //MessageBox.Show(er.ToString());
            }
            return returnName;
        }
    }
}
