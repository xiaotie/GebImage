/*************************************************************************
 *  Copyright (c) 2011 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 *  
 *  修改记录:
 *      2012.12.13 Hu Fei  使用BlobList作为分析结果。
 * 
 ************************************************************************/

using System;
using System.Collections.Generic;

namespace Geb.Image.Analysis
{
    /// <summary>
    /// RichBlob 是一个比 SimpleBlob 更为复杂，它包含了Blob信息及Blob之间的关联。
    /// </summary>
    public partial class Blob : IComparable<Blob>
    {
        public static readonly PointS EmptyPoint = new PointS(Int16.MinValue, Int16.MinValue);
        public static readonly Rect EmptyRegion = new Rect(0, 0, 0, 0);

        #region Blob的基本信息

        /// <summary>
        /// Blob 的编号，用以区分不同的 blob。Id 为负数。
        /// </summary>
        public Int32 Id { get; set; }

        /// <summary>
        /// Blob 的颜色
        /// </summary>
        public Bgr24 Color { get; set; }

        /// <summary>
        /// Blob 的面积（像素数量）
        /// </summary>
        public Int32 Area
        {
            get { return Points.Count; }
        }

        /// <summary>
        /// Blob的点
        /// </summary>
        public List<PointS> Points { get; set; }

        private Int32 _width = -1;
        private Int32 _height = -1;
        private Int32 _x;
        private Int32 _y;

        public Int32 Width 
        {
            get { return _width; }
            set { _width = value; }
        }

        public Int32 Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public Int32 X 
        {
            get { return _x; }
            set { _x = value; }
        }

        public Int32 Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public Rect Region
        {
            get
            {
                if (_width <= 0) MeasureRegion();
                return new Rect(_x, _y, _width, _height);
            }
            set
            {
                X = value.X;
                Y = value.Y;
                Width = value.Width;
                Height = value.Height;
            }
        }

        public Object Tag { get; set; }

        #endregion

        #region AnchorPoint

        /// <summary>
        /// Blob 中的一个点。
        /// </summary>
        public PointS AnchorPoint { get; set; }

        #endregion

        #region 重心

        private PointS _center;

        /// <summary>
        /// 重心
        /// </summary>
        public PointS Center
        {
            get
            {
                if (_center == EmptyPoint)
                {
                    MeasureCenter();
                }

                return _center;
            }
        }

        #endregion

        public Blob()
        {
            Reset();
            Points = new List<PointS>(64);
        }

        public Blob Clone()
        {
            Blob copy = new Blob();
            copy.Id = this.Id;
            copy.Color = this.Color;
            copy.AnchorPoint = this.AnchorPoint;
            copy.X = this.X;
            copy.Y = this.Y;
            copy.Width = this.Width;
            copy.Height = this.Height;
            copy.Points.AddRange(this.Points);
            return copy;
        }

        public void CloneFrom(Blob copy, Boolean clonePoints = false)
        {
            this.Id = copy.Id;
            this.Color = copy.Color;
            this.AnchorPoint = copy.AnchorPoint;
            this.X = copy.X;
            this.Y = copy.Y;
            this.Width = copy.Width;
            this.Height = copy.Height;
            if (clonePoints == true)
            {
                this.Points.Clear();
                this.Points.AddRange(copy.Points);
            }
        }

        #region 重置待测量参数

        public void Reset()
        {
            _width = -1;
            _height = -1;
            _center = EmptyPoint;
        }

        #endregion

        #region Fill Image Methods

        /// <summary>
        /// 向图像中blob所在位置填充颜色
        /// </summary>
        /// <param name="img">被填充的图像</param>
        /// <param name="color">颜色</param>
        public void Fill(ImageBgr24 img, Bgr24 color)
        {
            foreach (PointS item in Points)
            {
                img[item] = color;
            }
        }

        public void Fill(ImageU8 img, Byte color)
        {
            foreach (PointS item in Points)
            {
                img[item] = color;
            }
        }

        #endregion

        #region Measure Methods

