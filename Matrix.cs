using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortRenderWithCSharp {
    public class Matrix {
        /// <summary>
        /// 获得缩放矩阵
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Matrix4x4 GetScaleMatrix(float x, float y, float z) {
            Matrix4x4 matrix = new Matrix4x4();
            matrix.Identity();
            matrix.value[0, 0] = x;
            matrix.value[1, 1] = y;
            matrix.value[2, 2] = z;
            return matrix;
        }

        /// <summary>
        /// 获得平移矩阵，这里的向量使用列矩阵的标准，那么平移矩阵是每行最后一个元素放置
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Matrix4x4 GetTranslateMatrix(float x, float y, float z) {
            Matrix4x4 matrix = new Matrix4x4();
            matrix.Identity();
            matrix.value[0, 3] = x;
            matrix.value[1, 3] = y;
            matrix.value[2, 3] = z;
            return matrix;
        }

        /// <summary>
        /// 获得绕x轴进行旋转的旋转矩阵,
        /// 输入的是角度
        /// </summary>
        /// <returns></returns>
        public static Matrix4x4 GetRotateXMatrix(float xAngle) {
            float radian = xAngle * 3.1415926f / 180;
            Matrix4x4 matrix = new Matrix4x4();

            // 将当前矩阵设为单位矩阵
            matrix.Identity();

            matrix.value[1, 1] = (float)Math.Cos(radian);
            matrix.value[1, 2] = -(float)Math.Sin(radian);
            matrix.value[2, 1] = (float)Math.Sin(radian);
            matrix.value[2, 2] = (float)Math.Cos(radian);

            return matrix;
        }

        /// <summary>
        /// 获得绕y轴进行旋转的旋转矩阵,
        /// 输入的是角度
        /// </summary>
        /// <returns></returns>
        public static Matrix4x4 GetRotateYMatrix(float yAngle) {
            float radian = yAngle * 3.1415926f / 180;
            Matrix4x4 matrix = new Matrix4x4();

            // 将当前矩阵设为单位矩阵
            matrix.Identity();

            matrix.value[0, 0] = (float)Math.Cos(radian);
            matrix.value[0, 2] = (float)Math.Sin(radian);
            matrix.value[2, 0] = -(float)Math.Sin(radian);
            matrix.value[2, 2] = (float)Math.Cos(radian);

            return matrix;
        }

        /// <summary>
        /// 获得绕z轴进行旋转的旋转矩阵,
        /// 输入的是角度
        /// </summary>
        /// <returns></returns>
        public static Matrix4x4 GetRotateZMatrix(float zAngle) {
            float radian = zAngle * 3.1415926f / 180;
            Matrix4x4 matrix = new Matrix4x4();

            // 将当前矩阵设为单位矩阵
            matrix.Identity();

            matrix.value[0, 0] = (float)Math.Cos(radian);
            matrix.value[0, 1] = -(float)Math.Sin(radian);
            matrix.value[1, 0] = (float)Math.Sin(radian);
            matrix.value[1, 1] = (float)Math.Cos(radian);

            return matrix;
        }

        /// <summary>
        /// 获得旋转矩阵
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Matrix4x4 GetRotateMatrix(float x, float y, float z) {

            Matrix4x4 zRotateMatrix = GetRotateZMatrix(z);
            Matrix4x4 xRotateMatrix = GetRotateXMatrix(x);
            Matrix4x4 yRotateMatrix = GetRotateYMatrix(y);

            // 变换顺序 y->x->z
            return zRotateMatrix * xRotateMatrix * yRotateMatrix;
        }


        /// <summary>
        /// 根据顶点的位置、旋转方向、缩放程度构建M矩阵
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotate"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static Matrix4x4 GetModelMatrix(Vector3 position, Vector3 rotate, Vector3 scale) {

            Matrix4x4 scaleMatrix = GetScaleMatrix(scale.X, scale.Y, scale.Z);
            Matrix4x4 rotateMatrix = GetRotateMatrix((int)rotate.X, (int)rotate.Y, (int)rotate.Z);
            Matrix4x4 translateMatrix = GetTranslateMatrix(position.X, position.Y, position.Z);


            /*
                这里是将向量当做列矩阵处理，所以矩阵是右乘操作，
                变换顺序是

                缩放 -> 旋转 -> 平移
            */
            return translateMatrix * rotateMatrix * scaleMatrix;
        }

        /// <summary>
        /// 获得观察矩阵,基于UVN相机模型
        /// </summary>
        /// <param name="position">摄像机位置</param>
        /// <param name="target">相机观察位置</param>
        /// <param name="up">相机向上的方向(粗略地)</param>
        /// <returns></returns>
        public static Matrix4x4 GetViewMatrix(Vector3 position, Vector3 target, Vector3 upDir) {
            // 获得从摄像机指向目标的方向
            Vector3 forwardDir = target - position;
            // 根据forwardDir和upDir的叉积,计算出摄像机向右的向量
            Vector3 rightDir = Vector3.Cross(forwardDir, upDir);
            // 根据计算得到的正确的向右方向和forwardDir,计算正确的向上的方向
            upDir = Vector3.Cross(rightDir, forwardDir);

            // 将三个正交基归一化
            forwardDir.Normlize();
            rightDir.Normlize();
            upDir.Normlize();

            // 现在就得到了三个标准正交基,构建旋转矩阵
            Matrix4x4 viewMatrix = new Matrix4x4();
            viewMatrix.Identity();
            // 已知旋转后空间的三个标准正交基在未旋转空间的坐标表示,
            // 求从未旋转空间到旋转后空间的变化(其中forwardDir是z轴,rightDir是x轴,upDir是y轴)
            // 将他们按列摆放

            // 第一列放x轴
            viewMatrix.value[0, 0] = rightDir.X;
            viewMatrix.value[1, 0] = rightDir.Y;
            viewMatrix.value[2, 0] = rightDir.Z;

            // 第二列放y轴
            viewMatrix.value[0, 1] = upDir.X;
            viewMatrix.value[1, 1] = upDir.Y;
            viewMatrix.value[2, 1] = upDir.Z;

            // 第三列放z轴
            viewMatrix.value[0, 2] = forwardDir.X;
            viewMatrix.value[1, 2] = forwardDir.Y;
            viewMatrix.value[2, 2] = forwardDir.Z;

            // 相当于将摄像机移回原点
            Matrix4x4 translateMatrix = GetTranslateMatrix(-position.X, -position.Y, -position.Z);

            // 观察空间使用右手坐标系,需要对z分量进行取反操作
            Matrix4x4 negateZMatrix = GetScaleMatrix(1, 1, -1);

            // 变换顺序 先平移后旋转
            viewMatrix = negateZMatrix * viewMatrix * translateMatrix;

            return viewMatrix;
        }


        /// <summary>
        /// 获得透视投影矩阵
        /// </summary>
        /// <param name="angle">摄像机的FOV角度</param>
        /// <param name="near">摄像机距离近裁剪平面的距离</param>
        /// <param name="far">摄像机距离远裁剪平面的距离</param>
        /// <param name="right">近裁剪平面中心距离右边的距离</param>
        /// <param name="left">近裁剪平面中心距离左边的距离</param>
        /// <param name="top">近裁剪平面距离上边的距离</param>
        /// <param name="bottom">近裁剪平面距离下边的距离</param>
        /// <returns></returns>
        public static Matrix4x4 GetProjectionMatrixWithFrustum(
            int angle,
            int near, int far,
            int right, int left,
            int top, int bottom) {

            // 角度转弧度
            float FOVRadian = angle * 3.1415926f / 180f;

            // 初始化矩阵
            Matrix4x4 matrix = new Matrix4x4();

            // 近平面宽度与高度的比值
            float aspect = right / top;

            float cot = (float)(Math.Cos(FOVRadian / 2) / Math.Sin(FOVRadian / 2));

            matrix.value[0, 0] = cot / aspect;
            matrix.value[1, 1] = cot;
            matrix.value[2, 2] = -((far + near) / (far - near));
            matrix.value[2, 3] = -((2 * near * far) / (far - near));
            matrix.value[3, 2] = -1;

            return matrix;
        }
    }
}
