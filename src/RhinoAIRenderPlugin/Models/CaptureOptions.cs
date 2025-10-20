using System.Drawing;

namespace RhinoAIRender
{
    /// <summary>
    /// Options for viewport capture operations.
    /// </summary>
    public class CaptureOptions
    {
        /// <summary>
        /// Gets or sets the capture resolution width in pixels.
        /// </summary>
        public int Width { get; set; } = 1920;

        /// <summary>
        /// Gets or sets the capture resolution height in pixels.
        /// </summary>
        public int Height { get; set; } = 1080;

        /// <summary>
        /// Gets or sets the image format for saving.
        /// </summary>
        public string Format { get; set; } = "PNG";

        /// <summary>
        /// Gets or sets the output directory path.
        /// </summary>
        public string OutputDirectory { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether to use antialiasing.
        /// </summary>
        public bool UseAntialiasing { get; set; } = true;

        /// <summary>
        /// Gets or sets the capture quality for depth maps (0-1).
        /// </summary>
        public float DepthQuality { get; set; } = 1.0f;
    }
}