        public unsafe void MeasureRegion()
        {
            int xMin = int.MaxValue;
            int xMax = int.MinValue;
            int yMin = xMin;
            int yMax = xMax;

            foreach (PointS item in Points)
            {
                xMin = Math.Min(xMin, item.X);
                xMax = Math.Max(xMax, item.X);
                yMin = Math.Min(yMin, item.Y);
                yMax = Math.Max(yMax, item.Y);
            }

            _x = xMin;
            _y = yMin;
            _width = xMax - xMin + 1;
            _height = yMax - yMin + 1;
        }

        private unsafe void MeasureCenter()
        {
            int x = 0;
            int y = 0;
            int count = Points.Count;
            foreach (PointS p in Points)
            {
                x += p.X;
                y += p.Y;
            }
            x = x / count;
            y = y / count;
            _center = new PointS(x, y);
        }

        #endregion

        #region GrowByFloodFill

        /// <summary>
        /// 通过 FloodFill 增长方式得到 blob 的边缘点与内部点
        /// </summary>
        /// <param name="blobMap">对应的blobMap</param>
        /// <param name="location">Anchor point</param>
        private void GrowByFloodFill(ImageInt32 blobMap, PointS location)
        {
            int color = blobMap[location];
            int anchorColor = color;
            int replacedColor = Id;

            if (anchorColor == replacedColor) return;

            AnchorPoint = location;

            int width = blobMap.Width;
            int height = blobMap.Height;

            Stack<PointS> points = new Stack<PointS>();
            points.Push(location);

            int ww = width - 1;
            int hh = height - 1;

            while (points.Count > 0)
            {
                PointS p = points.Pop();
                blobMap[p.Y, p.X] = replacedColor;

                int count = 0;

                if (p.X > 0)
                {
                    int val = blobMap[p.Y, p.X - 1];
                    if (val == anchorColor)
                    {
                        count++;
                        blobMap[p.Y, p.X - 1] = replacedColor;
                        points.Push(new PointS(p.X - 1, p.Y));
                    }
                    else if (val == replacedColor)
                    {
                        count++;
                    }
                }

                if (p.X < ww)
                {
                    int val = blobMap[p.Y, p.X + 1];
                    if (val == anchorColor)
                    {
                        count++;
                        blobMap[p.Y, p.X + 1] = replacedColor;
                        points.Push(new PointS(p.X + 1, p.Y));
                    }
                    else if (val == replacedColor)
                    {
                        count++;
                    }
                }

                if (p.Y > 0)
                {
                    int val = blobMap[p.Y - 1, p.X];
                    if (val == anchorColor)
                    {
                        count++;
                        blobMap[p.Y - 1, p.X] = replacedColor;
                        points.Push(new PointS(p.X, p.Y - 1));
                    }
                    else if (val == replacedColor)
                    {
                        count++;
                    }
                }

                if (p.Y < hh)
                {
                    int val = blobMap[p.Y + 1, p.X];
                    if (val == anchorColor)
                    {
                        count++;
                        blobMap[p.Y + 1, p.X] = replacedColor;
                        points.Push(new PointS(p.X, p.Y + 1));
                    }
                    else if (val == replacedColor)
                    {
                        count++;
                    }
                }

                Points.Add(p);
            }
        }

        #endregion

        #region 分析图像，得到 BlobSet

        /// <summary>
        /// 分析图像，得到 BlobSet，忽略背景色
        /// </summary>
        /// <param name="img">待分析图像</param>
        /// <param name="bgColor">背景色</param>
        /// <returns>得到的BlobSet</returns>
        public unsafe static BlobList Count(ImageBgr24 img, Bgr24 bgColor)
        {
            return CountCore(img, true, bgColor);
        }

        /// <summary>
        /// 分析图像，得到 BlobSet，忽略背景色
        /// </summary>
        /// <param name="img">待分析图像</param>
        /// <param name="bgColor">背景色</param>
        /// <returns>得到的 BlobSet</returns>
        public unsafe static BlobList Count(ImageU8 img, byte bgColor)
        {
            return CountCore(img, true, bgColor);
        }

        /// <summary>
        /// 分析图像，得到 BlobSet
        /// </summary>
        /// <param name="img">待分析图像</param>
        /// <returns>得到的 BlobList</returns>
        public unsafe static BlobList Count(ImageBgr24 img)
        {
            return CountCore(img, false, Bgr24.WHITE);
        }

