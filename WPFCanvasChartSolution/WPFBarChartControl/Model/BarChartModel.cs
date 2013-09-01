﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using IgorCrevar.WPFCanvasChart;
using IgorCrevar.WPFCanvasChart.Interpolators;

namespace IgorCrevar.WPFBarChartControl.Model
{
    public class BarChartModel
    {
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
        public IList<Point> Data { get; set; }
        /// <summary>
        /// set to NaN if you want minimum depends on points data, otherwise set fixed y min
        /// </summary>
        public double FixedYMin { get; set; }

        public BarChartModel()
        {
            FixedYMin = double.NaN;
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DDDDDDDD"));
        }
    }
}