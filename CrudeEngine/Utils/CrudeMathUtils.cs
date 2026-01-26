using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CrudeEngine.Utils
{
    public class CrudeMathUtils
    {
        public static float[] ToFloatArrayColumnMajor(Matrix4x4 matrix)
        {
            return new float[]
            {
                matrix.M11, matrix.M21, matrix.M31, matrix.M41,
                matrix.M12, matrix.M22, matrix.M32, matrix.M42,
                matrix.M13, matrix.M23, matrix.M33, matrix.M43,
                matrix.M14, matrix.M24, matrix.M34, matrix.M44
            };
        }

        public static float Min(float a, float b)
        {
            return a < b ? a : b;
        }
        public static float Max(float a, float b)
        {
            return a > b ? a : b;
        }
    }
}
