/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing.Imaging;

namespace Geb.Image
{
    [StructLayout(LayoutKind.Explicit)]
    public struct PointS
    {
        [FieldOffset(0)]
        public Int16 X;
        [FieldOffset(2)]
        public Int16 Y;
    }
}
