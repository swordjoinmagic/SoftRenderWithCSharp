using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using SortRenderWithCSharp;

public class SoftRenderForm : Form {
    // 帧缓冲，后备缓冲区，用于双缓冲时将该后备缓冲区交换到前置缓冲区
    Bitmap screenBuffer;
    // 帧缓冲的绘图画面，用来清除帧缓冲
    Graphics screenBufferGraphics;

    // 用于贴在图元上的2D贴图
    Bitmap texture2D;

    // 法线贴图
    Bitmap normalTexture;

    // 前置缓冲区（也就是当前显示的部分）
    Graphics g;

    // 深度缓冲区
    float[,] ZBuffer;
    // 是否开启深度写入(默认为否)
    bool IsZWritting;
    // 是否开启深度测试(默认为否)
    bool IsZTest;

    // 是否开启计算颜色的函数
    bool fragmentShaderOn;

    // 是否开启光照渲染（同时也决定了是否对顶点的法线进行计算）
    bool LightingOn;
    // 平行光光源的方向，这里将默认方向设为从摄像机观察方向指向摄像机
    Vector3 DirectionLight;
    Color01 lightColor = Color01.White; // 平行光颜色

    // 屏幕宽高度（同时也作分辨率使用）
    private int screenHeight = 500, screenWidth = 500;

    // 摄像机类
    private Camera camera;

    private long lastUpdateTime;

    /// <summary>
    /// 初始化
    /// </summary>
    public SoftRenderForm() {

        Height = screenHeight;
        Width = screenWidth;

        // 初始化前置缓冲区
        g = this.CreateGraphics();

        // 初始化屏幕缓冲区
        screenBuffer = new Bitmap(screenWidth, screenHeight);
        screenBufferGraphics = Graphics.FromImage(screenBuffer);

        // 初始化深度缓冲区
        ZBuffer = new float[screenWidth+1,screenHeight+1];


        // 开启深度测试
        IsZTest = true;
        // 开启深度写入
        IsZWritting = true;

        // 开启光照渲染
        LightingOn = true;

        // 开启逐像素计算
        fragmentShaderOn = true;

        // 初始化摄像机
        camera = new Camera();

        // 读取贴图
        //texture2D = new Bitmap("D:\\C#Instance\\LerningRenderPiplineWithSoftRender\\SortRenderWithCSharp\\SortRenderWithCSharp\readmeImage\\29126173.bmp");
        texture2D = new Bitmap("C:\\Users\\Administrator\\Desktop\\29126173.bmp");
        //texture2D = new Bitmap("D:\\UnityInstance\\资源\\Unity Shadeers Book Assert\\Unity_Shaders_Book-master\\Assets\\Textures\\Chapter7\\Grid.png");

        // 读取法线贴图
        normalTexture = new Bitmap(256, 256);
        //normalTexture = new Bitmap("D:\\UnityInstance\\Shader Collection\\Assets\\Textures\\Chapter7\\Brick_Normal.bmp");
        //normalTexture = new Bitmap("D:\\C#Instance\\LerningRenderPiplineWithSoftRender\\SortRenderWithCSharp\\SortRenderWithCSharp\readmeImage\\distorition1.bmp");

        // 读取OBJ文件
        //OBJLoader.LoadOBJ("../../cubea.obj");
        SpaceShip = OBJLoader.LoadOBJ("../../enemry_spaceship.obj");

        StartRender();
    }

    protected override void OnActivated(EventArgs e) {
        base.OnActivated(e);
    }

    protected override void OnKeyDown(KeyEventArgs e) {
        base.OnKeyDown(e);
        switch (e.KeyCode) {
            case Keys.Right:
                cameraPositionX += 0.01f;
                break;
            case Keys.Left:
                cameraPositionX -= 0.01f;
                break;
            case Keys.Up:
                cameraPositionY -= 0.01f;
                break;
            case Keys.Down:
                cameraPositionY += 0.01f;
                break;
            case Keys.Z:
                cameraPositionZ += 0.01f;
                break;
            case Keys.X:
                cameraPositionZ -= 0.01f;
                break;
        }
    }

    protected override void OnMouseWheel(MouseEventArgs e) {
        base.OnMouseWheel(e);

    }

