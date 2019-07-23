using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortRenderWithCSharp.Lights {
    /// <summary>
    /// 平行光,
    /// 平行光没有位置属性,且
    /// </summary>
    public class DirectionalLight : Light {

        // 光源方向,默认为x轴方向
        private Vector3 direction = new Vector3(1,0,0);

        public DirectionalLight(Vector3 direction) {
            this.direction = direction;
        }
        public DirectionalLight(Vector3 direction,Color01 color) {
            this.direction = direction;
            lightColor = color;
        }

        public override float GetAtten(Vector3 targetPosition) {
            // 平行光不会衰减
            return 1;
        }

        public override Vector3 GetDirection(Vector3 targetPosition) {
            // 归一化
            direction.Normlize();
            return direction;
        }
    }
}
