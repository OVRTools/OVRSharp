using FluentAssertions;
using NUnit.Framework;
using OVRSharp.Graphics;
using Valve.VR;

namespace OVRSharp.Tests.Graphics
{
    public class CompositorTests<T> where T : ICompositorAPI, new()
    {
        private Application app;
        private ICompositorAPI compositor;

        [OneTimeSetUp]
        public void Setup()
        {
            app = new Application(Application.ApplicationType.Background);
            compositor = new T();
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

        // This test is mostly here to make sure we are deallocating resources properly.
        [Test]
        public void ShouldWithstandRapidCalls()
        {
            Assert.Ignore();
            for (var i = 0; i < 1000; i++)
            {
                var bitmap = compositor.GetMirrorImage(EVREye.Eye_Left);
                bitmap.Height.Should().BeGreaterThan(0);
                bitmap.Width.Should().BeGreaterThan(0);

                bitmap.Dispose();
            }
        }
    }
}
