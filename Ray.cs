using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortRenderWithCSharp {
    /// <summary>
    /// 射线类,
    /// 由射线起点,射线方向,射线长度三个属性进行定义
    /// </summary>
    public class Ray {
        // 射线起始坐标
        private Vector3 initPos;
        // 射线的方向
        private Vector3 direction;
        // 射线的最大长度
        private float range;

        public Ray(Vector3 pos,Vector3 dir,float distance) {
            InitPos = pos;
            Direction = dir;
            Range = distance;
        }

        public Vector3 InitPos { get => initPos; set => initPos = value; }
        public Vector3 Direction { get => direction; set => direction = value; }
        public float Range { get => range; set => range = value; }

        /// <summary>
        /// 获取平面与射线的交点,
        /// 首先已知射线的表达式为 
        /// p = p0+ut,其中p0为射线起始点,u为射线方向,t为射线长度,p为射线终点,
        /// 平面的表达式为:
        /// n·(a-a0) = 0;
        /// 其中n为面法线,a为平面内任意一点,a0为法向量与平面的交点
        /// 
        /// 设平面与射线的交点为点P,则有如下式子:
        /// 
        /// 1. P = p0+ut;
        /// 2. n·(P-a0) = 0;
        /// 
        /// 联立方程,可以解得:
        /// 
        /// t = (n·a0-n·p0) / (n·u)
        /// 
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="plane"></param>
        /// <returns></returns>
        public static bool GetIntersectionPoint(Ray ray,Plane plane,out Vector3 point) {
            point = Vector3.Zero;

            // 当射线方向与平面法线垂直时,射线与平面平行,无交点
            if (Vector3.Dot(plane.Normal, ray.Direction) == 0) return false;

            float a = Vector3.Dot(plane.Normal, plane.Point);
            float b = Vector3.Dot(plane.Normal, ray.initPos);
            float c = Vector3.Dot(plane.Normal, ray.direction);

            float t = ( Vector3.Dot(plane.Normal,plane.Point) - Vector3.Dot(plane.Normal,ray.initPos) ) / Vector3.Dot(plane.Normal, ray.direction);

            // 当交点为方向向量反向方向时，设为无交点
            // 当长度t大于射线长度range,无交点
            if (t < 0 || t > ray.Range) return false;

            point = ray.initPos + ray.direction * t;
            return true;
        }
    }
}
