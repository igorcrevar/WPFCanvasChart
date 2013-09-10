using System;
using System.Windows;
using System.Windows.Controls;
using IgorCrevar.WPFChartControl.Model;
using IgorCrevar.WPFChartControl.ViewModel;
using IgorCrevar.WPFChartControl.Drawer;
using IgorCrevar.WPFCanvasChart;
using IgorCrevar.WPFCanvasChart.Interpolators;

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
        public static readonly DependencyProperty DrawerProperty =
             DependencyProperty.Register("Drawer", typeof(AbstractChartDrawer),
             typeof(ChartControl), new FrameworkPropertyMetadata(null, OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ChartControl control = (ChartControl)source;
            AbstractChartDrawer drawer = (AbstractChartDrawer)e.NewValue;
            control.Update(drawer);
        }

        public AbstractChartDrawer Drawer
        {
            get
            {
                return (AbstractChartDrawer)GetValue(DrawerProperty);
            }

            set
            {
                SetValue(DrawerProperty, value);
            }
        }

        private void Update(AbstractChartDrawer drawer)
        {
            if (drawer == null)
            {
                return;
            }

            if (drawer.Settings == null)
            {
                drawer.Settings = new WPFCanvasChartSettings(); //default settings
            }

            if (drawer.XAxisInterpolator == null)
            {
                drawer.XAxisInterpolator = new WPFCanvasChartIntInterpolator();
            }

            if (drawer.YAxisInterpolator == null)
            {
                drawer.YAxisInterpolator = new WPFCanvasChartFloatInterpolator();
            }

            viewModel.Update(drawer);
            drawer.Update(Canvas, HorizScroll, VertScroll);
        }

        public void Dispose()
        {
            if (Drawer != null)
            {
                Drawer.Dispose();
            }
        }
    }
}
