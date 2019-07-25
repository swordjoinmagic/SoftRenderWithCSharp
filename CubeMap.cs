using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortRenderWithCSharp {
    /// <summary>
    /// 立方体纹理,
    /// 拥有六个面的纹理,
    /// 通过3D向量进行采样,从该立方体中心出发的向量与任意一个面的交点即为采样点
    /// </summary>
    public class CubeMap {
        // 纹理数组
        // 纹理顺序是 
        // front,back,left,right,top,bottom
        private Bitmap[] textures;
        // 用于采样的六个面,顺序与纹理顺序一致
        private Plane[] planes;

        public Bitmap[] Textures { get => textures; set => textures = value; }
        public Plane[] Planes { get => planes; set => planes = value; }

        /// <summary>
        /// 根据一个从立方体中心出发的方向向量来对立方体纹理进行采样,
        /// 该方向向量必须为归一化的向量
        /// </summary>
        /// <param name="cubeMap"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static Color01 TexCube(CubeMap cubeMap,Vector3 dir) {

            // 用于采样的射线
            Ray ray = new Ray(Vector3.Zero,dir,10f);

            // 分别于判断该射线与6个面的相交情况,如果与任意一个面相交,那么根据交点进行采样
            for (int i=0;i<6;i++) {
                Plane plane = cubeMap.Planes[i];

                // 如果相交,那么根据交点进行采样
                if (Ray.GetIntersectionPoint(ray, plane, out Vector3 point)) {

                }
            }

            return Color01.White;
        }
    }
}