    /// <summary>
    /// 清除后备缓冲区
    /// </summary>
    private void ClearScreenBuffer() {

        // 清除颜色缓冲区
        screenBufferGraphics.Clear(System.Drawing.Color.Black);

        // 清除深度缓冲区
        Array.Clear(ZBuffer,0,ZBuffer.Length);
    }

    /// <summary>
    /// 在后备缓冲区中,对点(x,y)画上一个color的颜色
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="color"></param>
    private void DrawPixel(int x, int y, Color01 color) {
        screenBuffer.SetPixel(x, y, color.ToColor());
    }
    private void DrawPixel(int x, int y, Color color) {
        screenBuffer.SetPixel(x, y, color);
    }

    /// <summary>
    /// 显示帧率
    /// </summary>
    public void ShowFPS() {
        Console.Clear();

        // dateTime.now.ticks/10000 = 
        // 从0001 /01/01/00：00开始到现在的毫秒数

        long nowMs = DateTime.Now.Ticks / 10000;

        // 两次窗口更新之间经历的毫秒数
        long deltaTime = nowMs - lastUpdateTime;

        Console.WriteLine("delta"+deltaTime);

        // 更新lastUpdateTime变量（即最后一次更新时间）
        lastUpdateTime = nowMs;

        // deltaTime表示ms转s，帧数表示1秒中内能更新几次
        float fps = (float)1 / ((float)deltaTime / 1000);

        Console.WriteLine(String.Format("帧率: {0:G4} FPS", fps));
        if (screenBufferGraphics != null)
            screenBufferGraphics.DrawString(String.Format("帧率: {0:G4} FPS", fps), new Font("Verdana", 12), new SolidBrush(Color.Red), new PointF(1.0f, 1.0f));
    }

    /// <summary>
    /// 开始渲染
    /// </summary>
    private void StartRender() {
        // 1000ms / 60表示 共60帧，每帧渲染一次
        System.Timers.Timer timer = new System.Timers.Timer(0.0001f);
        timer.Elapsed += new System.Timers.ElapsedEventHandler(Render);
        timer.AutoReset = true;
        timer.Enabled = true;
        timer.Start();

    }

    int angel = 0;
    float cameraPositionX = 0;
    float cameraPositionY = 0;
    float cameraPositionZ = -10;
    // 每一帧渲染图形的方法
    public void Render(object sender, System.Timers.ElapsedEventArgs args) {

        lock (screenBuffer) {
            // 清空后置缓冲区
            ClearScreenBuffer();
            // 显示FPS
            ShowFPS();

            #region 绘制区域

            //DrawCube();
            DrawSpaceShip();


            #endregion


            // 交换缓冲，将后备缓冲交换到前置缓冲
            g.DrawImage(screenBuffer, 0, 0);
        }
    }

    #region 光栅化线段与三角图元

    /// <summary>
    /// 对应任意一种情况的光栅化线段的方法,
    /// 是Bresenham朴素算法的优化版,不出现浮点运算
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <param name="color"></param>
    private void DrawLine(Vertex v1,Vertex v2) {

        int x1 = (int)v1.pos.X;
        int y1 = (int)v1.pos.Y;
        int x2 = (int)v2.pos.X;
        int y2 = (int)v2.pos.Y;

        // 线段起点与终点在x轴上的距离(可能为负)
        int dx = x2 - x1;
        // 线段起点和终点在y轴上的距离(可能为负)
        int dy = y2 - y1;

        // 计算从x1到x2的方向是正方向还是负方向
        // 1<<1 = 2; 0<<1 = 0
        int stepX = ((dx > 0 ? 1 : 0) << 1) - 1;
        // 计算从y1到y2的方向是正方向还是负方向
        int stepY = ((dy > 0 ? 1 : 0) << 1) - 1;

        dx = Math.Abs(dx); dy = Math.Abs(dy);
    
        // 误差
        int eps = 0;

        if (dx > dy) {
            int y = y1;
            // 当x轴距离差更大时,将x作为自增变量
            for (int x = x1; x != x2; x += stepX) {

                float t = (float)(x - x1) / (float)(x2 - x1);      
                
                // 当前顶点
                Vertex vertex = Vertex.LerpVertexData(v1, v2, t);
                vertex.pos.X = x;
                vertex.pos.Y = y;

                float z = vertex.pos.Z;

                // 对当前像素进行深度测试
                if (IsZTest)
                    if (!ZTest(x, y, z)) continue;

                // 透视插值矫正
                float realZ = 1.0f / z;
                vertex.u *= realZ;      // 变回原来的u
                vertex.v *= realZ;      // 变回原来的v

                float u = vertex.u;
                float v = vertex.v;

                // 对顶点颜色进行插值
                Color01 color = Color01.LerpColor(v1.color, v2.color, t);

                // 对纹理贴图进行采样
                Color01 textureColor = Texture.Tex2D(texture2D, u, v);

                Color01 finalColor = textureColor*color;

                if (fragmentShaderOn)
                    DrawPixel(x, y, FragmentShader(vertex));
                else
                    DrawPixel(x, y, finalColor);

                // 增量误差
                eps += dy;

                // 误差大于0.5,那么y++
                if ((eps << 1) >= dx) {
                    y += stepY;
                    eps -= dx;
                }

            }
        } else {
            int x = x1;
            // 当y轴距离差更大时,将y作为自增变量
            for (int y = y1; y != y2; y += stepY) {

                float t = (float)(y - y1) / (float)(y2 - y1);

                Color01 color = Color01.LerpColor(v1.color,v2.color,t);
                DrawPixel(x, y, color);

                // 增量误差
                eps += dx;

                if ((eps << 1) >= dy) {
                    x += stepX;
                    eps -= dy;
                }
            }
        }
    }

