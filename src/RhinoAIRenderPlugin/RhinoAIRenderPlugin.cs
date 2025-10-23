using System;
using Rhino;
using Rhino.PlugIns;

namespace RhinoAIRender
{
  /// <summary>
  /// Plugin class for Rhino AI Render functionality.
  /// Handles plugin initialization and command registration.
  /// </summary>
  public class RhinoAIRenderPlugin : PlugIn
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="RhinoAIRenderPlugin"/> class.
    /// </summary>
    public RhinoAIRenderPlugin()
    {
      Instance = this;
    }

    /// <summary>
    /// Gets the singleton instance of the plugin.
    /// </summary>
    public static RhinoAIRenderPlugin Instance { get; private set; }

    /// <summary>
    /// Called when the plugin is loaded.
    /// Registers commands and initializes the plugin.
    /// </summary>
    /// <param name="errorMessage">Error message if loading fails.</param>
    /// <returns>The load return code.</returns>
    protected override LoadReturnCode OnLoad(ref string errorMessage)
    {
      RhinoApp.WriteLine("Rhino AI Render Plugin loaded.");

      // Commands are automatically registered by Rhino based on the Command class

      return LoadReturnCode.Success;
    }
  }
}
