using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Globalization;

namespace IgorCrevar.WPFCanvasChart
{
    public class WPFCanvasChartSettings
    {
        private Pen penForGrid;
        private Pen penForAxis;
        private Brush brushBackground;
        private Brush brushForText;

        public WPFCanvasChartSettings()
        {
            CoordXSteps = 10;
            CoordYSteps = 5;
            FontSize = 3;
            Language = "en-us";
            FontName = "Verdana";
            PenForGrid = new Pen(Brushes.Black, 0.15);
            PenForAxis = new Pen(Brushes.Black, 0.5);
            BrushBackground = (Brush)new BrushConverter().ConvertFrom("#DDDDDDDD");
            BrushForText = (Brush)new BrushConverter().ConvertFrom("#FF000000");
            MaxXZoomStep = 20.0f;
            MaxYZoomStep = 20.0f;
            ZoomXYAtSameTime = false;
            AreGridsEnabled = true;
            ChartBackgroundBrush = Brushes.White;
        }

        #region Properties

        public int CoordXSteps { get; set; }
        public int CoordYSteps { get; set; }
        public int FontSize { get; set; }
        public Typeface TypeFace { get; set; }
        public CultureInfo CultureInfo { get; set; }
        public float MaxXZoomStep { get; set; }
        public float MaxYZoomStep { get; set; }
        public bool ZoomXYAtSameTime { get; set; }
        public bool AreGridsEnabled { get; set; }
        public Brush ChartBackgroundBrush { get; set; }

        public string Language
        {
            set
            {
                CultureInfo = CultureInfo.GetCultureInfo(value);
            }
        }

        public string FontName
        {
            set
            {
                TypeFace = new Typeface(value);
            }
        }

        public Pen PenForGrid
        {
            get
            {
                return penForGrid;
            }
            set
            {
                penForGrid = value;
                penForGrid.Freeze();
            }
        }

        public Pen PenForAxis
        {
            get
            {
                return penForAxis;
            }
            set
            {
                penForAxis = value;
                penForAxis.Freeze();
            }
        }

        public Brush BrushBackground
        {
            get
            {
                return brushBackground;
            }
            set
            {
                brushBackground = value;
                brushBackground.Freeze();
            }
        }

        public Brush BrushForText
        {
            get
            {
                return brushForText;
            }

            set
            {
                brushForText = value;
                brushForText.Freeze();
            }
        }

        #endregion Properties
    }
}
