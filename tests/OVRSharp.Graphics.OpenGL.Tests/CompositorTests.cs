using NUnit.Framework;
using OVRSharp.Tests.Graphics;

namespace OVRSharp.Graphics.OpenGL.Tests
{
    [TestFixture]
    public class OpenGLCompositorTests : CompositorTests<OpenGLCompositor>
    {
        protected override OpenGLCompositor InstantiateCompositorAPI()
        {
            return new OpenGLCompositor();
        }
    }
}