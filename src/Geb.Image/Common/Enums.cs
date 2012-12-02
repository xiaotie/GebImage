/*************************************************************************
 *  Copyright (c) 2012 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image
{
    /// <summary>
    /// Dispose 策略
    /// </summary>
    public enum DisposePolicy
    {
        /// <summary>
        /// 不进行Dispose
        /// </summary>
        None = 0,

        /// <summary>
        /// Dispose调用方
        /// </summary>
        DisposeCaller = 1
    }
}
