using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Media;

namespace IgorCrevar.WPFCanvasChart.ChartTypes
{
    /// <summary>
    /// This is not right choice(using inheritance to achieve different axis drawings), because we will have 3 implementations of base class
    /// last one which will override DrawXAxis and DrawYAxis will have same code as first and second subclass.
    /// Its better to have IAxisDrawer interface and adecvate implementations
    /// </summary>
    public class WPFCanvasChartIntegerXAxis : WPFCanvasChart
    {
        public WPFCanvasChartIntegerXAxis(Canvas canvas, ScrollBar horizScrollBar, ScrollBar vertScrollBar, IWPFCanvasChartDrawer drawer,
            WPFCanvasChartSettings settings)
            : base(canvas, horizScrollBar, vertScrollBar, drawer, settings)
        {
        }

        protected override void DrawXAxis(System.Windows.Media.DrawingContext ctx, System.Windows.Media.DrawingContext chartCtx)
        {
            // greater zoom has more steps
            int noOfSteps = GetNoOfStepsForXAxis();
            int xAxisDiff = (int)(maxX - minX);
            noOfSteps = Math.Min(xAxisDiff, noOfSteps);
            for (int i = 0; i <= noOfSteps; ++i)
            {
                int currentValue = (int)Math.Round(xAxisDiff * (float)i / noOfSteps) + (int)minX;
                var currentPosition = Point2ChartPoint(new Point(currentValue, 0.0));
                DrawVerticalGrid(chartCtx, currentPosition.X);
                // draw axis value at current step
                if (IsXInsideVisiblePart(currentPosition.X))
                {
                    DrawXAxisText(ctx, currentValue, currentPosition.X);
                }
            }
        }
    }
}
