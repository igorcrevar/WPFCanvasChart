using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace IgorCrevar.WPFChartControl.Model
{
    public class LegendItem
    {
        public LegendItem(Color color, string name)
        {
            Color = color;
            Name = name;
        }

        public Color Color { get; set; }
        public string Name { get; set; }
    }
}
