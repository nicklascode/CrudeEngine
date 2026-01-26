using CrudeEngine.ECS;
using CrudeEngine.Graphics.Mesh;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudeEngine.Graphics.Renderer
{
    public class Renderer : Component
    {
        private MeshVBO meshVBO;
        private RenderInfo renderInfo;
        private bool wireframe = false;

        public Renderer() { }

        public void Render()
        {
            renderInfo.RenderProperties.Enable();
            renderInfo.Shader.Bind();
            renderInfo.Shader.UpdateUniforms(entity);
            meshVBO.Draw();
            renderInfo.Shader.Unbind();
        }

        public override void Update(float deltaTime = 0)
        {
            base.Update(deltaTime);

            if(Input.Instance.IsKeyPressed(SDL2.SDL.SDL_Keycode.SDLK_F2))
            {
                SetWireframe(!IsWireframe());
            }
        }

        public MeshVBO GetMeshVBO()
        {
            return meshVBO;
        }

        public void SetMeshVBO(MeshVBO meshVBO)
        {
            this.meshVBO = meshVBO;
        }

        public RenderInfo GetRenderInfo()
        {
            return renderInfo;
        }

        public void SetRenderInfo(RenderInfo renderInfo)
        {
            this.renderInfo = renderInfo;
        }

        public void SetWireframe(bool wireframe)
        {
            this.wireframe = wireframe;
            if (wireframe)
            {
                Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            }
            else
            {
                Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            }
        }

        public bool IsWireframe()
        {
            return wireframe;
        }
    }
}
