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
        
        public LineSeriesChartDrawer(IList<IList<Point>> chartPoints, double lineTickness = 1.0d)
        {
            this.chartPoints = chartPoints;
            this.lineTickness = lineTickness;
        }

        public LineSeriesChartDrawer(IList<Point> chartPoints, double lineTickness = 1.0d)
            : this(new List<IList<Point>>() { chartPoints }, lineTickness)
        {
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
