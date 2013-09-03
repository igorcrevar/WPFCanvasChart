using System;
using IgorCrevar.WPFCanvasChart;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using IgorCrevar.WPFChartControl.Model;
using System.Windows;
using System.Windows.Media;
using IgorCrevar.WPFCanvasChart.Interpolators;
using System.Collections.Generic;

namespace IgorCrevar.WPFChartControl.Drawer
{
    /// <summary>
    /// Base class for all charts drawers
    /// </summary>
    public abstract class AbstractChartDrawer : IWPFCanvasChartDrawer, IDisposable
    {
        private WPFCanvasChart.WPFCanvasChart chart = null;

        public AbstractChartDrawer()
        {
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DDDDDDDD"));
        }

        public void Update(Canvas canvas, ScrollBar horizScroll, ScrollBar vertScroll)
        {
            Dispose();
            chart = new WPFCanvasChart.WPFCanvasChart(canvas,
               HorizScrollVisibility == System.Windows.Visibility.Visible ? horizScroll : null,
               VertScrollVisibility == System.Windows.Visibility.Visible ? vertScroll : null,
               this,
               Settings,
               XAxisInterpolator,
               YAxisInterpolator);
            OnUpdate();
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
        public abstract void OnChartMouseDown(double x, double y);
        protected abstract void OnUpdate();
        #endregion Abstract Methods

        #region Properties
        protected WPFCanvasChart.WPFCanvasChart Chart { get { return chart; } }
         
        public string XAxisText { get; set; }
        public string YAxisText { get; set; }
        public Visibility HorizScrollVisibility { get; set; }
        public Visibility VertScrollVisibility { get; set; }
        public Visibility LegendVisibility { get; set; }
        public IList<LegendItem> Legend { get; set; }
        public Brush Background { get; set; }
        public WPFCanvasChartSettings Settings { get; set; }
        public IWPFCanvasChartInterpolator XAxisInterpolator { get; set; }
        public IWPFCanvasChartInterpolator YAxisInterpolator { get; set; }
        #endregion
    }
}
