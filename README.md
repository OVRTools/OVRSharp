# OVRSharp

OVRSharp is a high-level idiomatic C# interface for working with the OpenVR API.

## Installation

> **Note:** While OVRSharp does technically come packaged with `openvr_api.dll`, you will need to distribute it yourself alongside your application executable. You can download it [here](https://github.com/ValveSoftware/openvr/tree/master/bin) for the platform you are targeting.
>
> We are currently exploring options to automatically distribute the appropriate DLL alongside OVRSharp.

OVRSharp is available on [NuGet](https://www.nuget.org/packages/OVRSharp), so you can install it just like any other NuGet package:

```shell
# .NET CLI
dotnet add package OVRSharp

# VS PowerShell
Install-Package OVRSharp
```

## Usage

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

If you encounter anything that is not natively supported by OVRSharp, you can always use the internal `Valve.VR` APIs. An instance of `CVRSystem` is accessible through `Application`, as `OVRSystem`.

If you run into something you would really like to see supported, feel free to open an issue describing your use case! That will help us prioritize what functionality to add next.

## Examples

OVRSharp is currently being used in [WhereIsForward](https://github.com/OVRTools/WhereIsForward), and in some unreleased projects.

## License

MIT