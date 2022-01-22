/*************************************************************************
 *  Copyright (c) 2013 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 *  
 *  修改记录:
 * 
 ************************************************************************/

using System;
using System.Collections.Generic;

namespace Geb.Image.Analysis
{
    /// <summary>
    /// 分水岭分割算法
    /// </summary>
    public class WatershedTransform 
    {
        #region 内部类

        private class Queue
        {
            List<Pixel> queue = new List<Pixel>();

            public int Count
            {
                get { return queue.Count; }
            }

            public void AddToEnd(Pixel p)
            {
                queue.Add(p);
            }

            public Pixel RemoveAtFront()
            {
                Pixel temp = queue[0];
                queue.RemoveAt(0);
                return temp;
            }

            public bool IsEmpty
            {
                get { return (queue.Count == 0); }
            }

            public override string ToString()
            {
                return base.ToString() + " Count = " + queue.Count.ToString();
            }
        }

        private class Pixel
        {
            public const int INIT = -1;
            public const int MASK = -2;
            public const int WSHED = 0;

            public int X;
            public int Y;
            public int Label;
            public int Distance;

            public Pixel()
            {
                this.X = -1;
                this.Y = -1;
                this.Label = -1000;
                this.Distance = -1000;
            }

            public Pixel(int x, int y)
            {
                this.X = x;
                this.Y = y;
                this.Label = INIT;
                this.Distance = 0;
            }
        }

        #endregion

        #region 变量
        private Pixel FictitiousPixel = new Pixel();
        private Queue queue = new Queue();
        private List<List<Pixel>> _levelSet;
        private Pixel[] _pixelMap;
        private List<Pixel> _cache;
        private int _width = 0;
        private int _height = 0;
        #endregion

        #region 构造函数

        public WatershedTransform()
        {
            _cache = new List<Pixel>(4);
            _levelSet = new List<List<Pixel>>(256);
            for (int i = 0; i < 256; i++)  _levelSet.Add(new List<Pixel>(32));
        }

        #endregion

        private unsafe void BuildMapAndLevelSet(ImageU8 image)
        {
            _width = image.Width;
            _height = image.Height;
            _pixelMap = new Pixel[_width * _height];
            byte* pImage = (byte*)(image.Start);
            int idx = 0;
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++, idx++)
                {
                    Pixel p = new Pixel(x, y);
                    _pixelMap[idx] = p;
                    _levelSet[pImage[idx]].Add(p);
                }
            }
        }

        private void Segment()
        {
            int blobId = 0;

            // Geodesic SKIZ (skeleton by influence zones) of each level height
            for (int h = 0; h < _levelSet.Count; h++)
            {
                // get all pixels for current height
                foreach (Pixel p in _levelSet[h])
                {
                    p.Label = Pixel.MASK;
                    // for each pixel on current height get neighbouring pixels
                    FetchNeighbours(p, _width);
                    // initialize queue with neighbours at level h of current basins or watersheds
                    foreach (Pixel n in _cache)
                    {
                        if (n.Label > 0 || n.Label == Pixel.WSHED)
                        {
                            p.Distance = 1;
                            queue.AddToEnd(p);
                            break;
                        }
                    }
                }
                int distance = 1;
                queue.AddToEnd(FictitiousPixel);
                // extend basins
                while (true)
                {
                    Pixel p = queue.RemoveAtFront();
                    if (p.X < 0)
                    {
                        if (queue.IsEmpty)
                            break;
                        else
                        {
                            queue.AddToEnd(FictitiousPixel);
                            distance++;
                            p = queue.RemoveAtFront();
                        }
                    }
                    FetchNeighbours(p, _width);
                    // labelling p by inspecting neighbours
                    foreach (Pixel np in _cache)
                    {
                        if (np.Distance <= distance &&
                           (np.Label > 0 || np.Label == Pixel.WSHED))
                        {
                            if (np.Label > 0)
                            {
                                if (p.Label == Pixel.MASK)
                                    p.Label = np.Label;
                                else if (p.Label != np.Label)
                                    p.Label = Pixel.WSHED;
                            }
                            else if (p.Label == Pixel.MASK)
                            {
                                p.Label = Pixel.WSHED;
                            }
                        }
                        // neighbouringPixel is plateau pixel
                        else if (np.Label == Pixel.MASK && np.Distance == 0)
                        {
                            np.Distance = distance + 1;
                            queue.AddToEnd(np);
                        }
                    }
                }
                 
                // detect and process new minima at height level h
                foreach (Pixel p in _levelSet[h])
                {
                    // reset distance to zero
                    p.Distance = 0;
                    // if true then p is inside a new minimum 
                    if (p.Label == Pixel.MASK)
                    {
                        // create new label
                        blobId++;
                        p.Label = blobId;
                        queue.AddToEnd(p);
                        while (!queue.IsEmpty)
                        {
                            Pixel q = queue.RemoveAtFront();
                            // check neighbours of q
                            FetchNeighbours(q, _width);
                            foreach (Pixel neighbouringPixel in _cache)
                            {
                                if (neighbouringPixel.Label == Pixel.MASK)
                                {
                                    neighbouringPixel.Label = blobId;
                                    queue.AddToEnd(neighbouringPixel);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void FetchNeighbours(Pixel centerPixel, int width)
        {
            _cache.Clear();
            if ((centerPixel.X - 1) >= 0)
                _cache.Add(_pixelMap[(centerPixel.X - 1) + centerPixel.Y * width]);
            if ((centerPixel.Y - 1) >= 0)
                _cache.Add(_pixelMap[centerPixel.X + (centerPixel.Y - 1) * width]);
            if ((centerPixel.X + 1) < _width)
                _cache.Add(_pixelMap[(centerPixel.X + 1) + centerPixel.Y * width]);
            if ((centerPixel.Y + 1) < _height)
                _cache.Add(_pixelMap[centerPixel.X + (centerPixel.Y + 1) * width]);
        }

        private void DrawWatershedLines(ImageU8 data, Byte color)
        {
            int length = data.Length;
            for (int i = 0; i < length; i++)
            {
                if (_pixelMap[i].Label == Pixel.WSHED) data[i] = color;
            }
        }

        public ImageU8 ApplySegment(ImageU8 image, Byte watershedLineColor = 0)
        {
            BuildMapAndLevelSet(image);
            Segment();
            DrawWatershedLines(image, watershedLineColor);
            return image;
        }
    }
}
