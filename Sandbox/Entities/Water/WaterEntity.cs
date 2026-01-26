using CrudeEngine.ECS;
using CrudeEngine.Graphics;
using CrudeEngine.Graphics.Renderer;
using CrudeEngine.Graphics.Renderer.RenderProperties;
using Sandbox.Entities.Terrain;
using Sandbox.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Entities.Water
{
    public class WaterEntity : Entity
    {
        public WaterEntity()
        {
            // Initialize the terrain entity with a TerrainGenComponent
            BaseMaterial baseMaterial = new BaseMaterial(new Vector3(1.0f, 1.0f, 1.0f));
            AddComponent("Material", new MaterialComponent(baseMaterial));
            Renderer renderer = new Renderer();
            renderer.SetRenderInfo(new RenderInfo(new DefaultRenderProperties(), new WaterShader()));
            AddComponent("Renderer", renderer);
            AddComponent("Water", new WaterComponent(256,256));
        }
    }
}
