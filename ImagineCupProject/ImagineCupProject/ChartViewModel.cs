using System;
using System.Collections.Generic;
using System.ComponentModel;
using ImagineCupProject.WPFCanvasChart;
using ImagineCupProject.WPFCanvasChart.Interpolators;
using System.Windows;
using System.Windows.Media;
using ImagineCupProject.WPFChartControl.Model;
using ImagineCupProject.WPFChartControl.Drawer;
using System.Data.SqlClient;

namespace ImagineCupProject
{
    class ChartViewModel : INotifyPropertyChanged
    {
        int code1;
        int code2;
        int code3;
        int code4;
        int code5;
        int code6;
        int time1, time2, time3, time4, time5, time6, time7, time8, time9, time10, time11, time12;
        class CustomInterpolator : WPFCanvasChartIntInterpolator
        {
            public override string Format(double value)
            {
                var months = new string[] {
                    "Code1", "Code2", "Code3", "Code4", "Code5", "Code6"
                };
                int v = (int)value;
                return v >= 0 && v < months.Length ? months[v] : string.Empty;
            }

            public override string FormatLongestValue()
            {
                return "Code1";
            }
        }

        private AbstractChartDrawer barChartDrawer;
        private AbstractChartDrawer lineSeriesChartDrawer;

        //Azure DB에서 데이터출력
        public void PrintData()
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

                        if(reader.GetString(1).ToString().Substring(12, 2).Equals("00") | reader.GetString(1).ToString().Substring(12, 2).Equals("01"))
                        {
                            time1++;
                        }
                        else if (reader.GetString(1).ToString().Substring(12, 2).Equals("02") | reader.GetString(1).ToString().Substring(12, 2).Equals("03"))
                        {
                            time2++;
                        }
                        else if (reader.GetString(1).ToString().Substring(12, 2).Equals("04") | reader.GetString(1).ToString().Substring(12, 2).Equals("05"))
                        {
                            time3++;
                        }
                        else if (reader.GetString(1).ToString().Substring(12, 2).Equals("06") | reader.GetString(1).ToString().Substring(12, 2).Equals("07"))
                        {
                            time4++;
                        }
                        else if (reader.GetString(1).ToString().Substring(12, 2).Equals("08") | reader.GetString(1).ToString().Substring(12, 2).Equals("09"))
                        {
                            time5++;
                        }
                        else if (reader.GetString(1).ToString().Substring(12, 2).Equals("10") | reader.GetString(1).ToString().Substring(12, 2).Equals("11"))
                        {
                            time6++;
                        }
                        else if (reader.GetString(1).ToString().Substring(12, 2).Equals("12") | reader.GetString(1).ToString().Substring(12, 2).Equals("13"))
                        {
                            time7++;
                        }
                        else if (reader.GetString(1).ToString().Substring(12, 2).Equals("14") | reader.GetString(1).ToString().Substring(12, 2).Equals("15"))
                        {
                            time8++;
                        }
                        else if (reader.GetString(1).ToString().Substring(12, 2).Equals("16") | reader.GetString(1).ToString().Substring(12, 2).Equals("17"))
                        {
                            time9++;
                        }
                        else if (reader.GetString(1).ToString().Substring(12, 2).Equals("18") | reader.GetString(1).ToString().Substring(12, 2).Equals("19"))
                        {
                            time10++;
                        }
                        else if (reader.GetString(1).ToString().Substring(12, 2).Equals("20") | reader.GetString(1).ToString().Substring(12, 2).Equals("21"))
                        {
                            time11++;
                        }
                        else if (reader.GetString(1).ToString().Substring(12, 2).Equals("22") | reader.GetString(1).ToString().Substring(12, 2).Equals("23"))
                        {
                            time12++;
                        }

                        if (reader.GetString(6).ToLower().Contains("terror"))
                        {
                            code1++;
                        }
                        else if (reader.GetString(6).ToLower().Equals("disaster"))
                        {
                            code2++;
                        }
                        else if (reader.GetString(6).ToLower().Equals("fire"))
                        {
                            code3++;
                        }
                        else if (reader.GetString(6).ToLower().Equals("violence"))
                        {
                            code4++;
                        }
                        else if (reader.GetString(6).ToLower().Equals("motor"))
                        {
                            code5++;
                        }
                        else
                        {
                            code6++;
                        }
                    }
                    reader.Close();
                    connection.Close();
                }
                // totalText.Text = "Operator JW receive " + totalNumber + " call";

            }
            catch (SqlException er)
            {
                MessageBox.Show(er.ToString());
            }
        }

        public ChartViewModel()
        {
            PrintData();
            var rnd = new Random();
            BarChartDrawer = new BarChartDrawer(new Point[]{
                new Point(0, code1),
                new Point(1, code2),
                new Point(2, code3),
                new Point(3, code4),
                new Point(4, code5),
                new Point(5, code6),
            })
            {
                VertScrollVisibility = Visibility.Collapsed,
                Legend = new LegendItem[]
                {
                    new LegendItem(Colors.Navy, ""),
                    new LegendItem(Colors.Blue, ""),
                    new LegendItem(Colors.Brown, ""),
                    new LegendItem(Colors.Chocolate, ""),
                    new LegendItem(Colors.Gray, ""),
                    new LegendItem(Colors.Tomato, ""),
                }
                ,
                YAxisText = "",
                XAxisText = "",
                Settings = new WPFCanvasChartSettings(),
                YAxisInterpolator = new WPFCanvasChartFloatInterpolator(),
                XAxisInterpolator = new CustomInterpolator(),
                FixedYMin = 0.0d,
                LegendWidth = 100.0d,
            };

            var serie1 = new List<Point>();

            serie1.Add(new Point(1, time1));
            serie1.Add(new Point(2, time2));
            serie1.Add(new Point(3, time3));
            serie1.Add(new Point(4, time4));
            serie1.Add(new Point(5, time5));
            serie1.Add(new Point(6, time6));
            serie1.Add(new Point(7, time7));
            serie1.Add(new Point(8, time8));
            serie1.Add(new Point(9, time9));
            serie1.Add(new Point(10, time10));
            serie1.Add(new Point(11, time11));
            serie1.Add(new Point(12, time12));

            LineSeriesChartDrawer = new LineSeriesChartDrawer(new List<IList<Point>>{
                serie1
            });

        }

        public AbstractChartDrawer BarChartDrawer
        {
            get
            {
                return barChartDrawer;
            }

            set
            {
                if (barChartDrawer != value)
                {
                    barChartDrawer = value;
                    OnPropertyChanged("BarChartDrawer");
                }
            }
        }

        public AbstractChartDrawer LineSeriesChartDrawer
        {
            get
            {
                return lineSeriesChartDrawer;
            }

            set
            {
                if (lineSeriesChartDrawer != value)
                {
                    lineSeriesChartDrawer = value;
                    OnPropertyChanged("LineSeriesChartDrawer");
                }
            }
        }

        #region INotifyPropertyChanged part
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion
    }
}