    /// <summary>
    /// 光栅化平顶三角形(基于扫描线法)
    /// 
    /// 假设平顶三角形三个顶点A(x1,y1),B(x2,y2),C(x3,y3),
    /// 那么其中AB是平顶,分别在AC/BC上取xLeft,xRight两个点,
    /// 自增y,向下逐步画扫描线,即可光栅化一个平顶三角形
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <param name="x3"></param>
    /// <param name="y3"></param>
    /// <param name="color"></param>
    private void DrawTopFlatTriangle(Vertex v1,Vertex v2,Vertex v3) {

        float x1 = v1.pos.X;
        float y1 = v1.pos.Y;
        float x2 = v2.pos.X;
        float y2 = v2.pos.Y;
        float x3 = v3.pos.X;
        float y3 = v3.pos.Y;
        

        // 令y自增,并在平顶三角形的左右两条边上取两个点
        // 画线段
        for (float y = y1; y <= y3; y++) {

            //if (y3 - y1 == 0 || y3 - y2 == 0) continue;

            // 使用直线方程(两点式)获得左端点和右端点
            // 需要注意此方法不适用于垂直x轴和y轴的直线            

            // 左端点
            float xLeft = (y - y1) * (x3 - x1) / (y3 - y1) + x1;

            // 根据y进行插值
            float t = (y - y1) / (y3 - y1);

            Vertex vLeft = Vertex.LerpVertexData(v1,v3,t);
            vLeft.pos.X = xLeft;
            vLeft.pos.Y = y;

            // 右端点
            float xRight = (y - y2) * (x3 - x2) / (y3 - y2) + x2;

            Vertex vRight = Vertex.LerpVertexData(v2,v3,t);
            vRight.pos.X = xRight;
            vRight.pos.Y = y;

            DrawLine(vLeft,vRight);
        }
    }

    /// <summary>
    /// 绘制平底三角形
    /// 
    /// 设有平底三角形ABC,则A(x1,y1),B(x2,y2),C(x3,y3),
    /// 则BC是底,要在AB,AC上找两个端点对三角形进行绘制
    /// </summary>
    private void DrawBottomFlatTriangle(Vertex v1,Vertex v2,Vertex v3) {

        float x1 = v1.pos.X ;
        float y1 = v1.pos.Y;
        float x2 = v2.pos.X;
        float y2 = v2.pos.Y;
        float x3 = v3.pos.X;
        float y3 = v3.pos.Y;

        // 从上到下扫描
        for (float y = y1; y <= y2; y++) {
            // 处理除0错误
            if (y2 - y1 == 0 || y3 - y1 == 0) continue;

            // 利用y进行插值
            float t = (y - y1) / (y2 - y1); // 插值系数

            // 左端点
            float xLeft = (y - y1) * (x2 - x1) / (y2 - y1) + x1;

            Vertex vLeft = Vertex.LerpVertexData(v1,v2,t);
            vLeft.pos.X = xLeft;
            vLeft.pos.Y = y;

            // 右端点
            float xRight = (y - y1) * (x3 - x1) / (y3 - y1) + x1;

            Vertex vRight = Vertex.LerpVertexData(v1,v3,t);
            vRight.pos.X = xRight;
            vRight.pos.Y = y;

            DrawLine(vLeft,vRight);
        }

    }


