using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace IgorCrevar.WPFCanvasChart
{
    public interface IWPFCanvasChartDrawer
    {
        void Draw(DrawingContext ctx);
        void OnChartMouseDown(double x, double y);
    }
}
