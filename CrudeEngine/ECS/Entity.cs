using CrudeEngine.Graphics.Renderer;
using CrudeEngine.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudeEngine.ECS
{
    public class Entity
    {
        protected Transform transform;
        protected Dictionary<string, Component> components;

        public Entity()
        {
            transform = new Transform();
            components = new Dictionary<string, Component>();
        }

        public Transform Transform
        {
            get { return transform; }
        }

        public void AddComponent(string name, Component component)
        {
            if (!components.ContainsKey(name))
            {
                components.Add(name, component);
                component.SetEntity(this);
                component.Start();
            }
            else
            {
                throw new Exception($"Component {name} already exists in the entity.");
            }
        }
        public T GetComponent<T>() where T : Component
        {
            foreach (var component in components.Values)
            {
                if (component is T typedComponent)
                {
                    return typedComponent;
                }
            }

            Console.WriteLine($"Component of type {typeof(T).Name} not found in the entity.");
            return null;
        }

        public void RemoveComponent(string name)
        {
            if (components.ContainsKey(name))
            {
                components.Remove(name);
            }
            else
            {
                throw new Exception($"Component {name} does not exist in the entity.");
            }
        }

        public void Start()
        {
            foreach (var component in components.Values)
            {
                component.Start();
            }
        }

        public void Update(float deltaTime)
        {
            foreach (var component in components.Values)
            {
                component.Update(deltaTime);
            }
        }

        public void Render()
        {
            foreach (var component in components.Values)
            {
                if (component is Renderer renderer)
                {
                    renderer.Render();
                }
            }
        }
    }
}
