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
        class CustomBarXAxisInterpolator : IWPFCanvasChartInterpolator
        {
            public virtual string Format(double value)
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

        private AbstractChartDrawer barChartDrawer;
        private AbstractChartDrawer lineSeriesChartDrawer;
        private AbstractChartDrawer stackedBarChartDrawer;

        public MainViewModel()
        {
            var rnd = new Random();
            BarChartDrawer = new BarChartDrawer(new Point[]{
                new Point(1.0d, rnd.Next(100)),
                new Point(2.0d, rnd.Next(100)),
                new Point(3.0d, rnd.Next(100)),
                new Point(4.0d, rnd.Next(100)),
            })
            {
                HorizScrollVisibility = Visibility.Visible,
                VertScrollVisibility = Visibility.Collapsed,
                LegendVisibility = Visibility.Visible,
                Settings = new WPFCanvasChartSettings(),
                YAxisText = "Number of people",
                YAxisInterpolator = new WPFCanvasChartFloatInterpolator(),
                XAxisInterpolator = new CustomBarXAxisInterpolator(),
                Legend = new LegendItem[]
                {
                    new LegendItem(Colors.Blue, "Programmers"),
                    new LegendItem(Colors.Red, "Designers"),
                    new LegendItem(Colors.Yellow, "Admins"),
                    new LegendItem(Colors.Brown, "Management"),
                },
                FixedYMin = 0.0d,
            };

            var serie1 = new List<Point>();
            var serie2 = new List<Point>();
            for (int i = 0; i < rnd.Next(1000) + 1000; ++i)
            {
                serie1.Add(new Point(i, rnd.NextDouble() * 200 - 100));
                serie2.Add(new Point(i + 1, rnd.NextDouble() * 200 - 100));
            }

            LineSeriesChartDrawer = new LineSeriesChartDrawer(new List<IList<Point>>{
                serie1, serie2
            })
            {
                Settings = new WPFCanvasChartSettings(),
                XAxisText = "Layer",
                YAxisText = "Value",
                YAxisInterpolator = new WPFCanvasChartFloatInterpolator(),
                XAxisInterpolator = new WPFCanvasChartIntInterpolator(),
                Legend = new LegendItem[]
                {
                    new LegendItem(Colors.Blue, "Something"),
                    new LegendItem(Colors.Red, "Else"),
                },                
            };

            StackedBarChartDrawer = new StackedBarChartDrawer(new List<StackedBarItem>
            {
               new StackedBarItem(2007, new double[] { 50, 30, 20 }),
               new StackedBarItem(2008, new double[] { 100, 0, 100 }),
               new StackedBarItem(2009, new double[] { 40, 10, 30 }),
               new StackedBarItem(2010, new double[] { 100, 50, 50 }),
               new StackedBarItem(2011, new double[] { 80, 90, -10 }),
               new StackedBarItem(2012, new double[] { 30, 20, 10 }),
               new StackedBarItem(2013, new double[] { 20, 5, 15 }),
            })
            {
                XAxisText = "Power",
                YAxisText = "Year",
                Settings = new WPFCanvasChartSettings(),
                YAxisInterpolator = new WPFCanvasChartFloatInterpolator(),
                XAxisInterpolator = new WPFCanvasChartIntInterpolator(),
                Legend = new LegendItem[]
                {
                    new LegendItem(Colors.Blue, "Active"),
                    new LegendItem(Colors.Red, "Reactive"),
                    new LegendItem(Colors.Yellow, "Total"),
                },
                FixedYMin = 0.0d,
            };
        }

        public AbstractChartDrawer BarChartDrawer
        {
            get
            {
                return barChartDrawer;
            }

            set
            {
                if (barChartDrawer != value)
                {
                    barChartDrawer = value;
                    OnPropertyChanged("BarChartDrawer");
                }
            }
        }

        public AbstractChartDrawer LineSeriesChartDrawer
        {
            get
            {
                return lineSeriesChartDrawer;
            }

            set
            {
                if (lineSeriesChartDrawer != value)
                {
                    lineSeriesChartDrawer = value;
                    OnPropertyChanged("LineSeriesChartDrawer");
                }
            }
        }

        public AbstractChartDrawer StackedBarChartDrawer
        {
            get
            {
                return stackedBarChartDrawer;
            }

            set
            {
                if (stackedBarChartDrawer != value)
                {
                    stackedBarChartDrawer = value;
                    OnPropertyChanged("StackedBarChartDrawer");
                }
            }
        }

        #region INotifyPropertyChanged part
        public event PropertyChangedEventHandler PropertyChanged;
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
