using OVRSharp.Exceptions;
using System;
using System.Threading;
using Valve.VR;

namespace OVRSharp
{
    public class Overlay
    {
        public enum TrackedDeviceRole
        {
            None,
            Hmd,
            RightHand = ETrackedControllerRole.RightHand,
            LeftHand = ETrackedControllerRole.LeftHand
        }

        /// <summary>
        /// This event is fired when mouse movement is detected.
        /// </summary>
        public event EventHandler<VREvent_t> OnMouseMove;
        public event EventHandler<VREvent_t> OnMouseDown;
        public event EventHandler<VREvent_t> OnMouseUp;
        public event EventHandler<VREvent_t> OnUnknown;

        public readonly string Key;
        public readonly string Name;
        public readonly bool IsDashboardOverlay;

        /// <summary>
        /// The rate at which to poll for overlay events, in milliseconds.
        /// </summary>
        public int PollingRate = 20;

        /// <summary>
        /// The pointer to the underlying OpenVR object for this overlay.
        /// </summary>
        public ulong Handle
        {
            get
            {
                return overlayHandle;
            }
        }

        /// <summary>
        /// The tracked device that the overlay is currently attached to.
        /// If set to <see cref="TrackedDeviceRole.None"/>, the overlay
        /// will switch back to absolute transform mode.
        /// </summary>
        public TrackedDeviceRole TrackedDevice
        {
            get
            {
                return trackedDevice;
            }

            set
            {
                if (IsDashboardOverlay) return;

                trackedDevice = value;

                EVROverlayError err;

                if (value == TrackedDeviceRole.None)
                {
                    err = OpenVR.Overlay.SetOverlayTransformAbsolute(overlayHandle, ETrackingUniverseOrigin.TrackingUniverseStanding, ref transform);
                    if (err != EVROverlayError.None) throw new Exception($"Could not set transform absolute: {err}");
                    return;
                }

                uint index = value == TrackedDeviceRole.Hmd
                    ? OpenVR.k_unTrackedDeviceIndex_Hmd
                    : OpenVR.System.GetTrackedDeviceIndexForControllerRole((ETrackedControllerRole)value);

                err = OpenVR.Overlay.SetOverlayTransformTrackedDeviceRelative(overlayHandle, index, ref transform);
                if (err != EVROverlayError.None) throw new Exception($"Could not set transform relative: {err}");

                trackedDeviceIndex = index;
            }
        }

        /// <summary>
        /// Sets the part of the texture to use for the overlay. UV Min
        /// is the upper left corner and UV Max is the lower right corner.
        /// By default overlays use the entire texture.
        /// </summary>
        public VRTextureBounds_t TextureBounds
        {
            set
            {
                AssertNoError(OpenVR.Overlay.SetOverlayTextureBounds(overlayHandle, ref value));
            }

            get
            {
                VRTextureBounds_t bounds = new VRTextureBounds_t();
                AssertNoError(OpenVR.Overlay.GetOverlayTextureBounds(overlayHandle, ref bounds));
                return bounds;
            }
        }

        /// <summary>
        /// Sets/gets the mouse scaling factor that is used for mouse events.
        /// The actual texture may be a different size, but this is typically
        /// the size of the underlying UI in pixels.
        /// </summary>
        public HmdVector2_t MouseScale
        {
            set
            {
                AssertNoError(OpenVR.Overlay.SetOverlayMouseScale(overlayHandle, ref value));
            }

            get
            {
                HmdVector2_t scale = new HmdVector2_t();
                AssertNoError(OpenVR.Overlay.GetOverlayMouseScale(overlayHandle, ref scale));
                return scale;
            }
        }

        /// <summary>
        /// Dashboard overlays are always <see cref="VROverlayInputMethod.Mouse"/>. Other
        /// overlays default to <see cref="VROverlayInputMethod.None"/>, but can be set to
        /// use automatic mouse interaction using this function.
        /// </summary>
        public VROverlayInputMethod InputMethod
        {
            set
            {
                AssertNoError(OpenVR.Overlay.SetOverlayInputMethod(overlayHandle, value));
            }

            get
            {
                VROverlayInputMethod method = VROverlayInputMethod.None;
                AssertNoError(OpenVR.Overlay.GetOverlayInputMethod(overlayHandle, ref method));
                return method;
            }
        }

        /// <summary>
        /// Sets/gets the width of the overlay quad in meters. By default overlays are rendered
        /// on a quad that is 1 meter across.
        /// <br/><br/>
        /// Overlays are rendered at the aspect ratio of their underlying texture and texture
        /// bounds. Height is a function of width and that aspect ratio. An overlay that is 1.6
        /// meters wide and has a 1920x1080 texture would be 0.9 meters tall.
        /// </summary>
        public float WidthInMeters
        {
            set
            {
                AssertNoError(OpenVR.Overlay.SetOverlayWidthInMeters(overlayHandle, value));
            }

            get
            {
                float width = 0.0f;
                AssertNoError(OpenVR.Overlay.GetOverlayWidthInMeters(overlayHandle, ref width));
                return width;
            }
        }


