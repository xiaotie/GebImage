using System;
using System.Collections.Generic;
using System.Drawing;

namespace Geb.Controls
{
    public struct Border
    {
        public Color Color ;
        public float Thickness;
        public Boolean BorderLeftVisible { get; set; }
        public Boolean BorderRightVisible { get; set; }
        public Boolean BorderTopVisible { get; set; }
        public Boolean BorderBottomVisible { get; set; }
    }
}
