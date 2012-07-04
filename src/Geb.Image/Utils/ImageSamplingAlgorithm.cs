/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Geb.Image.Sampling
{
    using Geb.Utils;

    /// <summary>
    /// 随机均匀采样。采样样本两两之间尽量小
    /// </summary>
    public class ImageSamplingAlgorithm
    {
        private struct PairDistance : IComparable<PairDistance>
        {
            public Int32 Index0 { get; set; }
            public Int32 Index1 { get; set; }
            public Int32 DistanceSquare { get; set; }

            #region IComparable<PairDistance> Members

            public int CompareTo(PairDistance other)
            {
                return DistanceSquare.CompareTo(other.DistanceSquare);
            }

            #endregion
        }

        /// <summary>
        /// 随机均匀采样算法。
        /// </summary>
        /// <param name="srcList"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<Point> RandomUniformSampling(List<Point> srcList, Int32 count)
        {
            List<Point> resultList = new List<Point>(count);
            Int32 numNeedRemoved = srcList.Count - count;
            if (numNeedRemoved < 1)
            {
                resultList.AddRange(srcList);
                return resultList;
            }

            // 将序列随机打乱。RandomPermute是扩展方法。
            srcList.RandomPermute();

            // 如果srcList的数量巨大，则随机抽取部分点。由于上面已经随机大乱了，使用GetRange方法便可。
            if (srcList.Count > count * 3)
            {
                srcList = srcList.GetRange(0, count * 3);
                numNeedRemoved = srcList.Count - count;
            }

            // mask 记录点的删除状况。若位于mask[i]=1，则代表第i个点未被删除，若为0，则代表已删除
            Int32[] mask = new Int32[srcList.Count];
            for (int i = 0; i < mask.Length; i++)
            {
                mask[i] = 1;
            }

            // 计算全部点对，并计算距离的平方
            List<PairDistance> list = new List<PairDistance>(srcList.Count * (1 + srcList.Count / 2));
            for (int i = 0; i < srcList.Count; i++)
            {
                for (int j = i + 1; j < srcList.Count; j++)
                {
                    Point p0 = srcList[i];
                    Point p1 = srcList[j];
                    Int32 deltaX = p0.X - p1.X;
                    Int32 deltaY = p0.Y - p1.Y;
                    list.Add(new PairDistance { Index0 = i, Index1 = j, DistanceSquare = deltaX * deltaX + deltaY * deltaY });
                }
            }

            // 进行排序
            list.Sort();

            // 遍历list，直至足够的点被移除
            int startIndex = 0;
            while (numNeedRemoved > 0)
            {
                PairDistance pair = list[startIndex];

                // 如果点对的两点均未被移除，则将其中一点移除。
                if (mask[pair.Index0] != 0 && mask[pair.Index1] != 0)
                {
                    mask[pair.Index1] = 0;
                    numNeedRemoved--;
                }
                startIndex++;
            }

            // 根据mask中的记录，得到全部采样点
            for (int i = 0; i < mask.Length; i++)
            {
                if (mask[i] == 1) resultList.Add(srcList[i]);
            }
            return resultList;
        }

        /// <summary>
        /// Ramer–Douglas–Peucker algorithm，见 http://en.wikipedia.org/wiki/Ramer%E2%80%93Douglas%E2%80%93Peucker_algorithm
        /// </summary>
        /// <param name="srcList"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<Point> DouglasPeuckerSampling(List<Point> srcList, Int32 count)
        {
            throw new NotImplementedException();
        }
    }
}
