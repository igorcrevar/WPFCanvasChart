using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace IgorCrevar.WPFCanvasChart.Internals
{
    internal class WPFCanvasChartVisualHost : FrameworkElement
    {
        private VisualCollection children;
        private DrawingVisual drawingVisual;

        public WPFCanvasChartVisualHost()
        {
            this.ClipToBounds = true;
            drawingVisual = new DrawingVisual();
            children = new VisualCollection(this);
            children.Add(drawingVisual);
        }

        public DrawingVisual Drawing { get { return drawingVisual; } }

        protected override int VisualChildrenCount
        {
            get { return children.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= children.Count)
                throw new ArgumentOutOfRangeException();

            return children[index];
        }
    }
}
