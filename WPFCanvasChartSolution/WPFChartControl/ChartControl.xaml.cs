using System;
using System.Windows;
using System.Windows.Controls;
using IgorCrevar.WPFChartControl.Model;
using IgorCrevar.WPFChartControl.ViewModel;
using IgorCrevar.WPFChartControl.Drawer;

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

            if (drawer.Legend == null)
            {
                throw new ArgumentNullException("Drawer.Legend is null. It must contains at least colors for lines, bars, etc depending on chart type");
            }

            if (drawer.Settings == null)
            {
                throw new ArgumentNullException("Drawer.Settings is null");
            }

            if (drawer.XAxisInterpolator == null)
            {
                throw new ArgumentNullException("Drawer.XAxisInterpolator is null");
            }

            if (drawer.YAxisInterpolator == null)
            {
                throw new ArgumentNullException("Drawer.YAxisInterpolator is null");
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
