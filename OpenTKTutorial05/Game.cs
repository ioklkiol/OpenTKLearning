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

//Texture
public class Game : GameWindow
{
    private Shader _Shader;

    private int _VBO;
    private int _VAO;
    private int _EBO;
    private int _TexID;
    private int _TexID2;

    private float[] _VertData;

    public Game() : base(600, 600, GraphicsMode.Default, "Texture Test",
        GameWindowFlags.Default, DisplayDevice.Default, 3, 3, GraphicsContextFlags.ForwardCompatible)
    { }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        _Shader = new Shader("Shader/vs.glsl", "Shader/fs.glsl");

        _VertData = new float[]
        {
            // positions          // colors           // texture coords
                0.5f,  0.5f, 0.0f,   1.0f, 0.0f, 0.0f,   1.0f, 1.0f, // top right
                0.5f, -0.5f, 0.0f,   0.0f, 1.0f, 0.0f,   1.0f, 0.0f, // bottom right
                -0.5f, -0.5f, 0.0f,   0.0f, 0.0f, 1.0f,   0.0f, 0.0f, // bottom left
                -0.5f,  0.5f, 0.0f,   1.0f, 1.0f, 0.0f,   0.0f, 1.0f  // top left      
        };

        _VAO = GL.GenVertexArray();
        GL.BindVertexArray(_VAO);

        int[] indices = new int[]
        {
            0,1,3,      //first triangle
            1,2,3       //second triangle
        };

        _EBO = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EBO);
        GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int)
            * indices.Length, indices, BufferUsageHint.StaticDraw);

        _VBO = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_VertData.Length
            * sizeof(float)), _VertData, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));

        GL.EnableVertexAttribArray(0);
        GL.EnableVertexAttribArray(1);
        GL.EnableVertexAttribArray(2);

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        GL.BindVertexArray(0);

        _TexID = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, _TexID);
        GL.TextureParameter(_TexID, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TextureParameter(_TexID, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        GL.TextureParameter(_TexID,TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TextureParameter(_TexID, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        Bitmap image = new Bitmap("Resource/tex.jpg");
        BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
            ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, data.Width, data.Height,0,
            OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        image.UnlockBits(data);
        image.Dispose();
        
        _TexID2 = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, _TexID2);
        GL.TextureParameter(_TexID2, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TextureParameter(_TexID2, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        GL.TextureParameter(_TexID2,TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TextureParameter(_TexID2, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        image = new Bitmap("Resource/tex2.png");
        image.RotateFlip(RotateFlipType.Rotate180FlipX);
        data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
            ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, data.Width, data.Height,0,
            OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        image.UnlockBits(data);
        image.Dispose();

        _Shader.Use();
        _Shader.SetInt("texture1", 0);
        _Shader.SetInt("texture2", 1);

        GL.ClearColor(Color.CornflowerBlue);
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