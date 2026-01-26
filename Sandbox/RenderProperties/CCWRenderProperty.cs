using CrudeEngine.Graphics.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.RenderProperties
{
    public class CCWRenderProperty : IRenderProperties
    {
        public void Enable()
        {
            // Enable counter-clockwise culling
            OpenGL.Gl.Enable(OpenGL.EnableCap.CullFace);
            OpenGL.Gl.CullFace(OpenGL.CullFaceMode.Back);
            OpenGL.Gl.FrontFace(OpenGL.FrontFaceDirection.Ccw);
        }
        public void Disable()
        {
            // Disable culling
            OpenGL.Gl.Disable(OpenGL.EnableCap.CullFace);
            OpenGL.Gl.CullFace(OpenGL.CullFaceMode.FrontAndBack);
            OpenGL.Gl.FrontFace(OpenGL.FrontFaceDirection.Cw); // Reset to default clockwise front face
        }
    }
}
