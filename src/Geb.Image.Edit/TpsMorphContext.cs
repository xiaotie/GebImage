/*************************************************************************
 *  Copyright (c) 2013 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 *  
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image.Edit
{
    public class TpsMorphContext
    {
        /// <summary>
        /// 原始图像的备份
        /// </summary>
        private ImageBgra32 Image;

        private float XOffset;
        private float YOffset;

        /// <summary>
        /// 对原始图像进行划分的网格点
        /// </summary>
        private List<TriangleF> Mesh = new List<TriangleF>();

        /// <summary>
        /// 新控制点对应的网格点
        /// </summary>
        private List<TriangleF> MeshNew = new List<TriangleF>();

        /// <summary>
        /// 旧控制点
        /// </summary>
        private List<PointF> CtrlPointsOld = new List<PointF>();

        /// <summary>
        /// 新控制点
        /// </summary>
        private List<PointF> CtrlPointsNew = new List<PointF>();

        /// <summary>
        /// 清理资源及控制点
        /// </summary>
        public void Clear()
        {
            Mesh.Clear();
            MeshNew.Clear();
            CtrlPointsOld.Clear();
            CtrlPointsNew.Clear();
            if (Image != null)
            {
                Image.Dispose();
                Image = null;
            }
        }

        public ImageBgra32 GetImage(Boolean showGridsAndCtrlPoints = false)
        {
            ImageBgra32 img = Image.Clone() as ImageBgra32;
            if (showGridsAndCtrlPoints == true)
            {
                foreach (TriangleF t in Mesh)
                {
                    img.DrawLine(t.P0, t.P1, Bgra32.RED);
                    img.DrawLine(t.P0, t.P2, Bgra32.RED);
                    img.DrawLine(t.P1, t.P2, Bgra32.RED);
                }

                foreach(PointF p in this.CtrlPointsOld)
                {
                    img.DrawCircle(p.X, p.Y, Bgra32.BLUE, 5);
                }
            }
            return img;
        }

        /// <summary>
        /// 加载图像
        /// </summary>
        /// <param name="img">输入图像</param>
        public void Load(ImageBgra32 img)
        {
            Clear();
            const int gridSize = 30;
            Image = img.Clone() as ImageBgra32;

            int width = img.Width;
            int height = img.Height;
            float wGridCount = width / gridSize + 1;
            float hGridCount = height / gridSize + 1;

            float wStep = width / wGridCount;
            float hStep = height / hGridCount;

            List<float> wVals = new List<float>();
            List<float> hVals = new List<float>();

            for (int i = 0; i <= wGridCount; i++)
            {
                if (i != wGridCount)
                {
                    wVals.Add(i * wStep);
                }
                else
                {
                    wVals.Add(width - 1);
                }
            }

            for (int i = 0; i <= hGridCount; i++)
            {
                if (i != hGridCount)
                {
                    hVals.Add(i * hStep);
                }
                else
                {
                    hVals.Add(height - 1);
                }
            }

            for (int wGrid = 1; wGrid <= wGridCount; wGrid++)
            {
                for (int hGrid = 1; hGrid <= hGridCount; hGrid++)
                {
                    PointF p00 = new PointF(wVals[wGrid - 1], hVals[hGrid - 1]);
                    PointF p01 = new PointF(wVals[wGrid - 1], hVals[hGrid]);
                    PointF p10 = new PointF(wVals[wGrid], hVals[hGrid - 1]);
                    PointF p11 = new PointF(wVals[wGrid], hVals[hGrid]);
                    TriangleF t0 = new TriangleF(p00, p01, p10);
                    TriangleF t1 = new TriangleF(p01, p10, p11);
                    Mesh.Add(t0);
                    Mesh.Add(t1);
                    MeshNew.Add(t0);
                    MeshNew.Add(t1);
                }
            }
        }

        /// <summary>
        /// 获取变形后的图像
        /// </summary>
        /// <returns>变形后的图像</returns>
        public MorphResult GetImageMorphed(Boolean showGridsAndCtrlPoints = false)
        {
            if (CtrlPointsOld.Count < 4 || IsCtrlPointsUnMoved() == true)
            {
                MorphResult mr = new MorphResult();
                mr.Image = GetImage(showGridsAndCtrlPoints);
                return mr;
            }
            else
            {
                TpsInterpolation2D tps = new TpsInterpolation2D(CtrlPointsOld, CtrlPointsNew);
                for (int i = 0; i < Mesh.Count; i++)
                {
                    TriangleF t = Mesh[i];
                    t.P0 = tps.Interpolate(t.P0);
                    t.P1 = tps.Interpolate(t.P1);
                    t.P2 = tps.Interpolate(t.P2);
                    MeshNew[i] = t;
                }
                return GetImageMorphed(Mesh, MeshNew, showGridsAndCtrlPoints);
            }
        }

        private unsafe MorphResult GetImageMorphed(List<TriangleF> oldMesh, List<TriangleF> newMesh, Boolean showGridsAndCtrlPoints = false)
        {
            // 计算图像尺寸和偏移量
            float maxX = float.MinValue;
            float minX = float.MaxValue;
            float maxY = float.MinValue;
            float minY = float.MaxValue;

            foreach (TriangleF t in newMesh)
            {
                maxX = Math.Max(maxX, t.P0.X);
                maxX = Math.Max(maxX, t.P1.X);
                maxX = Math.Max(maxX, t.P2.X);
                minX = Math.Min(minX, t.P0.X);
                minX = Math.Min(minX, t.P1.X);
                minX = Math.Min(minX, t.P2.X);
                maxY = Math.Max(maxY, t.P0.Y);
                maxY = Math.Max(maxY, t.P1.Y);
                maxY = Math.Max(maxY, t.P2.Y);
                minY = Math.Min(minY, t.P0.Y);
                minY = Math.Min(minY, t.P1.Y);
                minY = Math.Min(minY, t.P2.Y);
            }

            int minXInt = (int)Math.Floor(minX);
            int minYInt = (int)Math.Floor(minY);
            int maxXInt = (int)Math.Floor(maxX) + 1;
            int maxYInt = (int)Math.Floor(maxY) + 1;

            MorphResult m = new MorphResult();

            m.XOffset = minXInt;
            m.YOffset = minYInt;

            int width = maxXInt - minXInt + 1;
            int height = maxYInt - minYInt + 1;

            XOffset = minXInt;
            YOffset = minYInt;

            ImageBgra32 img = new ImageBgra32(width, height);
            m.Image = img;

            // 根据偏移量调整坐标
            List<TriangleF> meshAdjusted = new List<TriangleF>();
            foreach (TriangleF t in newMesh)
            {
                TriangleF newT = t;
                newT.P0.X -= minXInt;
                newT.P0.Y -= minYInt;
                newT.P1.X -= minXInt;
                newT.P1.Y -= minYInt;
                newT.P2.X -= minXInt;
                newT.P2.Y -= minYInt;
                meshAdjusted.Add(newT);
            }


            // imgInx 记录每个像素点所属Mesh三角形的索引idx。计算索引图像的过程等同于用索引值填充三角形。
            ImageInt32 imgIdx = new ImageInt32(width, height, img.Start);
            imgIdx.Fill(-1);
            Int32* p,p0,p1;
            bool f0, f1;
            for (Int32 idx = 0; idx < meshAdjusted.Count; idx++)
            {
                TriangleF t = meshAdjusted[idx];

                // 先绘制三角形
                imgIdx.DrawLine(t.P0, t.P1, idx);
                imgIdx.DrawLine(t.P0, t.P2, idx);
                imgIdx.DrawLine(t.P1, t.P2, idx);

                // 计算三角形的边界
                maxX = float.MinValue;
                minX = float.MaxValue;
                maxY = float.MinValue;
                minY = float.MaxValue;
                maxX = Math.Max(maxX, t.P0.X);
                maxX = Math.Max(maxX, t.P1.X);
                maxX = Math.Max(maxX, t.P2.X);
                minX = Math.Min(minX, t.P0.X);
                minX = Math.Min(minX, t.P1.X);
                minX = Math.Min(minX, t.P2.X);
                maxY = Math.Max(maxY, t.P0.Y);
                maxY = Math.Max(maxY, t.P1.Y);
                maxY = Math.Max(maxY, t.P2.Y);
                minY = Math.Min(minY, t.P0.Y);
                minY = Math.Min(minY, t.P1.Y);
                minY = Math.Min(minY, t.P2.Y);

                minXInt = (int)Math.Floor(minX);
                minYInt = (int)Math.Floor(minY);
                maxXInt = (int)Math.Floor(maxX) + 1;
                maxYInt = (int)Math.Floor(maxY) + 1;

                // 使用多边形填充算法填充三角形
                for (int h = minYInt; h <= maxYInt; h++)
                {
                    p0 = imgIdx.Start + h * width + minXInt;
                    p1 = imgIdx.Start + h * width + maxXInt;

                    for (int w = minXInt; w <= maxXInt; w++)
                    {
                        f0 = (*p0 == idx);
                        f1 = (*p1 == idx);

                        if (f0 == false) p0++; // 未扫描到线
                        if (f1 == false) p1--; // 未扫描到线
                        if (f0 == true && f1 == true) break;
                    }

                    // 填充三角形
                    for (p = p0; p <= p1; p++)
                    {
                        *p = idx;
                    }
                }
            }

            // 计算 meshAdjusted => Mesh 的仿射变换参数
            List<AffineTransform> list = new List<AffineTransform>();
            for (Int32 i = 0; i < meshAdjusted.Count; i++)
            {
                TriangleF t1 = meshAdjusted[i];
                TriangleF t2 = Mesh[i];
                AffineTransform at = new AffineTransform(t1, t2);
                list.Add(at);
            }

            Bgra32 empty = Bgra32.EMPTY;
            Bgra32* pArgb32;
            double x0 = 0;
            double y0 = 0;
            double a, b, a0,a1,b0,b1;
            int xOld0, xOld1, yOld0, yOld1;
            int wMax = Image.Width - 1;
            int hMax = Image.Height - 1;
            Bgra32 v00, v01, v10, v11;
            // 进行仿射变换
            for (int h = 0; h < height; h++)
            {
                p = imgIdx.Start + h * width;
                pArgb32 = img.Start + h * width;
                for (int w = 0; w < width; w++, p++, pArgb32++)
                {
                    if (*p == -1) // 透明像素
                    {
                        *pArgb32 = empty;
                    }
                    else
                    {
                        list[*p].Transform(w,h,ref x0, ref y0);

                        xOld0 = (int)Math.Floor(x0);
                        yOld0 = (int)Math.Floor(y0);
                        xOld1 = xOld0 + 1;
                        yOld1 = yOld0 + 1;
                        xOld0 = Math.Max(0, Math.Min(xOld0, wMax));
                        yOld0 = Math.Max(0, Math.Min(yOld0, hMax));
                        xOld1 = Math.Max(0, Math.Min(xOld1, wMax));
                        yOld1 = Math.Max(0, Math.Min(yOld1, hMax));
                        v00 = Image[yOld0, xOld0];
                        v01 = Image[yOld1,xOld0];
                        v10 = Image[yOld0,xOld1];
                        v11 = Image[yOld1,xOld1];

                        a = x0 - xOld0;
                        b = y0 - yOld0;
                        a0 = (1 - a) * (1 - b);
                        b0 = a * (1 - b);
                        a1 = b * (1 - a);
                        b1 = a * b;

                        //*pArgb32 = v00;
                        pArgb32->Blue = (Byte)Math.Max(0,Math.Min(255,(int)(a0 * v00.Blue + a1 * v01.Blue + b0 * v10.Blue + b1 * v11.Blue)));
                        pArgb32->Green = (Byte)Math.Max(0, Math.Min(255, (int)(a0 * v00.Green + a1 * v01.Green + b0 * v10.Green + b1 * v11.Green)));
                        pArgb32->Red = (Byte)Math.Max(0, Math.Min(255, (int)(a0 * v00.Red + a1 * v01.Red + b0 * v10.Red + b1 * v11.Red)));
                        pArgb32->Alpha = (Byte)Math.Max(0, Math.Min(255, (int)(a0 * v00.Alpha + a1 * v01.Alpha + b0 * v10.Alpha + b1 * v11.Alpha)));
                    }
                }
            }

            if (showGridsAndCtrlPoints == true)
            {
                foreach (TriangleF t in meshAdjusted)
                {
                    img.DrawLine(t.P0, t.P1, Bgra32.RED);
                    img.DrawLine(t.P0, t.P2, Bgra32.RED);
                    img.DrawLine(t.P1, t.P2, Bgra32.RED);
                }

                foreach (PointF cp in this.CtrlPointsNew)
                {
                    img.DrawCircle(cp.X - m.XOffset, cp.Y - m.YOffset, Bgra32.BLUE, 5);
                }
            }

            imgIdx.Dispose();
            return m;
        }

        /// <summary>
        /// 添加控制点。若在相同位置存在控制点，或者控制点在图像外部，则添加失败。
        /// 添加控制点不会导致图像变形。
        /// </summary>
        /// <param name="p">控制点的位置</param>
        /// <returns>是否添加成功</returns>
        public Boolean AddCtrlPoint(PointF p)
        {
            foreach (PointF item in CtrlPointsNew)
            {
                if (AtSamePosition(item, p) == true)
                {
                    return false;
                }
            }

            PointF pOld = BackMap(p);
            if (pOld.X >= 0)
            {
                CtrlPointsNew.Add(p);
                CtrlPointsOld.Add(pOld);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 计算点变形前的位置。如果所返回点的坐标小于0，则代表所查找的点未在图像范围内。
        /// </summary>
        /// <param name="p">变形后的点</param>
        /// <returns>变形之前的点</returns>
        private PointF BackMap(PointF p)
        {
            PointF pOld = new PointF();
            pOld.X = -1;
            pOld.Y = -1;
            for(int i = 0; i < MeshNew.Count; i ++)
            {
                TriangleF tNew = MeshNew[i];
                if (tNew.Contains(p) == true)
                {
                    TriangleF tOld = Mesh[i];

                    // 计算仿射变换 tNew => tOld
                    AffineTransform t = new AffineTransform(tNew, tOld);
                    t.Transform(p, ref pOld);

                    return pOld;
                }
            }
            return pOld;
        }

        private Boolean AtSamePosition(PointF p0, PointF p1)
        {
            return Math.Abs(p0.X - p1.X) + Math.Abs(p0.Y - p1.Y) < 1;
        }

        private Boolean IsCtrlPointsUnMoved()
        {
            for (int i = 0; i < CtrlPointsNew.Count; i++)
            {
                if (CtrlPointsOld[i] != CtrlPointsNew[i]) return false;
            }
            return true;
        }

        /// <summary>
        /// 移动控制点
        /// </summary>
        /// <param name="index">控制点的索引</param>
        /// <param name="pNew">控制点的新的位置</param>
        public void MoveCtrlPoint(int index, PointF pNew)
        {
            if (index < 0 || index >= CtrlPointsNew.Count) return;

            CtrlPointsNew[index] = pNew;
            if (CtrlPointsOld.Count < 4)
            {
                CtrlPointsOld[index] = pNew;
            }
        }
    }
}
