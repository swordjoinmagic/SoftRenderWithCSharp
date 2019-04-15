
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

    // 前置缓冲区（也就是当前显示的部分）
    Graphics g;

    // 屏幕宽高度（同时也作分辨率使用）
    private int screenHeight = 500, screenWidth = 500;

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

        texture2D = new Bitmap("D:\\preview.bmp");

        StartRender();
    }

    protected override void OnActivated(EventArgs e) {
        base.OnActivated(e);
    }

    protected override void OnKeyDown(KeyEventArgs e) {
        base.OnKeyDown(e);
        switch (e.KeyCode) {
            case Keys.Right:
                cameraPositionX -= 0.01f;
                break;
            case Keys.Left:
                cameraPositionX += 0.01f;
                break;
            case Keys.Up:
                cameraPositionY += 0.01f;
                break;
            case Keys.Down:
                cameraPositionY -= 0.01f;
                break;
        }
    }

    /// <summary>
    /// 清除后备缓冲区
    /// </summary>
    private void ClearScreenBuffer() {
        screenBufferGraphics.Clear(System.Drawing.Color.Black);
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
    // 每一帧渲染图形的方法
    public void Render(object sender, System.Timers.ElapsedEventArgs args) {

        lock (screenBuffer) {
            // 清空后置缓冲区
            ClearScreenBuffer();
            // 显示FPS
            ShowFPS();

            #region 绘制区域

            // 顶点1/2/3
            Vector3 v1 = new Vector3(0, 0, 0);
            Vector3 v2 = new Vector3(0, 2 * 0.2f, 0);
            Vector3 v3 = new Vector3(2 * 0.2f, 0, 0);            
            Vector3 v4 = new Vector3(2 * 0.2f, 2 * 0.2f, 0);

            Vertex vertex1 = new Vertex(v1, new Color01(1, 0, 0, 1), 0, 0);
            Vertex vertex2 = new Vertex(v2, new Color01(0, 1, 0, 1), 0, 1);
            Vertex vertex3 = new Vertex(v3, new Color01(0, 0, 1, 1), 1, 0);
            Vertex vertex4 = new Vertex(v4, new Color01(0, 0, 1, 1), 1, 1);
            Vertex vertex22 = new Vertex(v2, new Color01(0, 1, 0, 1), 0, 1);
            Vertex vertex33 = new Vertex(v3, new Color01(0, 0, 1, 1), 1, 0);

            Vertex[] vertices = new Vertex[] { vertex1,vertex2,vertex3,vertex22,vertex4,vertex33 };
            int[] triangle = new int[] {0,1,2,3,4,5};

            // 坐标/旋转与缩放
            angel = (angel + 1) % 720;
            Vector3 rotation = new Vector3(angel, 0, 0);
            Vector3 scale = new Vector3(1, 1, 1);
            Vector3 worldPosition = new Vector3(0, 0, 0);


            // 摄像机各参数
            Vector3 cameraPostion = new Vector3(cameraPositionX, cameraPositionY, -3);        // 摄像机位置
            Vector3 targetPosition = new Vector3(0, 0, 0);        // 摄像机观察位置
            Vector3 cameraUpDir = new Vector3(0, 1, 0);           // 摄像机向上的向量(粗略的)
            int Near = 1;       // 距离近裁剪平面距离
            int Far = 10;       // 距离远裁剪平面距离
            int top = 1;        // 近平面中心距离上边的距离
            int bottom = -1;    // 近平面中心距离下边的距离
            int right = 1;      // 近平面中心距离右边的距离
            int left = -1;      // 近平面距离左边的距离
            int angle = 30;     // 摄像机的FOV角度


            // 构建M矩阵
            Matrix4x4 modelMatrix = GetModelMatrix(worldPosition, rotation, scale);
            // 构建V矩阵
            Matrix4x4 viewMatrix = GetViewMatrix(cameraPostion, targetPosition, cameraUpDir);
            // 构建P矩阵
            Matrix4x4 projectionMatrix = GetProjectionMatrixWithFrustum(angle, Near, Far, right, left, top, bottom);

            // 构建MVP矩阵
            Matrix4x4 MVPMatrix = projectionMatrix * viewMatrix * modelMatrix;

            //DrawPrimitive(vertex1,vertex2,vertex3,MVPMatrix);
            //DrawTriangle(100, 100, 400, 0, 300, 300, Color.White);
            DrawElement(vertices,triangle,MVPMatrix);

            #endregion

            // 交换缓冲，将后备缓冲交换到前置缓冲
            g.DrawImage(screenBuffer, 0, 0);
        }
    }


    #region Test测试区域

    /// <summary>
    /// 基于Bresenham快速直线算法光栅化线段
    /// 
    /// 这里只假设一种情况,即 dx > dy,斜率 m>0 && m<1
    /// 
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    private void DrawLineWithOneKind(
        int x1, int y1,
        int x2, int y2,
        Color color
        ) {

        #region 朴素算法(涉及浮点数)
        //int dx = Math.Abs(x2-x1);
        //int dy = Math.Abs(y2-y1);

        //// 获得y相对于x的增量
        //float delta = (float)dy / dx;

        //// 误差
        //float eps = 0;

        //// x自增,判断是应该在 (x,y)的位置画点还是 (x,y+1)
        //int y = y1;
        //for (int x=x1;x<=x2;x++) {
        //    // 画点
        //    DrawPixel(x,y,color);

        //    eps += delta;

        //    // 误差大于0.5,那么y++
        //    if (eps >= 0.5) {
        //        y++;
        //        eps -= 1;
        //    }
        //}
        #endregion

        #region 优化算法(不涉及浮点数)

        int dx = Math.Abs(x2 - x1);
        int dy = Math.Abs(y2 - y1);

        // 误差
        int eps = 0;

        // x自增,判断是应该在 (x,y)的位置画点还是 (x,y+1)
        int y = y1;
        for (int x = x1; x <= x2; x++) {
            // 画点
            DrawPixel(x, y, color);

            eps += dy;

            // 误差大于0.5,那么y++
            if (eps * 2 >= dx) {
                y++;
                eps -= dx;
            }
        }

        #endregion

    }


    /// <summary>
    /// 对应任意一种情况的光栅化线段的方法,
    /// 是Bresenham朴素算法的优化版,不出现浮点运算
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <param name="color"></param>
    private void DrawLine(
        int x1, int y1,
        int x2, int y2,
        Color color) {

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
                DrawPixel(x, y, color);

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
    private void DrawTopFlatTriangle(
        int x1, int y1,
        int x2, int y2,
        int x3, int y3,
        Color color) {

        // 令y自增,并在平顶三角形的左右两条边上取两个点
        // 画线段
        for (int y = y1; y <= y3; y++) {

            if (y3 - y1 == 0 || y3 - y2 == 0) continue;

            // 使用直线方程(两点式)获得左端点和右端点
            // 需要注意此方法不适用于垂直x轴和y轴的直线

            // 左端点
            int xLeft = (y - y1) * (x3 - x1) / (y3 - y1) + x1;
            // 右端点
            int xRight = (y - y2) * (x3 - x2) / (y3 - y2) + x2;

            DrawLine(xLeft, y, xRight, y, color);
        }
    }

    /// <summary>
    /// 绘制平底三角形
    /// 
    /// 设有平底三角形ABC,则A(x1,y1),B(x2,y2),C(x3,y3),
    /// 则BC是底,要在AB,AC上找两个端点对三角形进行绘制
    /// </summary>
    private void DrawBottomFlatTriangle(
        int x1, int y1,
        int x2, int y2,
        int x3, int y3,
        Color color) {

        // 从上到下扫描
        for (int y = y1; y <= y2; y++) {
            // 处理除0错误
            if (y2 - y1 == 0 || y3 - y1 == 0) continue;
            // 左端点
            int xLeft = (y - y1) * (x2 - x1) / (y2 - y1) + x1;

            // 右端点
            int xRight = (y - y1) * (x3 - x1) / (y3 - y1) + x1;

            DrawLine(xLeft, y, xRight, y, color);
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
    private void DrawTriangle(
        int x1, int y1,
        int x2, int y2,
        int x3, int y3,
        Color color) {

        // 对三个顶点的y值升序排序,
        if (y3 < y1) {
            // 交换顶点1/2
            int temp = y1; y1 = y3; y3 = temp;
            temp = x1; x1 = x3; x3 = temp;
        } else if (y2 < y1) {
            // 交换顶点1/3
            int temp = y1; y1 = y2; y2 = temp;
            temp = x1; x1 = x2; x2 = temp;
        }
        // 经过上面的操作,现在(x1,y1)的y值是最小的,现在将(x3,y3)变成最大的
        if (y2 > y3) {
            // 交换顶点2/3
            int temp = y2; y2 = y3; y3 = temp;
            temp = x2; x2 = x3; x3 = temp;
        }

        // 现在三个顶点的顺序按照y值升序排列分别是A(x1,y1)<B(x2,y2)<C(x3,y3)


        if (y1 == y2) {
            // 平顶三角形
            DrawTopFlatTriangle(x1, y1, x2, y2, x3, y3, color);
        } else if (y2 == y3) {
            // 平底三角形
            DrawBottomFlatTriangle(x1, y1, x2, y2, x3, y3, color);
        } else {
            // 普通三角形

            // 在AC上找到一点D(使用两点式),其y值与B点相同
            int Dy = y2;
            int Dx = (Dy - y1) * (x3 - x1) / (y3 - y1) + x1;

            // 将B点和D点按照x值升序排列(保证B点在D点左边)
            if (Dx < x2) {
                int temp = Dx; Dx = x2; x2 = temp;
            }

            // 绘制平底三角形ABD
            DrawBottomFlatTriangle(x1, y1, x2, y2, Dx, Dy, color);
            // 绘制平顶三角形BDC
            DrawTopFlatTriangle(x2, y2, Dx, Dy, x3, y3, color);
        }
    }

    #endregion


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
                float u = MathF.LerpFloat(v1.u,v2.u,t);
                float v = MathF.LerpFloat(v1.v,v2.v,t);

                // 对顶点颜色进行插值
                Color01 color = Color01.LerpColor(v1.color,v2.color,t);    
                
                // 对纹理贴图进行采样
                Color01 textureColor = Texture.Tex2D(texture2D,u,v);

                DrawPixel(x, y, textureColor);

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
        } else if (v2.pos.Y < v1.pos.Y) {
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
    /// 获得缩放矩阵
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public Matrix4x4 GetScaleMatrix(float x, float y, float z) {
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
    public Matrix4x4 GetTranslateMatrix(float x, float y, float z) {
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
    public Matrix4x4 GetRotateXMatrix(int xAngle) {
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
    public Matrix4x4 GetRotateYMatrix(int yAngle) {
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
    public Matrix4x4 GetRotateZMatrix(int zAngle) {
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
    public Matrix4x4 GetRotateMatrix(int x, int y, int z) {

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
    public Matrix4x4 GetModelMatrix(Vector3 position, Vector3 rotate, Vector3 scale) {

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
    public Matrix4x4 GetViewMatrix(Vector3 position, Vector3 target, Vector3 upDir) {
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

        // 再加上自身的平移矩阵
        viewMatrix = GetTranslateMatrix(position.X, position.Y, position.Z) * viewMatrix;

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
    Matrix4x4 GetProjectionMatrixWithFrustum(
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
    public void ScreenMapping(Vertex vertex) {
        // 进行齐次除法
        vertex.pos.X /= vertex.pos.W;
        vertex.pos.Y /= vertex.pos.W;
        vertex.pos.Z /= vertex.pos.W;

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

        // 此时可以将Z坐标存入深度缓冲中
    }


    /// <summary>
    /// 在屏幕上绘制一个三角形图元
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <param name="v3"></param>
    public void DrawPrimitive(Vertex v1,Vertex v2,Vertex v3,Matrix4x4 mvp) {
        // 将各顶点作为列矩阵进行MVP变换
        v1.pos = mvp * v1.pos;
        v2.pos = mvp * v2.pos;
        v3.pos = mvp * v3.pos;

        // 判断有顶点是否应该被裁剪
        if (CVVCutting(v1.pos) ||
            CVVCutting(v2.pos) ||
            CVVCutting(v3.pos)) return;

        // 对顶点进行屏幕映射操作
        ScreenMapping(v1);
        ScreenMapping(v2);
        ScreenMapping(v3);

        // 使用这个三个顶点的屏幕坐标绘制三角形图元
        DrawTriangle(v1, v2, v3);
        //DrawTriangle(
        //    (int)v1.pos.X, (int)v1.pos.Y,
        //    (int)v2.pos.X, (int)v2.pos.Y,
        //    (int)v3.pos.X, (int)v3.pos.Y,
        //    Color.White
        //    );
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
    #endregion
}