    /// <summary>
    /// 绘制普通三角形
    /// 
    /// 思路是:
    /// 将三个顶点中,
    /// 从y值排在中间的顶点开始,画一条平行于x轴的平行线,
    /// 这样,就把一个普通三角形分为一个平底三角形,一个平顶三角形了
    /// 
    /// 假设普通三角形ABC,有顶点A(x1,y1),B(x2,y2),C(x3,y3),
    /// 这里假设顶点B的y值排在AC中间即 y1 小于 y2 小于 y3  
    /// 
    /// 那么在直线AC上找到一点y值与B相等的点D,画直线BD,就将普通三角形分割成平底和平顶三角形了
    /// 分割后,ABD是平底三角形,BDC是平顶三角形
    /// 
    /// </summary>
    private void DrawTriangle(Vertex v1,Vertex v2,Vertex v3) {

        // 对三个顶点的y值升序排序,
        if (v3.pos.Y < v1.pos.Y) {
            // 交换顶点1/3
            Vertex temp = v1; v1 = v3; v3 = temp;
        }
        if (v2.pos.Y < v1.pos.Y) {
            // 交换顶点1/2
            Vertex temp = v1; v1 = v2; v2 = temp;
        }

        // 经过上面的操作,现在(x1,y1)的y值是最小的,现在将(x3,y3)变成最大的
        if (v2.pos.Y > v3.pos.Y) {
            // 交换顶点2/3
            Vertex temp = v2; v2 = v3; v3 = temp;
        }

        // 现在三个顶点的顺序按照y值升序排列分别是A(x1,y1)<B(x2,y2)<C(x3,y3)


        if (v1.pos.Y == v2.pos.Y) {
            // 平顶三角形
            DrawTopFlatTriangle(v1,v2,v3);
        } else if (v2.pos.Y == v3.pos.Y) {
            // 平底三角形
            DrawBottomFlatTriangle(v1, v2, v3);
        } else {
            // 普通三角形

            // 在AC上找到一点D(使用两点式),其y值与B点相同
            float Dy = v2.pos.Y;
            float Dx = (Dy - v1.pos.Y) * (v3.pos.X - v1.pos.X) / (v3.pos.Y - v1.pos.Y) + v1.pos.X;
            // 根据DY，在AC上对点D进行插值
            float t = (Dy - v1.pos.Y) / (v3.pos.Y - v1.pos.Y);    // 插值系数
            Vertex vD = Vertex.LerpVertexData(v1,v3,t);
            vD.pos.X = Dx;
            vD.pos.Y = Dy;

            // 将B点和D点按照x值升序排列(保证B点在D点左边)
            if (Dx < v2.pos.X) {  // 如果D点在B点左边，那么交换他们，使B点成为D点，D点成为B点
                Vertex temp = vD; vD = v2; v2 = temp;
            }

            // 绘制平底三角形ABD
            DrawBottomFlatTriangle(v1,v2,vD);
            // 绘制平顶三角形BDC
            DrawTopFlatTriangle(v2, vD, v3);
        }
    }
    #endregion


    #region MVP变换

    /// <summary>
    /// 在顶点进行屏幕映射之前,
    /// 判断该顶点在不在摄像机的视椎体内,
    /// 这个操作即为CVV剔除or裁剪,这里只考虑剔除操作  
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool CVVCutting(Vector3 pos) {
        if (pos.X > pos.W || pos.X < -pos.W)
            return true;

        if (pos.Y > pos.W || pos.Y < -pos.W)
            return true;

        if (pos.Z > pos.W || pos.Z < -pos.W)
            return true;

        return false;
    }

