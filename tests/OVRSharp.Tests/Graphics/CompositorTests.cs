using FluentAssertions;
using NUnit.Framework;
using OVRSharp.Graphics;
using OVRSharp.Exceptions;
using Valve.VR;

namespace OVRSharp.Tests.Graphics
{
    /// <summary>
    /// Tests to run against <see cref="ICompositorAPI"/> implementations.
    /// </summary>
    /// <typeparam name="T">Your implementation of <see cref="ICompositorAPI"/> to test.</typeparam>
    public abstract class CompositorTests<T> where T : ICompositorAPI
    {
        private Application app;
        private ICompositorAPI compositor;

        /// <summary>
        /// Instantiates the instance of your <see cref="ICompositorAPI"/>
        /// that will be used for testing.
        /// </summary>
        protected abstract T InstantiateCompositorAPI();

        [OneTimeSetUp]
        public void Setup()
        {
            try
            {
                app = new Application(Application.ApplicationType.Background);
            }
            catch(OpenVRSystemException<EVRInitError> e)
            {
                switch(e.Error)
                {
                    case EVRInitError.Init_InstallationNotFound:
                    case EVRInitError.Init_VRClientDLLNotFound:
                        Assert.Ignore("OpenVR runtime not found; skipping integration tests.");
                        break;
                    case EVRInitError.Init_NoServerForBackgroundApp:
                        Assert.Ignore("OpenVR runtime not running; skipping integration tests.");
                        break;
                    default:
                        throw;
                }
            }

            compositor = InstantiateCompositorAPI();
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
