using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows;
using IgorCrevar.WPFCanvasChart.Internals;
using IgorCrevar.WPFCanvasChart.Interpolators;

namespace IgorCrevar.WPFCanvasChart
{
    public class WPFCanvasChart : IDisposable
    {
        protected double minX, maxX;
        protected double minY, maxY;

        private Canvas canvas;

        private bool isPanning = false;
        private Point mousePosition;
        private IWPFCanvasChartDrawer drawer;

        private readonly WPFCanvasChartVisualHost chartHost = new WPFCanvasChartVisualHost();
        private readonly WPFCanvasChartVisualHost axisHost = new WPFCanvasChartVisualHost();
        private readonly WPFCanvasChartVisualHost xyAxisValuesHost = new WPFCanvasChartVisualHost();

        private readonly TranslateTransform chartTransform = new TranslateTransform();

        private ScrollBar horizScrollBar;
        private ScrollBar vertScrollBar;

        private double xScale = 1.0d;
        private double yScale = 1.0d;
        private double xMarginLeft;
        private double xMarginRight;
        private double yMargin;

        private double virtualWidth;
        private double virtualHeight;

        private bool yZoomEnabled;
        private bool xZoomEnabled;
        private WPFCanvasChartSettings settings;
        private IWPFCanvasChartInterpolator yAxisInterpolator;
        private IWPFCanvasChartInterpolator xAxisInterpolator;
        private bool isDisposed = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="canvas">Canvas object</param>
        /// <param name="horizScrollBar">Horizontal ScrollBar object</param>
        /// <param name="vertScrollBar">Verical ScrollBar object. Can be null - that means vertical scroll is disabled</param>
        /// <param name="drawer">ICustomChartCanvasDrawer implementator. This object will implement actual chart drawing(lines, etc)</param>
        /// <param name="settings">WPFCanvasChartSettings instance - settings for chart</param>
        /// <param name="xAxisInterpolator">value interpolator for x axis</param>
        /// <param name="yAxisInterpolator">value interpolator for y axis</param>
        public WPFCanvasChart(Canvas canvas, ScrollBar horizScrollBar, ScrollBar vertScrollBar, IWPFCanvasChartDrawer drawer,
            WPFCanvasChartSettings settings, IWPFCanvasChartInterpolator xAxisInterpolator, IWPFCanvasChartInterpolator yAxisInterpolator)
        {
            this.canvas = canvas;
            this.drawer = drawer;
            this.settings = settings;
            this.horizScrollBar = horizScrollBar;
            this.vertScrollBar = vertScrollBar;
            this.yZoomEnabled = vertScrollBar != null;
            this.xZoomEnabled = horizScrollBar != null;
            this.xAxisInterpolator = xAxisInterpolator;
            this.yAxisInterpolator = yAxisInterpolator;

            chartHost.Drawing.Transform = chartTransform;

            canvas.Children.Add(axisHost);
            canvas.Children.Add(xyAxisValuesHost);
            canvas.Children.Add(chartHost);

            UpdateHostsSizes();
            InitCanvasHandlers();
        }

        /// <summary>
        /// Set maximum and minimum for X and Y coordinate. This boundaries determine axis point drawing!
        /// </summary>
        /// <param name="minX">minimum for x coord</param>
        /// <param name="maxX">maximum for x coord</param>
        /// <param name="minY">minimum for y coord</param>
        /// <param name="maxY">maximum for y coord</param>
        public void SetMinMax(double minX, double maxX, double minY, double maxY)
        {
            this.minX = minX;
            this.maxX = maxX;
            this.minY = minY;
            this.maxY = maxY;
            UpdateHostsSizes();
        }

