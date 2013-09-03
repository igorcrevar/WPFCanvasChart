using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IgorCrevar.WPFCanvasChart.Interpolators
{
    public class WPFCanvasChartFloatInterpolator : IWPFCanvasChartInterpolator
    {
        private string formatString;

        public WPFCanvasChartFloatInterpolator(string formatString = "F2")
        {
            this.formatString = formatString;
        }

        public IEnumerable<double> GetSteps(double min, double max, int noOfSteps)
        {
            double valueStep = Math.Abs(max - min) / noOfSteps;
            double currentValue = min;
            for (int i = 0; i <= noOfSteps; ++i)
            {
                yield return currentValue;
                currentValue += valueStep;
            }
        }

        public virtual string Format(double value)
        {
            return value.ToString(formatString);
        }

        public virtual string FormatLongestValue()
        {
            return null;
        }
    }

    public class WPFCanvasChartIntInterpolator : IWPFCanvasChartInterpolator
    {
        public IEnumerable<double> GetSteps(double min, double max, int noOfSteps)
        {
            // greater zoom has more steps
            double stepDouble = (max - min) / noOfSteps + 0.5d;
            int step = Math.Max(1, (int)stepDouble);
            for (int i = (int)min; i <= (int)max; i += step)
            {
                yield return i;
            }
        }

        public virtual string Format(double value)
        {
            return ((int)value).ToString();
        }

        public virtual string FormatLongestValue()
        {
            return null;
        }
    }

    /// <summary>
    /// Same as WPFCanvasChartIntInterpolator but outputs all empty strings on axis
    /// </summary>
    public class WPFCanvasChartIntEmptyInterpolator : WPFCanvasChartIntInterpolator
    {
        public override string Format(double value)
        {
            return string.Empty;
        }
    }
}
