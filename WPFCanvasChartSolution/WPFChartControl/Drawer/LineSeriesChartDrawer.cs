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
        public struct DotsSettings
        {
            public DotsSettings(Brush brush, Pen pen, double size, bool isEnabled = true)
            {
                DotBrush = brush;
                DotBrush.Freeze();
                DotPen = pen;
                DotPen.Freeze();
                Size = size;
                IsEnabled = isEnabled;
            }

            public Brush DotBrush;
            public Pen DotPen;
            public double Size;
            public bool IsEnabled;
        }

        private IList<IList<Point>> chartPoints;
        public double LineTickness { get; set; }
        public DotsSettings Dots { get; set; }
        
        public LineSeriesChartDrawer(IList<IList<Point>> chartPoints)
        {
            this.chartPoints = chartPoints;
            Dots = new DotsSettings(new SolidColorBrush(), new Pen(), 1.0d, false);
            this.LineTickness = 1.0d;
        }

        public LineSeriesChartDrawer(IList<Point> chartPoints)
            : this(new List<IList<Point>>() { chartPoints })
        {
        }

        private void DrawDot(Point point, DrawingContext ctx)
        {
            if (Dots.IsEnabled)
            {
                ctx.DrawEllipse(Dots.DotBrush, Dots.DotPen, point, Dots.Size, Dots.Size);
            }
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

                Pen pen = new Pen(new SolidColorBrush(Legend[j].Color), LineTickness);
                pen.Freeze();
                Point prevPoint = Chart.Point2ChartPoint(seriePoints[0]);
                DrawDot(prevPoint, ctx);
                for (int i = 1; i < seriePoints.Count; ++i)
                {
                    var currPoint = Chart.Point2ChartPoint(seriePoints[i]);
                    ctx.DrawLine(pen, prevPoint, currPoint);
                    prevPoint = currPoint;
                    DrawDot(prevPoint, ctx);
                }
            }
        }

        public override MinMax GetMinMax()
        {
            MinMax minMax = new MinMax(true);
            foreach (var serie in chartPoints)
            {
                foreach (var p in serie)
                {
                    minMax.Update(p, p);
                }
            }

            if (minMax.minY == minMax.maxY)
            {
                minMax.maxY += 1.0d;
                minMax.minY -= 1.0d;
            }

            return minMax;
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
        }
    }
}
