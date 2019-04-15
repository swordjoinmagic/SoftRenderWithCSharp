using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortRenderWithCSharp {
    /// <summary>
    /// 顶点对象
    /// </summary>
    public class Vertex {
        public Vector3 pos;
        // 该顶点对应的uv值
        public float u, v;
        // 顶点颜色
        public Color01 color;

        public Vertex() : this(Vector3.Zero, Color01.White,0,0) { }

        public Vertex(Vector3 pos,Color01 c,float u,float v) {
            this.pos = pos;
            this.color = c;
            this.u = u;
            this.v = v;
        }

        // 顶点数据差值
        public static Vertex LerpVertexData(Vertex left,Vertex right,float t) {
            Vertex result = new Vertex();

            // 对z值进行插值
            result.pos.Z = MathF.LerpFloat(left.pos.Z,right.pos.Z,t);
            // 对颜色属性进行插值
            result.color = Color01.LerpColor(left.color,right.color,t);
            // 对uv进行插值(此处插值不正确,对于投影变换
            // 需进行 透视插值矫正)
            result.u = MathF.LerpFloat(left.u,right.u,t);
            result.v = MathF.LerpFloat(left.v, right.v,t);

            return result;
        }
    }
}
