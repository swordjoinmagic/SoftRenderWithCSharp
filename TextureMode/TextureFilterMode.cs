
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortRenderWithCSharp.TextureMode {
    // 纹理过滤模式
    public enum TextureFilterMode {
        Point,      // 最近点采样
        Bilinear,   // 双线性过滤
        Trilinear,  // 三线性过滤
    }
}
