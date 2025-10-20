# TeddyRhinoPlugin (RhinoCommon C# plugin)

This workspace contains a minimal Rhino 7 RhinoCommon C# plugin scaffold.

What you get:
- Visual Studio solution file (`TeddyRhinoPlugin.sln`)
- C# Class Library project targeting .NET Framework 4.8 (`src\TeddyRhinoPlugin\TeddyRhinoPlugin.csproj`)
- A plugin class (`Plugin.cs`) derived from `Rhino.PlugIns.PlugIn`
- A sample command (`Commands\HelloCommand.cs`) that shows a message
- `.gitignore`, `LICENSE` (MIT), and a simple `.vscode/tasks.json` to run MSBuild

Quick build & load (Visual Studio recommended):
1. Open `TeddyRhinoPlugin.sln` in Visual Studio (2019/2022).
2. If Visual Studio can't resolve `RhinoCommon.dll`, edit the project property `RhinoCommonPath` in `TeddyRhinoPlugin.csproj` or add a reference to `C:\Program Files\Rhino 7\System\RhinoCommon.dll` (default Rhino 7 install path). The csproj contains a helpful default value you can change.
3. Build (Debug/Any CPU).
4. Load the plugin in Rhino:
   - Open Rhino 7, use `Tools > Options > Plug-ins > Install` and point at the built plugin (.rhp/.dll). Or use the `PluginManager` command and `Install`/`Load` and point to the compiled assembly in `bin\Debug`.

Notes & follow-ups:
- Porting to Rhino 8 / .NET 6+: Rhino 8 uses an updated SDK and different targeting; see README for guidance.
- This scaffold assumes Rhino is installed on the same machine and `RhinoCommon.dll` is available at `C:\Program Files\Rhino 7\System\` by default. If Rhino is installed elsewhere, update the project file.

If you want I can also:
- Add a NuGet-based approach (if RhinoCommon becomes available via NuGet for your setup)
- Create a Rhino Installer Engine script to produce a `.rhp` installer
- Add unit tests or CI
