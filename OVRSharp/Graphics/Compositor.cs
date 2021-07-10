using System.Drawing;
using Valve.VR;

namespace OVRSharp.Graphics
{
    public interface ICompositorAPI
    {
        Bitmap GetMirrorImage(EVREye eye = EVREye.Eye_Left);
    }
}