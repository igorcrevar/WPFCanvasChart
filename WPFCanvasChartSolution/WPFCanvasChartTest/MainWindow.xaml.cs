using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using IgorCrevar.WPFCanvasChart;
using IgorCrevar.WPFCanvasChart.Interpolators;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    partial class MainWindow : Window, IWPFCanvasChartDrawer
    {
        enum ChartType
        {
            None, Big, Small, Bar
        };

        private IWPFCanvasChartComponent cc = null;
        private List<Point> pointsList = new List<Point>();
        private Pen pen = new Pen(Brushes.Red, 1);
        private Brush brush = Brushes.Red;
        private WPFCanvasChartSettings settings;
        private ChartType chartType = ChartType.None;
        private IWPFCanvasChartInterpolator xAxisInterpolator;
        private IWPFCanvasChartInterpolator yAxisInterpolator;

        public MainWindow()
        {
            InitializeComponent();
            this.AreGridEnabledButton.IsChecked = true;
            settings = new WPFCanvasChartSettings();
            settings.MaxXZoomStep = 200.0f;
            settings.MaxYZoomStep = 200.0f;
            pen.Freeze();
            brush.Freeze();
            this.Loaded += (sender, e) =>
            {
                // chart must created after all UI elements are loaded (canvas, scroll bars, etc...)
                settings.HandleSizeChanged = false;
                settings.FontSize = 4;
                settings.PenForGrid = new Pen((Brush)new BrushConverter().ConvertFromString("#66000000"), 0.3);
                settings.PenForAxis = new Pen((Brush)new BrushConverter().ConvertFromString("#CC000000"), 0.5);
                xAxisInterpolator = new WPFCanvasChartIntInterpolator();
                yAxisInterpolator = new WPFCanvasChartFloatInterpolator();
                cc = new WPFCanvasChartComponent();
                cc.Init(this.Canvas, HorizScroll, VertScroll, this, settings, xAxisInterpolator, yAxisInterpolator);
                cc.SetMinMax(-5, 5, 10, 20);
                cc.DrawChart();
            };

            this.Closed += (sender, e) =>
            {
                cc.Dispose();
            };
        }

        public void Draw(DrawingContext ctx)
        {
            switch (chartType)
            {
                case ChartType.Small:
                    DrawSmall(ctx);
                    break;
                case ChartType.Big:
                    DrawBig(ctx);
                    break;
                case ChartType.Bar:
                    DrawBar(ctx);
                    break;
                default:
                    // do nothing
                    break;
            }
        }

        private void DrawBar(DrawingContext ctx)
        {
            var points = pointsList.ConvertAll(i => cc.Point2ChartPoint(i));
            double width = points[1].X - points[0].X;
            width = width * 2 / 3;
            double yStart = cc.Point2ChartPoint(new Point(0.0, 0.0)).Y;
            foreach (var p in points)
            {
                ctx.DrawRectangle(brush, pen, new Rect(p.X - width / 2, p.Y, width, yStart - p.Y));
            }
        }

        private void DrawBig(DrawingContext ctx)
        {
            Point start = cc.Point2ChartPoint(pointsList[0]);
            for (int i = 1; i < pointsList.Count; i++)
            {
                Point end = cc.Point2ChartPoint(pointsList[i]);
                ctx.DrawLine(pen, start, end);
                start = end;
            }
        }

        public void DrawSmall(DrawingContext ctx)
        {
            var p1 = cc.Point2ChartPoint(new Point(2, -3));
            var p2 = cc.Point2ChartPoint(new Point(4, 10));
            ctx.DrawLine(pen, p1, p2);

            p1 = cc.Point2ChartPoint(new Point(4, 10));
            p2 = cc.Point2ChartPoint(new Point(7, -1.5));
            ctx.DrawLine(pen, p1, p2);
        }

        public void OnChartMouseDown(double x, double y)
        {
            MessageBox.Show(this, string.Format("Position {0} : {1}", xAxisInterpolator.Format(x), yAxisInterpolator.Format(y)),
                "WPFCanvasChart Left Mouse Click");
        }

        private void Refresh()
        {
            cc.DrawChart();
        }

        private void AreGridEnabledButton_Click(object sender, RoutedEventArgs e)
        {
            settings.AreGridsEnabled = !settings.AreGridsEnabled;
            Refresh();
        }

        private void ZoomBothAtSameTime_Click(object sender, RoutedEventArgs e)
        {
            settings.ZoomXYAtSameTime = !settings.ZoomXYAtSameTime;
        }

        private void SmallChartMenuItem_Click(object sender, RoutedEventArgs e)
        {
            settings.CoordXSteps = 10;
            chartType = ChartType.Small;
            cc.SetMinMax(2, 10, -5, 11.5);
            Refresh();
        }

        private void BigChartMenuItem_Click(object sender, RoutedEventArgs e)
        {
            settings.CoordXSteps = 10;
            chartType = ChartType.Big;

            pointsList.Clear();
            var rnd = new Random();
            int count = rnd.Next(1000) + 5000;
            int startX = rnd.Next(100) - 50;
            for (int i = 0; i < count; ++i)
            {
                pointsList.Add(new Point(i + startX, rnd.NextDouble() * 400000 - 100000));
            }

            Point min = pointsList[0];
            Point max = pointsList[0];
            foreach (var point in pointsList)
            {
                min.X = Math.Min(min.X, point.X);
                max.X = Math.Max(max.X, point.X);
                min.Y = Math.Min(min.Y, point.Y);
                max.Y = Math.Max(max.Y, point.Y);
            }

            cc.SetMinMax(min.X, max.X, min.Y, max.Y);
            Refresh();
        }

        private void BarChartMenuItem_Click(object sender, RoutedEventArgs e)
        {
            settings.CoordXSteps = 4 + new Random().Next(4);
            chartType = ChartType.Bar;
            var rnd = new Random();
            double maxY = double.MinValue;
            pointsList.Clear();
            for (int i = 0; i < settings.CoordXSteps; ++i)
            {
                Point p = new Point(i + 1, rnd.NextDouble() * 100);
                maxY = Math.Max(maxY, p.Y);
                pointsList.Add(p);
            }

            cc.SetMinMax(0, settings.CoordXSteps + 1, 0, maxY * 10 / 9);

            Refresh();
        }

        public void OnChartMouseOver(double x, double y)
        {
        }
    }
}
