using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudeEngine.ECS
{
    public class Component
    {
        protected Entity entity = new Entity();
        public virtual void Start()
        {
        }
        public virtual void Update(float deltaTime = 0)
        {
        }

        public Entity GetEntity()
        {
            return entity;
        }
        public void SetEntity(Entity entity)
        {
            this.entity = entity;
        }
    }
}
