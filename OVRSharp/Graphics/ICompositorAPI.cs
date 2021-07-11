using System.Drawing;
using Valve.VR;

namespace OVRSharp.Graphics
{
    /// <summary>
    /// An interface for graphics-related compositor API methods.<br/><br/>
    /// 
    /// You can find implementations for different graphics APIs on NuGet as
    /// OVRSharp.Graphics.DirectX and OVRSharp.Graphics.OpenGL. Anyone else is also
    /// free to implement their own version of this for other graphics APIs and
    /// publish them.
    /// </summary>  
    public interface ICompositorAPI
    {
        /// <summary>
        /// Capture a screenshot of the headset view.
        /// </summary>
        /// <param name="eye">The eye to capture.</param>
        Bitmap GetMirrorImage(EVREye eye = EVREye.Eye_Left);
    }
}