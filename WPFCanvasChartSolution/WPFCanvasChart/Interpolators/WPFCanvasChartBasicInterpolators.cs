﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IgorCrevar.WPFCanvasChart.Interpolators
{
    public class WPFCanvasChartFloatInterpolator : IWPFCanvasChartInterpolator
    {
        public void Execute(double min, double max, int noOfSteps, Action<double> action)
        {
            double valueStep = Math.Abs(max - min) / noOfSteps;
            double currentValue = min;
            for (int i = 0; i <= noOfSteps; ++i)
            {
                action(currentValue);
                currentValue += valueStep;
            }
        }
    }

    public class WPFCanvasChartIntInterpolator : IWPFCanvasChartInterpolator
    {
        public void Execute(double min, double max, int noOfSteps, Action<double> action)
        {
            // greater zoom has more steps
            double stepDouble = (max - min) / noOfSteps + 0.5d;
            int step = Math.Max(1, (int)stepDouble);
            for (int i = (int)min; i <= (int)max; i += step)
            {
                action(i);
            }
        }
    }
}