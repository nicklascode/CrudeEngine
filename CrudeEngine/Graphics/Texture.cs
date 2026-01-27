using System;
using System.Runtime.InteropServices;
using StbImageSharp;
using OpenGL;
using CrudeEngine.IO;

namespace CrudeEngine.Graphics
{
    public struct TextureParameters
    {
        public TextureParameterName Name;
        public int Value;
        public TextureParameters(TextureParameterName name, int value)
        {
            Name = name;
            Value = value;
        }
    }

    public class Texture
    {
        public int TextureId { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        private byte[] _rawData;
        private Dictionary<TextureParameterName, int> _textureParameters;

        private Texture()
        {
            TextureId = 0;
            Width = 0;
            Height = 0;
            _rawData = null;
            _textureParameters = new Dictionary<TextureParameterName, int>();
            _textureParameters[TextureParameterName.TextureMinFilter] = Gl.LINEAR;
            _textureParameters[TextureParameterName.TextureMagFilter] = Gl.LINEAR;
            _textureParameters[TextureParameterName.TextureWrapS] = Gl.REPEAT;
            _textureParameters[TextureParameterName.TextureWrapT] = Gl.REPEAT;
        }
        public Texture(int width, int height, byte[] rawData) : this()
        {
            Width = width;
            Height = height;
            _rawData = rawData;
            UploadToGPU();
        }

        public static Texture LoadTexture(string filePath)
        {
            string fullPath = Path.Combine(AssetLoader.ROOT_PATH, filePath);
            using (var stream = File.OpenRead(fullPath))
            {
                ImageResult imageResult = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                if (imageResult == null)
                    throw new Exception($"Failed to load texture from {fullPath}");

                var texture = new Texture
                {
                    Width = imageResult.Width,
                    Height = imageResult.Height,
                    _rawData = imageResult.Data
                };

                texture.UploadToGPU();
                return texture;
            }
        }

        private void UploadToGPU()
        {
            TextureId = (int)Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2d, (uint)TextureId);

            unsafe
            {
                fixed (byte* dataPtr = _rawData)
                {
                    Gl.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba,
                        Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte,
                        (IntPtr)dataPtr);
                }
            }

            // Set texture parameters
            foreach (var param in _textureParameters)
            {
                Gl.TexParameter(TextureTarget.Texture2d, param.Key, param.Value);
            }

            Gl.BindTexture(TextureTarget.Texture2d, 0); // Unbind after uploading
        }

        public void Bind(int unit = 0)
        {
            Gl.ActiveTexture(TextureUnit.Texture0 + unit);
            Gl.BindTexture(TextureTarget.Texture2d, (uint)TextureId);
        }

        public void Unbind()
        {
            Gl.BindTexture(TextureTarget.Texture2d, 0);
        }

        public void SetParameter(TextureParameterName name, int value)
        {
            if (_textureParameters.ContainsKey(name))
            {
                _textureParameters[name] = value;
                Gl.TexParameter(TextureTarget.Texture2d, name, value);
            }
            else
            {
                throw new ArgumentException($"Texture parameter {name} is not recognized.");
            }
        }
    }

}
