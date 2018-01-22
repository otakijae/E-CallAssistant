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
                    "January", "February", "March", "April", "Maj", "June"
                };
                int v = (int)value;
                return v >= 0 && v < months.Length ? months[v] : string.Empty;
            }

            public override string FormatLongestValue()
            {
                return "February";
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
                    new LegendItem(Colors.Blue, "Programmers"),
                    new LegendItem(Colors.Blue, "Designers"),
                    new LegendItem(Colors.Blue, "Admins"),
                    new LegendItem(Colors.Brown, "Management"),
                    new LegendItem(Colors.Blue, "Man1231agement"),
                    new LegendItem(Colors.Red, "Mana12333gement"),
                }
                ,
                YAxisText = "Power",
                XAxisText = "Month",
                Settings = new WPFCanvasChartSettings(),
                YAxisInterpolator = new WPFCanvasChartFloatInterpolator(),
                XAxisInterpolator = new CustomInterpolator(),
                FixedYMin = 0.0d,
                LegendWidth = 120.0d,
            };

            var serie1 = new List<Point>();
            for (int i = 1; i < 12; ++i)
            {
                serie1.Add(new Point(i, rnd.NextDouble() * 100));
            }

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
