using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace CrudeEngine.Math
{
    public class Transform
    {
        protected Vector3 position = Vector3.Zero;
        protected Quaternion rotation = Quaternion.Identity;
        protected Vector3 scale = Vector3.One;

        public Transform()
        {
            position = Vector3.Zero;
            rotation = Quaternion.Identity;
            scale = Vector3.One;
        }
        public Transform(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }
        public Transform(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
            scale = Vector3.One;
        }
        public Transform(Vector3 position)
        {
            this.position = position;
            rotation = Quaternion.Identity;
            scale = Vector3.One;
        }

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
        public Quaternion Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        public Vector3 Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public Vector3 GetForward()
        {
            return Vector3.Transform(Vector3.UnitZ, rotation);
        }
        public Vector3 GetRight()
        {
            return Vector3.Transform(Vector3.UnitX, rotation);
        }
        public Vector3 GetUp()
        {
            return Vector3.Transform(Vector3.UnitY, rotation);
        }

        public string ToString()
        {
            return $"Position: {position}, Rotation: {rotation}, Scale: {scale}";
        }

        internal Transform Clone()
        {
            throw new NotImplementedException();
        }
    }
}
