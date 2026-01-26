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
            // Initialization code
        }
        public virtual void Update(float deltaTime = 0)
        {
            // Update logic
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
