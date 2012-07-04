/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geb.Image
{
    public struct Cmyk
    {
        public byte C;
        public byte M;
        public byte Y;
        public byte K;

        public Rgb24 ToRgb24()
        {
            //return new Rgb24(255 - C - K, 255 - M - K, 255 - Y - K);

            float MyC = C / 100.0f;
            float MyM = M / 100.0f;
            float MyY = Y / 100.0f;
            float MyK = K / 100.0f;

            int R = (int)((1 - (MyC * (1 - MyK) + MyK)) * 255);
            int G = (int)((1 - (MyM * (1 - MyK) + MyK)) * 255);
            int B = (int)((1 - (MyY * (1 - MyK) + MyK)) * 255);

            if (R < 0) R = 0;
            if (G < 0) G = 0;
            if (B < 0) B = 0;
            if (R > 255) R = 255;
            if (G > 255) G = 255;
            if (B > 255) B = 255;
            return new Rgb24(R, G, B);
        }

        public override string ToString()
        {
            return String.Format("C:{0}%,M:{1}%,Y:{2}%,K:{3}%", C, M, Y, K);
        }

        public static Cmyk CreateFrom(byte red, byte green, byte blue)
        {
            Cmyk cmyk = new Cmyk();

            //cmyk.C = (byte)(255 - red);
            //cmyk.M = (byte)(255 - green);
            //cmyk.Y = (byte)(255 - blue);

            //cmyk.K = (byte)Math.Min(Math.Min(cmyk.C, cmyk.M), cmyk.Y);
            //cmyk.C = (byte)(cmyk.C - cmyk.K);
            //cmyk.M = (byte)(cmyk.M - cmyk.K);
            //cmyk.Y = (byte)(cmyk.Y - cmyk.K);

            //return cmyk;

            cmyk.K = (byte)(Math.Min(Math.Min(255 - red, 255 - green), 255 - blue) / 2.55);
            int MyR = (int)(red / 2.55);
            int Div = 100 - cmyk.K;
            if (Div <= 0) Div = 1;
            cmyk.C = (byte)(100 * (100 - MyR - cmyk.K) / Div);
            int MyG = (int)(green / 2.55);
            cmyk.M = (byte)(100 * (100 - MyG - cmyk.K) / Div);
            int MyB = (int)(blue / 2.55);
            cmyk.Y = (byte)(100 * (100 - MyB - cmyk.K) / Div);
            return cmyk;
        }

        public static Cmyk CreateFrom(Rgb24 rgb)
        {
            return CreateFrom(rgb.Red, rgb.Green, rgb.Blue);
        }
    }
}
