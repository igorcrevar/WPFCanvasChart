using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace IgorCrevar.WPFChartControl.Drawer
{
    public class MultiChartDrawer : AbstractChartDrawer
    {
        private IEnumerable<AbstractChartDrawer> drawers;

        public MultiChartDrawer(IEnumerable<AbstractChartDrawer> drawers)
        {
            this.drawers = drawers;
        }

        public override void Draw(System.Windows.Media.DrawingContext ctx)
        {
            foreach (var it in drawers)
            {
                it.Draw(ctx);
            }
        }

        public override void OnChartMouseDown(double x, double y)
        {
            foreach (var it in drawers)
            {
                it.OnChartMouseDown(x, y);
            }
        }

        public override MinMax GetMinMax()
        {
            MinMax minMax = new MinMax(true);
            foreach (var it in drawers)
            {
                minMax.Update(it.GetMinMax());
            }

            return minMax;
        }
    }
}
