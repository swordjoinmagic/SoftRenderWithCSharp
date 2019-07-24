using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortRenderWithCSharp.Lights {

    /// <summary>
    /// 聚光灯,即圆锥形的光束,
    /// 具有3个属性,
    /// SpotDir: 聚光灯的朝向,类似摄像机的forwardDir,
    /// LightDir: 从片元指向光源的方向
    /// φ: 聚光灯半径的切光角,即半角
    /// θ: LightDir和SpotDir向量之间的夹角,如果目标片元在聚光灯内的话θ应小于φ
    /// </summary>
    public class SpotLight : Light {
        // φ,角度,聚光灯照耀范围的半角,此为外圆锥的半角
        private float outerAngle;
        // φ的cos值
        private float cosPhi;

        // 聚光灯内圆锥的半角
        private float innerAngle;
        private float cosInnerAngle;

        // SpotDir,聚光灯指向的方向
        private Vector3 spotDir;

        public SpotLight(float outerAngle,float innerAngle, Vector3 spotDir,Color01 lightColor,Vector3 position) {
            this.outerAngle = outerAngle;
            this.cosPhi = (float)Math.Cos(outerAngle * MathF.Deg2Rad);
            this.spotDir = spotDir;

            

            this.innerAngle = innerAngle;
            this.cosInnerAngle = (float)Math.Cos(innerAngle * MathF.Deg2Rad);

            this.lightColor = lightColor;

            this.position = position;
        }

        public override float GetAtten(Vector3 targetPosition) {

            // θ,片元指向光源的方向与聚光灯朝向的夹角(cos)
            float cosTheta = Vector3.Dot(-GetDirection(targetPosition).normlize,spotDir.normlize);
            // Epsilon是内外圆锥的余弦差值
            float epsilon = cosInnerAngle - cosPhi;
            // 聚光灯强度
            float intensity = MathF.Clamp01((cosTheta - cosPhi) / epsilon);

            return intensity;
        }
    }
}
