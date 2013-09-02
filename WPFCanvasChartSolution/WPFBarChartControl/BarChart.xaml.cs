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
        private BarChartViewModel viewModel;
       
        public BarChart()
        {
            InitializeComponent();
            this.viewModel = new BarChartViewModel();
            this.BarChartMainGrid.DataContext = viewModel;
        }

        // Dependency Property
        public static readonly DependencyProperty ModelProperty =
             DependencyProperty.Register("Model", typeof(BarChartModel),
             typeof(BarChart), new FrameworkPropertyMetadata(null, OnPropertyChanged));

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
            if (model == null)
            {
                return;
            }

            if (model.BarChartDrawer == null)
            {
                throw new ArgumentNullException("BarChartDrawer is null!");
            }

            viewModel.Update(model);
            model.BarChartDrawer.Update(model, Canvas, HorizScroll, VertScroll);
        }

        public void Dispose()
        {
            if (Model != null && Model.BarChartDrawer != null)
            {
                Model.BarChartDrawer.Dispose();
            }
        }
    }
}
