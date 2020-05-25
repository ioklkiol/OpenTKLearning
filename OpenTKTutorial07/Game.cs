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

    private int _TexID;
    private int _TexID2;

    private int attribute_texCoord;
    private int attribute_vpos;
    private float[] coord;

    private Matrix4 mviewdata;

    private float time;
    private int uniform_mview;

    private int vbo_coord;
    private int vbo_position;

    private float[] vertData;

    public Game() : base(600, 600, GraphicsMode.Default, "CoordinateSystems Test")
    { }
    private void InitProgram()
    {
        _Shader = new Shader("Shader/vs.glsl", "Shader/fs.glsl");

        attribute_vpos = GL.GetAttribLocation(_Shader.PID, "vPosition");
        attribute_texCoord = GL.GetAttribLocation(_Shader.PID, "aTexCoord");
        uniform_mview = GL.GetUniformLocation(_Shader.PID, "modelView");

        GL.GenBuffers(1, out vbo_position);
        GL.GenBuffers(1, out vbo_coord);

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

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
            OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        image.UnlockBits(data);
        image.Dispose();

        _Shader.Use();
        _Shader.SetInt("texture1", 0);
        _Shader.SetInt("texture2", 1);
    }
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        InitProgram();

        vertData = new float[]
        {
            
                  //背
                -0.5f, -0.5f, -0.5f,
                0.5f, -0.5f, -0.5f,
                0.5f, 0.5f, -0.5f,
                0.5f, 0.5f, -0.5f,
                -0.5f, 0.5f, -0.5f,
                -0.5f, -0.5f, -0.5f,

                //正
                -0.5f, -0.5f, 0.5f,
                0.5f, -0.5f, 0.5f,
                0.5f, 0.5f, 0.5f,
                0.5f, 0.5f, 0.5f,
                -0.5f, 0.5f, 0.5f,
                -0.5f, -0.5f, 0.5f,

                //左
                -0.5f, 0.5f, 0.5f,
                -0.5f, 0.5f, -0.5f,
                -0.5f, -0.5f, -0.5f,
                -0.5f, -0.5f, -0.5f,
                -0.5f, -0.5f, 0.5f,
                -0.5f, 0.5f, 0.5f,

                //右
                0.5f, 0.5f, 0.5f,
                0.5f, 0.5f, -0.5f,
                0.5f, -0.5f, -0.5f,
                0.5f, -0.5f, -0.5f,
                0.5f, -0.5f, 0.5f,
                0.5f, 0.5f, 0.5f,

                //下
                -0.5f, -0.5f, -0.5f,
                0.5f, -0.5f, -0.5f,
                0.5f, -0.5f, 0.5f,
                0.5f, -0.5f, 0.5f,
                -0.5f, -0.5f, 0.5f,
                -0.5f, -0.5f, -0.5f,

                //上
                -0.5f, 0.5f, -0.5f,
                0.5f, 0.5f, -0.5f,
                0.5f, 0.5f, 0.5f,
                0.5f, 0.5f, 0.5f,
                -0.5f, 0.5f, 0.5f,
                -0.5f, 0.5f, -0.5f
        };

        coord = new float[]
           {
                0.0f, 0.0f,
                1.0f, 0.0f,
                1.0f, 1.0f,
                1.0f, 1.0f,
                0.0f, 1.0f,
                0.0f, 0.0f,

                0.0f, 0.0f,
                1.0f, 0.0f,
                1.0f, 1.0f,
                1.0f, 1.0f,
                0.0f, 1.0f,
                0.0f, 0.0f,

                1.0f, 0.0f,
                1.0f, 1.0f,
                0.0f, 1.0f,
                0.0f, 1.0f,
                0.0f, 0.0f,
                1.0f, 0.0f,

                1.0f, 0.0f,
                1.0f, 1.0f,
                0.0f, 1.0f,
                0.0f, 1.0f,
                0.0f, 0.0f,
                1.0f, 0.0f,

                0.0f, 1.0f,
                1.0f, 1.0f,
                1.0f, 0.0f,
                1.0f, 0.0f,
                0.0f, 0.0f,
                0.0f, 1.0f,

                0.0f, 1.0f,
                1.0f, 1.0f,
                1.0f, 0.0f,
                1.0f, 0.0f,
                0.0f, 0.0f,
                0.0f, 1.0f
            };

        mviewdata = Matrix4.Identity;

        GL.ClearColor(Color.CornflowerBlue);
        GL.PointSize(5);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);
        GL.Viewport(0, 0, Width, Height);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.Enable(EnableCap.DepthTest);

        GL.EnableVertexAttribArray(attribute_vpos);
        GL.EnableVertexAttribArray(attribute_texCoord);

        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, _TexID);
        GL.ActiveTexture(TextureUnit.Texture1);
        GL.BindTexture(TextureTarget.Texture2D, _TexID2);

        GL.DrawArrays(PrimitiveType.Triangles,0, vertData.Length/3);

        GL.DisableVertexAttribArray(attribute_vpos);
        GL.DisableVertexAttribArray(attribute_texCoord);

        GL.Flush();
        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);
       
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
        GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertData.Length * sizeof(float)), vertData,
            BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(attribute_vpos, 3, VertexAttribPointerType.Float, false, 0, 0);

        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_coord);
        GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(coord.Length*sizeof(float)),coord,
            BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(attribute_texCoord, 2, VertexAttribPointerType.Float, true, 0, 0);

        time += (float)e.Time;

        //这里的mviewdata其实是model，view，projection三个矩阵相乘
        mviewdata = Matrix4.CreateRotationY(0.55f * time) * Matrix4.CreateRotationX(0.15f * time) *
            Matrix4.CreateTranslation(0.0f, 0.0f, -3.0f) * Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f),
            ClientSize.Width / (float)ClientSize.Height, 1.0f, 40.0f);
        GL.UniformMatrix4(uniform_mview, false, ref mviewdata);

        _Shader.Use();
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }

    private void ProcessInput()
    {
        if (Keyboard.GetState().IsKeyDown(Key.Escape))
            Exit();
    }
}