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

        public override void Draw(DrawingContext ctx)
        {
            if (chartPoints.Count == 0)
            {
                return;
            }

            var points = chartPoints.Select(i => Chart.Point2ChartPoint(i)).ToArray();
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

            double yStart = Chart.Point2ChartPoint(new Point(0.0, MinMaxValue.minY)).Y;
            for (int i = 0; i < points.Length; ++i)
            {
                Point p = points[i];
                Brush brush = new SolidColorBrush(Legend[i].Color);
                Pen pen = new Pen(brush, 1.0d);
                ctx.DrawRectangle(brush, pen, new Rect(p.X - width / 2, p.Y, width, yStart - p.Y));
            }
        }

        public override MinMax GetMinMax()
        {
            MinMax minMax = new MinMax(true);
            if (chartPoints.Count == 0)
            {
                return minMax;
            }

            foreach (var p in chartPoints)
            {
                minMax.Update(p, p);
            }
           
            minMax.minX -= 1;
            minMax.maxX += 1;
            return minMax;
        }

        protected override void OnUpdate()
        {
            if (Legend == null)
            {
                Legend = new List<LegendItem>();
                for (int i = 0; i < chartPoints.Count; ++i)
                {
                    Legend.Add(new LegendItem(Colors.Blue, string.Empty));
                }
            }

            if (chartPoints.Count != Legend.Count)
            {
                throw new ArgumentException(string.Format(
                    "chartPoints.Count = {0} and Legend.Count = {1}. Lists must contains same number of elements",
                    chartPoints.Count, Legend.Count));
            }
        }
    }
}
