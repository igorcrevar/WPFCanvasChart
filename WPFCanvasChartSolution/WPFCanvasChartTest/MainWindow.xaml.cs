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
using IgorCrevar.WPFCanvasChart.ChartTypes;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    partial class MainWindow : Window, IWPFCanvasChartDrawer
    {
        private WPFCanvasChart cc = null;
        private bool isSmallActive = true;
        private List<Point> pointsList = new List<Point>();
        private Pen pen = new Pen(Brushes.Red, 1);
        private WPFCanvasChartSettings settings;

        public MainWindow()
        {
            InitializeComponent();
            this.AreGridEnabledButton.IsChecked = true;
            settings = new WPFCanvasChartSettings();
            settings.MaxXZoomStep = 200.0f;
            settings.MaxYZoomStep = 200.0f;
            pen.Freeze();
            GeneratePointsList();
        }

        private void SetMinMax()
        {
            if (isSmallActive)
            {
                cc.SetMinMax(2, 10, -5, 11.5);
            }
            else
            {
                double minX = pointsList[0].X;
                double maxX = pointsList[0].X;
                double minY = pointsList[0].Y;
                double maxY = pointsList[0].Y;
                foreach (var point in pointsList)
                {
                    minX = Math.Min(minX, point.X);
                    maxX = Math.Max(maxX, point.X);
                    minY = Math.Min(minY, point.Y);
                    maxY = Math.Max(maxY, point.Y);
                }

                cc.SetMinMax(minX, maxX, minY, maxY);
            }
        }

        public void Draw(DrawingContext ctx)
        {
            if (isSmallActive)
            {
                DrawSmall(ctx);
            }
            else
            {
                DrawBig(ctx);
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

        public string FormatXAxis(double x)
        {
            return ((int)x).ToString();
        }

        public string FormatYAxis(double y)
        {
            return y.ToString("F1");
        }

        public void OnWPFCanvasChartMouseUp(double x, double y)
        {
            MessageBox.Show(this, string.Format("Position {0} : {1}", FormatXAxis(x), FormatYAxis(y)),
                "WPFCanvasChart Left Mouse Click");
        }

        private void GeneratePointsList()
        {
            var rnd = new Random();
            int count = rnd.Next(8000) + 5000;
            for (int i = 0; i < count; ++i)
            {
                pointsList.Add(new Point(i, rnd.NextDouble() * 400000 - 100000));
            }
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            isSmallActive = !isSmallActive;
            Refresh();
        }

        private void Refresh()
        {
            if (cc != null)
            {
                cc.Dispose();
            }

            cc = new WPFCanvasChartIntegerXAxis(this.Canvas, HorizScroll, VertScroll, this, settings);
            SetMinMax();
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
    }
}
