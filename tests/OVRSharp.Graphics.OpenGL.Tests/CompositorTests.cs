using FluentAssertions;
using NUnit.Framework;

namespace OVRSharp.Graphics.OpenGL.Tests
{
    public class CompositorTests
    {
        private Application app;
        private ICompositorAPI compositor;

        [OneTimeSetUp]
        public void Setup()
        {
            app = new Application(Application.ApplicationType.Background);
            compositor = new OpenGLCompositor();
        }

        [Test]
        public void ShouldGetMirrorTextureSuccessfully()
        {
            var bitmap = compositor.GetMirrorImage(Valve.VR.EVREye.Eye_Right);
            bitmap.Height.Should().BeGreaterThan(0);
            bitmap.Width.Should().BeGreaterThan(0);
        }
    }
}