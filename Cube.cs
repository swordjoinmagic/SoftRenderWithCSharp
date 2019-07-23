using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortRenderWithCSharp {
    class Cube : Mesh{
        public Cube() {

            #region 顶点定义

            const float UNIT_SIZE = 0.2f;

            // 顶点1/2/3
            Vector3 v1 = new Vector3(0, 0, 0);
            Vector3 v2 = new Vector3(0, 2 * UNIT_SIZE, 0);
            Vector3 v3 = new Vector3(2 * UNIT_SIZE, 0, 0);
            Vector3 v4 = new Vector3(2 * UNIT_SIZE, 2 * UNIT_SIZE, 0);
            Vector3 v5 = new Vector3(2 * UNIT_SIZE, 2 * UNIT_SIZE, -2 * UNIT_SIZE);
            Vector3 v6 = new Vector3(2 * UNIT_SIZE, 0, -2 * UNIT_SIZE);

            Vector3 v7 = new Vector3(0, 0, -2 * UNIT_SIZE);
            Vector3 v8 = new Vector3(0, 0, 0);
            Vector3 v9 = new Vector3(0, 2 * UNIT_SIZE, -2 * UNIT_SIZE);
            Vector3 v10 = new Vector3(0, 2 * UNIT_SIZE, 0);

            Vector3 v11 = new Vector3(0, 0, -2 * UNIT_SIZE);
            Vector3 v12 = new Vector3(0, 2 * UNIT_SIZE, -2 * UNIT_SIZE);
            Vector3 v13 = new Vector3(2 * UNIT_SIZE, 0, -2 * UNIT_SIZE);
            Vector3 v14 = new Vector3(2 * UNIT_SIZE, 2 * UNIT_SIZE, -2 * UNIT_SIZE);

            Vector3 v15 = new Vector3(0, 2 * UNIT_SIZE, 0);
            Vector3 v16 = new Vector3(2 * UNIT_SIZE, 2 * UNIT_SIZE, 0);
            Vector3 v17 = new Vector3(0, 2 * UNIT_SIZE, -2 * UNIT_SIZE);
            Vector3 v18 = new Vector3(2 * UNIT_SIZE, 2 * UNIT_SIZE, -2 * UNIT_SIZE);

            Vector3 v19 = new Vector3(0, 0, 0);   // 左下
            Vector3 v20 = new Vector3(0, 0, -2 * UNIT_SIZE);    // 左上
            Vector3 v21 = new Vector3(2 * UNIT_SIZE, 0, 0);     // 右下
            Vector3 v22 = new Vector3(2 * UNIT_SIZE, 0, -2 * UNIT_SIZE);    // 右上

            // 正面
            Vertex leftdown_Forward = new Vertex(v1, new Color01(1, 0, 0, 1), 0, 0);
            Vertex leftup_Forward = new Vertex(v2, new Color01(0, 1, 0, 1), 0, 1);
            Vertex rightdown_Forward = new Vertex(v3, new Color01(0, 0, 1, 1), 1, 0);
            Vertex rightup_Forward = new Vertex(v4, new Color01(0, 0, 1, 1), 1, 1);

            // 右侧面
            Vertex leftdown_Right = new Vertex(v3, new Color01(0, 1, 0, 1), 0, 0);
            Vertex leftup_Right = new Vertex(v4, new Color01(1, 0, 0, 1), 0, 1);
            Vertex rightdown_Right = new Vertex(v6, new Color01(0, 0, 1, 1), 1, 0);
            Vertex rightup_Right = new Vertex(v5, new Color01(0, 0, 1, 1), 1, 1);

            // 左侧面
            Vertex leftup_Left = new Vertex(v9, new Color01(1, 0, 0, 1), 0, 1);
            Vertex leftdown_Left = new Vertex(v7, new Color01(0, 1, 0, 1), 0, 0);
            Vertex rightdown_Left = new Vertex(v8, new Color01(0, 0, 1, 1), 1, 0);
            Vertex rightup_Left = new Vertex(v10, new Color01(0, 0, 1, 1), 1, 1);

            // 背面
            Vertex leftup_Back = new Vertex(v12, new Color01(1, 0, 0, 1), 0, 1);
            Vertex leftdown_Back = new Vertex(v11, new Color01(0, 1, 0, 1), 0, 0);
            Vertex rightdown_Back = new Vertex(v13, new Color01(0, 0, 1, 1), 1, 0);
            Vertex rightup_Back = new Vertex(v14, new Color01(0, 0, 1, 1), 1, 1);

            // 上面
            Vertex leftup_Up = new Vertex(v17, new Color01(1, 0, 0, 1), 0, 1);
            Vertex leftdown_Up = new Vertex(v15, new Color01(0, 1, 0, 1), 0, 0);
            Vertex rightdown_Up = new Vertex(v16, new Color01(0, 0, 1, 1), 1, 0);
            Vertex rightup_Up = new Vertex(v18, new Color01(0, 0, 1, 1), 1, 1);

            // 下面
            Vertex leftup_Down = new Vertex(v20, new Color01(1, 0, 0, 1), 0, 1);
            Vertex leftdown_Down = new Vertex(v19, new Color01(0, 1, 0, 1), 0, 0);
            Vertex rightdown_Down = new Vertex(v21, new Color01(0, 0, 1, 1), 1, 0);
            Vertex rightup_Down = new Vertex(v22, new Color01(0, 0, 1, 1), 1, 1);
            
            // 指向屏幕外
            Vector3 forwardV = new Vector3(0, 0, 1);
            // 指向屏幕内
            Vector3 backV = new Vector3(0, 0, -1);
            // 指向左边
            Vector3 leftV = new Vector3(-1, 0, 0);
            // 指向右边
            Vector3 rightV = new Vector3(1, 0, 0);
            // 指向上
            Vector3 upV = new Vector3(0, 1, 0);
            // 指向下
            Vector3 downV = new Vector3(0, -1, 0);

            #endregion

            vertices = new Vertex[] {
                // 正面
                leftdown_Forward, leftup_Forward, rightdown_Forward, rightup_Forward,
                // 右侧面
                leftdown_Right,leftup_Right,rightdown_Right,rightup_Right,
                // 左侧面
                leftdown_Left,leftup_Left,rightdown_Left,rightup_Left,
                // 背面
                leftdown_Back,leftup_Back,rightdown_Back,rightup_Back,
                // 上面
                leftdown_Up,leftup_Up,rightdown_Up,rightup_Up,
                // 下面
                leftdown_Down,leftup_Down,rightdown_Down,rightup_Down,
            };

            triangles = new int[] {
                // 正面
                0,1,2,
                1,3,2,
                // 右侧面
                4,5,6,
                5,7,6,
                // 左侧面
                8,9,10,
                9,11,10,
                // 背面
                12,13,14,
                13,15,14,
                // 上面
                16,17,18,
                17,19,18,
                // 下面
                20,21,22,
                21,23,22
            };

            normals = new Vector3[] {
                // 正面
                forwardV,forwardV,forwardV,forwardV,
                // 右侧面
                rightV,rightV,rightV,rightV,
                // 左侧面
                leftV,leftV,leftV,leftV,
                // 背面
                backV,backV,backV,backV,
                // 上面
                upV,upV,upV,upV,
                // 下面
                downV,downV,downV,downV,
            };

            CalculateVerticsTangent(vertices,triangles);

            foreach (int i in triangles) {
                vertices[i].normal = normals[i];
            }
        }
    }
}
