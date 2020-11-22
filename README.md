# OVRSharp

OVRSharp is a high-level idiomatic C# interface for working with the OpenVR API.

Right now, our main goal with this is to make working with the overlay API easier and more C#-like, as well as being entirely platform-agnostic. So you could use this all on its own in your own .NET Core project, or throw it into Unity and work with it there.

Later on, we plan on supporting basically the entirety of the OpenVR API, so you don't need to deal with pointers and stuff. Just write C# as you would expect.

So you can do this:

```csharp
Application app;

try {
  app = new Application(Application.ApplicationType.Overlay);
} catch(OpenVRSystemException e) {
  // Errors are exceptions!
}

var overlay = new Overlay("cool_overlay", "Cool Overlay", true) {
  WidthInMeters = 3.8f
};

overlay.SetTextureFromFile(@"C:\path\to\file.png");
overlay.SetThumbnailTextureFromFile(@"C:\path\to\thumb.png");
```

Instead of this:

```csharp
var err = EVRInitError.None;
var vrSystem = OpenVR.Init(ref err, EVRApplicationType.VRApplication_Overlay);

if (err != EVRInitError.None)
{
  // whatever error handling
}

// Create a dashboard overlay
var overlayErr = EVROverlayError.None;

ulong overlayHandle;
ulong thumbnailHandle;

overlayErr = OpenVR.Overlay.CreateDashboardOverlay("cool_overlay", "Cool Overlay", ref overlayHandle, ref thumbnailHandle);

if (overlayErr != EVROverlayError.None)
{
  // whatever error handling
}

overlayErr = OpenVR.Overlay.SetOverlayWidthInMeters(overlayHandle, 3.8f);

if (overlayErr != EVROverlayError.None)
{
  // whatever error handling
}

// Set the dashboard overlay up. First, the main overlay's texture
overlayErr = OpenVR.Overlay.SetOverlayFromFile(overlayHandle, @"C:\path\to\file.png");

if (overlayErr != EVROverlayError.None)
{
  // whatever error handling
}

// Then the thumbnail.
overlayErr = OpenVR.Overlay.SetOverlayFromFile(thumbnailHandle, @"C:\path\to\thumb.png");

if (overlayErr != EVROverlayError.None)
{
  // whatever error handling
}
```

Docs coming Soon™️.

## Examples

OVRSharp is currently being used in [WhereIsForward](https://github.com/OVRTools/WhereIsForward), and in some unreleased projects.