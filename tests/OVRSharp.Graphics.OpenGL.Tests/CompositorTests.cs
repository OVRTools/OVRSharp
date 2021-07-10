using FluentAssertions;
using NUnit.Framework;
using Valve.VR;

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
        [TestCase(EVREye.Eye_Left)]
        [TestCase(EVREye.Eye_Right)]
        public void ShouldGetMirrorTextureSuccessfully(EVREye eye)
        {
            var bitmap = compositor.GetMirrorImage(eye);
            bitmap.Height.Should().BeGreaterThan(0);
            bitmap.Width.Should().BeGreaterThan(0);
        }
    }
}