    /// <summary>
    /// 将一个处于裁剪空间内的顶点(还未进行齐次除法),
    /// 变换到NDC坐标空间下,然后再将该顶点映射到屏幕空间上    
    /// </summary>
    /// <param name="vertex"></param>
    public Vertex ScreenMapping(Vertex v) {

        Vertex vertex = v.DeepyCopy();

        // 进行齐次除法
        vertex.pos.X /= vertex.pos.W;
        vertex.pos.Y /= vertex.pos.W;

        // 进行齐次除法后,此时顶点坐标被转换到NDC坐标下
        // 这里的NDC坐标采用OpenGL风格,
        // 即其中各分量范围如: 1<= x,y,z <= 1

        // 现要将x,y分量映射到屏幕上,需要将这两个分量
        // 缩放到[0,1]区间,并乘于各自屏幕分辨率
        // 这里可以进行经典的 *0.5+0.5的操作

        // 将x坐标映射到屏幕上
        vertex.pos.X = (vertex.pos.X * 0.5f + 0.5f) * screenWidth;
        // 将y坐标映射到屏幕上
        vertex.pos.Y = (vertex.pos.Y * 0.5f + 0.5f) * screenHeight;        

        // 透视插值矫正,这里将顶点的uv值乘于1/z,
        // 这样在光栅化的时候插值就是均匀的
        float reciprocalW = 1.0f / vertex.pos.W;
        vertex.pos.Z = reciprocalW;
        vertex.u *= vertex.pos.Z;
        vertex.v *= vertex.pos.Z;

        return vertex;
    }


    /// <summary>
    /// 在屏幕上绘制一个三角形图元
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <param name="v3"></param>
    public void DrawPrimitive(Vertex v1,Vertex v2,Vertex v3,Matrix4x4 mvp) {
        // 将各顶点作为列矩阵进行MVP变换
        v1.pos = mvp * v1.modelSpacePos;
        v2.pos = mvp * v2.modelSpacePos;
        v3.pos = mvp * v3.modelSpacePos;

        // 判断有顶点是否应该被裁剪
        if (CVVCutting(v1.pos) ||
            CVVCutting(v2.pos) ||
            CVVCutting(v3.pos)) return;

        // 对顶点进行屏幕映射操作
        Vertex vertex1 = ScreenMapping(v1);
        Vertex vertex2 = ScreenMapping(v2);
        Vertex vertex3 = ScreenMapping(v3);

        // 使用这个三个顶点的屏幕坐标绘制三角形图元
        DrawTriangle(vertex1, vertex2, vertex3);
    }

    /// <summary>
    /// 	根据传递的Vertex数组和triangleIndex数组来绘制3D图形
    /// 		triangleIndex表示顶点的顺序，triangleIndex[1] = 1，
    /// 		表示第二个顶点是Vertex数组的第二个元素
    /// 		绘制的规则是，
    /// 		每三个成一个三角形进行绘制
    /// </summary>
    /// <param name="vertices"></param>
    /// <param name="triangle"></param>
    /// <param name="mvp"></param>
    public void DrawElement(Vertex[] vertices,int[] triangle,Matrix4x4 mvp) {
        if (triangle.Length % 3 != 0) return;
        for (int i=0;i<triangle.Length;i+=3) {
            Vertex v1 = vertices[triangle[i]];
            Vertex v2 = vertices[triangle[i+1]];
            Vertex v3 = vertices[triangle[i + 2]];

            DrawPrimitive(v1,v2,v3,mvp);
        }
    }

    public void DrawElement(Mesh mesh,Matrix4x4 mvp) {
        int[] triangle = mesh.triangles;
        Vertex[] vertices = mesh.vertices;
        if (triangle.Length % 3 != 0) return;
        for (int i = 0; i < triangle.Length; i += 3) {
            Vertex v1 = vertices[triangle[i]];
            Vertex v2 = vertices[triangle[i + 1]];
            Vertex v3 = vertices[triangle[i + 2]];

            DrawPrimitive(v1, v2, v3, mvp);
        }
    }
    #endregion

    #region 深度写入/深度测试

