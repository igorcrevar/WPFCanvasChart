using System;
using System.Windows;
using System.Windows.Controls;
using IgorCrevar.WPFBarChartControl.Drawer;
using IgorCrevar.WPFBarChartControl.Model;

namespace IgorCrevar.WPFBarChartControl
{
    /// <summary>
    /// Interaction logic for BarChart.xaml
    /// </summary>
    public partial class BarChart : UserControl, IDisposable
    {
        private BarChartDrawer barChartDrawer;
        private BarChartViewModel viewModel;
       
        public BarChart()
        {
            InitializeComponent();
            barChartDrawer = new BarChartDrawer();
            this.viewModel = new BarChartViewModel();
            this.BarChartMainGrid.DataContext = viewModel;
        }

        // Dependency Property
        public static readonly DependencyProperty ModelProperty =
             DependencyProperty.Register("Model", typeof(BarChartModel),
             typeof(BarChart), new FrameworkPropertyMetadata(new BarChartModel(), OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            BarChart control = (BarChart)source;
            BarChartModel model = (BarChartModel)e.NewValue;
            control.Update(model);
        }

        public BarChartModel Model
        {
            get
            {
                return (BarChartModel)GetValue(ModelProperty);
            }

            set
            {
                SetValue(ModelProperty, value);
            }
        }

        private void Update(BarChartModel model)
        {
            viewModel.Update(model);
            barChartDrawer.Update(model, Canvas, HorizScroll, VertScroll);
        }

        public void Dispose()
        {
            barChartDrawer.Dispose();
        }
    }
}
