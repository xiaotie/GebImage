/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image
{
    public interface IColorConverter
    {
        unsafe void Copy(Rgb24* from, void* to, int length);
        unsafe void Copy(Argb32* from, void* to, int length);
        unsafe void Copy(Byte* from, void* to, int length);
    }
}
