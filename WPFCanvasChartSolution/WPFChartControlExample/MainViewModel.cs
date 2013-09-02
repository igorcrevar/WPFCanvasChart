using System;
using System.Collections.Generic;
using System.ComponentModel;
using IgorCrevar.WPFCanvasChart;
using IgorCrevar.WPFCanvasChart.Interpolators;
using System.Windows;
using System.Windows.Media;
using IgorCrevar.WPFChartControl.Model;
using IgorCrevar.WPFChartControl.Drawer;

namespace WPFChartControlExample
{
    class MainViewModel : INotifyPropertyChanged
    {
        class BarXAxisInterpolator : IWPFCanvasChartInterpolator
        {
            public string Format(double value)
            {
                return string.Empty;
            }

            public void Execute(double min, double max, int noOfSteps, Action<double> action)
            {
                double stepDouble = (max - min) / noOfSteps + 0.5d;
                int step = Math.Max(1, (int)stepDouble);
                for (int i = (int)min + step; i <= (int)max; i += step)
                {
                    action(i);
                }
            }
        }

        private ChartModel barChartModel;

        public MainViewModel()
        {
            var rnd = new Random();
            BarChartModel = new ChartModel()
            {
                HorizScrollVisibility = Visibility.Visible,
                VertScrollVisibility = Visibility.Collapsed,
                LegendVisibility = Visibility.Visible,
                Settings = new WPFCanvasChartSettings(),
                XAxisText = "Type of workers",
                YAxisText = "Number of people",
                YAxisInterpolator = new WPFCanvasChartFloatInterpolator(),
                XAxisInterpolator = new BarXAxisInterpolator(),
                Legend = new List<LegendItem>()
                {
                    new LegendItem(Colors.Blue, "Programmers"),
                    new LegendItem(Colors.Red, "Designers"),
                    new LegendItem(Colors.Yellow, "Admins"),
                    new LegendItem(Colors.Brown, "Management"),
                },
                ChartDrawer = new BarChartDrawer(new List<Point>
                {
                    new Point(1.0d, rnd.Next(100)),
                    new Point(2.0d, rnd.Next(100)),
                    new Point(3.0d, rnd.Next(100)),
                    new Point(4.0d, rnd.Next(100)),
                }),
                FixedYMin = 0.0d,
            };

            var serie1 = new List<Point>() 
            {
                new Point(10.0d, -20.50d),
                new Point(30.0d, 30.58d),
                new Point(50.0d, 10.00d),
            };

            var serie2 = new List<Point>() 
            {
                new Point(5.0d, 100.0d),
                new Point(20.0d, 40.0d),
                new Point(50.0d, 90.00d),
            };
            LineSeriesChartModel = new ChartModel()
            {
                HorizScrollVisibility = Visibility.Visible,
                VertScrollVisibility = Visibility.Visible,
                LegendVisibility = Visibility.Visible,
                Settings = new WPFCanvasChartSettings(),
                XAxisText = "Layer",
                YAxisText = "Value",
                YAxisInterpolator = new WPFCanvasChartFloatInterpolator(),
                XAxisInterpolator = new WPFCanvasChartIntInterpolator(),
                Legend = new List<LegendItem>()
                {
                    new LegendItem(Colors.Blue, "Something"),
                    new LegendItem(Colors.Red, "Else"),
                },
                ChartDrawer = new LineSeriesChartDrawer(new List<IList<Point>>()
                {
                    serie1, serie2
                }) 
            };
        }

        public ChartModel BarChartModel
        {
            get
            {
                return barChartModel;
            }

            set
            {
                if (barChartModel != value)
                {
                    barChartModel = value;
                    OnPropertyChanged("BarChartModel");
                }
            }
        }

        public ChartModel LineSeriesChartModel
        {
            get
            {
                return lineSeriesChartModel;
            }

            set
            {
                if (lineSeriesChartModel != value)
                {
                    lineSeriesChartModel = value;
                    OnPropertyChanged("LineSeriesChartModel");
                }
            }
        }

        #region INotifyPropertyChanged part
        public event PropertyChangedEventHandler PropertyChanged;
        private ChartModel lineSeriesChartModel;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion
    }
}
