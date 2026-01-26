using CrudeEngine.ECS;
using CrudeEngine.Graphics.Light;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudeEngine.SceneSystem
{
    public class Scene
    {
        private List<Entity> entities;
        private List<Light> lights;

        public Scene()
        {
            entities = new List<Entity>();
            lights = new List<Light>();
        }

        public void AddEntity(Entity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            entities.Add(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            entities.Remove(entity);
        }

        public void AddLight(Light light)
        {
            if (light == null) throw new ArgumentNullException(nameof(light));
            lights.Add(light);
        }

        public void RemoveLight(Light light)
        {
            if (light == null) throw new ArgumentNullException(nameof(light));
            lights.Remove(light);
        }

        public void Clear()
        {
            entities.Clear();
            lights.Clear();
        }
        public IEnumerable<Entity> GetEntities()
        {
            return entities.AsReadOnly();
        }
        public IEnumerable<Light> GetLights()
        {
            return lights.AsReadOnly();
        }
        public virtual void Start()
        {
            foreach (var entity in entities)
            {
                entity.Start();
            }
        }

        public virtual void Update(float deltaTime)
        {
            foreach (var entity in entities)
            {
                entity.Update(deltaTime);
            }
        }

        public virtual void Render()
        {
            foreach (var entity in entities)
            {
                entity.Render();
            }
        }
    }
}
