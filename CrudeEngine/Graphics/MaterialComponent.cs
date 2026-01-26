
using CrudeEngine.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CrudeEngine.Graphics
{
    public class MaterialComponent : Component
    {
        private BaseMaterial material;

        public MaterialComponent(BaseMaterial material)
        {
            this.material = material;
        }

        public MaterialComponent() : this(new BaseMaterial(new Vector3(1.0f, 1.0f, 1.0f))) { }

        public T GetMaterial<T>() where T : BaseMaterial
        {
            return material as T;
        }

        public void SetMaterial<T>(T material) where T : BaseMaterial
        {
            this.material = material;
        }
    }
}
