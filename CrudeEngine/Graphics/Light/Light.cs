using CrudeEngine.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CrudeEngine.Graphics.Light
{
    public class Light
    {
        public Transform Transform { get; private set; }
        public Vector3 Color { get; private set; }
        public float Intensity { get; private set; }

        public Light(Vector3 position, Vector3 color, float intensity)
        {
            Transform = new Transform(position);
            Color = color;
            Intensity = intensity;
        }
    }
}
