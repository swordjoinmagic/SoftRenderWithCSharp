using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortRenderWithCSharp {
    public class Mesh {

        #region 顶点的MVP矩阵，原则上来说，mesh不应该有这个属性，但是这里为了计算方便，引用一份它的MVP矩阵，方便光照计算

        // 记录MVP矩阵的三个引用，这里记录的都是引用，所以不会特别消耗内存
        public Matrix4x4 mMatrix;
        public Matrix4x4 vMatrix;
        public Matrix4x4 pMatrix;

        #endregion

        public Vertex[] vertices;
        public int[] triangles;
        public Vector3[] normals;
        public Vector3[] tangents;

        public Mesh() { }
        public Mesh(Vertex[] vertices,int[] triangles,Vector3[] normals) {
            this.vertices = vertices;
            this.triangles = triangles;
            this.normals = normals;                            

            CalculateVerticsTangent(vertices,triangles);
        }

        /// <summary>
        /// 计算顶点数组中所有顶点的法线方向（正方向）
        /// 
        /// 计算方法是：
        ///     计算一个三角形图元内，该顶点指向另外两个顶点的向量的叉积，即为该顶点的法线正方向
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="triangle"></param>
        /// <returns></returns>
        public void CalculateVerticsTangent(Vertex[] vertices, int[] triangle) {
            // 凑不成一系列三角形图元就退出
            if (triangle.Length % 3 != 0) return;

            for (int i = 0; i < triangle.Length; i += 3) {
                Vertex v1 = vertices[triangle[i]];
                Vertex v2 = vertices[triangle[i + 1]];
                Vertex v3 = vertices[triangle[i + 2]];

                float a = 1.0f / ((v2.u - v1.u) * (v3.v - v1.v) - (v2.v - v1.v) * (v3.u - v1.u));
                Vector3 Q0 = v2.pos - v1.pos;
                Vector3 Q1 = v3.pos - v1.pos;

                // 面切线
                Vector3 tangentF = new Vector3(
                    x: (v3.v - v1.v) * Q0.X + (v1.v - v2.v) * Q1.X,
                    y: (v3.v - v1.v) * Q0.Y + (v1.v - v2.v) * Q1.Y,
                    z: (v3.v - v1.v) * Q0.Z + (v1.v - v2.v) * Q1.Z
                ) * a;
                // 面副切线
                Vector3 binormalF = new Vector3(
                    x: (v1.u - v3.u) * Q0.X + (v2.u - v1.u) * Q1.X,
                    y: (v1.u - v3.u) * Q0.Y + (v2.u - v1.u) * Q1.Y,
                    z: (v1.u - v3.u) * Q0.Z + (v2.u - v1.u) * Q1.Z
                );

                v1.tangent = tangentF - v1.normal * (Vector3.Dot(tangentF, v1.normal));
                v1.tangent.Normlize();
                v2.tangent = tangentF - v2.normal * (Vector3.Dot(tangentF, v2.normal));
                v2.tangent.Normlize();
                v3.tangent = tangentF - v3.normal * (Vector3.Dot(tangentF, v3.normal));
                v3.tangent.Normlize();

                v1.tangent.W = Vector3.Dot(Vector3.Cross(v1.normal, v1.tangent), binormalF) > 0 ? 1 : -1;
                v2.tangent.W = Vector3.Dot(Vector3.Cross(v2.normal, v2.tangent), binormalF) > 0 ? 1 : -1;
                v3.tangent.W = Vector3.Dot(Vector3.Cross(v3.normal, v3.tangent), binormalF) > 0 ? 1 : -1;
            }

        }
    }
}
