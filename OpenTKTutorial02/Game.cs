using System;
using System.Drawing;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenTKTutorial2
{
    class Game:GameWindow
    {
        //定义程序的ID(它的地址)，我们在代码中不存储程序对象本身，而是存储一个可以引用的地址，程序本身存储在显卡中
        int pgmID;

        int vsID;   //顶点着色器
        int fsID;   //片段着色器

        //在顶点着色器上有多个输入，我们需要告诉它们地址来给出顶点着色器的位置和颜色信息
        int attribute_vcol;
        int attribute_vpos;
        int uniform_mview;

        //建立着色器和程序之后给一些东西来绘制，使用顶点缓冲区对象(VBO)  Vertex Buffer Object
        int vbo_position;  
        int vbo_color;
        int vbo_mview;


        /*缓冲区需要一些数据，位置和颜色都为Vector3类型，模型视图为Matrix4类型。
        将他们存储在一个数组中，这样可以更有效地将数据发送到缓冲区*/
        Vector3[] vertdata;
        Vector3[] coldata;
        Matrix4[] mviewdata;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            InitProgram();

            vertdata = new Vector3[] {
                new Vector3(-0.8f, -0.8f, 0f),
                new Vector3(0.8f,-0.8f,0f),
                new Vector3(0f,0.8f,0f)};

            coldata = new Vector3[] {
                new Vector3(1f, 0f, 0f),
                new Vector3(0f,0f,1f),
                new Vector3(0f,1f,0f)};

            mviewdata = new Matrix4[] {
                Matrix4.Identity
            };

            //修改窗口标题
            Title = "Hello OpenTK!";
            //设置背景颜色为蓝色
            GL.ClearColor(Color.CornflowerBlue);
            GL.PointSize(5f);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Viewport(0, 0, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);

            //将数据、着色器发送到显卡，告诉它我们想要使用的变量
            GL.EnableVertexAttribArray(attribute_vpos);
            GL.EnableVertexAttribArray(attribute_vcol);

            //告诉它图元的类型
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            //清理工作
            GL.DisableVertexAttribArray(attribute_vpos);
            GL.DisableVertexAttribArray(attribute_vcol);

            GL.Flush();

            SwapBuffers();      //交换缓冲区
        }
        //当正确调整窗口大小时，告诉OpenGL如何调整新窗口大小
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
                (float)Math.PI / 4, Width / (float)Height, 1, 64);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            //绑定缓冲区，告诉OpenTK如果我们发送任何数据，我们将使用该缓冲区
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            //发送数据，告诉PoenTK我们发送长度为(vertdata.Length * Vector3.SizeInBytes)
            //的vertdata到缓冲区
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
                (IntPtr)(vertdata.Length * Vector3.SizeInBytes),
                vertdata, BufferUsageHint.StaticDraw);
            //告诉它使用这个缓冲区vPosition变量,这将需要3个float值
            GL.VertexAttribPointer(attribute_vpos, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_color);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
                (IntPtr)(coldata.Length * Vector3.SizeInBytes),
                coldata, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(attribute_vcol, 3, VertexAttribPointerType.Float, true, 0, 0);

            //发送模型矩阵
            GL.UniformMatrix4(uniform_mview, false, ref mviewdata[0]);

            //使用该程序
            GL.UseProgram(pgmID);
            //清除缓冲区绑定
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        }



        void InitProgram()
        {
            //GL.CreateProgram()函数返回一个新程序对象的ID，我们将它储存到pgmID中
            pgmID = GL.CreateProgram();

            LoadShader("vs.glsl", ShaderType.VertexShader, pgmID, out vsID);
            LoadShader("fs.glsl", ShaderType.FragmentShader, pgmID, out fsID);

            //程序被编译以后需要被链接
            GL.LinkProgram(pgmID);
            Console.WriteLine(GL.GetProgramInfoLog(pgmID));

            attribute_vpos = GL.GetAttribLocation(pgmID, "vPosition");
            attribute_vcol = GL.GetAttribLocation(pgmID, "vColor");
            uniform_mview = GL.GetUniformLocation(pgmID, "modelview");

            if (attribute_vpos == -1 || attribute_vcol == -1 || uniform_mview == -1)
            {
                Console.WriteLine("attribute_vpos == -1:" + (attribute_vpos == -1));
                Console.WriteLine("attribute_vcol == -1:" + (attribute_vcol == -1));
                Console.WriteLine("uniform_mview == -1:" + (uniform_mview == -1));
            }

            //创建缓冲区对象
            GL.GenBuffers(1, out vbo_position);
            GL.GenBuffers(1, out vbo_color);
            GL.GenBuffers(1, out vbo_mview);
            

        }

        //写一个加载器来读取我们的着色器并添加到程序中，返回着色器的地址
        void LoadShader(String fineName, ShaderType type, int program, out int address)
        {
            address = GL.CreateShader(type);
            using (StreamReader sr = new StreamReader(fineName))
            {
                GL.ShaderSource(address, sr.ReadToEnd());
            }
            GL.CompileShader(address);
            GL.AttachShader(program, address);
            Console.WriteLine(GL.GetShaderInfoLog(address));
        }
    }
}
