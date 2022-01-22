/*************************************************************************
 *  Copyright (c) 2012 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 *  
 *  修改记录:
 *      2012.12.13 Hu Fei  添加 BlobList 类，作为 Blob 的分析结果
 *      2013.03.15 Hu Fei  添加 MeasureRegion 方法
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image.Analysis
{
    /// <summary>
    /// BlobList 是 Blob 容器类。
    /// </summary>
    public class BlobList : List<Blob>
    {
        public BlobList(int capacity = 12)
            : base(capacity)
        {
        }

        public void MeasureRegion()
        {
            foreach (Blob item in this)
            {
                item.MeasureRegion();
            }
        }

        public Blob GetBlobByID(int blobID)
        {
            foreach (Blob item in this)
            {
                if (item.Id == blobID) return item;
            }
            return null;
        }

        /// <summary>
        /// 比较两个Blob所在区域的X值，X值小的在左边。
        /// </summary>
        public void SortByX()
        {
            this.Sort(new BlobXComparer());
        }

        /// <summary>
        /// 比较两个Blob所在区域的Y值，Y值小的在左边。
        /// </summary>
        public void SortByY()
        {
            this.Sort(new BlobYComparer());
        }

        #region 内部类

        /// <summary>
        /// 比较两个Blob所在区域的X值，X值小的在左边。
        /// </summary>
        private class BlobXComparer : IComparer<Blob>
        {
            public int Compare(Blob x, Blob y)
            {
                return x.X - y.X;
            }
        }

        /// <summary>
        /// 比较两个Blob所在区域的Y值，Y值小的在左边。
        /// </summary>
        private class BlobYComparer : IComparer<Blob>
        {
            public int Compare(Blob x, Blob y)
            {
                return x.Y - y.Y;
            }
        }

        #endregion
    }
}
