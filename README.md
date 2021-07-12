# OVRSharp

OVRSharp is a high-level idiomatic C# interface for working with the OpenVR API.

## Installation

> **Note:** OVRSharp comes with its own version of `openvr_api.dll` and will automatically copy it to your project's output directory.

OVRSharp is available on [NuGet](https://www.nuget.org/packages/OVRSharp), so you can install it just like any other NuGet package:

```shell
# .NET CLI
dotnet add package OVRSharp

# VS PowerShell
Install-Package OVRSharp
```

## Usage

Check out the [Getting Started](https://github.com/OVRTools/OVRSharp/wiki/Getting-Started) guide to learn how to build your first OpenVR overlay with OVRSharp.

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

### Overlay transformations

Matrix manipulation is not something easy, OVRSharp provides extension methods to convert the [`HmdMatrix34_t`](https://github.com/ValveSoftware/openvr/blob/4c85abcb7f7f1f02adaf3812018c99fc593bc341/headers/openvr.h#L32-L40) structure to and from the [`Matrix4x4`](https://docs.microsoft.com/en-us/dotnet/api/system.numerics.matrix4x4?view=net-5.0) structure.

The purpose is to ease manipulations, the `Matrix4x4` struct contains [many static methods to apply translations, rotations, etc.](https://docs.microsoft.com/en-us/dotnet/api/system.numerics.matrix4x4?view=net-5.0#methods) That way you can transform your `Matrix4x4` with a high level of abstraction and then convert it to a `HmdMatrix34_t` before passing it to OpenVR. Here's an example:

```csharp
using OVRSharp;
using OVRSharp.Math; // Use "Math" to be able call the "ToHmdMatrix34_t()" and "ToMatrix4x4()" methods.

float radians = (float)((Math.PI / 180) * 90); // 90 degrees to radians
var rotation = Matrix4x4.CreateRotationX(radians); // Lay the overlay flat by rotating it by 90 degrees
var translation = Matrix4x4.CreateTranslation(0, 1, 0); // Raise the overlay one meter above the ground
var transform = Matrix4x4.Multiply(rotation, translation); // Combine the transformations in one matrix

Overlay overlay = new("key", "name")
{
    Transform = transform.ToHmdMatrix34_t(), // Convert the Matrix4x4 to a HmdMatrix34_t and pass it to OpenVR
};
```

## Examples

OVRSharp is currently being used in [WhereIsForward](https://github.com/OVRTools/WhereIsForward), and in some unreleased projects.

## License

MIT