        private void InitCanvasHandlers()
        {
            if (settings.HandleSizeChanged)
            {
                canvas.SizeChanged += Canvas_SizeChanged;
            }

            canvas.MouseWheel += Canvas_MouseWheel;
            canvas.MouseMove += Canvas_MouseMove;
            canvas.MouseUp += Canvas_MouseUp;
            canvas.MouseDown += Canvas_MouseDown;
            if (xZoomEnabled)
            {
                horizScrollBar.ValueChanged += HorizScrollBar_ValueChanged;
            }

            if (yZoomEnabled)
            {
                vertScrollBar.ValueChanged += VertScrollBar_ValueChanged;
            }
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateHostsSizes();
            DrawChart();
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                isPanning = false;
                canvas.Cursor = System.Windows.Input.Cursors.Arrow;
                canvas.ReleaseMouseCapture();
            }
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                mousePosition = e.GetPosition(canvas);
                isPanning = true;
                canvas.Cursor = System.Windows.Input.Cursors.Hand;
                canvas.CaptureMouse(); // mouse up anywhere will be sent to this control
            }
            else if (e.ChangedButton == MouseButton.Left)
            {
                var pos = e.GetPosition(chartHost);
                if (pos.X >= 0 && pos.Y >= 0 && pos.X < chartHost.Width && pos.Y < chartHost.Height)
                {
                    pos = ChartPoint2Point(pos);
                    drawer.OnChartMouseDown(pos.X, pos.Y);
                }
            }
        }

        private void VertScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double value = -e.NewValue;
            chartTransform.Y = value;
            DrawXYAxisValues();
        }

        private void HorizScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double value = -e.NewValue;
            chartTransform.X = value;
            DrawXYAxisValues();
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isPanning)
            {
                Point newPosition = e.GetPosition(canvas);
                if (xZoomEnabled)
                {
                    horizScrollBar.Value -= newPosition.X - mousePosition.X;
                }

                if (yZoomEnabled)
                {
                    vertScrollBar.Value -= newPosition.Y - mousePosition.Y;
                }

                mousePosition = newPosition;
            }
        }

        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // fixed zoom step
            double zoomStep = e.Delta > 0 ? 1.1d : 0.9d;
            if (settings.ZoomXYAtSameTime)
            {
                ZoomX(zoomStep);
                ZoomY(zoomStep);
            }
            else if ((Keyboard.Modifiers & ModifierKeys.Control) == 0)
            {
                ZoomX(zoomStep);
            }
            else
            {
                ZoomY(zoomStep);
            }

            DrawChart();
        }

        private void CheckBoundary(ref double value, double min, double max)
        {
            if (value < min)
            {
                value = min;
            }
            else if (value > max)
            {
                value = max;
            }
        }

        private void UpdateHostsSizes()
        {
            xScale = yScale = 1.0d;
            DetermineMargins();

            axisHost.Width = canvas.Width;
            axisHost.Height = canvas.Height;

            chartHost.Width = canvas.Width - xMarginLeft - xMarginRight;
            chartHost.Height = canvas.Height - yMargin * 2;
            chartHost.Margin = new Thickness(xMarginLeft, yMargin, xMarginRight, yMargin);

            xyAxisValuesHost.Width = canvas.Width;
            xyAxisValuesHost.Height = canvas.Height;

            virtualWidth = chartHost.Width;
            virtualHeight = chartHost.Height;

            SetHorizScrollBar();
            SetVertScrollBar();
        }

        private void SetHorizScrollBar()
        {
            if (xZoomEnabled)
            {
                horizScrollBar.Minimum = 0;
                // maximum is full width - what user see
                horizScrollBar.Maximum = virtualWidth - chartHost.Width;
                horizScrollBar.ViewportSize = chartHost.Width;
            }
        }

        private void SetVertScrollBar()
        {
            if (yZoomEnabled)
            {
                vertScrollBar.Minimum = 0;
                vertScrollBar.Maximum = virtualHeight - chartHost.Height;
                vertScrollBar.ViewportSize = chartHost.Height;
            }
        }

        private void ZoomX(double step)
        {
            if (!xZoomEnabled)
            {
                return;
            }

            double oldX = (horizScrollBar.Value + horizScrollBar.ViewportSize / 2) / xScale;
            double width = virtualWidth * step;
            CheckBoundary(ref width, chartHost.Width, chartHost.Width * settings.MaxXZoomStep);
            xScale = width / chartHost.Width;
            virtualWidth = width;
            SetHorizScrollBar();
            // fix scroll position
            horizScrollBar.Value = oldX * xScale - horizScrollBar.ViewportSize / 2;
        }

        private void ZoomY(double step)
        {
            if (!yZoomEnabled)
            {
                return;
            }

            double oldY = yZoomEnabled ? (vertScrollBar.Value + vertScrollBar.ViewportSize / 2) / yScale : 0.0d;
            double height = yZoomEnabled ? virtualHeight * step : virtualHeight;
            CheckBoundary(ref height, chartHost.Height, chartHost.Height * settings.MaxYZoomStep);
            yScale = height / chartHost.Height;
            virtualHeight = height;
            SetVertScrollBar();
            vertScrollBar.Value = oldY * yScale - vertScrollBar.ViewportSize / 2;
        }

        // this method sets xMargin and yMargin, because every text on x and y axis has its width and height
        private void DetermineMargins()
        {
            FormattedText ft = GetFormattedText(yAxisInterpolator.Format(minY));
            xMarginLeft = ft.Width;
            ft = GetFormattedText(yAxisInterpolator.Format(maxY));
            if (xMarginLeft < ft.Width)
            {
                xMarginLeft = ft.Width;
            }

            ft = GetFormattedText(xAxisInterpolator.Format(minX));
            yMargin = ft.Height;
            xMarginRight = ft.Width / 2;
            if (xMarginLeft < ft.Width / 2)
            {
                xMarginLeft = ft.Width / 2;
            }

            ft = GetFormattedText(xAxisInterpolator.Format(maxX));
            if (yMargin < ft.Height)
            {
                yMargin = ft.Height;
            }

            if (xMarginLeft < ft.Width / 2)
            {
                xMarginLeft = ft.Width / 2;
            }

            if (xMarginRight < ft.Width / 2)
            {
                xMarginRight = ft.Width / 2;
            }

            yMargin += 5.0d;
            xMarginLeft += 5.0d;
            xMarginRight += 5.0d;
        }

        /// <summary>
        /// Draw chart. First draw axis and other stuff and finally call drawer Draw method which will manually draw actuall charting lines, bars etc. 
        /// drawer Draw method should use CanvasPoint2Point method for converting actual points!
        /// </summary>
        public void DrawChart()
        {
            DrawAxisLines();
            DrawChartAndValues();
        }

        private void DrawAxisLines()
        {
            using (var ctx = axisHost.Drawing.RenderOpen())
            {
                // fill hole rectangle
                ctx.DrawRectangle(settings.BrushBackground, new Pen(settings.BrushBackground, 0.0d),
                    new Rect(0, 0, axisHost.Width, axisHost.Height));
                // y axis
                ctx.DrawLine(settings.PenForAxis, new Point(xMarginLeft, yMargin), new Point(xMarginLeft, axisHost.Height - yMargin));
                // x axis
                ctx.DrawLine(settings.PenForAxis, new Point(xMarginLeft, axisHost.Height - yMargin), 
                    new Point(axisHost.Width - xMarginRight, axisHost.Height - yMargin));
            }
        }

        private void DrawXYAxisValues()
        {
            using (var xyAxisCtx = xyAxisValuesHost.Drawing.RenderOpen())
            {
                DrawXAxis(xyAxisCtx, null);
                DrawYAxis(xyAxisCtx, null);
            }
        }

        private void DrawChartAndValues()
        {
            using (var chartCtx = chartHost.Drawing.RenderOpen())
            using (var xyAxisCtx = xyAxisValuesHost.Drawing.RenderOpen())
            {
                // clear chart rectangle
                chartCtx.DrawRectangle(settings.ChartBackgroundBrush, new Pen(settings.ChartBackgroundBrush, 0.0d), 
                    new Rect(0, 0, virtualWidth, virtualHeight));
                // draw values and grids
                DrawXAxis(xyAxisCtx, chartCtx);
                DrawYAxis(xyAxisCtx, chartCtx);
                // draw actual chart - call user defined drawer
                drawer.Draw(chartCtx);
            }
        }

        #region Helper methods for subclasses
        protected void DrawXAxisText(DrawingContext ctx, double value, double desiredXPosition)
        {
            FormattedText formattedText = GetFormattedText(xAxisInterpolator.Format(value));
            double x = desiredXPosition + chartTransform.X + xMarginLeft - formattedText.Width / 2;
            ctx.DrawText(formattedText, new Point(x, 2 + xyAxisValuesHost.Height - yMargin));
        }

        protected void DrawYAxisText(DrawingContext ctx, double value, double desiredYPosition)
        {
            FormattedText formattedText = GetFormattedText(yAxisInterpolator.Format(value));
            double y = desiredYPosition + chartTransform.Y + yMargin - formattedText.Height / 2;
            ctx.DrawText(formattedText, new Point(xMarginLeft - formattedText.Width - 2, y));
        }

        protected FormattedText GetFormattedText(string txt)
        {
            return new FormattedText(txt, settings.CultureInfo, FlowDirection.LeftToRight, settings.TypeFace, settings.FontSize, 
                settings.BrushForText);
        }

        protected int GetNoOfStepsForXAxis()
        {
            return (int)(settings.CoordXSteps * xScale);
        }

        protected int GetNoOfStepsForYAxis()
        {
            return (int)(settings.CoordYSteps * yScale);
        }

        protected void DrawVerticalGrid(DrawingContext ctx, double x)
        {
            if (ctx != null && settings.AreGridsEnabled)
            {
                ctx.DrawLine(settings.PenForGrid, new Point(x, 0), new Point(x, virtualHeight));
            }
        }

        protected void DrawHorizontalGrid(DrawingContext ctx, double y)
        {
            if (ctx != null && settings.AreGridsEnabled)
            {
                ctx.DrawLine(settings.PenForGrid, new Point(0, y), new Point(virtualWidth, y));
            }
        }

        protected bool IsXInsideVisiblePart(double x)
        {
            return x >= -chartTransform.X && x <= chartHost.Width - chartTransform.X + 1;
        }

        protected bool IsYInsideVisiblePart(double y)
        {
            double relativeY = y + chartTransform.Y;
            return relativeY >= 0 && relativeY <= chartHost.Height;
        }
        #endregion Helper methods for subclasses

        protected virtual void DrawXAxis(DrawingContext ctx, DrawingContext chartCtx)
        {
            xAxisInterpolator.Execute(minX, maxX, GetNoOfStepsForXAxis(), (currentValue) =>
            {
                var currentPosition = Point2ChartPoint(new Point(currentValue, 0.0));
                DrawVerticalGrid(chartCtx, currentPosition.X);
                // draw axis value at current step if possible
                if (IsXInsideVisiblePart(currentPosition.X))
                {
                    DrawXAxisText(ctx, currentValue, currentPosition.X);
                }
            });
        }

        protected virtual void DrawYAxis(DrawingContext ctx, DrawingContext chartCtx)
        {
            yAxisInterpolator.Execute(minY, maxY, GetNoOfStepsForYAxis(), (currentValue) =>
            {
                var currentPosition = Point2ChartPoint(new Point(0.0, currentValue));
                DrawHorizontalGrid(chartCtx, currentPosition.Y);
                // draw axis value at current step if possible
                if (IsYInsideVisiblePart(currentPosition.Y))
                {
                    // draw axis value at current step
                    DrawYAxisText(ctx, currentValue, currentPosition.Y);
                }
            });
        }

        /// <summary>
        /// Convert chart point to point inside chart host
        /// </summary>
        /// <param name="p">Original point</param>
        /// <returns>new point instance</returns>
        public Point Point2ChartPoint(Point p)
        {
            var np = new Point();
            np.X = virtualWidth * Math.Abs(p.X - minX) / Math.Abs(maxX - minX);
            np.Y = virtualHeight * Math.Abs(p.Y - minY) / Math.Abs(maxY - minY);
            np.Y = virtualHeight - np.Y;
            return np;
        }

        /// <summary>
        /// Convert point from chart host(canvas) to original chart point
        /// </summary>
        /// <param name="p">Canvas point</param>
        /// <returns>new point instance</returns>
        public Point ChartPoint2Point(Point p)
        {
            var np = new Point();
            double sX = (p.X - chartTransform.X) / virtualWidth;
            double realY = p.Y - chartTransform.Y;
            double sY = (virtualHeight - realY) / virtualHeight;
            np.X = minX + Math.Abs(maxX - minX) * sX;
            np.Y = minY + Math.Abs(maxY - minY) * sY;
            return np;
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                drawer = null;
                if (settings.HandleSizeChanged)
                {
                    canvas.SizeChanged -= Canvas_SizeChanged;
                }

                canvas.MouseWheel -= Canvas_MouseWheel;
                canvas.MouseMove -= Canvas_MouseMove;
                canvas.MouseUp -= Canvas_MouseUp;
                canvas.MouseDown -= Canvas_MouseDown;
                canvas.Children.Clear();
                if (xZoomEnabled)
                {
                    horizScrollBar.ValueChanged -= HorizScrollBar_ValueChanged;
                }

                if (yZoomEnabled)
                {
                    vertScrollBar.ValueChanged -= VertScrollBar_ValueChanged;
                }

                isDisposed = true;
            }
        }
    }
}
