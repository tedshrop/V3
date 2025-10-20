using Rhino;
using Rhino.Commands;
using Rhino.Input;
using System.IO;

namespace RhinoAIRender.Commands
{
    /// <summary>
    /// Command to capture a depth map of the current viewport.
    /// </summary>
    public class CaptureDepthCommand : Command
    {
        /// <summary>
        /// Gets the English name of the command.
        /// </summary>
        public override string EnglishName => "AIR_CaptureDepth";

        /// <summary>
        /// Runs the capture depth command.
        /// </summary>
        /// <param name="doc">The active Rhino document.</param>
        /// <param name="mode">The run mode.</param>
        /// <returns>The command result.</returns>
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // Output directory - using temp for now. Edit to user input later.
            var outputDir = Path.GetTempPath();

            var service = new ViewportCaptureService();
            var options = new CaptureOptions
            {
                OutputDirectory = outputDir
            };

            RhinoApp.WriteLine("Capturing depth map...");
            var result = service.CaptureDepth(options);

            if (result.Success)
            {
                RhinoApp.WriteLine($"Depth capture successful. Saved to: {result.FilePath}");
                return Result.Success;
            }
            else
            {
                RhinoApp.WriteLine($"Depth capture failed: {result.ErrorMessage}");
                return Result.Failure;
            }
        }
    }
}
