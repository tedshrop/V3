using System.Drawing;
using System.Threading.Tasks;

namespace RhinoAIRender
{
    /// <summary>
    /// Result of a viewport capture operation.
    /// </summary>
    public class CaptureResult
    {
        /// <summary>
        /// Gets the captured bitmap image.
        /// </summary>
        public Bitmap Image { get; private set; }

        /// <summary>
        /// Gets the file path where the image was saved, if any.
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the capture was successful.
        /// </summary>
        public bool Success { get; private set; }

        /// <summary>
        /// Gets the error message if the capture failed.
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// Gets the capture type (Beauty, Depth, Segmentation).
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// Creates a successful capture result.
        /// </summary>
        /// <param name="image">The captured image.</param>
        /// <param name="type">The capture type.</param>
        /// <param name="filePath">The saved file path, if any.</param>
        public CaptureResult(Bitmap image, string type, string filePath = null)
        {
            Image = image;
            Type = type;
            FilePath = filePath;
            Success = true;
        }

        /// <summary>
        /// Creates a failed capture result.
        /// </summary>
        /// <param name="type">The capture type.</param>
        /// <param name="errorMessage">The error message.</param>
        public CaptureResult(string type, string errorMessage)
        {
            Type = type;
            ErrorMessage = errorMessage;
            Success = false;
        }

        /// <summary>
        /// Saves the captured image to the specified path.
        /// </summary>
        /// <param name="path">The file path to save to.</param>
        public void Save(string path)
        {
            if (Image != null)
            {
                var format = path.ToLower().EndsWith(".png") ? System.Drawing.Imaging.ImageFormat.Png : System.Drawing.Imaging.ImageFormat.Jpeg;
                Image.Save(path, format);
                FilePath = path;
            }
        }
    }
}
