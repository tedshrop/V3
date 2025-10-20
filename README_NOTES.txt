Notes:
- If Visual Studio reports a missing reference, edit the csproj property <RhinoCommonPath> to point at the Rhino 7 System folder on your machine (e.g., C:\Program Files\Rhino 7\System\).
- To load a plugin in Rhino: use 'PluginManager' command or Tools > Options > Plug-ins > Install, and point to the compiled assembly.
- This scaffold creates a library (.dll). Depending on your Rhino SDK and packaging, you may need to create an .rhp installer; for development loading the DLL is usually sufficient via the Plugin Manager.
