using System.ComponentModel;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Media;
using IgorCrevar.WPFChartControl.Model;

namespace IgorCrevar.WPFChartControl.ViewModel
{
    class ChartViewModel : INotifyPropertyChanged
    {
        private string xAxisText = string.Empty;
        private string yAxisText = string.Empty;
        private Visibility horizScrollVisibility = Visibility.Collapsed;
        private Visibility vertScrollVisibility = Visibility.Collapsed;
        private Visibility legendVisibility = Visibility.Collapsed;
        private ObservableCollection<LegendItem> legend = new ObservableCollection<LegendItem>();
        private Brush background;

        public void Update(ChartModel model)
        {
            XAxisText = model.XAxisText;
            YAxisText = model.YAxisText;
            HorizScrollVisibility = model.HorizScrollVisibility;
            VertScrollVisibility = model.VertScrollVisibility;
            LegendVisibility = model.LegendVisibility;
            Background = model.Background;
            legend.Clear();
            foreach (var it in model.Legend)
            {
                legend.Add(it);
            }
        }

        public string XAxisText 
        {
            get
            {
                return xAxisText;
            }

            set
            {
                if (xAxisText != value)
                {
                    xAxisText = value;
                    OnPropertyChanged("XAxisText");
                }
            }
        }

        public string YAxisText
        {
            get
            {
                return yAxisText;
            }

            set
            {
                if (yAxisText != value)
                {
                    yAxisText = value;
                    OnPropertyChanged("YAxisText");
                }
            }
        }

        public Visibility HorizScrollVisibility
        {
            get
            {
                return horizScrollVisibility;
            }

            set
            {
                if (horizScrollVisibility != value)
                {
                    horizScrollVisibility = value;
                    OnPropertyChanged("HorizScrollVisibility");
                }
            }
        }

        public Visibility VertScrollVisibility
        {
            get
            {
                return vertScrollVisibility;
            }

            set
            {
                if (vertScrollVisibility != value)
                {
                    vertScrollVisibility = value;
                    OnPropertyChanged("VertScrollVisibility");
                }
            }
        }

        public Visibility LegendVisibility
        {
            get
            {
                return legendVisibility;
            }

            set
            {
                if (legendVisibility != value)
                {
                    legendVisibility = value;
                    OnPropertyChanged("LegendVisibility");
                }
            }
        }

        public ObservableCollection<LegendItem> Legend
        {
            get
            {
                return legend;
            }
        }

        public Brush Background
        {
            get
            {
                return background;
            }

            set
            {
                if (background != value)
                {
                    background = value;
                    OnPropertyChanged("Background");
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
