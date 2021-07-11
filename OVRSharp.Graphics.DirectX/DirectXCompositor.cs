using OVRSharp.Exceptions;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using Valve.VR;

namespace OVRSharp.Graphics.DirectX
{
    public class DirectXCompositor : ICompositorAPI
    {
        private readonly Device device;

        public DirectXCompositor()
        {
            device = new Device(SharpDX.Direct3D.DriverType.Hardware, DeviceCreationFlags.Debug);
        }

        public Bitmap GetMirrorImage(EVREye eye = EVREye.Eye_Left)
        {
            var srvPtr = IntPtr.Zero;

            var result = OpenVR.Compositor.GetMirrorTextureD3D11(eye, device.NativePointer, ref srvPtr);
            if (result != EVRCompositorError.None)
                throw new OpenVRSystemException<EVRCompositorError>("Failed to get mirror texture from OpenVR", result);

            var srv = new ShaderResourceView(srvPtr);
            var tex = srv.Resource.QueryInterface<Texture2D>();
            var texDesc = tex.Description;

            var bitmap = new Bitmap(texDesc.Width, texDesc.Height);
            var boundsRect = new Rectangle(0, 0, texDesc.Width, texDesc.Height);

            using(var cpuTex = new Texture2D(device, new Texture2DDescription
            {
                CpuAccessFlags = CpuAccessFlags.Read,
                BindFlags = BindFlags.None,
                Format = texDesc.Format,
                Width = texDesc.Width,
                Height = texDesc.Height,
                OptionFlags = ResourceOptionFlags.None,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Staging
            }))
            {
                // Copy texture to CPU so we can read from it
                device.ImmediateContext.CopyResource(tex, cpuTex);
                OpenVR.Compositor.ReleaseMirrorTextureD3D11(srvPtr);

                var mapSource = device.ImmediateContext.MapSubresource(cpuTex, 0, MapMode.Read, MapFlags.None);
                var mapDest = bitmap.LockBits(boundsRect, ImageLockMode.WriteOnly, bitmap.PixelFormat);
                var sourcePtr = mapSource.DataPointer;
                var destPtr = mapDest.Scan0;

                for (int y = 0; y < texDesc.Height; y++)
                {
                    Utilities.CopyMemory(destPtr, sourcePtr, texDesc.Width * 4);
                    sourcePtr = IntPtr.Add(sourcePtr, mapSource.RowPitch);
                    destPtr = IntPtr.Add(destPtr, mapDest.Stride);
                }

                bitmap.UnlockBits(mapDest);
                device.ImmediateContext.UnmapSubresource(cpuTex, 0);
            }

            FlipChannels(ref bitmap);
            return bitmap;
        }

        private static void FlipChannels(ref Bitmap bitmap)
        {
            var bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            var data = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadWrite,
                bitmap.PixelFormat
            );

            var length = System.Math.Abs(data.Stride) * bitmap.Height;

            unsafe
            {
                byte* rgbValues = (byte*)data.Scan0.ToPointer();
                for (int i = 0; i < length; i += bytesPerPixel)
                {
                    byte dummy = rgbValues[i];
                    rgbValues[i] = rgbValues[i + 2];
                    rgbValues[i + 2] = dummy;
                }
            }

            bitmap.UnlockBits(data);
        }
    }
}
