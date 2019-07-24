﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortRenderWithCSharp {
    class MathF {

        public static float Deg2Rad = (float)(Math.PI/180f);

        /// <summary>
        /// 返回x的小数部分
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float Frac(float x) {
            return x-(int)(x);
        }

        /// <summary>
        /// 快速平方根
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float Sqrt(float x) {
            unsafe {
                float a = x;
                uint i = *(uint*)&x;
                i = (i + 0x3f76cf62) >> 1;
                x = *(float*)&i;
                x = (x + a / x) * 0.5f;
                return x;
            }
        }

        public static float LerpFloat(float v1,float v2,float t) {
            return v1 + (v2 - v1) * t;
        }

        public static float Clamp01(float f) {
            if (f >= 1f)
                return 1f;
            else if (f <= 0)
                return 0;

            return f;
        }

        public static int Clamp(int value,int min,int max) {
            if (value < min) return min;
            else if (value > max) return max;
            return value;
        }
    }
}
