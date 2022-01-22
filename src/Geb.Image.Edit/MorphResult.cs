/*************************************************************************
 *  Copyright (c) 2013 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 *  
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image.Edit
{
    public class MorphResult
    {
        /// <summary>
        /// 变形后的图像
        /// </summary>
        public ImageBgra32 Image;

        /// <summary>
        /// 变形后的图像相对于原始图像的X轴偏移
        /// </summary>
        public Int32 XOffset = 0;

        /// <summary>
        /// 变形后的图像相对于原始图像的Y轴偏移
        /// </summary>
        public Int32 YOffset = 0;
    }
}
