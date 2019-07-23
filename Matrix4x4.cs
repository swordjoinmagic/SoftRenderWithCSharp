using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortRenderWithCSharp {
    /// <summary>
    /// 4x4的矩阵对象
    /// </summary>
    public class Matrix4x4 {        
        // 矩阵内部值
        public float[,] value = new float[4,4];

        /// <summary>
        /// 将当前矩阵设为单位矩阵
        /// </summary>
        public void Identity() {
            for (int i=0;i<4;i++) {
                value[i, i] = 1;
            }
        }

        /// <summary>
        /// 将当前矩阵转置
        /// </summary>
        public void Transpose() {
            for (int i = 0; i < 4; i++) {
                for (int j = i; j < 4; j++) {
                    float temp = value[i, j];
                    value[i, j] = value[j, i];
                    value[j, i] = temp;
                }
            }
        }

        /// <summary>
        /// 矩阵乘法
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix4x4 operator *(Matrix4x4 left,Matrix4x4 right) {
            Matrix4x4 matrix = new Matrix4x4();

            for (int i=0;i<4;i++) {
                for (int j=0;j<4;j++) {

                    // 初始化
                    matrix.value[i, j] = 0;

                    // (i,j)表示矩阵第i行乘于另一个矩阵的第j列
                    for (int k=0;k<4;k++) {
                        matrix.value[i, j] += left.value[i, k] * right.value[k,j];
                    }

                }
            }

            return matrix;
        }

        /// <summary>
        /// 矩阵数乘
        /// </summary>
        /// <param name="left"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Matrix4x4 operator *(Matrix4x4 left,float value) {

            Matrix4x4 matrix = new Matrix4x4() ;

            for (int i=0;i<4;i++) {
                for (int j=0;j<4;j++) {
                    matrix.value[i,j] = left.value[i,j] *value;
                }
            }

            return matrix;
        }

        /// <summary>
        /// 矩阵与向量相乘，这里将向量作为列矩阵看待，所以这里相当于进行了一次矩阵乘法
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector3 operator *(Matrix4x4 left,Vector3 right) {
            return new Vector3 {
                X = left.value[0, 0] * right.X + left.value[0, 1] * right.Y + left.value[0, 2] * right.Z + left.value[0, 3] * right.W,
                Y = left.value[1, 0] * right.X + left.value[1, 1] * right.Y + left.value[1, 2] * right.Z + left.value[1, 3] * right.W,
                Z = left.value[2, 0] * right.X + left.value[2, 1] * right.Y + left.value[2, 2] * right.Z + left.value[2, 3] * right.W,
                W = left.value[3, 0] * right.X + left.value[3, 1] * right.Y + left.value[3, 2] * right.Z + left.value[3, 3] * right.W,
            };
        }
    }
}
