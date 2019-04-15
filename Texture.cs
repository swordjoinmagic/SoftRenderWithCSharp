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
            int width = (int)(u * texture2D.Width);
            int height = (int)(v * texture2D.Height);
            Color color = texture2D.GetPixel(width,height);
            return Color01.FromColor(color);
        }
    }
}
