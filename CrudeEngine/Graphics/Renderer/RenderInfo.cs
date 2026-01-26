using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudeEngine.Graphics.Renderer
{
    public class RenderInfo
    {
        private IRenderProperties _renderProperties;
        private Shader _shader;

        public RenderInfo(IRenderProperties renderProperties, Shader shader)
        {
            _renderProperties = renderProperties;
            _shader = shader;
        }

        public IRenderProperties RenderProperties
        {
            get { return _renderProperties; }
            set { _renderProperties = value; }
        }
        public Shader Shader
        {
            get { return _shader; }
            set { _shader = value; }
        }
    }
}