    /// <summary>
    /// 对一个像素点开启深度测试,如果通过深度测试,那么将该物体的深度写入深度缓冲区
    /// 
    /// 深度测试的方法如下:
    /// 
    ///     将该片元与深度缓冲区中当前片元的深度进行比较,
    ///     如果要绘制的像素深度小于深度缓冲区同样位置的深度,那么绘制该像素,并将新的深度写入深度缓冲区
    ///     如果要绘制的像素深度大于深度缓冲区同样位置的深度,那么不绘制该像素
    ///     
    ///     因为在屏幕映射时,将Z改为1/Z,所以上面的大小关系要进行翻转
    /// </summary>
    /// <param name="px"></param>
    /// <param name="py"></param>
    /// <param name="pz">当前要绘制的像素的深度</param>
    /// <returns></returns>
    public bool ZTest(int px,int py,float pz) {
        // 如果要绘制的片元的深度小于深度缓冲区中的片元,就进行绘制,并进行深度写入
        // PS:深度缓冲区中当前的深度是1/Z(这个Z是视角空间下的Z),所以大小关系反转
        if (pz >= ZBuffer[px,py]) {
            // 进行深度写入
            if (IsZWritting)
                ZBuffer[px, py] = pz;
            return true;
        }
        return false;
    }

    #endregion

    #region 绘制更多图元

    int lightAngle;
    public Vector3 cameraPostion;


    Mesh SpaceShip;
    public void DrawSpaceShip() {
        Vector3 rotation = Vector3.Zero;
        Vector3 scale = Vector3.One;
        Vector3 worldPosition = Vector3.Zero;
        // 构建M矩阵
        Matrix4x4 modelMatrix = Matrix.GetModelMatrix(worldPosition, rotation, scale);
        // 构建V矩阵
        Matrix4x4 viewMatrix = camera.GetViewMatrix();
        // 构建P矩阵
        Matrix4x4 projectionMatrix = camera.GetProjectionMatrix();

        // 构建MVP矩阵
        Matrix4x4 MVPMatrix = projectionMatrix * viewMatrix * modelMatrix;

        // 给每个顶点引用一份MVP矩阵
        foreach (int i in SpaceShip.triangles) {
            Vertex v = SpaceShip.vertices[i];
            v.mMatrix = modelMatrix;
            v.vMatrix = viewMatrix;
            v.pMatrix = projectionMatrix;
        }

        DrawElement(SpaceShip, MVPMatrix);
    }

    /// <summary>
    /// 绘制立方体
    /// </summary>
    public void DrawCube() {

        // 坐标/旋转与缩放
        angel = (angel + 1) % 720;
        Vector3 rotation = new Vector3(angel, angel, angel);
        Vector3 scale = new Vector3(1, 1, 1);
        Vector3 worldPosition = new Vector3(0, 0, 0);

        // 构建M矩阵
        Matrix4x4 modelMatrix = Matrix.GetModelMatrix(worldPosition, rotation, scale);
        // 构建V矩阵
        Matrix4x4 viewMatrix = camera.GetViewMatrix();
        // 构建P矩阵
        Matrix4x4 projectionMatrix = camera.GetProjectionMatrix();

        // 构建MVP矩阵
        Matrix4x4 MVPMatrix = projectionMatrix * viewMatrix * modelMatrix;

        Cube cube = new Cube();
        // 给每个顶点引用一份MVP矩阵
        foreach (int i in cube.triangles) {
            Vertex v = cube.vertices[i];
            v.mMatrix = modelMatrix;
            v.vMatrix = viewMatrix;
            v.pMatrix = projectionMatrix;
        }

        DrawElement(cube, MVPMatrix);
    }

    #region 光照模型

    /// <summary>
    /// 基于半兰伯特光照模型的光照
    /// </summary>
    /// <param name="vertex"></param>
    /// <returns></returns>
    public Color01 LightingWithLambert(Vertex vertex) {
        // 将顶点的法线变换到世界空间下，对于一个只包含旋转变换的变换矩阵，他是正交矩阵
        // 使用m矩阵变换法线(仅适用于只发生旋转的物体)
        Vector3 worldNormal = vertex.mMatrix * vertex.normal;
        worldNormal.Normlize();

        // 根据平行光方向及当前法线方向，
        // 计算当前像素的辐照度

        // 使用半兰伯特表达式计算辐照度
        float radiance = Vector3.Dot(worldNormal, DirectionLight) * 0.5f + 0.5f;
        // 获得贴图颜色
        Color01 albedo = Texture.Tex2D(texture2D, vertex.u, vertex.v);

        // 使用Blinn-Phong模型计算高光反射

        // 获得顶点当前所在世界坐标
        Vector3 worldPos = vertex.mMatrix * vertex.modelSpacePos;

        // 获得世界坐标下的视角方向
        Vector3 worldViewDir = cameraPostion - worldPos;

        // 计算half向量
        Vector3 h = (worldViewDir + DirectionLight);
        h.Normlize();

        // 光泽度
        float gloss = 20f;

        // 计算高光反射
        Color01 specular = lightColor * (float)Math.Pow(Math.Max(0, Vector3.Dot(h, worldNormal)), gloss);
        // 计算漫反射光照
        Color01 diffuse = albedo * radiance * lightColor;

        Color01 finalColor = diffuse;
        finalColor.A = 1;

        return finalColor;
    }

