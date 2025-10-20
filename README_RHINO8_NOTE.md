Rhino 8 / .NET 6+ notes:

- Rhino 8 introduces different hosting and may support .NET 6 or newer. Porting will likely require converting the project to SDK-style (`<Project Sdk=\"Microsoft.NET.Sdk\">`) and targeting `net6.0-windows` while following the Rhino 8 developer guidance.
- You may need to switch to the updated RhinoCommon or Rhino.NET SDK packages if provided.
- I can provide a migration patch to an SDK-style project if you want to target Rhino 8.
