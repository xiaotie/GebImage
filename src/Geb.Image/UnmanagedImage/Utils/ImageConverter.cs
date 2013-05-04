/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Geb.Image
{
    public sealed class UnmanagedImageConverter
    {
        /* 1024*(([0..511]./255)**(1./3)) */
        static ushort[] icvLabCubeRootTab = new ushort[] {
        0,161,203,232,256,276,293,308,322,335,347,359,369,379,389,398,
        406,415,423,430,438,445,452,459,465,472,478,484,490,496,501,507,
        512,517,523,528,533,538,542,547,552,556,561,565,570,574,578,582,
        586,590,594,598,602,606,610,614,617,621,625,628,632,635,639,642,
        645,649,652,655,659,662,665,668,671,674,677,680,684,686,689,692,
        695,698,701,704,707,710,712,715,718,720,723,726,728,731,734,736,
        739,741,744,747,749,752,754,756,759,761,764,766,769,771,773,776,
        778,780,782,785,787,789,792,794,796,798,800,803,805,807,809,811,
        813,815,818,820,822,824,826,828,830,832,834,836,838,840,842,844,
        846,848,850,852,854,856,857,859,861,863,865,867,869,871,872,874,
        876,878,880,882,883,885,887,889,891,892,894,896,898,899,901,903,
        904,906,908,910,911,913,915,916,918,920,921,923,925,926,928,929,
        931,933,934,936,938,939,941,942,944,945,947,949,950,952,953,955,
        956,958,959,961,962,964,965,967,968,970,971,973,974,976,977,979,
        980,982,983,985,986,987,989,990,992,993,995,996,997,999,1000,1002,
        1003,1004,1006,1007,1009,1010,1011,1013,1014,1015,1017,1018,1019,1021,1022,1024,
        1025,1026,1028,1029,1030,1031,1033,1034,1035,1037,1038,1039,1041,1042,1043,1044,
        1046,1047,1048,1050,1051,1052,1053,1055,1056,1057,1058,1060,1061,1062,1063,1065,
        1066,1067,1068,1070,1071,1072,1073,1074,1076,1077,1078,1079,1081,1082,1083,1084,
        1085,1086,1088,1089,1090,1091,1092,1094,1095,1096,1097,1098,1099,1101,1102,1103,
        1104,1105,1106,1107,1109,1110,1111,1112,1113,1114,1115,1117,1118,1119,1120,1121,
        1122,1123,1124,1125,1127,1128,1129,1130,1131,1132,1133,1134,1135,1136,1138,1139,
        1140,1141,1142,1143,1144,1145,1146,1147,1148,1149,1150,1151,1152,1154,1155,1156,
        1157,1158,1159,1160,1161,1162,1163,1164,1165,1166,1167,1168,1169,1170,1171,1172,
        1173,1174,1175,1176,1177,1178,1179,1180,1181,1182,1183,1184,1185,1186,1187,1188,
        1189,1190,1191,1192,1193,1194,1195,1196,1197,1198,1199,1200,1201,1202,1203,1204,
        1205,1206,1207,1208,1209,1210,1211,1212,1213,1214,1215,1215,1216,1217,1218,1219,
        1220,1221,1222,1223,1224,1225,1226,1227,1228,1229,1230,1230,1231,1232,1233,1234,
        1235,1236,1237,1238,1239,1240,1241,1242,1242,1243,1244,1245,1246,1247,1248,1249,
        1250,1251,1251,1252,1253,1254,1255,1256,1257,1258,1259,1259,1260,1261,1262,1263,
        1264,1265,1266,1266,1267,1268,1269,1270,1271,1272,1273,1273,1274,1275,1276,1277,
        1278,1279,1279,1280,1281,1282,1283,1284,1285,1285,1286,1287,1288,1289,1290,1291
        };

        const float labXr_32f = 0.433953f /* = xyzXr_32f / 0.950456 */;
        const float labXg_32f = 0.376219f /* = xyzXg_32f / 0.950456 */;
        const float labXb_32f = 0.189828f /* = xyzXb_32f / 0.950456 */;

        const float labYr_32f = 0.212671f /* = xyzYr_32f */;
        const float labYg_32f = 0.715160f /* = xyzYg_32f */;
        const float labYb_32f = 0.072169f /* = xyzYb_32f */;

        const float labZr_32f = 0.017758f /* = xyzZr_32f / 1.088754 */;
        const float labZg_32f = 0.109477f /* = xyzZg_32f / 1.088754 */;
        const float labZb_32f = 0.872766f /* = xyzZb_32f / 1.088754 */;

        const float labRx_32f = 3.0799327f  /* = xyzRx_32f * 0.950456 */;
        const float labRy_32f = (-1.53715f) /* = xyzRy_32f */;
        const float labRz_32f = (-0.542782f)/* = xyzRz_32f * 1.088754 */;

        const float labGx_32f = (-0.921235f)/* = xyzGx_32f * 0.950456 */;
        const float labGy_32f = 1.875991f   /* = xyzGy_32f */ ;
        const float labGz_32f = 0.04524426f /* = xyzGz_32f * 1.088754 */;

        const float labBx_32f = 0.0528909755f /* = xyzBx_32f * 0.950456 */;
        const float labBy_32f = (-0.204043f)  /* = xyzBy_32f */;
        const float labBz_32f = 1.15115158f   /* = xyzBz_32f * 1.088754 */;

        const float labT_32f = 0.008856f;

        const int lab_shift = 10;

        const float labLScale2_32f = 903.3f;

        const int labXr = (int)((labXr_32f) * (1 << (lab_shift)) + 0.5);
        const int labXg = (int)((labXg_32f) * (1 << (lab_shift)) + 0.5);
        const int labXb = (int)((labXb_32f) * (1 << (lab_shift)) + 0.5);

        const int labYr = (int)((labYr_32f) * (1 << (lab_shift)) + 0.5);
        const int labYg = (int)((labYg_32f) * (1 << (lab_shift)) + 0.5);
        const int labYb = (int)((labYb_32f) * (1 << (lab_shift)) + 0.5);

        const int labZr = (int)((labZr_32f) * (1 << (lab_shift)) + 0.5);
        const int labZg = (int)((labZg_32f) * (1 << (lab_shift)) + 0.5);
        const int labZb = (int)((labZb_32f) * (1 << (lab_shift)) + 0.5);

        const float labLScale_32f = 116.0f;
        const float labLShift_32f = 16.0f;

        const int labSmallScale = (int)((31.27 /* labSmallScale_32f*(1<<lab_shift)/255 */ ) * (1 << (lab_shift)) + 0.5);

        const int labSmallShift = (int)((141.24138 /* labSmallScale_32f*(1<<lab) */ ) * (1 << (lab_shift)) + 0.5);

        const int labT = (int)((labT_32f * 255) * (1 << (lab_shift)) + 0.5);

        const int labLScale = (int)((295.8) * (1 << (lab_shift)) + 0.5);
        const int labLShift = (int)((41779.2) * (1 << (lab_shift)) + 0.5);
        const int labLScale2 = (int)((labLScale2_32f * 0.01) * (1 << (lab_shift)) + 0.5);

        public static unsafe void Copy(Byte* from, Byte* to, int length)
        {
            if (length < 1) return;
            Byte* end = from + length;
            while (from != end)
            {
                *to = *from;
                from++;
                to++;
            }
        }

        public static unsafe void ToLab24(Rgb24* from, Lab24* to, int length=1)
        {
            // 使用 OpenCV 中的算法实现

            if (length < 1) return;

            Rgb24* end = from + length;

            int x, y, z;
            int l, a, b;
            bool flag;

            while (from != end)
            {
                Byte red = from->Red;
                Byte green = from->Green;
                Byte blue = from->Blue;

                x = blue * labXb + green * labXg + red * labXr;
                y = blue * labYb + green * labYg + red * labYr;
                z = blue * labZb + green * labZg + red * labZr;

                flag = x > labT;

                x = (((x) + (1 << ((lab_shift) - 1))) >> (lab_shift));

                if (flag)
                    x = icvLabCubeRootTab[x];
                else
                    x = (((x * labSmallScale + labSmallShift) + (1 << ((lab_shift) - 1))) >> (lab_shift));

                flag = z > labT;
                z = (((z) + (1 << ((lab_shift) - 1))) >> (lab_shift));

                if (flag == true)
                    z = icvLabCubeRootTab[z];
                else
                    z = (((z * labSmallScale + labSmallShift) + (1 << ((lab_shift) - 1))) >> (lab_shift));

                flag = y > labT;
                y = (((y) + (1 << ((lab_shift) - 1))) >> (lab_shift));

                if (flag == true)
                {
                    y = icvLabCubeRootTab[y];
                    l = (((y * labLScale - labLShift) + (1 << ((2 * lab_shift) - 1))) >> (2 * lab_shift));
                }
                else
                {
                    l = (((y * labLScale2) + (1 << ((lab_shift) - 1))) >> (lab_shift));
                    y = (((y * labSmallScale + labSmallShift) + (1 << ((lab_shift) - 1))) >> (lab_shift));
                }

                a = (((500 * (x - y)) + (1 << ((lab_shift) - 1))) >> (lab_shift)) + 129;
                b = (((200 * (y - z)) + (1 << ((lab_shift) - 1))) >> (lab_shift)) + 128;

                // 据Imageshop(http://www.cnblogs.com/Imageshop/) 测试，l不会超出[0,255]范围。
                // l = l > 255 ? 255 : l < 0 ? 0 : l;

                a = a > 255 ? 255 : a < 0 ? 0 : a;
                b = b > 255 ? 255 : b < 0 ? 0 : b;

                to->L = (byte)l;
                to->A = (byte)a;
                to->B = (byte)b;

                from++;
                to++;
            }
        }

        public static unsafe void ToLab24(Argb32* from, Lab24* to, int length = 1)
        {
            // 使用 OpenCV 中的算法实现

            if (length < 1) return;

            Argb32* end = from + length;

            int x, y, z;
            int l, a, b;
            bool flag;

            while (from != end)
            {
                Byte red = from->Red;
                Byte green = from->Green;
                Byte blue = from->Blue;

                x = blue * labXb + green * labXg + red * labXr;
                y = blue * labYb + green * labYg + red * labYr;
                z = blue * labZb + green * labZg + red * labZr;

                flag = x > labT;

                x = (((x) + (1 << ((lab_shift) - 1))) >> (lab_shift));

                if (flag)
                    x = icvLabCubeRootTab[x];
                else
                    x = (((x * labSmallScale + labSmallShift) + (1 << ((lab_shift) - 1))) >> (lab_shift));

                flag = z > labT;
                z = (((z) + (1 << ((lab_shift) - 1))) >> (lab_shift));

                if (flag == true)
                    z = icvLabCubeRootTab[z];
                else
                    z = (((z * labSmallScale + labSmallShift) + (1 << ((lab_shift) - 1))) >> (lab_shift));

                flag = y > labT;
                y = (((y) + (1 << ((lab_shift) - 1))) >> (lab_shift));

                if (flag == true)
                {
                    y = icvLabCubeRootTab[y];
                    l = (((y * labLScale - labLShift) + (1 << ((2 * lab_shift) - 1))) >> (2 * lab_shift));
                }
                else
                {
                    l = (((y * labLScale2) + (1 << ((lab_shift) - 1))) >> (lab_shift));
                    y = (((y * labSmallScale + labSmallShift) + (1 << ((lab_shift) - 1))) >> (lab_shift));
                }

                a = (((500 * (x - y)) + (1 << ((lab_shift) - 1))) >> (lab_shift)) + 129;
                b = (((200 * (y - z)) + (1 << ((lab_shift) - 1))) >> (lab_shift)) + 128;

                l = l > 255 ? 255 : l < 0 ? 0 : l;
                a = a > 255 ? 255 : a < 0 ? 0 : a;
                b = b > 255 ? 255 : b < 0 ? 0 : b;

                to->L = (byte)l;
                to->A = (byte)a;
                to->B = (byte)b;

                from++;
                to++;
            }
        }

        public static unsafe void ToLab24(Byte* from, Lab24* to, int length = 1)
        {
            // 使用 OpenCV 中的算法实现

            if (length < 1) return;

            Byte* end = from + length;
            
            int x, y, z;
            int l, a, b;
            bool flag;

            while (from != end)
            {
                Byte val = *from;
                Byte red = val;
                Byte green = val;
                Byte blue = val;

                x = blue * labXb + green * labXg + red * labXr;
                y = blue * labYb + green * labYg + red * labYr;
                z = blue * labZb + green * labZg + red * labZr;

                flag = x > labT;

                x = (((x) + (1 << ((lab_shift) - 1))) >> (lab_shift));

                if (flag)
                    x = icvLabCubeRootTab[x];
                else
                    x = (((x * labSmallScale + labSmallShift) + (1 << ((lab_shift) - 1))) >> (lab_shift));

                flag = z > labT;
                z = (((z) + (1 << ((lab_shift) - 1))) >> (lab_shift));

                if (flag == true)
                    z = icvLabCubeRootTab[z];
                else
                    z = (((z * labSmallScale + labSmallShift) + (1 << ((lab_shift) - 1))) >> (lab_shift));

                flag = y > labT;
                y = (((y) + (1 << ((lab_shift) - 1))) >> (lab_shift));

                if (flag == true)
                {
                    y = icvLabCubeRootTab[y];
                    l = (((y * labLScale - labLShift) + (1 << ((2 * lab_shift) - 1))) >> (2 * lab_shift));
                }
                else
                {
                    l = (((y * labLScale2) + (1 << ((lab_shift) - 1))) >> (lab_shift));
                    y = (((y * labSmallScale + labSmallShift) + (1 << ((lab_shift) - 1))) >> (lab_shift));
                }

                a = (((500 * (x - y)) + (1 << ((lab_shift) - 1))) >> (lab_shift)) + 129;
                b = (((200 * (y - z)) + (1 << ((lab_shift) - 1))) >> (lab_shift)) + 128;

                l = l > 255 ? 255 : l < 0 ? 0 : l;
                a = a > 255 ? 255 : a < 0 ? 0 : a;
                b = b > 255 ? 255 : b < 0 ? 0 : b;

                to->L = (byte)l;
                to->A = (byte)a;
                to->B = (byte)b;

                from++;
                to++;
            }
        }

        public static unsafe void ToRgb24(Lab24* from, Rgb24* to, int length=1)
        {
            if (length < 1) return;

            // 使用 OpenCV 中的算法实现
            const float coeff0 = 0.39215686274509809f;
            const float coeff1 = 0.0f;
            const float coeff2 = 1.0f;
            const float coeff3 = (-128.0f);
            const float coeff4 = 1.0f;
            const float coeff5 = (-128.0f);

            Lab24* end = from + length;
            float x, y, z,l,a,b;
            int blue, green, red;

            while (from != end)
            {
                l = from->L * coeff0 + coeff1;
                a = from->A * coeff2 + coeff3;
                b = from->B * coeff4 + coeff5;

                l = (l + labLShift_32f) * (1.0f / labLScale_32f);
                x = (l + a * 0.002f);
                z = (l - b * 0.005f);

                y = l * l * l;
                x = x * x * x;
                z = z * z * z;

                blue = (int)((x * labBx_32f + y * labBy_32f + z * labBz_32f) * 255 + 0.5);
                green = (int)((x * labGx_32f + y * labGy_32f + z * labGz_32f) * 255 + 0.5);
                red = (int)((x * labRx_32f + y * labRy_32f + z * labRz_32f) * 255 + 0.5);

                red = red < 0 ? 0 : red > 255 ? 255 : red;
                green = green < 0 ? 0 : green > 255 ? 255 : green;
                blue = blue < 0 ? 0 : blue > 255 ? 255 : blue;

                to->Red = (byte)red;
                to->Green = (byte)green;
                to->Blue = (byte)blue;

                from++;
                to++;
            }
        }

        public static unsafe void ToRgb24(Argb32* from, Rgb24* to, int length = 1)
        {
            if (length < 1) return;
            Argb32* end = from + length;
            while (from != end)
            {
                *to = *((Rgb24*)from);
                from++;
                to++;
            }
        }

        public static unsafe void ToRgb24(byte* from, Rgb24* to, int length = 1)
        {
            if (length < 1) return;

            Byte* end = from + length;
            while (from != end)
            {
                Byte val = *from;
                to->Blue = val;
                to->Green = val;
                to->Red = val;
                from++;
                to++;
            }
        }

        public static unsafe void ToArgb32(Rgb24* from, Argb32* to, int length = 1)
        {
            if (length < 1) return;
            
            Rgb24* end = from + length;
            while (from != end)
            {
                to->Blue = from->Blue;
                to->Green = from->Green;
                to->Red = from->Red;
                to->Alpha = 255;
                from++;
                to++;
            }
        }

        public static unsafe void ToArgb32(SignedArgb64* from, Argb32* to, int length = 1)
        {
            if (length < 1) return;

            SignedArgb64* end = from + length;
            while (from != end)
            {
                to->Blue = (byte)from->Blue;
                to->Green = (byte)from->Green;
                to->Red = (byte)from->Red;
                to->Alpha = (byte)from->Alpha;
                from++;
                to++;
            }
        }

        public static unsafe void ToSignedArgb64(Rgb24* from, SignedArgb64* to, int length = 1)
        {
            if (length < 1) return;

            Rgb24* end = from + length;
            while (from != end)
            {
                to->Blue = from->Blue;
                to->Green = from->Green;
                to->Red = from->Red;
                to->Alpha = 255;
                from++;
                to++;
            }
        }

        public static unsafe void ToSignedArgb64(Argb32* from, SignedArgb64* to, int length = 1)
        {
            if (length < 1) return;

            Argb32* end = from + length;
            while (from != end)
            {
                to->Blue = from->Blue;
                to->Green = from->Green;
                to->Red = from->Red;
                to->Alpha = from->Alpha;
                from++;
                to++;
            }
        }

        public static unsafe void ToByte(Rgb24* from, byte* to, int length = 1)
        {
            if (length < 1) return;

            Rgb24* end = from + length;
            while (from != end)
            {
                *to = (Byte)(from->Blue * 0.114 + from->Green * 0.587 + from->Red * 0.299);
                from++;
                to++;
            }
        }

        public static unsafe void ToByte(Argb32* from, byte* to, int length = 1)
        {
            if (length < 1) return;

            Argb32* end = from + length;
            while (from != end)
            {
                *to = (Byte)(from->Blue * 0.114 + from->Green * 0.587 + from->Red * 0.299);
                from++;
                to++;
            }
        }

        public static unsafe void ToArgb32(Byte* from, Argb32* to, int length = 1)
        {
            if (length < 1) return;

            Byte* end = from + length;
            while (from != end)
            {
                Byte val = *from;
                to->Blue = val;
                to->Green = val;
                to->Red = val;
                to->Alpha = 255;
                from++;
                to++;
            }
        }

        public static unsafe void ToSignedArgb64(Byte* from, SignedArgb64* to, int length = 1)
        {
            if (length < 1) return;

            Byte* end = from + length;
            while (from != end)
            {
                Byte val = *from;
                to->Blue = val;
                to->Green = val;
                to->Red = val;
                to->Alpha = 255;
                from++;
                to++;
            }
        }
    }
}
