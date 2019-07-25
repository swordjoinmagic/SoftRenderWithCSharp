using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortRenderWithCSharp {
    /// <summary>
    /// 平面类,
    /// 由面法线,法线与平面的交点,平面内任意一点组成
    /// </summary>
    public class Plane {
        // 面法线
        private Vector3 normal;
        // 面法线与平面的交点
        private Vector3 point;

        public Vector3 Normal { get => normal; set => normal = value; }
        public Vector3 Point { get => point; set => point = value; }

        public Plane(Vector3 normal,Vector3 point) {
            this.normal = normal;
            this.point = point;
        }
    }
}
