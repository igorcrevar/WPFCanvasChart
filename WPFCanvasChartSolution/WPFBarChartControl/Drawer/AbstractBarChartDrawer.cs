using System;
using IgorCrevar.WPFCanvasChart;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using IgorCrevar.WPFBarChartControl.Model;

namespace IgorCrevar.WPFBarChartControl.Drawer
{
    /// <summary>
    /// Base class for all bar charts drawers
    /// </summary>
    abstract class AbstractBarChartDrawer : IWPFCanvasChartDrawer, IDisposable
    {
        private WPFCanvasChart.WPFCanvasChart chart = null;
        private BarChartModel model;

        protected WPFCanvasChart.WPFCanvasChart Chart { get { return chart; } }
        protected BarChartModel Model { get { return model; } }
                
        public void Update(BarChartModel model, Canvas canvas, ScrollBar horizScroll, ScrollBar vertScroll)
        {
            this.model = model;
            Dispose();
            chart = new WPFCanvasChart.WPFCanvasChart(canvas,
               model.HorizScrollVisibility == System.Windows.Visibility.Visible ? horizScroll : null,
               model.VertScrollVisibility == System.Windows.Visibility.Visible ? vertScroll : null,
               this,
               model.Settings,
               model.XAxisInterpolator,
               model.YAxisInterpolator);
            OnModelUpdate();
        }

        public void Dispose()
        {
            if (chart != null)
            {
                chart.Dispose();
            }
        }

        #region Abstract Methods
        public abstract void Draw(System.Windows.Media.DrawingContext ctx);
        public abstract void OnWPFCanvasChartMouseUp(double x, double y);
        protected abstract void OnModelUpdate();
        #endregion Abstract Methods
    }
}
