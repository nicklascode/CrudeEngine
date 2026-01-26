using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CrudeEngine.Graphics
{
    public class BaseMaterial
    {
        public Vector3 Color { get; set; }
        public Texture? Texture { get; set; }

        public BaseMaterial()
        {
            Color = new Vector3(1.0f, 1.0f, 1.0f); // Default color is white
        }

        public BaseMaterial(Vector3 color)
        {
            Color = color;
        }

        public void SetColor(Vector3 color)
        {
            Color = color;
        }
        public Vector3 GetColor()
        {
            return Color;
        }

        public void SetTexture(Texture texture)
        {
            Texture = texture;
        }

        public Texture? GetTexture()
        {
            return Texture;
        }
    }
}
