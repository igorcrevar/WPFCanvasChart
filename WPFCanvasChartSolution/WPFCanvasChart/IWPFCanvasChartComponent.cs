using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using IgorCrevar.WPFCanvasChart.Interpolators;
using System.Windows;

namespace IgorCrevar.WPFCanvasChart
{
    public interface IWPFCanvasChartComponent : IDisposable
    {
        void Init(Canvas canvas, ScrollBar horizScrollBar, ScrollBar vertScrollBar, IWPFCanvasChartDrawer drawer,
            WPFCanvasChartSettings settings, IWPFCanvasChartInterpolator xAxisInterpolator, IWPFCanvasChartInterpolator yAxisInterpolator);
        void SetMinMax(double minX, double maxX, double minY, double maxY);
        void DrawChart();
        Point Point2ChartPoint(Point p);
        Point ChartPoint2Point(Point p);
    }
}
