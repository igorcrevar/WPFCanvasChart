using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using IgorCrevar.WPFChartControl.Model;

namespace IgorCrevar.WPFChartControl.Drawer
{
    public class LineSeriesChartDrawer : AbstractChartDrawer
    {
        private IList<IList<Point>> chartPoints;
        private double lineTickness;
        private Action<Point> onMouseDown;

        public LineSeriesChartDrawer(IList<IList<Point>> chartPoints, double lineTickness = 1.0d, Action<Point> onMouseDown = null)
        {
            this.chartPoints = chartPoints;
            this.lineTickness = lineTickness;
            this.onMouseDown = onMouseDown;
        }

        public LineSeriesChartDrawer(IList<Point> chartPoints, double lineTickness = 1.0d, Action<Point> onMouseDown = null)
            : this(new List<IList<Point>>() { chartPoints }, lineTickness, onMouseDown)
        {
        }

        protected override void OnUpdate()
        {
            if (Legend == null)
            {
                Legend = new List<LegendItem>();
                for (int i = 0; i < chartPoints.Count; ++i)
                {
                    Legend.Add(new LegendItem(Colors.Black, string.Empty));
                }
            }

            if (chartPoints.Count != Legend.Count)
            {
                throw new ArgumentException(string.Format(
                    "chartPoints.Count = {0} and Legend.Count = {1}. Lists must contains same number of elements",
                    chartPoints.Count, Legend.Count));
            }

            Point min = new Point(double.MaxValue, double.MaxValue);
            Point max = new Point(double.MinValue, double.MinValue);
            foreach (var serie in chartPoints)
            {
                foreach (var p in serie)
                {
                    min.X = Math.Min(min.X, p.X);
                    min.Y = Math.Min(min.Y, p.Y);

                    max.X = Math.Max(max.X, p.X);
                    max.Y = Math.Max(max.Y, p.Y);
                }
            }

            if (min.X == double.MaxValue)
            {
                Chart.SetMinMax(0, 10, 0, 10);
            }
            else
            {
                if (min.Y == max.Y)
                {
                    max.Y += 1.0d;
                    min.Y -= 1.0d;
                }

                Chart.SetMinMax(min.X, max.X, min.Y, max.Y);
            }

            Chart.DrawChart();
        }

        public override void Draw(DrawingContext ctx)
        {
            for (int j = 0; j < chartPoints.Count; ++j)
            {
                var seriePoints = chartPoints[j];
                if (seriePoints.Count < 2)
                {
                    continue;
                }

                Pen pen = new Pen(new SolidColorBrush(Legend[j].Color), lineTickness);
                pen.Freeze();
                Point prevPoint = Chart.Point2ChartPoint(seriePoints[0]);
                for (int i = 1; i < seriePoints.Count; ++i)
                {
                    var currPoint = Chart.Point2ChartPoint(seriePoints[i]);
                    ctx.DrawLine(pen, prevPoint, currPoint);
                    prevPoint = currPoint;
                }
            }
        }

        public override void OnChartMouseDown(double x, double y)
        {
            if (onMouseDown != null)
            {
                onMouseDown(new Point(x, y));
            }
        }
    }
}
