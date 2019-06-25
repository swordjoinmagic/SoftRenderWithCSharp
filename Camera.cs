using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortRenderWithCSharp {
    public class Camera {

        Vector3 upDir = new Vector3(0, 1, 0);
        Vector3 forwardDir = new Vector3(0,0,1);

        public Vector3 position = new Vector3(0,0,-10);
        public Vector3 rotation;
        int Near = 1;       // 距离近裁剪平面距离
        int Far = 10;       // 距离远裁剪平面距离
        int top = 1;        // 近平面中心距离上边的距离
        int bottom = -1;    // 近平面中心距离下边的距离
        int right = 1;      // 近平面中心距离右边的距离
        int left = -1;      // 近平面距离左边的距离
        int angle = 30;     // 摄像机的FOV角度

        public Camera(Vector3 position, Vector3 rotation, int near, int far, int top, int bottom, int right, int left, int angle) {
            this.position = position;
            this.rotation = rotation;
            Near = near;
            Far = far;
            this.top = top;
            this.bottom = bottom;
            this.right = right;
            this.left = left;
            this.angle = angle;
        }

        public Camera(Vector3 position, Vector3 rotation) {
            this.position = position;
            this.rotation = rotation;
        }

        public Camera() { }

        public Matrix4x4 GetProjectionMatrix() {
            return Matrix.GetProjectionMatrixWithFrustum(angle, Near, Far, right, left, top, bottom);
        }

        public Matrix4x4 GetViewMatrix() {
            Vector3 tempForwardDir = Matrix.GetRotateMatrix(rotation.X, rotation.Y, rotation.Z) * forwardDir;
            return Matrix.GetViewMatrix(position, position+tempForwardDir, upDir);
        }
    }
}
