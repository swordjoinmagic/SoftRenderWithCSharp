using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortRenderWithCSharp {
    /// <summary>
    /// 读取一个OBJ，
    /// 并将其的顶点序列放到vertices数组中，
    /// 将其的纹理坐标序列放到uvs数组中，
    /// 将其的法线序列放到normals数组中，
    /// 将其的顶点索引序列放到triangle中，
    /// 最后再根据它的uv索引和法线索引给这个顶点赋值
    /// </summary>
    public class OBJLoader {

        public static Vertex[] vertices;
        public static int[] triangles;

        /// <summary>
        /// 根据OBJ文件的文件名来读取他
        /// </summary>
        /// <param name="fileName"></param>
        public static void LoadOBJ(string fileName) {
            List<Vertex> tVertex = new List<Vertex>();
            List<Vector3> tpos = new List<Vector3>();
            List<Vector3> tNormals = new List<Vector3>();
            List<Vector2> tuvs = new List<Vector2>();
            List<int> ttriangles = new List<int>();

            StreamReader reader = new StreamReader(fileName);
            string line;
            string[] param;
            while ((line=reader.ReadLine())!=null) {

                if (line.Length <= 2) continue;

                string anotherCommand = line.Substring(0, 2);
                // 转为全大写
                anotherCommand = anotherCommand.ToUpper();
                if (anotherCommand == "VT" || anotherCommand == "TU" || anotherCommand == "TV") {
                    // 表示输入的是纹理坐标
                    param = line.Split(' ');
                    float u = float.Parse(param[1]);
                    float v = float.Parse(param[2]);
                    tuvs.Add(new Vector2(u, v));

                    continue;
                } else if (anotherCommand == "VN") {
                    // 表示输入的是法向量
                    param = line.Split(' ');
                    float x = float.Parse(param[1]);
                    float y = float.Parse(param[2]);
                    float z = float.Parse(param[3]);
                    tNormals.Add(new Vector3(x, y, z));

                    continue;
                }

                char command = line[0];

                switch (command) {
                    // 表示这是个顶点
                    // 顶点后面跟着三个浮点数，表示该顶点在模型空间下的坐标
                    case 'v':
                    case 'V':
                        param = line.Split(' ');
                        float x = float.Parse(param[1]);
                        float y = float.Parse(param[2]);
                        float z = float.Parse(param[3]);
                        Vector3 ttpos = (new Vector3(x,y,z));
                        tVertex.Add(new Vertex(ttpos,Color01.White,0,0));
                        break;
                    case 'f':
                    case 'F':
                        // 表示读取的是一个三角面
                        // 这个三角面的格式如下:
                        // f Vertex1/Texture1/Normal1 Vertex2/Texture2/Normal2 Vertex3/Texture3/Normal3

                        
                        param = line.Split(' ');

                        for (int i = 1; i < 4; i++) {
                            string[] ps = param[i].Split('/');

                            // 需要注意的是输入的索引是从1开始的,这里要将其-1

                            // 顶点索引
                            int triangle = int.Parse(ps[0])-1;
                            // uv索引
                            int uvIndex = int.Parse(ps[1])-1;
                            // 法线索引
                            int normalIndex = int.Parse(ps[2])-1;

                            // 获得目标顶点
                            Vertex v = tVertex[triangle];

                            // 设置法线
                            v.normal = tNormals[normalIndex];

                            // 设置uv
                            v.u = tuvs[uvIndex].u;
                            v.v = tuvs[uvIndex].v;

                            // 添加进索引中
                            ttriangles.Add(triangle);

                        }

                        break;
                }

            }


            vertices = tVertex.ToArray();
            triangles = ttriangles.ToArray();
        }
    }
}
