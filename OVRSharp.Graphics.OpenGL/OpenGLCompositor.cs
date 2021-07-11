using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGLES3;
using OpenTK.Windowing.Desktop;
using OVRSharp.Exceptions;
using Valve.VR;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace OVRSharp.Graphics.OpenGL
{
    public class OpenGLCompositor : NativeWindow, ICompositorAPI
    {
        public OpenGLCompositor() : base(
            new NativeWindowSettings
            {
                StartVisible = false,
                Title = "OVRSharp Window",
                WindowState = OpenTK.Windowing.Common.WindowState.Minimized
            }
        ) { }

        public Bitmap GetMirrorImage(EVREye eye = EVREye.Eye_Left)
        {
            uint textureId = 0;
            var handle = new IntPtr();

            var result = OpenVR.Compositor.GetMirrorTextureGL(eye, ref textureId, handle);
            if (result != EVRCompositorError.None)
                throw new OpenVRSystemException<EVRCompositorError>("Failed to get mirror texture from OpenVR", result);

            OpenVR.Compositor.LockGLSharedTextureForAccess(handle);

            GL.BindTexture(TextureTarget.Texture2d, new TextureHandle((int)textureId));

            var height = 0;
            GL.GetTexParameteri(TextureTarget.Texture2d, GetTextureParameter.TextureHeight, ref height);
            
            var width = 0;
            GL.GetTexParameteri(TextureTarget.Texture2d, GetTextureParameter.TextureWidth, ref width);

            var bitmap = new Bitmap(width, height);
            var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb);
            
            GL.Finish();
            GL.ReadPixels(0, 0, width, height, OpenTK.Graphics.OpenGLES3.PixelFormat.Rgb, PixelType.UnsignedByte, data.Scan0);

            bitmap.UnlockBits(data);
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

            OpenVR.Compositor.UnlockGLSharedTextureForAccess(handle);
            OpenVR.Compositor.ReleaseSharedGLTexture(textureId, handle);

            return bitmap;
        }
    }
}