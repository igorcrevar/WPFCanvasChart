using System;
using System.Windows;
using System.Windows.Controls;
using IgorCrevar.WPFChartControl.Model;
using IgorCrevar.WPFChartControl.ViewModel;

namespace IgorCrevar.WPFChartControl
{
    /// <summary>
    /// Interaction logic for BarChart.xaml
    /// </summary>
    public partial class ChartControl : UserControl, IDisposable
    {
        private ChartViewModel viewModel;

        public ChartControl()
        {
            InitializeComponent();
            this.viewModel = new ChartViewModel();
            this.MainGrid.DataContext = viewModel;
        }

        // Dependency Property
        public static readonly DependencyProperty ModelProperty =
             DependencyProperty.Register("Model", typeof(ChartModel),
             typeof(ChartControl), new FrameworkPropertyMetadata(null, OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ChartControl control = (ChartControl)source;
            ChartModel model = (ChartModel)e.NewValue;
            control.Update(model);
        }

        public ChartModel Model
        {
            get
            {
                return (ChartModel)GetValue(ModelProperty);
            }

            set
            {
                SetValue(ModelProperty, value);
            }
        }

        private void Update(ChartModel model)
        {
            if (model == null)
            {
                return;
            }

            if (model.ChartDrawer == null)
            {
                throw new ArgumentNullException("ChartDrawer is null!");
            }

            viewModel.Update(model);
            model.ChartDrawer.Update(model, Canvas, HorizScroll, VertScroll);
        }

        public void Dispose()
        {
            if (Model != null && Model.ChartDrawer != null)
            {
                Model.ChartDrawer.Dispose();
            }
        }
    }
}
