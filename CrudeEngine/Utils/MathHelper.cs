using CrudeEngine.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CrudeEngine.Utils
{
    public class MathHelper
    {
        public static float DegreesToRadians(float fieldOfView)
        {
            return fieldOfView * (MathF.PI / 180f);
        }

        public static Matrix4x4 CreateTransformMatrix(Transform transform)
        {
            Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation(transform.Position);
            Matrix4x4 rotationMatrix = Matrix4x4.CreateFromQuaternion(transform.Rotation);
            Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(transform.Scale);
            return scaleMatrix * rotationMatrix * translationMatrix;
        }

        public static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        public static float GetAngleBetween(Vector3 from, Vector3 to)
        {
            float dot = Vector3.Dot(from, to);
            float magnitudeProduct = from.Length() * to.Length();
            if (magnitudeProduct == 0) return 0; // Avoid division by zero
            return MathF.Acos(dot / magnitudeProduct);
        }
    }
}