        /// <summary>
        /// Use to draw overlay as a curved surface. Curvature is a percentage from (0..1] where
        /// 1 is a fully closed cylinder.
        /// <br/>
        /// For a specific radius, curvature can be computed as:
        /// <code>overlay.width / (2 PI r)</code>
        /// </summary>
        public float Curvature
        {
            set
            {
                AssertNoError(OpenVR.Overlay.SetOverlayCurvature(overlayHandle, value));
            }

            get
            {
                float curvature = 0.0f;
                AssertNoError(OpenVR.Overlay.GetOverlayCurvature(overlayHandle, ref curvature));
                return curvature;
            }
        }

        /// <summary>
        /// Sets/gets the alpha of the overlay quad. Use 1.0 for 100 percent opacity to 0.0 for 0
        /// percent opacity.
        /// <br/>
        /// By default, overlays are rendering at 100 percent alpha (1.0).
        /// </summary>
        public float Alpha
        {
            set
            {
                AssertNoError(OpenVR.Overlay.SetOverlayAlpha(overlayHandle, value));
            }

            get
            {
                float alpha = 0.0f;
                AssertNoError(OpenVR.Overlay.GetOverlayAlpha(overlayHandle, ref alpha));
                return alpha;
            }
        }

        private HmdMatrix34_t transform;
        public HmdMatrix34_t Transform {
            set
            {
                if (IsDashboardOverlay) return;

                transform = value;

                // absolute transform (no tracked devices)
                if(TrackedDevice == TrackedDeviceRole.None)
                {
                    AssertNoError(OpenVR.Overlay.SetOverlayTransformAbsolute(overlayHandle, ETrackingUniverseOrigin.TrackingUniverseStanding, ref value));
                    return;
                }

                // relative transform (oops, we are assuming things)
                AssertNoError(OpenVR.Overlay.SetOverlayTransformTrackedDeviceRelative(overlayHandle, trackedDeviceIndex, ref value));
            }

            get
            {
                return transform;
            }
        }

        private TrackedDeviceRole trackedDevice = TrackedDeviceRole.None;
        private uint trackedDeviceIndex = 0;

        private ulong overlayHandle;
        private ulong thumbnailHandle;
        private Thread pollThread;

        /// <summary>
        /// Instantiate a new OpenVR overlay with the specified key
        /// and name.
        /// </summary>
        /// 
        /// <param name="key">
        /// The key to create the overlay with. Keys must
        /// be globally unique and may not be longer than
        /// k_unVROverlayMaxKeyLength including the terminating
        /// null.
        /// </param>
        /// 
        /// <param name="name">
        /// The friendly, user-visible name of the overlay.
        /// When appropriate the name should be provided in
        /// UTF-8 in the current system language. This name
        /// may not be longer than k_unVROverlayMaxNameLength
        /// including the terminating null.
        /// </param>
        public Overlay(string key, string name, bool dashboardOverlay = false)
        {
            if (OpenVR.Overlay == null)
                throw new NullReferenceException("OpenVR has not been initialized. Please initialize it by instantiating a new Application.");

            EVROverlayError err;

            if (dashboardOverlay)
                err = OpenVR.Overlay.CreateDashboardOverlay(key, name, ref overlayHandle, ref thumbnailHandle);
            else
                err = OpenVR.Overlay.CreateOverlay(key, name, ref overlayHandle);

            if (err != EVROverlayError.None)
                throw new OpenVRSystemException<EVROverlayError>($"Could not initialize overlay: {err}", err);

            Key = key;
            Name = name;
            IsDashboardOverlay = dashboardOverlay;
        }

        /// <summary>
        /// Wrap an existing OpenVR overlay with its handle.
        /// </summary>
        /// 
        /// <param name="overlayHandle">
        /// The handle of the overlay to wrap.
        /// </param>
        public Overlay(ulong overlayHandle)
        {
            this.overlayHandle = overlayHandle;

            // TODO: set Key and Name accordingly
            //StringBuilder keyVal = new StringBuilder((int)OpenVR.k_unVROverlayMaxKeyLength);
            //OpenVR.Overlay.GetOverlayKey(overlayHandle, keyVal, keyVal.Length)
        }

        /// <summary>
        /// Start polling for overlay events. While polling, the
        /// events <see cref="OnMouseDown"/>, <see cref="OnMouseMove"/>, etc.
        /// will be fired as they are received. If this overlay is already polling,
        /// this method is a no-op.
        /// 
        /// <br/><br/>
        /// 
        /// Use <see cref="StopPolling"/> to stop polling for events.
        /// </summary>
        public void StartPolling()
        {
            if (pollThread != null) return;

            pollThread = new Thread(new ThreadStart(Poll));
            pollThread.Start();
        }

        /// <summary>
        /// Stop polling for overlay events. If the overlay is
        /// not polling, this method is a no-op.
        /// 
        /// <br/><br/>
        /// 
        /// Use <see cref="StartPolling"/> to begin polling for events.
        /// </summary>
        public void StopPolling()
        {
            if (pollThread == null) return;
        }

