using System;
using System.Collections.Generic;
using System.ComponentModel;
using ImagineCupProject.WPFCanvasChart;
using ImagineCupProject.WPFCanvasChart.Interpolators;
using System.Windows;
using System.Windows.Media;
using ImagineCupProject.WPFChartControl.Model;
using ImagineCupProject.WPFChartControl.Drawer;

namespace ImagineCupProject
{
    class ChartViewModel : INotifyPropertyChanged
    {
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

        public ChartViewModel()
        {
            var rnd = new Random();
            BarChartDrawer = new BarChartDrawer(new Point[]{
                new Point(0, 10),
                new Point(1, 12),
                new Point(2, 10),
                new Point(3, 20),
                new Point(4, 4),
                new Point(5, 10),
            })
            {
                VertScrollVisibility = Visibility.Collapsed,
                Legend = new LegendItem[]
                {
                    new LegendItem(Colors.Navy, "Programmers"),
                    new LegendItem(Colors.Blue, "Designers"),
                    new LegendItem(Colors.Brown, "Admins"),
                    new LegendItem(Colors.Chocolate, "Management"),
                    new LegendItem(Colors.Gray, "Man1231agement"),
                    new LegendItem(Colors.Tomato, "Mana12333gement"),
                }
                ,
                YAxisText = "",
                XAxisText = "",
                Settings = new WPFCanvasChartSettings(),
                YAxisInterpolator = new WPFCanvasChartFloatInterpolator(),
                XAxisInterpolator = new CustomInterpolator(),
                FixedYMin = 0.0d,
                LegendWidth = 120.0d,
            };

            var serie1 = new List<Point>();

            serie1.Add(new Point(0, 0));
            serie1.Add(new Point(1, 10));
            serie1.Add(new Point(2, 15));
            serie1.Add(new Point(3, 13));
            serie1.Add(new Point(4, 15));
            serie1.Add(new Point(5, 18));
            serie1.Add(new Point(6, 18));
            serie1.Add(new Point(7, 20));
            serie1.Add(new Point(8, 13));
            serie1.Add(new Point(9, 10));


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
