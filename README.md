# Teddy's Rhino Plugin (AI Render Tools)

A RhinoCommon C# plugin for capturing AI-ready render images from Rhino viewports.

## Features

This plugin provides automated capture commands for generating beauty renders, depth maps, and segmentation masks from the current Rhino viewport. Ideal for AI/ML workflows that require consistent rendering data.

### Commands
- `AIR_CaptureBeauty`: Captures a beauty render in Rendered display mode
- `AIR_CaptureDepth`: Captures a depth map using programmatic ZBuffer display mode (with fallback to ShowZBuffer command)
- `AIR_CaptureSegmentation`: Captures a segmentation mask based on object layers

## Build & Install

1. **Prerequisites**: Visual Studio 2019/2022, .NET Framework 4.8, Rhino 7/8
2. Open `RhinoAIRenderPlugin.sln` in Visual Studio
3. Build (Debug/Any CPU)
4. Load the plugin in Rhino:
   - Use `Tools > Options > Plug-ins > Install` and select the built `.rhp` or `.dll` file
   - Or use the `PluginManager` command to install/load the assembly from `bin\Debug`

## Usage

After installing the plugin:
1. Open a model in Rhino
2. Run one of the capture commands (e.g., `AIR_CaptureDepth`)
3. Images are saved to the temp directory by default (configurable in future versions)

## Technical Details

- Target Framework: .NET Framework 4.8
- Tested on Rhino 7/8
- Uses programmatic display mode switching for reliability
- Includes fallback mechanisms for cross-version compatibility

## Project Structure

- `RhinoAIRenderPlugin.cs`: Main plugin class
- `Commands/`: Individual command implementations
- `Services/`: Viewport capture logic
- `Models/`: Data structures for capture results and options
