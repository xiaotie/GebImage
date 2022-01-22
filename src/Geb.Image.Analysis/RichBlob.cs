/*************************************************************************
 *  Copyright (c) 2011 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 *  
 *  修改记录:
 *      2012.12.13 Hu Fei  使用PointS作为存储位置，并使用RichBlobList作为分析结果。
 * 
 ************************************************************************/

using System;
using System.Collections.Generic;

namespace Geb.Image.Analysis
{
    /// <summary>
    /// RichBlob 类 比 Blob 类复杂，它可以分析更多的 Blob 参数，特别是 Blob 之间的相互关系。
    /// RichBlob.Count() 静态方法可以根据图像，生成 Blob 集合。
    /// </summary>
    public class RichBlob : IComparable<RichBlob>
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
            get { return ContourPoints.Count + InnerPoints.Count; }
        }

        /// <summary>
        /// 边缘点
        /// </summary>
        public List<PointS> ContourPoints { get; set; }

        /// <summary>
        /// 内部点
        /// </summary>
        public List<PointS> InnerPoints { get; set; }

        private List<int> _neighborIds;

        /// <summary>
        /// 邻居 blob 的 id
        /// </summary>
        public List<int> NeighborIds
        {
            get
            {
                if (_neighborIds == null) _neighborIds = new List<int>();
                ComputeNeighbors();
                return _neighborIds;
            }
        }

        public Object Tag { get; set; }

        #endregion

        #region BlobMap 和 AnchorPoint

        /// <summary>
        /// Blob 中的一个点。
        /// </summary>
        public PointS AnchorPoint { get; set; }

        /// <summary>
        /// 图像的 BlobMap 。BlobMap 的每个像素标识了Id，表明该像素属于的Blob的Id。
        /// </summary>
        internal ImageInt32 BlobMap { get; private set; }

        #endregion

        #region 最大内接圆

        private PointS _inscribedCircleCenter;

        /// <summary>
        /// 最大内接圆圆心
        /// </summary>
        public PointS InscribedCircleCenter
        {
            get
            {
                if (_inscribedCircleCenter == EmptyPoint)
                {
                    MesureInscribedCircle();
                }

                return _inscribedCircleCenter;
            }
        }

        private double _inscribedCircleRadius;

        /// <summary>
        /// 最大内接圆半径
        /// </summary>
        public double InscribedCircleRadius
        {
            get
            {
                if (_inscribedCircleRadius < 0)
                {
                    MesureInscribedCircle();
                }
                return _inscribedCircleRadius;
            }
        }

        #endregion

        #region 重心

        private PointS _contourCenter;

        /// <summary>
        /// 边缘的重心
        /// </summary>
        public PointS ContourCenter
        {
            get
            {
                if (_contourCenter == EmptyPoint)
                {
                    MeasureContourCenter();
                }

                return _contourCenter;
            }
        }

        #endregion

        #region Region

        private Rect _region;

        /// <summary>
        /// Blob 所属区域
        /// </summary>
        public Rect Region
        {
            get
            {
                if (_region == EmptyRegion)
                {
                    MeasureRegion();
                }
                return _region;
            }
            set
            {
                _region = value;
            }
        }

        public Int32 Width { get { return Region.Width; } }
        public Int32 Height { get { return Region.Height; } }
        public Int32 X { get { return Region.X; } }
        public Int32 Y { get { return Region.Y; } }

        #endregion

        public RichBlob()
        {
            Reset();
            ContourPoints = new List<PointS>(24);
            InnerPoints = new List<PointS>(24);
        }

        public RichBlob Clone(ImageInt32 objectMap)
        {
            RichBlob copy = new RichBlob();
            copy.Id = this.Id;
            copy.Color = this.Color;
            copy.AnchorPoint = this.AnchorPoint;
            copy.InnerPoints.AddRange(this.InnerPoints);
            copy.ContourPoints.AddRange(this.ContourPoints);
            copy.BlobMap = objectMap;
            copy._region = this._region;
            copy._inscribedCircleRadius = this._inscribedCircleRadius;
            if (this._neighborIds != null)
            {
                copy._neighborIds = new List<int>();
                copy._neighborIds.AddRange(this._neighborIds);
            }
            return copy;
        }

        public RichBlob Clone()
        {
            return Clone(this.BlobMap);
        }

        #region 重置待测量参数

        public void Reset()
        {
            _region = EmptyRegion;
            _inscribedCircleRadius = -1;
            _inscribedCircleCenter = EmptyPoint;
            _contourCenter = EmptyPoint;
            _neighborIds = null;
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
            FillContour(img, color);
            FillInner(img, color);
        }

        public void Fill(ImageU8 img, Byte color)
        {
            FillContour(img, color);
            FillInner(img, color);
        }

        public void Fill(ImageBgra32 img, Bgra32 color)
        {
            FillContour(img, color);
            FillInner(img, color);
        }

        /// <summary>
        /// 向图像中blob的边缘位置填充颜色
        /// </summary>
        /// <param name="img">被填充的图像</param>
        /// <param name="color">颜色</param>
        public void FillContour(ImageBgr24 img, Bgr24 color)
        {
            foreach (PointS item in ContourPoints)
            {
                img[item] = color;
            }
        }

        public void FillContour(ImageU8 img, byte color)
        {
            foreach (PointS item in ContourPoints)
            {
                img[item] = color;
            }
        }

        public void FillContour(ImageBgra32 img, Bgra32 color)
        {
            foreach (PointS item in ContourPoints)
            {
                img[item] = color;
            }
        }

        /// <summary>
        /// 向图像中blob的内部位置填充颜色
        /// </summary>
        /// <param name="img">被填充的图像</param>
        /// <param name="color">颜色</param>
        public void FillInner(ImageBgr24 img, Bgr24 color)
        {
            foreach (PointS item in InnerPoints)
            {
                img[item] = color;
            }
        }

        public void FillInner(ImageU8 img, Byte color)
        {
            foreach (PointS item in InnerPoints)
            {
                img[item] = color;
            }
        }

        public void FillInner(ImageBgra32 img, Bgra32 color)
        {
            foreach (PointS item in InnerPoints)
            {
                img[item] = color;
            }
        }

        #endregion

        #region Measure Methods

        private static Random Random = new Random();

        /// <summary>
        /// 将输入的IList在原地随机抽样，分成两部分，其中，前n项是一部分，其余各项为剩余部分
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="number"></param>
        private static void RandomSampleSplitInPlace<T>(IList<T> data, int number)
        {
            int count = data.Count;
            if (number < 1 || number >= count) return;

            int loops = number;

            if (number > (count >> 1))  // number 太大
            {
                loops = count - number;

                //从N个数中随机取出一个和最后一个元素交换,再从前面N-1个数中随机取一个和倒数第二个交换…
                for (int i = 0; i < loops; i++)
                {
                    int index0 = Random.Next(0, count - i);
                    int index1 = count - i - 1;
                    T tmp = data[index0];
                    data[index0] = data[index1];
                    data[index1] = tmp;
                }
            }
            else
            {
                //从N个数中随机取出一个和第一个元素交换,再从后面N-1个数中随机取一个和第二个交换…
                for (int i = 0; i < loops; i++)
                {
                    int index0 = Random.Next(i, count);
                    int index1 = i;
                    T tmp = data[index0];
                    data[index0] = data[index1];
                    data[index1] = tmp;
                }
            }

            return;
        }

        private unsafe void MesureInscribedCircle()
        {
            if (InnerPoints.Count == 0)
            {
                _inscribedCircleCenter = AnchorPoint;
                return;
            }

            int innerCount = Math.Min(1000, InnerPoints.Count);
            int contourCount = Math.Min(1000, ContourPoints.Count);
            if (innerCount > 1) RandomSampleSplitInPlace(InnerPoints, innerCount - 1);
            if (contourCount > 1) RandomSampleSplitInPlace(ContourPoints, contourCount - 1);

            PointS p = InnerPoints[0];
            int minDisSqaure = ComputeMinDistanceSquare(p, ContourPoints, contourCount);

            for (int i = 1; i < innerCount; i++)
            {
                PointS item = InnerPoints[i];
                int disSquare = ComputeMinDistanceSquare(item, ContourPoints, contourCount);
                if (disSquare > minDisSqaure)
                {
                    p = item;
                    minDisSqaure = disSquare;
                }
            }
            _inscribedCircleCenter = p;
            _inscribedCircleRadius = Math.Sqrt(minDisSqaure);
        }

        private unsafe void MeasureRegion()
        {
            int xMin = int.MaxValue;
            int xMax = int.MinValue;
            int yMin = xMin;
            int yMax = xMax;

            foreach (PointS item in ContourPoints)
            {
                xMin = Math.Min(xMin, item.X);
                xMax = Math.Max(xMax, item.X);
                yMin = Math.Min(yMin, item.Y);
                yMax = Math.Max(yMax, item.Y);
            }

            _region = new Rect(xMin, yMin, xMax - xMin + 1, yMax - yMin + 1);
        }

        private unsafe void MeasureContourCenter()
        {
            int x = 0;
            int y = 0;
            int count = ContourPoints.Count;
            foreach (PointS p in ContourPoints)
            {
                x += p.X;
                y += p.Y;
            }
            x = x / count;
            y = y / count;
            _contourCenter = new PointS(x, y);
        }

        private int ComputeMinDistanceSquare(PointS p, List<PointS> points, int count)
        {
            int min = int.MaxValue;
            for (int i = 0; i < count; i++)
            {
                PointS item = points[i];
                int deltaX = item.X - p.X;
                int deltaY = item.Y - p.Y;
                int disSquare = deltaX * deltaX + deltaY * deltaY;
                if (disSquare < min) min = disSquare;
            }
            return min;
        }

        private void ComputeNeighbors()
        {
            int hh = BlobMap.Height - 1;
            int ww = BlobMap.Width - 1;

            int id = this.Id;
            int nbId;   // 邻居 blob 的 id
            List<PointS> cps = ContourPoints;
            foreach (PointS cp in cps)
            {
                if (cp.X > 0)
                {
                    nbId = BlobMap[cp.Y, cp.X - 1]; // 左侧
                    if (nbId != id && _neighborIds.Contains(nbId) == false)
                    {
                        _neighborIds.Add(nbId);
                    }
                }

                if (cp.Y > 0)
                {
                    nbId = BlobMap[cp.Y - 1, cp.X]; // 上侧
                    if (nbId != id && _neighborIds.Contains(nbId) == false)
                    {
                        _neighborIds.Add(nbId);
                    }
                }

                if (cp.X < ww)
                {
                    nbId = BlobMap[cp.Y, cp.X + 1]; // 右侧
                    if (nbId != id && _neighborIds.Contains(nbId) == false)
                    {
                        _neighborIds.Add(nbId);
                    }
                }

                if (cp.Y < hh)
                {
                    nbId = BlobMap[cp.Y + 1, cp.X]; // 下侧
                    if (nbId != id && _neighborIds.Contains(nbId) == false)
                    {
                        _neighborIds.Add(nbId);
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// 判断当前的 blob 是否在指定的 blob 内部
        /// </summary>
        /// <param name="blob"></param>
        /// <returns></returns>
        //public Boolean IsInnerOf(RichBlob blob)
        //{
        //    if (blob.Region.IsContains(this.Region) == false) return false;

        //    int xStart = blob.Region.Left;
        //    int xEnd = blob.Region.Right;
        //    Dictionary<int, int> scoreDic = blob.ExtContourXDic;
        //    List<PointS> points = this.InnerPoints;

        //    for (int i = 0; i < points.Count; i++)
        //    {
        //        int score = 0;
        //        PointS point = points[i];
        //        int key = point.GetHashCode32();

        //        // 在边界上
        //        if (scoreDic.ContainsKey(key) == true)
        //            continue;

        //        int start = xStart;
        //        int end = xEnd + 1;

        //        if (point.X > xStart && xEnd > point.X && (point.X - xStart) > (xEnd - point.X))
        //        {
        //            start = point.X + 1;
        //        }
        //        else
        //        {
        //            end = point.X;
        //        }

        //        point.X = (Int16)start;
        //        key = point.GetHashCode32();
        //        int pointScore = 0;
        //        while (point.X < end)
        //        {
        //            if (scoreDic.TryGetValue(key, out pointScore) == true)
        //            {
        //                score += pointScore;
        //            }

        //            point.X++;
        //            key++;
        //        }

        //        if (score % 4 == 0)     // 在多边形的外部
        //            return false;
        //    }

        //    return true;
        //}

        private Boolean IsRegion(PointS p)
        {
            if (p.X < 0 || p.Y < 0 || p.X >= this.BlobMap.Width || p.Y >= this.BlobMap.Height)
                return false; // 为背景
            else return BlobMap[p] == this.Id;
        }

        public int GetPointHashCode(PointS p)
        {
            return this.BlobMap.Width * p.Y + p.X;
        }

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

            if (anchorColor == replacedColor) throw new Exception("Anchor color 同 Blob Id 相等，无法进行 FloodFill");

            AnchorPoint = location;
            BlobMap = blobMap;

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

                if (count == 4)
                {
                    InnerPoints.Add(p);
                }
                else
                {
                    ContourPoints.Add(p);
                }
            }
        }

        #endregion

        #region 分析图像，得到 RichBlobSet

        /// <summary>
        /// 分析图像，得到RichBlobSet，忽略背景色
        /// </summary>
        /// <param name="img">待分析图像</param>
        /// <param name="bgColor">背景色</param>
        /// <returns>得到的RichBlobSet</returns>
        public unsafe static RichBlobList Count(ImageBgr24 img, Bgr24 bgColor)
        {
            return CountCore(img, true, bgColor);
        }

        /// <summary>
        /// 分析图像，得到RichBlobSet，忽略背景色
        /// </summary>
        /// <param name="img">待分析图像</param>
        /// <param name="bgColor">背景色</param>
        /// <returns>得到的RichBlobSet。</returns>
        public unsafe static RichBlobList Count(ImageU8 img, byte bgColor)
        {
            return CountCore(img, true, bgColor);
        }

        /// <summary>
        /// 分析图像，得到RichBlobSet
        /// </summary>
        /// <param name="img">待分析图像</param>
        /// <returns>得到的RichBlobSet</returns>
        public unsafe static RichBlobList Count(ImageBgr24 img)
        {
            return CountCore(img, false, Bgr24.WHITE);
        }

        /// <summary>
        /// 分析灰度图像，得到RichBlobSet。
        /// </summary>
        /// <param name="img">待分析图像</param>
        /// <returns>得到的RichBlobSet。</returns>
        public unsafe static RichBlobList Count(ImageU8 img)
        {
            return CountCore(img, false, 0);
        }

        private unsafe static RichBlobList CountCore(ImageBgr24 img, Boolean useBgColor, Bgr24 bgColor)
        {
            RichBlobList blobs = new RichBlobList();
            ImageInt32 maps = BuildBlobMap(img, useBgColor, bgColor);
            blobs.BlobMap = maps;
            int width = img.Width;
            int height = img.Height;

            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    if (maps[h, w] > 0)
                    {
                        RichBlob b = new RichBlob();
                        b.Id = -blobs.Count - 1;
                        b.Color = img[h, w];
                        b.GrowByFloodFill(maps, new PointS(w, h));
                        blobs.Add(b);
                    }
                }
            }

            return blobs;
        }

        private unsafe static RichBlobList CountCore(ImageU8 img, Boolean useBgColor, byte bgColor)
        {
            RichBlobList blobs = new RichBlobList();
            ImageInt32 maps = BuildBlobMap(img, useBgColor, bgColor);
            blobs.BlobMap = maps;

            int width = img.Width;
            int height = img.Height;

            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    if (maps[h, w] > 0)
                    {
                        RichBlob b = new RichBlob();
                        b.Id = -blobs.Count - 1;
                        byte val = img[h, w];
                        b.Color = new Bgr24(val, val, val);
                        b.GrowByFloodFill(maps, new PointS(w, h));
                        blobs.Add(b);
                    }
                }
            }

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
            Byte color = this.Color.ToGray();
            Rect rect = this.Region;
            int xMin = rect.X;
            int yMin = rect.Y;
            int width = Math.Max(2, rect.Width);
            int height = Math.Max(2, rect.Height);
            ImageU8 img = new ImageU8(width, height);
            img.Fill(bgColor);
            foreach (PointS item in this.ContourPoints)
            {
                img[item.Y - yMin, item.X - xMin] = color;
            }
            foreach (PointS item in this.InnerPoints)
            {
                img[item.Y - yMin, item.X - xMin] = color;
            }
            return img;
        }

        #region IComparable<RichBlob> Members

        /// <summary>
        /// 根据面积进行排序
        /// </summary>
        /// <param name="other">other blob</param>
        /// <returns>面积大于对方则返回1，等于则返回0，小于则返回-1</returns>
        public int CompareTo(RichBlob other)
        {
            return other.Area.CompareTo(this.Area);
        }

        #endregion
    }
}
