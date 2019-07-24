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

        // 模型空间下的坐标
        public Vector3 modelSpacePos;
        // 初始时是模型空间下的坐标，经过屏幕映射后，变为屏幕空间下的坐标
        public Vector3 pos;
        // 该顶点对应的uv值
        public float u, v;
        // 顶点颜色
        public Color01 color;
        // 顶点法线
        public Vector3 normal;
        // 顶点切线
        public Vector3 tangent;

        public Vertex() : this(Vector3.Zero, Color01.White,0,0) { }

        public Vertex(Vector3 pos,Color01 c,float u,float v) {
            this.pos = pos;
            this.color = c;
            this.u = u;
            this.v = v;

            // 一并初始化
            this.modelSpacePos = pos;
        }

        // 顶点数据差值
        public static Vertex LerpVertexData(Vertex left,Vertex right,float t) {
            Vertex result = new Vertex();

            result.modelSpacePos = Vector3.LerpVector3(left.modelSpacePos,right.modelSpacePos,t); 
            // 对z值进行插值
            result.pos.Z = MathF.LerpFloat(left.pos.Z,right.pos.Z,t);
            // 对颜色属性进行插值
            result.color = Color01.LerpColor(left.color,right.color,t);
            // 对uv进行插值(此处插值不正确,对于投影变换
            // 需进行 透视插值矫正)
            result.u = MathF.LerpFloat(left.u,right.u,t);
            result.v = MathF.LerpFloat(left.v, right.v,t);

            // 对法线进行插值
            result.normal = Vector3.LerpVector3(left.normal,right.normal,t);

            // 对切线进行插值
            result.tangent = Vector3.LerpVector3(left.tangent,right.tangent,t);            

            return result;
        }

        public override string ToString() {
            return string.Format("({0},{1},{2},{3})",pos.X,pos.Y,pos.Z,pos.W);
        }


        public Vertex DeepyCopy() {
            Vertex result = new Vertex();

            result.modelSpacePos = modelSpacePos;

            result.pos.X = pos.X;
            result.pos.Y = pos.Y;
            result.pos.W = pos.W; 
            
            result.pos.Z = pos.Z;
            
            result.color = color;

            result.u = u;
            result.v = v;

            // 对法线进行插值
            result.normal = normal;

            // 对切线进行插值
            result.tangent = tangent;

            return result;
        }
    }
}