        /// <summary>
        /// 分析灰度图像，得到 BlobSet
        /// </summary>
        /// <param name="img">待分析图像</param>
        /// <returns>得到的 BlobList</returns>
        public unsafe static BlobList Count(ImageU8 img)
        {
            return CountCore(img, false, 0);
        }

        private unsafe static BlobList CountCore(ImageBgr24 img, Boolean useBgColor, Bgr24 bgColor)
        {
            BlobList blobs = new BlobList();
            ImageInt32 maps = BuildBlobMap(img, useBgColor, bgColor);

            int width = img.Width;
            int height = img.Height;

            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    if (maps[h, w] > 0)
                    {
                        Blob b = new Blob();
                        b.Id = -blobs.Count - 1;
                        b.Color = img[h, w];
                        b.GrowByFloodFill(maps, new PointS(w, h));
                        blobs.Add(b);
                    }
                }
            }
            maps.Dispose();
            return blobs;
        }

        private unsafe static BlobList CountCore(ImageU8 img, Boolean useBgColor, byte bgColor)
        {
            BlobList blobs = new BlobList();
            ImageInt32 maps = BuildBlobMap(img, useBgColor, bgColor);

            int width = img.Width;
            int height = img.Height;

            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    if (maps[h, w] > 0)
                    {
                        Blob b = new Blob();
                        b.Id = -blobs.Count - 1;
                        byte val = img[h, w];
                        b.Color = new Bgr24(val, val, val);
                        b.GrowByFloodFill(maps, new PointS(w, h));
                        blobs.Add(b);
                    }
                }
            }
            maps.Dispose();
            return blobs;
        }

        private unsafe static ImageInt32 BuildBlobMap(ImageBgr24 img, Boolean useBgColor, Bgr24 bgColor)
        {
            ImageInt32 maps = new ImageInt32(img.Width, img.Height);

            int length = img.Length;
            for (int i = 0; i < length; i++)
            {
                maps[i] = img[i].GetHashCode() + 1;
            }

            if (useBgColor == true)
            {
                int bgVal = bgColor.GetHashCode() + 1;
                for (int i = 0; i < length; i++)
                {
                    if (maps[i] == bgVal)
                    {
                        maps[i] = 0;
                    }
                }
            }

            return maps;
        }

        private unsafe static ImageInt32 BuildBlobMap(ImageU8 img, Boolean useBgColor, byte bgColor)
        {
            ImageInt32 maps = new ImageInt32(img.Width, img.Height);

            int length = img.Length;
            for (int i = 0; i < length; i++)
            {
                maps[i] = img[i] + 1;
            }

            if (useBgColor == true)
            {
                int bgVal = bgColor + 1;
                for (int i = 0; i < length; i++)
                {
                    if (maps[i] == bgVal)
                    {
                        maps[i] = 0;
                    }
                }
            }

            return maps;
        }

        #endregion

        /// <summary>
        /// 输出包含本Blob的最小灰度图像。非Blob部分填充为背景色。
        /// </summary>
        /// <param name="bgColor">背景色</param>
        /// <returns></returns>
        public ImageU8 ToGrayImage(byte bgColor)
        {
            if (this._width <= 0)
            {
                this.MeasureRegion();
            }

            Byte color = this.Color.ToGray();
            int xMin = X;
            int yMin = Y;
            int width = Math.Max(2, Width);
            int height = Math.Max(2, Height);
            ImageU8 img = new ImageU8(width, height);
            img.Fill(bgColor);
            foreach (PointS item in this.Points)
            {
                img[item.Y - yMin, item.X - xMin] = color;
            }
            return img;
        }

        #region IComparable Members

        /// <summary>
        /// 根据面积进行排序
        /// </summary>
        /// <param name="other">other blob</param>
        /// <returns>面积大于对方则返回1，等于则返回0，小于则返回-1</returns>
        public int CompareTo(Blob other)
        {
            return other.Area.CompareTo(this.Area);
        }

        #endregion
    }
}
