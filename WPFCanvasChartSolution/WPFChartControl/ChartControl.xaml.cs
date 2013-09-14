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
    public partial class ChartControl : UserControl
    {
        private ChartViewModel viewModel;
        
        public ChartControl()
        {
            InitializeComponent();
            this.viewModel = new ChartViewModel();
            this.MainGrid.DataContext = viewModel;
            Dispatcher.ShutdownStarted += (sender, e) =>
            {
                if (Drawer != null && Drawer.Chart != null)
                {
                    Drawer.Chart.Dispose();
                }
            };
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

            if (drawer.Chart == null)
            {
                drawer.Chart = new WPFCanvasChartComponent();
            }

            drawer.Chart.Dispose();
            drawer.Chart.Init(Canvas,
               drawer.HorizScrollVisibility == System.Windows.Visibility.Visible ? HorizScroll : null,
               drawer.VertScrollVisibility == System.Windows.Visibility.Visible ? VertScroll : null,
               drawer,
               drawer.Settings,
               drawer.XAxisInterpolator,
               drawer.YAxisInterpolator);

            viewModel.Update(drawer);
            drawer.Update();
        }
    }
}
