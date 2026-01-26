using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudeEngine.Graphics.Renderer.RenderProperties
{
    public class DefaultRenderProperties : IRenderProperties
    {
        public void Enable()
        {
            Gl.Enable(EnableCap.DepthTest);
            Gl.Enable(EnableCap.CullFace);
            Gl.CullFace(CullFaceMode.Back);

        }
        public void Disable()
        {
            Gl.Disable(EnableCap.DepthTest);
            Gl.Disable(EnableCap.CullFace);
            Gl.CullFace(CullFaceMode.FrontAndBack);
        }
    }
}
