using CrudeEngine.ECS;
using CrudeEngine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CrudeEngine.PhysicsEngine
{
    public class AABB : Component
    {
        public Vector3 Min { get; set; }
        public Vector3 Max { get; set; }
        public AABB(Vector3 min, Vector3 max)
        {
            // Take transform position into account when initializing AABB
            Vector3 realMin = min + entity.Transform.Position;
            Vector3 realMax = max + entity.Transform.Position;

            Min = new Vector3(
                CrudeMathUtils.Min(realMin.X, realMax.X),
                CrudeMathUtils.Min(realMin.Y, realMax.Y),
                CrudeMathUtils.Min(realMin.Z, realMax.Z)
            );
            Max = new Vector3(
                CrudeMathUtils.Max(realMin.X, realMax.X),
                CrudeMathUtils.Max(realMin.Y, realMax.Y),
                CrudeMathUtils.Max(realMin.Z, realMax.Z)
            );
        }

        public override void Update(float deltaTime = 0)
        {
            base.Update(deltaTime);
            // Update AABB position based on entity's transform position
            Vector3 position = entity.Transform.Position;
            Min = new Vector3(
                CrudeMathUtils.Min(Min.X, Max.X) + position.X,
                CrudeMathUtils.Min(Min.Y, Max.Y) + position.Y,
                CrudeMathUtils.Min(Min.Z, Max.Z) + position.Z
            );
            Max = new Vector3(
                CrudeMathUtils.Max(Min.X, Max.X) + position.X,
                CrudeMathUtils.Max(Min.Y, Max.Y) + position.Y,
                CrudeMathUtils.Max(Min.Z, Max.Z) + position.Z
            );
        }

        public bool IsColliding(AABB other)
        {
            return (Min.X <= other.Max.X && Max.X >= other.Min.X) &&
                   (Min.Y <= other.Max.Y && Max.Y >= other.Min.Y) &&
                   (Min.Z <= other.Max.Z && Max.Z >= other.Min.Z);
        }
    }
}
