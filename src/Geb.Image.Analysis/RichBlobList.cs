/*************************************************************************
 *  Copyright (c) 2012 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 *  
 *  修改记录:
 *      2012.12.13 Hu Fei  添加 RichBlobSet 类，作为 RichBlob 的分析结果
 * 
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image.Analysis
{
    /// <summary>
    /// RichBlobList 包含检测出来的 RichBlob 及 检测所产生的中间数据 BlobMap。可根据 BlobMap
    /// 对 Blob 做进一步的分析。
    /// </summary>
    public class RichBlobList : List<RichBlob>, IDisposable
    {
        public RichBlobList(int capacity = 12)
            : base(capacity)
        {
        }

        public RichBlob GetBlobByID(int blobID)
        {
            foreach (RichBlob item in this)
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
            this.Sort(new RichBlobXComparer());
        }

        /// <summary>
        /// 比较两个Blob所在区域的Y值，Y值小的在左边。
        /// </summary>
        public void SortByY()
        {
            this.Sort(new RichBlobYComparer());
        }

        /// <summary>
        /// BlobMap 储存了图像的每一个像素位置所对应的Blob Id。
        /// </summary>
        public ImageInt32 BlobMap { get; internal set; }

        #region 释放 BlobMap 所占内存

        /// <summary>
        /// 释放 BlobMap 所占内存
        /// </summary>
        public void FreeBlobMap()
        {
            if (BlobMap != null)
            {
                BlobMap.Dispose();
                BlobMap = null;
            }
        }
        
        #endregion

        #region 序列化方法

        private String SerializateBlobs()
        {
            StringBuilder sb = new StringBuilder();
            foreach (RichBlob item in this)
            {
                sb.Append(item.Id).Append(",");
                sb.Append(item.Color.GetHashCode()).Append(",");
                sb.Append(item.InscribedCircleCenter.X).Append(",");
                sb.Append(item.InscribedCircleCenter.Y).Append(",");
                sb.Append(item.InscribedCircleRadius).Append(";");
            }
            return sb.ToString();
        }

        private String SerializateBlobMap()
        {
            int length = BlobMap.Length;
            int i = 0;
            int val = BlobMap[i];
            int count = 1;
            List<Byte> data = new List<byte>();
            for (i = 1; i < length; i++)
            {
                Int32 newVal = BlobMap[i];
                if (newVal != val || count == 255)
                {
                    int tmp = val + 128 * 256;
                    Byte b0 = (byte)(tmp >> 8);
                    Byte b1 = (byte)(tmp << 24 >> 24);
                    Byte b2 = (byte)count;
                    data.Add(b0);
                    data.Add(b1);
                    data.Add(b2);

                    count = 1;
                    val = newVal;
                }
                else
                {
                    count++;
                }
            }

            {
                int tmp = val + 128 * 256;
                Byte b0 = (byte)(tmp >> 8);
                Byte b1 = (byte)(tmp << 24 >> 24);
                Byte b2 = (byte)count;
                data.Add(b0);
                data.Add(b1);
                data.Add(b2);
            }

            return Convert.ToBase64String(data.ToArray());
        }

        private int[] DeserializeObjectMap(String txt)
        {
            List<int> list = new List<int>();
            Byte[] data = Convert.FromBase64String(txt);
            for (int i = 0; i < data.Length; i += 3)
            {
                int v = data[i] * 256 + data[i + 1] - 128 * 256;
                int count = data[i + 2];
                for (int k = 0; k < count; k++)
                {
                    list.Add(v);
                }
            }
            return list.ToArray();
        }
        
        #endregion

        #region 内部类

        /// <summary>
        /// 比较两个Blob所在区域的X值，X值小的在左边。
        /// </summary>
        private class RichBlobXComparer : IComparer<RichBlob>
        {
            public int Compare(RichBlob x, RichBlob y)
            {
                return x.Region.X - y.Region.X;
            }
        }

        /// <summary>
        /// 比较两个Blob所在区域的Y值，Y值小的在左边。
        /// </summary>
        private class RichBlobYComparer : IComparer<RichBlob>
        {
            public int Compare(RichBlob x, RichBlob y)
            {
                return x.Region.Y - y.Region.Y;
            }
        }

        #endregion

        public void Dispose()
        {
            FreeBlobMap();
        }
    }
}
