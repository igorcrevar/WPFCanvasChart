using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IgorCrevar.WPFCanvasChart.Interpolators
{
    /// <summary>
    /// Axis values and grids are interpolated with implementators of this interface
    /// </summary>
    public interface IWPFCanvasChartInterpolator
    {
        void Execute(double min, double max, int noOfSteps, Action<double> action);
        string Format(double value);
        /// <summary>
        /// This method is used by DetermineMargins method inside WPFCanvasChart class
        /// Just return null if you not sure what to return as result
        /// </summary>
        /// <returns>string representation of longest value on axis</returns>
        string FormatLongestValue();
    }
}
