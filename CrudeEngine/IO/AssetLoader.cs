using CrudeEngine.Graphics;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudeEngine.IO
{
    public class AssetLoader
    {
        public static string ROOT_PATH { get; set; } = "";

        public static void SetRootPath(string path)
        {
            ROOT_PATH = path;
            Console.WriteLine($"ROOT_PATH: {ROOT_PATH}");
        }

        public static string LoadShader(string filePath)
        {
            try
            {
                string fullPath = System.IO.Path.Combine(ROOT_PATH, filePath);
                if (!System.IO.File.Exists(fullPath))
                {
                    throw new Exception($"Shader file not found: {fullPath}");
                }

                return System.IO.File.ReadAllText(fullPath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading shader: {ex.Message}", ex);
            }
        }

    }
}
