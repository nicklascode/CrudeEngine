using CrudeEngine.ECS;
using CrudeEngine.Graphics;
using CrudeEngine.Graphics.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Entities.Terrain
{
    public class TerrainEntity : Entity
    {
        public TerrainEntity()
        {
            // Initialize the terrain entity with a TerrainGenComponent
            BaseMaterial baseMaterial = new BaseMaterial(new Vector3(1.0f, 1.0f, 1.0f));
            baseMaterial.SetTexture(Texture.LoadTexture("Textures/SnowTexture.png"));
            AddComponent("Material", new MaterialComponent(baseMaterial));
            AddComponent("TerrainGen", new TerrainGenComponent(256, 256, 8f, 1));
        }
    }
}
