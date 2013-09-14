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
       
        public StackedBarChartDrawer(IList<StackedBarItem> values)
        {
            this.values = values;
        }

        protected override void  OnUpdate()
        {
            if (Legend == null)
            {
                throw new ArgumentNullException("Legend is null. Can not be null for StackedBar");
            } 
        }

        public override void Draw(System.Windows.Media.DrawingContext ctx)
        {
            if (values.Count == 0 || double.MinValue == MinMaxValue.maxX || double.MinValue == MinMaxValue.maxY)
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
                double yStart = Chart.Point2ChartPoint(new Point(0.0, MinMaxValue.minY)).Y;
                foreach (var yVal in yValues)
                {
                    var yCoord = Chart.Point2ChartPoint(new Point(0.0d, yVal.Value)).Y;
                    if (yStart - yCoord > 0.0d)
                    {
                        Brush brush = new SolidColorBrush(yVal.Color);
                        Pen pen = new Pen(brush, 1.0d);
                        ctx.DrawRectangle(brush, pen, new Rect(xCoord - width / 2, yCoord, width, yStart - yCoord));
                    }
                    yStart = yCoord;
                }
            }
        }

        public override MinMax GetMinMax()
        {
            MinMax minMax = new MinMax(true);
            foreach (var v in values)
            {
                minMax.Update(v.XValue, v.XValue, double.MaxValue, double.MinValue);
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
                minMax.Update(double.MaxValue, double.MinValue, v.YValues[0].Value, v.YValues[v.YValues.Count - 1].Value);
            }

            if (double.MinValue != minMax.maxX && double.MinValue != minMax.maxY)
            {
                minMax.minX -= 1;
                minMax.maxX += 1;
            }

            return minMax;
        }
    }
}
