using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;

namespace IgorCrevar.WPFChartControl.Drawer
{
    public class StackedBarItem
    {
        public StackedBarItem(double xvalue, IEnumerable<double> yvalues)
        {
            XValue = xvalue;
            YValues = yvalues.Select(i => new StackedBarYValue(i)).ToList();
        }

        public double XValue { get; set; }
        internal List<StackedBarYValue> YValues { get; set; }
    }

    internal class StackedBarYValue
    {
        public StackedBarYValue(double value)
        {
            Value = value;
        }

        public Color Color { get; set; }
        public double Value { get; set; }
    }

    public class StackedBarChartDrawer : AbstractChartDrawer
    {
        private IList<StackedBarItem> values;
        private bool isMinSet = false;
        private double minY;

        public StackedBarChartDrawer(IList<StackedBarItem> values)
        {
            this.values = values;
            FixedYMin = double.NaN;
        }

        protected override void OnUpdate()
        {
            Point min = new Point(double.MaxValue, double.MaxValue);
            Point max = new Point(double.MinValue, double.MinValue);
            foreach (var v in values)
            {
                min.X = Math.Min(min.X, v.XValue);
                max.X = Math.Max(max.X, v.XValue);
                if (v.YValues.Count < 1 || v.YValues.Count != Legend.Count)
                {
                    throw new ArgumentException(string.Format(
                        "StackedBarItem.YValues.Count = {0} and Legend.Count = {1}. Lists must contains same number of elements and > 0",
                        v.YValues.Count, Legend.Count));
                }

                // fix colors
                for (int i = 0; i < Legend.Count; ++i)
                {
                    v.YValues[i].Color = Legend[i].Color;
                }

                // sort
                v.YValues.Sort((a, b) => a.Value.CompareTo(b.Value));

                min.Y = Math.Min(min.Y, v.YValues[0].Value);
                max.Y = Math.Max(max.Y, v.YValues[v.YValues.Count - 1].Value);
            }

            if (double.MinValue != max.X && double.MinValue != max.Y)
            {
                if (min.Y == max.Y)
                {
                    max.Y += 1.0d;
                }

                minY = double.IsNaN(FixedYMin) || FixedYMin > min.Y ? min.Y : FixedYMin;
                Chart.SetMinMax(min.X - 1, max.X + 1, minY, max.Y);
                isMinSet = true;
            }
            else
            {
                Chart.SetMinMax(0, 2, 0, 5);
            }
        }

        public override void Draw(System.Windows.Media.DrawingContext ctx)
        {
            if (values.Count == 0 || !isMinSet)
            {
                return;
            }

            var xPoints = values.Select(i => Chart.Point2ChartPoint(new Point(i.XValue, 0.0d)).X).ToArray();
            double width = 0.0d;
            if (xPoints.Length == 1)
            {
                var p1 = Chart.Point2ChartPoint(new Point(xPoints[0] - 1, 0));
                var p2 = Chart.Point2ChartPoint(new Point(xPoints[0] + 1, 0));
                width = p2.X - p1.X;
                width = width * 1 / 3;
            }
            else
            {
                width = xPoints[1] - xPoints[0];
                width = width * 2 / 3;
            }

            for (int i = 0; i < values.Count; ++i)
            {
                var yValues = values[i].YValues;
                double xCoord = xPoints[i];
                double yStart = Chart.Point2ChartPoint(new Point(0.0, minY)).Y;
                foreach (var yVal in yValues)
                {
                    var yCoord = Chart.Point2ChartPoint(new Point(0.0d, yVal.Value)).Y;
                    Brush brush = new SolidColorBrush(yVal.Color);
                    Pen pen = new Pen(brush, 1.0d);
                    ctx.DrawRectangle(brush, pen, new Rect(xCoord - width / 2, yCoord, width, yStart - yCoord));
                    yStart = yCoord;
                }
            }
        }

        public override void OnChartMouseDown(double x, double y)
        {
        }

        /// <summary>
        /// set to NaN if you want minimum depends on points data, otherwise set fixed y min
        /// </summary>
        public double FixedYMin { get; set; }
    }
}
