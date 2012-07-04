/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image
{
    public class FilterKernel<T>
    {
        public Int32 Width { get; private set; }
        public Int32 Height { get; private set; }
        public Int32 Length { get { return Width * Height; } }

        public T[] Data { get; private set; }
        public Int32 Scale { get; private set; }

        public FilterKernel(T[] data, Int32 width, Int32 height, Int32 scale)
        {
            if (data == null) throw new ArgumentNullException("data");
            if (height < 1) throw new ArgumentOutOfRangeException("height");
            if (width < 1) throw new ArgumentOutOfRangeException("width");

            this.Width = width;
            this.Height = height;
            this.Scale = scale;
            this.Data = data;
        }
    }
}
