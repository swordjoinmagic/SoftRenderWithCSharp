using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortRenderWithCSharp {
    
    /// <summary>
    /// RGB值被归一到01区间的Color类
    /// </summary>
    public struct Color01 {

        float r, g, b, a;

        public float R { get => MathF.Clamp01(r); set => r = value; }
        public float G { get => MathF.Clamp01(g); set => g = value; }
        public float B { get => MathF.Clamp01(b); set => b = value; }
        public float A { get => MathF.Clamp01(a); set => a = value; }

        public static Color01 White = new Color01(1,1,1,1);
        public static Color01 Black = new Color01(0,0,0,1);
        

        public Color01(float r,float g,float b,float a) {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public static Color01 operator +(Color01 left,Color01 right) {
            return new Color01(
                left.r + right.r,
                left.g + right.g,
                left.b + right.b,
                left.a + right.a
                );
        }

        public static Color01 operator -(Color01 left,Color01 right) {
            return new Color01(
                left.r - right.r,
                left.g - right.g,
                left.b - right.b,
                left.a - right.a);
        }

        /// <summary>
        /// 颜色的乘法大多用于光照，他跟向量的乘法定义不同，
        /// 这里直接是分量相乘
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Color01 operator *(Color01 left,Color01 right) {
            return new Color01(
                left.r * right.r,
                left.g * right.g,
                left.b * right.b,
                left.a * right.a
                );
        }

        /// <summary>
        /// 颜色数乘
        /// </summary>
        /// <param name="left"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Color01 operator *(Color01 left,float value) {
            return new Color01(
                left.r * value,
                left.g * value,
                left.b * value,
                left.a * value
                );
        }

        public static Color01 operator /(Color01 left,float value) {
            return new Color01(
                left.r / value,
                left.g / value,
                left.b / value,
                left.a / value
                );
        }

        public static Color01 LerpColor(Color01 left,Color01 right,float t) {
            return left + (right - left) * t;
        }

        // 当前使用的Color变到Winform所用的Color类
        public Color ToColor() {
            return Color.FromArgb((int)(255*A),(int)(255*R),(int)(255*G),(int)(255*B));
        }

        // 将Color类转化为Color01类
        public static Color01 FromColor(Color color) {
            return new Color01(
                r: (float)color.R/byte.MaxValue,
                g: (float)color.G/ byte.MaxValue,
                b: (float)color.B/ byte.MaxValue,
                a: (float)color.A/ byte.MaxValue
                );
        }

        public override string ToString() {
            return string.Format("(r:{0},g:{1},b:{2},a:{3}",r,g,b,a);
        }
    }
}
