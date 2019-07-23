using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortRenderWithCSharp {
    /// <summary>
    /// 向量类
    /// </summary>
    public struct Vector3 {
        private float x, y, z,w;

        public float X { get => x; set => x = value; }
        public float Y { get => y; set => y = value; }
        public float Z { get => z; set => z = value; }
        public float W { get => w; set => w = value; }

        public static Vector3 Zero = new Vector3(0,0,0);
        public static Vector3 One = new Vector3(1,1,1);

        public Vector3(float x, float y, float z) {
            this.x = x;
            this.y = y;
            this.z = z;

            // 表示这是一个点
            this.w = 1;
        }

        /// <summary>
        /// 向量加法
        /// </summary>
        /// <param name="right"></param>
        /// <param name="left"></param>
        /// <returns></returns>
        public static Vector3 operator +(Vector3 right,Vector3 left) {
            Vector3 result = new Vector3 {
                x = right.x + left.x,
                y = right.y + left.y,
                z = right.z + left.z
            };
            return result;
        }

        /// <summary>
        /// 向量减法
        /// </summary>
        /// <param name="right"></param>
        /// <param name="left"></param>
        /// <returns></returns>
        public static Vector3 operator -(Vector3 right, Vector3 left) {
            Vector3 result = new Vector3 {
                x = right.x - left.x,
                y = right.y - left.y,
                z = right.z - left.z
            };
            return result;
        }

        /// <summary>
        /// 向量数乘
        /// </summary>
        /// <param name="right"></param>
        /// <param name="left"></param>
        /// <returns></returns>
        public static Vector3 operator *(Vector3 right, float value) {
            Vector3 result = new Vector3 {
                x = right.x * value,
                y = right.y * value,
                z = right.z * value
            };
            return result;
        }

        /// <summary>
        /// 向量除法
        /// </summary>
        /// <param name="right"></param>
        /// <param name="left"></param>
        /// <returns></returns>
        public static Vector3 operator /(Vector3 right, float value) {
            Vector3 result = new Vector3 {
                x = right.x / value,
                y = right.y / value,
                z = right.z / value
            };
            return result;
        }

        public static Vector3 operator -(Vector3 vector3) {
            return new Vector3(
                -vector3.x,
                -vector3.y,
                -vector3.z
                );
        }

        /// <summary>
        /// 向量点乘
        /// </summary>
        /// <param name="right"></param>
        /// <param name="left"></param>
        /// <returns></returns>
        public static float Dot(Vector3 right,Vector3 left) {
            return right.x * left.x + right.y * left.y + right.z * left.z;
        }

        /// <summary>
        /// 向量叉乘
        /// </summary>
        /// <param name="right"></param>
        /// <param name="left"></param>
        /// <returns></returns>
        public static Vector3 Cross(Vector3 right,Vector3 left) {
            return new Vector3 {
                x = left.y * right.z - left.z * right.y,
                y = left.z * right.x - left.x * right.z,
                z = left.x * right.y - left.y * right.x
            };
        }

        /// <summary>
        /// 获得此向量的模长
        /// </summary>
        /// <returns></returns>
        public float Magnitude() {
            return MathF.Sqrt(Dot(this,this));
        }

        /// <summary>
        /// 计算向量模长的平方
        /// </summary>
        /// <returns></returns>
        public float SqureMagnitude() {
            return Dot(this,this);
        }

        /// <summary>
        /// 归一化本向量
        /// </summary>
        public void Normlize() {
            // 获得向量模长
            float magnitude = this.Magnitude();

            x /= magnitude;
            y /= magnitude;
            z /= magnitude;
        }

        public Vector3 normlize {
            get { this.Normlize();return this; }
        }

        public static Vector3 LerpVector3(Vector3 left,Vector3 right,float t) {
            return left + (right - left) * t;
        }

        public override string ToString() {
            return string.Format("({0},{1},{2})",x,y,z);
        }
    }
}
