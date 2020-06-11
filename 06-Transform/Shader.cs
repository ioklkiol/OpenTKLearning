using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

public class Shader
{
    public int PID;

    public Shader(string pVertexPath, string pFragmentPath)
    {
        PID = GL.CreateProgram();

        string _V = new StreamReader(pVertexPath).ReadToEnd();
        string _F = new StreamReader(pFragmentPath).ReadToEnd();

        int _VID = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(_VID, _V);
        GL.CompileShader(_VID);
        GL.AttachShader(PID, _VID);

        int _FID = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(_FID, _F);
        GL.CompileShader(_FID);
        GL.AttachShader(PID, _FID);

        GL.LinkProgram(PID);

        GL.DeleteShader(_VID);
        GL.DeleteShader(_FID);
    }

    //激活程序
    public void Use()
    {
        GL.UseProgram(PID);
    }

    //uniform工具函数
    public void SetBool(string name, bool value)
    {
        GL.Uniform1(GL.GetUniformLocation(PID, name), value?1:0);
    }

    public void SetInt(string name, int value)
    {
        GL.Uniform1(GL.GetUniformLocation(PID, name), value);
    }

    public void SetFloat(string name, float value)
    {
        GL.Uniform1(GL.GetUniformLocation(PID, name), value);
    }

    public void Set4F(string name, float value1, float value2, float value3, float value4)
    {
        GL.Uniform4(GL.GetUniformLocation(PID, name), value1, value2, value3, value4);
    }

    public void SetMat4(string name, float[] mat4f)
    {
        GL.UniformMatrix4(GL.GetUniformLocation(PID, name), 1, false, mat4f);
    }

    public void SetMat4(string name, ref OpenTK.Matrix4 transform)
    {
        GL.UniformMatrix4(GL.GetUniformLocation(PID, name), false, ref transform);
    }
}