using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortRenderWithCSharp.Lights {
    /// <summary>
    /// 点光源，
    /// 不同于平行光，有位置属性，
    /// 每个片元受到的点光源光照的方向都不一样（平行光所有光照方向都一样），
    /// 且点光源有衰减属性，距离越远，点光源光照效果越不明显
    /// </summary>
    public class PointLight : Light {

        // 该点光源最大可以照亮的范围
        private float range;

        public PointLight(Vector3 position,float range) {
            this.position = position;
            this.range = range;
        }
        public PointLight(Vector3 position, float range,Color01 lightColor) {
            this.position = position;
            this.range = range;
            this.lightColor = lightColor;
        }


        public override float GetAtten(Vector3 targetPosition) {
            // 目标片元距离光源的距离
            float d = (targetPosition - position).Magnitude();

            // 光源衰减公式
            float atten = 1.0f / (Kc + Kl * d + Kq * d * d);
            return atten;
        }
    }
}
