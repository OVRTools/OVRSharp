using NUnit.Framework;
using OVRSharp.Tests.Graphics;

namespace OVRSharp.Graphics.DirectX.Tests
{
    [TestFixture]
    public class DirectXCompositorTests : CompositorTests<DirectXCompositor>
    {
        protected override DirectXCompositor InstantiateCompositorAPI() => DirectXCompositor.Instance;
    }
}