        /// <summary>
        /// Request that the OpenVR runtime displays the overlay
        /// in the scene. For VR Dashboard overlays only the dashboard
        /// manager is allowed to call this.
        /// 
        /// <br/><br/>
        /// 
        /// Use <see cref="Hide"/> to hide the overlay.
        /// </summary>
        public void Show()
        {
            if (IsDashboardOverlay) return;
            AssertNoError(OpenVR.Overlay.ShowOverlay(overlayHandle));
        }

        /// <summary>
        /// Request that the OpenVR runtime hides the overlay
        /// in the scene. For VR Dashboard overlays only the dashboard
        /// manager is allowed to call this.
        /// 
        /// <br/><br/>
        /// 
        /// Use <see cref="Show"/> to show the overlay.
        /// </summary>
        public void Hide()
        {
            if (IsDashboardOverlay) return;
            AssertNoError(OpenVR.Overlay.HideOverlay(overlayHandle));
        }

        /// <summary>
        /// Destroy the overlay. When an application calls VR_Shutdown
        /// all overlays created by that app are automatically destroyed.
        /// </summary>
        public void Destroy()
        {
            StopPolling();
            AssertNoError(OpenVR.Overlay.DestroyOverlay(overlayHandle));
        }

        /// <summary>
        /// Loads the specified file and sets that texture as the contents
        /// of the overlay. Textures can be up to 1920x1080 in size.
        /// PNG, JPG, and TGA files are supported in 24 or 32 bits.
        /// </summary>
        /// 
        /// <param name="path">
        /// Path to the file. If a relative path is provided it is assumed
        /// to be relative to the resource directory in the OpenVR runtime.
        /// </param>
        public void SetTextureFromFile(string path)
        {
            AssertNoError(OpenVR.Overlay.SetOverlayFromFile(overlayHandle, path));
        }

        /// <summary>
        /// Sets an existing application-created graphics resource as the
        /// texture for the overlay. The type of the pTexture depends on
        /// the eTextureType parameter.
        /// </summary>
        /// 
        /// <param name="texture">
        /// <see cref="Texture_t"/> struct describing the texture
        /// </param>
        public void SetTexture(Texture_t texture)
        {
            AssertNoError(OpenVR.Overlay.SetOverlayTexture(overlayHandle, ref texture));
        }

        /// <summary>
        /// Loads the specified file and sets that texture as the contents
        /// of the overlay's thumbnail. Textures can be up to 1920x1080 in size.
        /// PNG, JPG, and TGA files are supported in 24 or 32 bits.
        /// </summary>
        /// 
        /// <param name="path">
        /// Path to the file. If a relative path is provided it is assumed
        /// to be relative to the resource directory in the OpenVR runtime.
        /// </param>
        public void SetThumbnailTextureFromFile(string path)
        {
            // TODO: these IsDashboardOverlay checks should probably error, not silently return
            if (!IsDashboardOverlay) return;
            AssertNoError(OpenVR.Overlay.SetOverlayFromFile(thumbnailHandle, path));
        }

        /// <summary>
        /// Sets an existing application-created graphics resource as the
        /// texture for the overlay's thumbnail. The type of the pTexture depends on
        /// the eTextureType parameter.
        /// </summary>
        /// 
        /// <param name="texture">
        /// <see cref="Texture_t"/> struct describing the texture
        /// </param>
        public void SetThumbnailTexture(Texture_t texture)
        {
            if (!IsDashboardOverlay) return;
            AssertNoError(OpenVR.Overlay.SetOverlayTexture(thumbnailHandle, ref texture));
        }

        /// <summary>
        /// Sets or gets the specified flag for the specified overlay.
        /// </summary>
        /// 
        /// <param name="flag">Flag to set</param>
        /// <param name="value">Flag value to set</param>
        public void SetFlag(VROverlayFlags flag, bool value)
        {
            AssertNoError(OpenVR.Overlay.SetOverlayFlag(overlayHandle, flag, value));
        }

        private void AssertNoError(EVROverlayError error)
        {
            if (error == EVROverlayError.None) return;
            throw new OpenVRSystemException<EVROverlayError>($"An error occurred within an Overlay. {error}", error);
        }

        private void Poll()
        {
            while (true)
            {
                VREvent_t evt = new VREvent_t();
                var size = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(VREvent_t));

                if (!OpenVR.Overlay.PollNextOverlayEvent(overlayHandle, ref evt, size))
                {
                    Thread.Sleep(PollingRate);
                    continue;
                };

                switch ((EVREventType)evt.eventType)
                {
                    case EVREventType.VREvent_MouseMove:
                        OnMouseMove?.Invoke(this, evt);
                        break;
                    case EVREventType.VREvent_MouseButtonDown:
                        OnMouseDown?.Invoke(this, evt);
                        break;
                    case EVREventType.VREvent_MouseButtonUp:
                        OnMouseUp?.Invoke(this, evt);
                        break;
                    default:
                        OnUnknown?.Invoke(this, evt);
                        break;
                }
            }
        }
    }
}
