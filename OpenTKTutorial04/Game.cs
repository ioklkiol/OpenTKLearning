using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

//Shader
namespace OpenTKTutorial04
{
    class Game:GameWindow
    {
        private Shader _Shader;

        private int _VAO;
        private int _VBO;

        private Vector3[] _VertData;
        public Game() : base(600, 600, GraphicsMode.Default, "Shader Test",GameWindowFlags.Default, DisplayDevice.Default, 4, 0,
            GraphicsContextFlags.ForwardCompatible)
        { }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _Shader = new Shader("Shader/vs.glsl", "Shader/fs.glsl");

            //创建顶点数据
            _VertData = new[]
            {
                //位置
                new Vector3(0.5f,-0.5f,0f),
                new Vector3(0.0f,0.5f,0.0f),
                new Vector3(-0.5f,-0.5f,0.0f),

                //颜色
                new Vector3(1.0f,0.0f,0.0f),
                new Vector3(0.0f,1.0f,0.0f),
                new Vector3(0.0f,0.0f,1.0f)
            };

            /*两种方式都可以
            _VertData = new[]
            {
                //位置，颜色 混合
                new Vector3(-0.5f,-0.5f,0f),
                new Vector3(1.0f,0.0f,0.0f),

                new Vector3(0.0f,0.5f,0.0f),
                new Vector3(0.0f,1.0f,0.0f),

                new Vector3(0.5f,-0.5f,0.0f),
                new Vector3(0.0f,0.0f,1.0f)           
            };

            */

            _VAO = GL.GenVertexArray();
            GL.BindVertexArray(_VAO);//绑定VAO，接下来与VBO相关的操作都与VAO有关

            _VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
            //绑定数据到GPU
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_VertData.Length
                * Vector3.SizeInBytes), _VertData, BufferUsageHint.StaticDraw);
            //告诉OpenGL如何解析数据，第一个参数对应vs.glsl中的(location = 0)
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 9*sizeof(float));

            //第二种方式
            //GL.VertexAttribPointer(0,3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            //GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));

            //启用属性(location = 0)
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            //解绑VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            //解绑VAO
            GL.BindVertexArray(0);

            GL.ClearColor(Color.CornflowerBlue);

            //GL.PolygonMode(MaterialFace.FrontAndBack,PolygonMode.Line);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Viewport(0, 0, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);

            GL.BindVertexArray(_VAO);

            //指定Shader程序
            _Shader.Use();

            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);          
            //GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);

            GL.Flush();
            SwapBuffers();
           
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            ProcessInput();
         
        }

        private void ProcessInput()
        {
            if (Keyboard.GetState().IsKeyDown(Key.Escape))
                Exit();
        }
    }
}
