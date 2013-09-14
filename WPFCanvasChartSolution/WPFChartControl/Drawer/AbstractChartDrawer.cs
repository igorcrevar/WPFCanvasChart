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
    public struct MinMax
    {
        public MinMax(bool dummy)
        {
            minX = minY = double.MaxValue;
            maxX = maxY = double.MinValue;
        }

        public MinMax(double minX, double maxX, double minY, double maxY)
        {
            this.minX = minX;
            this.maxX = maxX;
            this.minY = minY;
            this.maxY = maxY;
        }

        public void Update(Point min, Point max)
        {
            Update(min.X, max.X, min.Y, max.Y);
        }

        public void Update(MinMax minMax)
        {
            Update(minMax.minX, minMax.maxX, minMax.minY, minMax.maxY);
        }

        public void Update(double minX, double maxX, double minY, double maxY)
        {
            this.minX = Math.Min(this.minX, minX);
            this.minY = Math.Min(this.minY, minY);

            this.maxX = Math.Max(this.maxX, maxX);
            this.maxY = Math.Max(this.maxY, maxY);
        }

        public void UpdateChart(IWPFCanvasChartComponent chart)
        {
            chart.SetMinMax(minX, maxX, minY, maxY);
        }

        public double minX;
        public double maxX;
        public double minY;
        public double maxY;
    }

    /// <summary>
    /// Base class for all charts drawers
    /// </summary>
    public abstract class AbstractChartDrawer : IWPFCanvasChartDrawer
    {
       public AbstractChartDrawer()
        {
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DDDDDDDD"));
            LegendWidth = double.NaN;
            DefaultMinMax = new MinMax(1, 12, 0, 10);
            FixedYMin = double.NaN;
        }

        #region Abstract Methods
        public abstract void Draw(System.Windows.Media.DrawingContext ctx);
        /// <summary>
        /// Get min/max for X and Y axis
        /// </summary>
        /// <param name="min">min for X and Y</param>
        /// <param name="max">max for X and Y</param>
        /// <returns>true if min max can be determined</returns>
        public abstract MinMax GetMinMax();
        #endregion Abstract Methods

        #region Properties
        public IWPFCanvasChartComponent Chart { get; set; }
        public string XAxisText { get; set; }
        public string YAxisText { get; set; }
        public Visibility HorizScrollVisibility { get; set; }
        public Visibility VertScrollVisibility { get; set; }
        public IList<LegendItem> Legend { get; set; }
        public Brush Background { get; set; }
        public WPFCanvasChartSettings Settings { get; set; }
        public IWPFCanvasChartInterpolator XAxisInterpolator { get; set; }
        public IWPFCanvasChartInterpolator YAxisInterpolator { get; set; }
        public MinMax DefaultMinMax { get; set; }
        /// <summary>
        /// Put here 0.0 if you dont want legend, or double.NaN(default) if you want auto width
        /// </summary>
        public double LegendWidth { get; set; }
        /// <summary>
        /// set to NaN if you want minimum depends on points data, otherwise set fixed y min
        /// </summary>
        public double FixedYMin { get; set; }
        protected MinMax MinMaxValue { get; set; }
        #endregion

        public void Update()
        {
            OnUpdate();
            var minMax = GetMinMax();
            if (minMax.minX != double.MaxValue)
            {
                minMax.minY = double.IsNaN(FixedYMin) || FixedYMin > minMax.minY ? minMax.minY : FixedYMin;
                MinMaxValue = minMax;
            }
            else
            {
                MinMaxValue = DefaultMinMax;
            }

            MinMaxValue.UpdateChart(Chart);
            Chart.DrawChart();
        }

        protected virtual void OnUpdate()
        {
        }

        public virtual void OnChartMouseOver(double x, double y)
        {
        }

        public virtual void OnChartMouseDown(double x, double y)
        {
        }
    }
}
