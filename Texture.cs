using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SortRenderWithCSharp.TextureMode;

namespace SortRenderWithCSharp {
    public class Texture {

        /// <summary>
        /// 根据uv值对一个位图进行采样
        /// 
        /// 采用Clamp模式对位图进行采样（即当uv大于1或小于0时，直接进行截断）
        /// </summary>
        /// <param name="texture2D"></param>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Color01 Tex2D(
            Bitmap texture2D,
            float u,float v,
            TextureFilterMode filterMode=TextureFilterMode.Point,
            TextureWrapMode wrapMode=TextureWrapMode.Clamp) {

            // 根据纹理采样模式设置uv
            switch (wrapMode) {
                case TextureWrapMode.Clamp:
                    // 采用Clamp模式，直接对uv进行截断
                    u = MathF.Clamp01(u);
                    v = MathF.Clamp01(v);
                    break;
                case TextureWrapMode.Repeat:
                    // 采用Repeat模式,uv只取小数点部位
                    u = MathF.Frac(u);
                    v = MathF.Frac(v);
                    break;
            }

            Color01 finalColor = Color01.White;

            // 根据纹理过滤模式设置当前uv点的像素颜色
            switch (filterMode) {              
                case TextureFilterMode.Point:
                    // 点采样模式(最近邻采样),
                    // 选取距离当前采样点(u,v)最近的纹素作为当前像素的颜色
                    int tx = (int)(u * (texture2D.Width - 1));
                    int ty = (int)(v * (texture2D.Height - 1));

                    tx = MathF.Clamp(tx, 0, texture2D.Width);
                    ty = MathF.Clamp(ty, 0, texture2D.Height);

                    Color color = texture2D.GetPixel(tx, ty);

                    finalColor = Color01.FromColor(color);
                    break;
                case TextureFilterMode.Bilinear:
                    // 双线性采样
                    // (不开启MipMap时,会作用于Level 0(即原始图片大小),
                    // 开启后会选择图片大小与3D图形大小最相近的一层)
                    // 选取与当前采样区域邻近的四个纹素的均值作为本次采样的结果



                    break;
            }

            return finalColor;
        }
    }
}
