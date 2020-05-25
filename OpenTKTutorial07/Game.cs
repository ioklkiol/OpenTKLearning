using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using GlmNet;
using System.Windows.Forms;

//Transform
public class Game : GameWindow
{
    private Shader _Shader;

    private int _VBO;
    private int _VAO;
    private int _EBO;
    private int _TexID;
    private int _TexID2;

    private float[] _VertData;

    public Game() : base(600, 600, GraphicsMode.Default, "Transform Test",
        GameWindowFlags.Default, DisplayDevice.Default, 3, 3, GraphicsContextFlags.ForwardCompatible)
    { }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        _Shader = new Shader("Shader/vs.glsl", "Shader/fs.glsl");

        _VertData = new float[]
        {
            // positions             // colors          
                0.5f,  0.5f, 0.0f,   1.0f,1.0f,    // top right
                0.5f, -0.5f, 0.0f,   1.0f, 0.0f,   // bottom right
                -0.5f, -0.5f, 0.0f,  0.0f, 0.0f,   // bottom left
                -0.5f,  0.5f, 0.0f,  0.0f, 1.0f,   // top left      
        };

        int[] indices = new int[]
        {
            0,1,3,      //first triangle
            1,2,3       //second triangle
        };

        _VAO = GL.GenVertexArray();
        GL.BindVertexArray(_VAO);

        _EBO = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EBO);
        GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int)
            * indices.Length, indices, BufferUsageHint.StaticDraw);

        _VBO = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_VertData.Length
            * sizeof(float)), _VertData, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

        GL.EnableVertexAttribArray(0);
        GL.EnableVertexAttribArray(1);

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);

        _TexID = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, _TexID);
        GL.TextureParameter(_TexID, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TextureParameter(_TexID, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        GL.TextureParameter(_TexID, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TextureParameter(_TexID, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        Bitmap image = new Bitmap("Resource/tex.jpg");
        BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
            ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, data.Width, data.Height, 0,
            OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        image.UnlockBits(data);
        image.Dispose();

        _TexID2 = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, _TexID2);
        GL.TextureParameter(_TexID2, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TextureParameter(_TexID2, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        GL.TextureParameter(_TexID2, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TextureParameter(_TexID2, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        image = new Bitmap("Resource/tex2.png");
        image.RotateFlip(RotateFlipType.Rotate180FlipX);
        data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
            ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, data.Width, data.Height, 0,
            OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        image.UnlockBits(data);
        image.Dispose();

        _Shader.Use();
        _Shader.SetInt("texture1", 0);
        _Shader.SetInt("texture2", 1);

        GL.ClearColor(Color.CornflowerBlue);

        vec4 v4 = new vec4(1f, 0f, 0f, 1f);
        //vec4 prev4 = v4;
        mat4 trans = mat4.identity();
        trans = glm.translate(trans, new vec3(1f, 1f, 0f));
        v4 = trans * v4;

        Matrix4 matrix4 = Matrix4.CreateTranslation(1f, 1f, 0f);
        Vector4 vector4 = new Vector4(1f, 0f, 0f, 1f);
        var a = vector4 * matrix4;

        /*
         * 
         * 特别注意！OpenTK的矩阵乘法是从左往右的，和GLM中相反
         * 
         */
        //{
        //    mat4 transform = mat4.identity();
        //    transform = glm.translate(transform, new vec3(0.5f, -0.5f, 0f));    //位移
        //    transform = glm.rotate(transform, 3.14f, new vec3(0f, 0f, 1f));     //旋转
        //    transform = glm.scale(transform, new vec3(0.5f, 0.5f, 0.5f));       //缩放
        //}

        //{
        //    Matrix4 transform = Matrix4.CreateScale(0.5f, 0.5f, 0.5f) *         //缩放
        //                          Matrix4.CreateRotationZ(3.14f) *              //旋转
        //                          Matrix4.CreateTranslation(0.5f, -0.5f, 0f);   //位移
        //}

    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);
        GL.Viewport(0, 0, Width, Height);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.Enable(EnableCap.DepthTest);

        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, _TexID);

        GL.ActiveTexture(TextureUnit.Texture1);
        GL.BindTexture(TextureTarget.Texture2D, _TexID2);

        _Shader.Use();

        //mat4 transform = mat4.identity();
        //transform = glm.translate(transform, new vec3(0.5f, -0.5f, 0f));
        //transform = glm.rotate(transform, (float)Environment.TickCount / 1000, new vec3(0f, 0f, 1f));
        //transform = glm.scale(transform, new vec3(0.5f, 0.5f, 0.5f));
        //_Shader.SetMat4("transform", transform.to_array());

        Matrix4 transform =
            Matrix4.CreateScale(0.5f, 0.5f, 0.5f) *
            Matrix4.CreateRotationZ((float)Environment.TickCount / 1000) *
            Matrix4.CreateTranslation(0.5f, -0.5f, 0f);
        _Shader.SetMat4("transform", ref transform);

        GL.BindVertexArray(_VAO);

        GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);

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