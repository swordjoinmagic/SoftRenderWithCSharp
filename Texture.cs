using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static Color01 Tex2D(Bitmap texture2D,float u,float v) {

            // 采用Clamp模式，直接对uv进行截断
            u = MathF.Clamp01(u);
            v = MathF.Clamp01(v);

            int tx = (int)(u * (texture2D.Width-1));
            int ty = (int)(v * (texture2D.Height-1));

            tx = MathF.Clamp(tx,0,texture2D.Width);
            ty = MathF.Clamp(ty, 0, texture2D.Height);

            Color color = texture2D.GetPixel(tx,ty);

            Color01 color01 = Color01.FromColor(color);
            return color01;
        }
    }
}
