using System.Numerics;
using Silk.NET.OpenGL;
using Color = Api.Color;

namespace T_T_PandorasBox.Rendering
{
    internal class Shader : IDisposable
    {
        private uint _handle;
        private readonly GL _gl;

        public Shader(GL gl, string shaderName)
        {
            _gl = gl;
            
            var vertex = LoadShader(ShaderType.VertexShader, Path.Combine("Resources", "Shaders", $"{shaderName}.vert"));
            var fragment = LoadShader(ShaderType.FragmentShader, Path.Combine("Resources", "Shaders", $"{shaderName}.frag"));
            _handle = _gl.CreateProgram();
            _gl.AttachShader(_handle, vertex);
            _gl.AttachShader(_handle, fragment);
            _gl.LinkProgram(_handle);
            _gl.GetProgram(_handle, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                throw new Exception($"Program failed to link with error: {_gl.GetProgramInfoLog(_handle)}");
            }
            _gl.DetachShader(_handle, vertex);
            _gl.DetachShader(_handle, fragment);
            _gl.DeleteShader(vertex);
            _gl.DeleteShader(fragment);
        }

        private uint LoadShader(ShaderType type, string path)
        {
            var src = File.ReadAllText(path);
            var handle = _gl.CreateShader(type);
            _gl.ShaderSource(handle, src);
            _gl.CompileShader(handle);
            var infoLog = _gl.GetShaderInfoLog(handle);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                throw new Exception($"Error compiling shader of type {type}, failed with error {infoLog}");
            }

            return handle;
        }

        public void Use()
        {
            _gl.UseProgram(_handle);
        }

        public int GetHash(string name)
        {
            return _gl.GetUniformLocation(_handle, name);
        }

        public void SetInt(int hash, int value)
        {
            _gl.Uniform1(hash, value);
        }
        
        public void SetInt(string name, int value)
        {
            var hash = GetHash(name);
            if (hash == -1)
            {
                throw new Exception($"{name} int not found on shader.");
            }
            SetInt(hash, value);
        }
        
        public void SetFloat(int hash, float value)
        {
            _gl.Uniform1(hash, value);
        }

        public void SetFloat(string name, float value)
        {
            var hash = GetHash(name);
            if (hash == -1)
            {
                throw new Exception($"{name} float not found on shader.");
            }
            SetFloat(hash, value);
        }

        public void SetVector(int hash, Vector2 value)
        {
            _gl.Uniform2(hash, value);
        }

        public void SetVector(string name, Vector2 value)
        {
            var hash = GetHash(name);
            if (hash == -1)
            {
                throw new Exception($"{name} vector2 not found on shader.");
            }
            SetVector(hash, value);
        }

        public void SetVector(int hash, Vector3 value)
        {
            _gl.Uniform3(hash, value);
        }
        
        public void SetVector(string name, Vector3 value)
        {
            var hash = GetHash(name);
            if (hash == -1)
            {
                throw new Exception($"{name} vector3 not found on shader.");
            }
            SetVector(hash, value);
        }

        public void SetColor(int hash, Color color)
        {
            _gl.Uniform4(hash, color.R, color.G, color.B, color.A);
        }

        public void SetColor(string name, Color color)
        {
            var hash = GetHash(name);
            if (hash == -1)
            {
                throw new Exception($"{name} uniform not found on shader.");
            }
            
            SetColor(hash, color);
        }

        public unsafe void SetMatrix(int hash, Matrix4x4 value)
        {
            _gl.UniformMatrix4(hash, 1, false, (float*)&value);
        }

        public unsafe void SetMatrix(string name, Matrix4x4 value)
        {
            var hash = GetHash(name);
            if (hash == -1)
            {
                throw new Exception($"{name} matrix not found on shader.");
            }
            SetMatrix(hash, value);
        }


        private bool _disposed = false;
        private void Dispose(bool disposing)
        {
            try
            {
                if (_disposed) return;
                if (_handle > 0)
                {
                    try
                    {
                        _gl.DeleteProgram(_handle);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to dispose shader {ex}");
                    }
                }

                _handle = 0;
                _disposed = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Shader()
        {
            Dispose(false);
        }
    }
}
