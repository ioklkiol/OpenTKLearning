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

namespace OpenTKTutorial03
{
    class Game:GameWindow
    {
        private string _VertexShader = new StreamReader("Shader/vs.glsl").ReadToEnd();
        private string _FragmentShader = new StreamReader("Shader/fs.glsl").ReadToEnd();

        private int _PID;
        private int _VsID;
        private int _FsID;

        private int _VAO;
        private int _VBO;
        private int _EBO;

        private Vector3[] _VertData;
        private int[] _IndiceData;
        public Game() : base(600, 600, GraphicsMode.Default, "VBO,VAO,EBO Test",GameWindowFlags.Default, DisplayDevice.Default, 4, 0,
            GraphicsContextFlags.ForwardCompatible)
        { }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _PID = GL.CreateProgram();

            _VsID = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(_VsID, _VertexShader);
            GL.CompileShader(_VsID);

            _FsID = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(_FsID, _FragmentShader);
            GL.CompileShader(_FsID);

            GL.AttachShader(_PID, _VsID);
            GL.AttachShader(_PID, _FsID);

            GL.LinkProgram(_PID);

            GL.DeleteShader(_VsID);
            GL.DeleteShader(_FsID);
  

            //创建顶点数据
            _VertData = new[]
            {
                new Vector3(-0.5f,-0.5f,0f),
                new Vector3(0.5f,-0.5f,0f),
                new Vector3(0.5f,0.5f,0f),
                new Vector3(-0.5f,0.5f,0f)
            };

            _IndiceData = new[]
            {
                0,1,3,
                1,2,3
            };

            _VAO = GL.GenVertexArray();
            GL.BindVertexArray(_VAO);//绑定VAO，接下来与VBO相关的操作都与VAO有关

            _VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
            //绑定数据到GPU
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_VertData.Length
                * Vector3.SizeInBytes), _VertData, BufferUsageHint.StaticDraw);
            //告诉OpenGL如何解析数据，第一个参数对应vs.glsl中的(location = 0)
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            _EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EBO);
            GL.BufferData<int>(BufferTarget.ElementArrayBuffer, (_IndiceData.Length
                * sizeof(int)), _IndiceData, BufferUsageHint.StaticDraw);

            //启用属性(location = 0)
            GL.EnableVertexAttribArray(0);
            //解绑VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            //解绑VAO
            GL.BindVertexArray(0);
            //解绑EBO
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            GL.ClearColor(Color.CornflowerBlue);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Viewport(0, 0, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);

            GL.BindVertexArray(_VAO);
            GL.UseProgram(_PID);

            //GL.DrawArrays(PrimitiveType.TriangleFan, 0, 4);
            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);

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
