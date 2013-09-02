using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using IgorCrevar.WPFChartControl.Model;
using System.Collections.Generic;

namespace IgorCrevar.WPFChartControl.Drawer
{
    public class BarChartDrawer : AbstractChartDrawer
    {
        private IList<Point> chartPoints;

        public BarChartDrawer(IList<Point> chartPoints)
        {
            this.chartPoints = chartPoints;
        }

        protected override void OnModelUpdate()
        {
            if (chartPoints.Count != Model.Legend.Count)
            {
                throw new ArgumentException(string.Format(
                    "chartPoints.Count = {0} and ChartModel instance Legend.Count = {1}. Lists must contains same number of elements",
                    chartPoints.Count, Model.Legend.Count));
            }

            if (chartPoints.Count == 0)
            {
                Chart.SetMinMax(0, 1, 0, 10);
            }
            else
            {
                Point min = chartPoints[0];
                Point max = chartPoints[0];
                for (int i = 1; i < chartPoints.Count; ++i)
                {
                    var p = chartPoints[i];
                    min.X = Math.Min(min.X, p.X);
                    min.Y = Math.Min(min.Y, p.Y);

                    max.X = Math.Max(max.X, p.X);
                    max.Y = Math.Max(max.Y, p.Y);
                }

                if (min.Y == max.Y)
                {
                    max.Y += 1.0d;
                }

                Chart.SetMinMax(min.X - 1, max.X + 1, double.IsNaN(Model.FixedYMin) ? min.Y : Model.FixedYMin, max.Y);
            }

            Chart.DrawChart();
        }

        public override void Draw(DrawingContext ctx)
        {
            if (chartPoints.Count == 0)
            {
                return;
            }

            var points = chartPoints.Select(i => Chart.Point2ChartPoint(i)).ToList();
            double width = 0.0d;
            if (chartPoints.Count == 1)
            {
                var p1 = Chart.Point2ChartPoint(new Point(points[0].X - 1, 0));
                var p2 = Chart.Point2ChartPoint(new Point(points[0].X + 1, 0));
                width = p2.X - p1.X;
                width = width * 1 / 3;
            }
            else
            {
                width = points[1].X - points[0].X;
                width = width * 2 / 3;
            }

            double yStart = Chart.Point2ChartPoint(new Point(0.0, 0.0)).Y;
            for (int i = 0; i < points.Count; ++i)
            {
                Point p = points[i];
                Brush brush = new SolidColorBrush(Model.Legend[i].Color);
                Pen pen = new Pen(brush, 1.0d);
                ctx.DrawRectangle(brush, pen, new Rect(p.X - width / 2, p.Y, width, yStart - p.Y));
            }
        }

        public override void OnChartMouseDown(double x, double y)
        {
        }
    }
}
