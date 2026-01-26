using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using CrudeEngine.ECS;
using CrudeEngine.Utils;
using OpenGL;

namespace CrudeEngine.Graphics.Renderer
{
    public abstract class Shader
    {
        protected uint _programId = 0;
        private Dictionary<string, int> _uniformLocations;

        protected Shader()
        {
            _programId = Gl.CreateProgram();
            _uniformLocations = new Dictionary<string, int>();

            if (_programId == 0)
            {
                ErrorCode c = Gl.GetError();
                throw new Exception($"Failed to create shader program. Error code: {c}");
            }
        }

        public void Bind()
        {
            Gl.UseProgram(_programId);
        }

        public void Unbind()
        {
            Gl.UseProgram(0);
        }

        public void AddUniform(string name)
        {
            int location = Gl.GetUniformLocation(_programId, name);
            if (location == -1)
            {
                throw new Exception($"Failed to get uniform location for {name}");
            }
            _uniformLocations[name] = location;
        }

        public void AddVertexShader(string shaderCode)
        {
            AddProgram(shaderCode, ShaderType.VertexShader);
        }

        public void AddFragmentShader(string shaderCode)
        {
            AddProgram(shaderCode, ShaderType.FragmentShader);
        }

        public void AddComputeShader(string shaderCode)
        {
            AddProgram(shaderCode, ShaderType.ComputeShader);
        }

        private void AddProgram(string shaderCode, ShaderType type)
        {
            // IMPORTANT: ShaderCode ref to code after loading the file using a loader ( Don't forget that Nicklas! )

            uint shaderId = Gl.CreateShader(type);
            if (shaderId == 0)
            {
                throw new Exception($"Failed to create shader of type {type}");
            }

            // Fix for Problem 1: Convert string to string[] as required by Gl.ShaderSource
            Gl.ShaderSource(shaderId, new string[] { shaderCode });

            Gl.CompileShader(shaderId);

            // Fix for Problem 2: Replace Gl.GetShaderiv with Gl.GetShader
            int status;
            Gl.GetShader(shaderId, ShaderParameterName.CompileStatus, out status);

            if (status == 0)
            {
                // Fix for Problem 3: Adjust Gl.GetShaderInfoLog to include required parameters
                int maxLength;
                Gl.GetShader(shaderId, ShaderParameterName.InfoLogLength, out maxLength);

                StringBuilder infoLog = new StringBuilder(maxLength);
                Gl.GetShaderInfoLog(shaderId, maxLength, out _, infoLog);

                throw new Exception($"Failed to compile shader: {infoLog}");
            }

            Gl.AttachShader(_programId, shaderId);
        }

        public void Link()
        {
            Gl.LinkProgram(_programId);

            // Check for link errors
            int status;
            Gl.GetProgram(_programId, ProgramProperty.LinkStatus, out status);

            if (status == 0)
            {
                // Retrieve the error log
                int maxLength;
                Gl.GetProgram(_programId, ProgramProperty.InfoLogLength, out maxLength);

                StringBuilder infoLog = new StringBuilder(maxLength);
                Gl.GetProgramInfoLog(_programId, maxLength, out _, infoLog);

                throw new Exception($"Failed to link shader program: {infoLog}");
            }
        }

        #region Uniform Setters
        public void SetUniform(string name, int value)
        {
            if (_uniformLocations.TryGetValue(name, out int location))
            {
                Gl.Uniform1(location, value);
            }
            else
            {
                throw new Exception($"Uniform {name} not found");
            }
        }
        public void SetUniform(string name, float value)
        {
            if (_uniformLocations.TryGetValue(name, out int location))
            {
                Gl.Uniform1(location, value);
            }
            else
            {
                throw new Exception($"Uniform {name} not found");
            }
        }
        public void SetUniform(string name, Vector2 value)
        {
            if (_uniformLocations.TryGetValue(name, out int location))
            {
                Gl.Uniform2(location, value.X, value.Y);
            }
            else
            {
                throw new Exception($"Uniform {name} not found");
            }
        }
        public void SetUniform(string name, Vector3 value)
        {
            if (_uniformLocations.TryGetValue(name, out int location))
            {
                Gl.Uniform3(location, value.X, value.Y, value.Z);
            }
            else
            {
                throw new Exception($"Uniform {name} not found");
            }
        }
        public void SetUniform(string name, Vector4 value)
        {
            if (_uniformLocations.TryGetValue(name, out int location))
            {
                Gl.Uniform4(location, value.X, value.Y, value.Z, value.W);
            }
            else
            {
                throw new Exception($"Uniform {name} not found");
            }
        }

        public void SetUniform(string name, Matrix4x4 value)
        {
            if (_uniformLocations.TryGetValue(name, out int location))
            {
                // Convert Matrix4x4 to a float array manually since 'ToArray' does not exist.  
                float[] matrixArray = new float[16];
                for(int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        matrixArray[i * 4 + j] = value[i, j];
                    }
                }

                Gl.UniformMatrix4(location, false, matrixArray);

            }
            else
            {
                throw new Exception($"Uniform {name} not found");
            }
        }

        public void SetUniform(string name, float[] value)
        {
            if (_uniformLocations.TryGetValue(name, out int location))
            {
                Gl.Uniform1(location, value);
            }
            else
            {
                throw new Exception($"Uniform {name} not found");
            }
        }

        #endregion

        public virtual void UpdateUniforms(Entity entity) { }

        public uint GetProgramId()
        {
            return _programId;
        }
    }
}
