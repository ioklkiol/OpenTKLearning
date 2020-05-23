using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace ConsoleTest
{
    class Game:GameWindow
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //修改窗口标题
            Title = "Hello OpenTK!";
            //设置背景颜色为蓝色
            GL.ClearColor(Color.CornflowerBlue);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //准备一个平面，我们可以在上面画三角形
            Matrix4 modelView = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelView);

            //绘制三角形
            GL.Begin(PrimitiveType.Triangles);
            GL.Vertex3(-1f, -1f, 4f);
            GL.Vertex3(1f, -1f, 4f);
            GL.Vertex3(0f, 1f, 4f);

            //为三角形添加色彩
            GL.Color3(1f, 0f, 0f);
            GL.Vertex3(-1f, -1f, 4f);
            GL.Color3(0f, 1f, 0f);
            GL.Vertex3(1f, -1f, 4f);
            GL.Color3(0f, 0f, 1f);
            GL.Vertex3(0f, 1f, 4f);

            GL.End();           //告诉OpenGL绘制已经结束
            SwapBuffers();      //交换缓冲区
        }
        //当正确调整窗口大小时，告诉OpenGL如何调整新窗口大小
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
                (float)Math.PI/4,Width/(float)Height,1,64);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }
    }
}
