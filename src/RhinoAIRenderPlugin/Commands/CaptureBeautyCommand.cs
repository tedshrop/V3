using Rhino;
using Rhino.Commands;
using Rhino.Input;
using System.IO;

namespace RhinoAIRender.Commands
{
    /// <summary>
    /// Command to capture a beauty render of the current viewport.
    /// </summary>
    public class CaptureBeautyCommand : Command
    {
        /// <summary>
        /// Gets the English name of the command.
        /// </summary>
        public override string EnglishName => "AIR_CaptureBeauty";

        /// <summary>
        /// Runs the capture beauty command.
        /// </summary>
        /// <param name="doc">The active Rhino document.</param>
        /// <param name="mode">The run mode.</param>
        /// <returns>The command result.</returns>
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // Output directory - ng temp for now. Edit to user input later.
            var outputDir = Path.GetTempPath();

            var service = new ViewportCaptureService();
            var options = new CaptureOptions
            {
                OutputDirectory = outputDir
            };

            RhinoApp.WriteLine("Capturing beauty render...");
            var result = service.CaptureBeauty(options);

            if (result.Success)
            {
                RhinoApp.WriteLine($"Beauty capture successful. Saved to: {result.FilePath}");
                return Result.Success;
            }
            else
            {
                RhinoApp.WriteLine($"Beauty capture failed: {result.ErrorMessage}");
                return Result.Failure;
            }
        }
    }
}
