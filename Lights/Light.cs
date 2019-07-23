using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortRenderWithCSharp.Lights {
    public abstract class Light {        

        // 光源位置
        public Vector3 position = Vector3.Zero;

        // 光源颜色,默认白色
        public Color01 lightColor = Color01.White;

        // 常数项
        public float Kc = 1.0f;

        // 可配置项
        // 一次项
        public float Kl = 0.7f;
        // 二次项
        public float Kq = 1.8f;

        /// <summary>
        /// 获得光源衰减值
        /// </summary>
        /// <param name="targetPosition">目标片元的位置,主要用于计算当前光源距离目标片元的距离</param>
        /// <returns></returns>
        public abstract float GetAtten(Vector3 targetPosition);

        /// <summary>
        /// 获得光源方向,
        /// 一般指物体指向光源的方向,
        /// 返回归一化的光源方向
        /// </summary>
        /// <param name="targetPosition">被光源照耀的物体的位置</param>
        /// <returns></returns>
        public virtual Vector3 GetDirection(Vector3 targetPosition) {
            Vector3 direction = -targetPosition + position;
            // 归一化
            direction.Normlize();

            return direction;
        }
    }
}
