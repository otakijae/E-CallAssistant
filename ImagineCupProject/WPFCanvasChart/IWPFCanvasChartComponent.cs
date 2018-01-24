using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using ImagineCupProject.WPFCanvasChart.Interpolators;
using System.Windows;

namespace ImagineCupProject.WPFCanvasChart
{
    public interface IWPFCanvasChartComponent : IDisposable
    {
        void Init(Canvas canvas, IWPFCanvasChartDrawer drawer,
           WPFCanvasChartSettings settings, IWPFCanvasChartInterpolator xAxisInterpolator, IWPFCanvasChartInterpolator yAxisInterpolator);
        void SetMinMax(double minX, double maxX, double minY, double maxY);
        void DrawChart();
        Point Point2ChartPoint(Point p);
        Point ChartPoint2Point(Point p);
    }
}