    /// <summary>
    /// 法线映射
    /// </summary>
    /// <param name="vertex"></param>
    /// <returns></returns>
    public Color01 NormalMappingWithLambert(Vertex vertex) {
        // 从法线贴图中取出法线(切线空间下的)
        Color01 UndecryptedNormal = Texture.Tex2D(normalTexture, vertex.u, vertex.v);
        // 把法线从[0,1]区间变回到[-1,1]区间
        Vector3 normal = new Vector3(
            UndecryptedNormal.R * 2 - 1,
            UndecryptedNormal.G * 2 - 1,
            UndecryptedNormal.B * 2 - 1
            );

        // 获得世界坐标下的法线
        Vector3 worldNormal = vertex.mMatrix * vertex.normal;
        worldNormal.Normlize();
        // 获得世界坐标下的切线
        Vector3 worldTangent = vertex.mMatrix * vertex.tangent;
        worldTangent.Normlize();
        // 获得世界坐标下的副切线
        Vector3 worldBinormal = Vector3.Cross(worldNormal, worldTangent) * vertex.tangent.W;
        worldBinormal.Normlize();

        // 构建 切线-世界 变换矩阵
        Matrix4x4 TtoW = new Matrix4x4();
        TtoW.Identity();

        TtoW.value[0, 0] = worldTangent.X;
        TtoW.value[1, 0] = worldTangent.Y;
        TtoW.value[2, 0] = worldTangent.Z;

        TtoW.value[0, 1] = worldBinormal.X;
        TtoW.value[1, 1] = worldBinormal.Y;
        TtoW.value[2, 1] = worldBinormal.Z;

        TtoW.value[0, 2] = worldNormal.X;
        TtoW.value[1, 2] = worldNormal.Y;
        TtoW.value[2, 2] = worldNormal.Z;

        // 将切线空间的法线变为世界坐标
        normal = TtoW * normal;
        normal.Normlize();

        // 增大凹凸比例
        normal.X *= 3;
        normal.Y *= 3;
        normal.Z *= MathF.Sqrt(1 - MathF.Clamp01(normal.X * normal.X + normal.Y * normal.Y));

        Color01 albedo = Texture.Tex2D(texture2D, vertex.u, vertex.v) * vertex.color;

        float radiance = Vector3.Dot(normal, DirectionLight) * 0.5f + 0.5f;

        Color01 diffuse = albedo * radiance;

        return diffuse;

    }

    /// <summary>
    /// 纹理映射
    /// </summary>
    /// <param name="vertex"></param>
    /// <returns></returns>
    public Color01 TextureMapping(Vertex vertex) {
        return Texture.Tex2D(texture2D,vertex.u,vertex.v);
    }

    /// <summary>
    /// 顶点色映射
    /// </summary>
    /// <returns></returns>
    public Color01 VertexColorMapping(Vertex vertex) {
        return vertex.color;
    }

    #endregion

    /// <summary>
    /// 山寨版片元着色器，
    /// 根据一个顶点的各属性，计算当前顶点（屏幕空间下）的像素的颜色
    /// </summary>
    /// <param name="vertex"></param>
    /// <returns></returns>
    public Color01 FragmentShader(Vertex vertex) {

        // 基于半兰伯特光照模型的光照
        //return LightingWithLambert(vertex);
        // 法线映射
        //return NormalMappingWithLambert(vertex);
        // 纹理映射
        //return TextureMapping(vertex);
        // 顶点色
        //return VertexColorMapping(vertex);

        return Color01.White;
    }


    #endregion
}

