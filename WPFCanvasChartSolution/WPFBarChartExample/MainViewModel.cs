using System;
using System.Collections.Generic;
using System.ComponentModel;
using IgorCrevar.WPFCanvasChart;
using IgorCrevar.WPFCanvasChart.Interpolators;
using System.Windows;
using System.Windows.Media;
using IgorCrevar.WPFBarChartControl.Model;

namespace WPFBarChartExample
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

        private BarChartModel barChartModel;

        public MainViewModel()
        {
            var rnd = new Random();
            BarChartModel = new BarChartModel()
            {
                HorizScrollVisibility = Visibility.Visible,
                VertScrollVisibility = Visibility.Collapsed,
                LegendVisibility = Visibility.Visible,
                Settings = new WPFCanvasChartSettings(),
                XAxisText = "Type of workers",
                YAxisText = "Number of people",
                YAxisInterpolator = new WPFCanvasChartIntInterpolator(),
                XAxisInterpolator = new BarXAxisInterpolator(),
                Legend = new List<LegendItem>()
                {
                    new LegendItem(Colors.Blue, "Programmers"),
                    new LegendItem(Colors.Red, "Designers"),
                    new LegendItem(Colors.Yellow, "Admins"),
                    new LegendItem(Colors.Brown, "Management"),
                },
                Data = new List<Point>
                {
                    new Point(1.0d, rnd.Next(100)),
                    new Point(2.0d, rnd.Next(100)),
                    new Point(3.0d, rnd.Next(100)),
                    new Point(4.0d, rnd.Next(100)),
                },
                FixedYMin = 0.0d,
            };
        }

        public string TestProperty
        {
            get
            {
                return "Hello world!";
            }
        }

        public BarChartModel BarChartModel
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
