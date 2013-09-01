using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace IgorCrevar.WPFBarChartControl.Drawer
{
    class BarChartDrawer : AbstractBarChartDrawer
    {
        protected override void OnModelUpdate()
        {
            if (Model.Data.Count == 0)
            {
                Chart.SetMinMax(0, 1, 0, 10);
            }
            else
            {
                Point min = Model.Data[0];
                Point max = Model.Data[0];
                for (int i = 1; i < Model.Data.Count; ++i)
                {
                    var p = Model.Data[i];
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
            if (Model.Data.Count == 0)
            {
                return;
            }

            var points = Model.Data.Select(i => Chart.Point2ChartPoint(i)).ToList();
            double width = 0.0d;
            if (Model.Data.Count == 1)
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

        public override void OnWPFCanvasChartMouseUp(double x, double y)
        {
        }
    }
}
