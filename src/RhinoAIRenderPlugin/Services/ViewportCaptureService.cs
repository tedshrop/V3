using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using Rhino;
using Rhino.Commands;
using Rhino.Display;
using Rhino.Geometry;
using Rhino.DocObjects;

namespace RhinoAIRender
{
    /// <summary>
    /// Service for capturing images from the Rhino viewport.
    /// Supports beauty, depth, and segmentation capture types.
    /// </summary>
    public class ViewportCaptureService
    {
        private void EnsureRedrawAndWait(RhinoView view, int ms = 200)
        {
            try
            {
                view?.Redraw();
                RhinoDoc.ActiveDoc?.Views?.Redraw();
            }
            catch
            {
                // Ignore redraw failures - best effort
            }

            // Small wait to allow the display pipeline to update
            Thread.Sleep(ms);
        }

        /// <summary>
        /// Captures a beauty render of the current viewport.
        /// </summary>
        /// <param name="options">The capture options.</param>
        /// <returns>The capture result.</returns>
        public CaptureResult CaptureBeauty(CaptureOptions options)
        {
            try
            {
                var doc = RhinoDoc.ActiveDoc;
                if (doc == null) return new CaptureResult("Beauty", "No active document");

                var view = doc.Views.ActiveView;
                if (view == null) return new CaptureResult("Beauty", "No active view");

                var viewport = view.ActiveViewport;
                var originalMode = viewport.DisplayMode;

                try
                {
                    // Force Rendered display mode (do not silently fall back to a mode that may render wireframe)
                    var renderedMode = DisplayModeDescription.FindByName("Rendered");
                    if (renderedMode == null)
                    {
                        RhinoApp.WriteLine("[CaptureBeauty] Rendered display mode not found. Aborting beauty capture.");
                        return new CaptureResult("Beauty", "Rendered display mode not found");
                    }

                    var __origName = originalMode != null ? originalMode.EnglishName : "<null>";
                    RhinoApp.WriteLine("[CaptureBeauty] Current mode: " + __origName + ", switching to: " + renderedMode.EnglishName);
                    if (renderedMode.Id != originalMode.Id)
                    {
                        viewport.DisplayMode = renderedMode;
                        EnsureRedrawAndWait(view, 300);
                    }

                    var bitmap = view.CaptureToBitmap(new Size(options.Width, options.Height));

                    var result = new CaptureResult(bitmap, "Beauty");

                    if (!string.IsNullOrEmpty(options.OutputDirectory))
                    {
                        var filePath = Path.Combine(options.OutputDirectory, $"beauty.{options.Format.ToLower()}");
                        result.Save(filePath);
                    }

                    return result;
                }
                finally
                {
                    // Restore original mode
                    if (viewport != null && originalMode != null)
                    {
                        viewport.DisplayMode = originalMode;
                        EnsureRedrawAndWait(view, 150);
                        RhinoApp.WriteLine("[CaptureBeauty] Restored mode to: " + originalMode.EnglishName);
                    }
                }
            }
            catch (System.Exception ex)
            {
                return new CaptureResult("Beauty", ex.Message);
            }
        }

        /// <summary>
        /// Captures a depth map of the current viewport.
        /// Placeholder implementation - returns a simple gradient for now.
        /// Proper raycast implementation should be added with correct intersection APIs.
        /// </summary>
        /// <param name="options">The capture options.</param>
        /// <returns>The capture result.</returns>
        public CaptureResult CaptureDepth(CaptureOptions options)
        {
            try
            {
                var doc = RhinoDoc.ActiveDoc;
                if (doc == null) return new CaptureResult("Depth", "No active document");

                var view = doc.Views.ActiveView;
                if (view == null) return new CaptureResult("Depth", "No active view");

                RhinoApp.WriteLine("[CaptureDepth] Generating placeholder depth map...");

                // Create a simple gradient bitmap as placeholder
                var bitmap = new Bitmap(options.Width, options.Height);

                for (int y = 0; y < options.Height; y++)
                {
                    for (int x = 0; x < options.Width; x++)
                    {
                        // Simple gradient from black to white
                        int grayValue = (int)(255.0 * x / options.Width);
                        var color = Color.FromArgb(grayValue, grayValue, grayValue);
                        bitmap.SetPixel(x, y, color);
                    }
                }

                var result = new CaptureResult(bitmap, "Depth");

                if (!string.IsNullOrEmpty(options.OutputDirectory))
                {
                    var filePath = Path.Combine(options.OutputDirectory, $"depth.{options.Format.ToLower()}");
                    result.Save(filePath);
                }

                RhinoApp.WriteLine("[CaptureDepth] Placeholder depth capture completed. Implement raycast for actual depth.");
                return result;
            }
            catch (System.Exception ex)
            {
                RhinoApp.WriteLine("[CaptureDepth] Error during depth capture: " + ex.Message);
                return new CaptureResult("Depth", ex.Message);
            }
        }

        /// <summary>
        /// Captures a segmentation mask of the current viewport.
        /// This implementation attempts a robust per-object override: force object color to come from layer and neutralize materials,
        /// capture, then restore original attributes.
        /// </summary>
        /// <param name="options">The capture options.</param>
        /// <returns>The capture result.</returns>
        public CaptureResult CaptureSegmentation(CaptureOptions options)
        {
            try
            {
                var doc = RhinoDoc.ActiveDoc;
                if (doc == null) return new CaptureResult("Segmentation", "No active document");

                var view = doc.Views.ActiveView;
                if (view == null) return new CaptureResult("Segmentation", "No active view");

                var viewport = view.ActiveViewport;
                var originalMode = viewport.DisplayMode;

                // Collect objects to override
                var rhObjects = new List<RhinoObject>();
                foreach (var ro in doc.Objects)
                {
                    if (ro != null) rhObjects.Add(ro);
                }

                // Save original attributes
                var originalAttributes = new Dictionary<Guid, ObjectAttributes>();
                foreach (var ro in rhObjects)
                {
                    if (ro == null) continue;
                    originalAttributes[ro.Id] = ro.Attributes.Duplicate();
                }

                try
                {
                    RhinoApp.WriteLine($"[CaptureSegmentation] Overriding {originalAttributes.Count} objects to use layer colors.");

                    // Apply overrides: force color from layer
                    foreach (var kv in originalAttributes)
                    {
                        var id = kv.Key;
                        var attr = kv.Value.Duplicate();

                        // Force color to come from layer
                        attr.ColorSource = ObjectColorSource.ColorFromLayer;
                        // We avoid setting MaterialSource to unknown enum values; color-from-layer should be sufficient for display color

                        // Apply attributes to object
                        doc.Objects.ModifyAttributes(id, attr, true);
                    }

                    // Try to use a shaded base display mode to capture flat colors (lighting may still affect appearance)
                    var segmentationMode = DisplayModeDescription.FindByName("Shaded") ?? DisplayModeDescription.FindByName("Rendered");
                    if (segmentationMode != null && segmentationMode.Id != originalMode.Id)
                    {
                        viewport.DisplayMode = segmentationMode;
                    }

                    EnsureRedrawAndWait(view, 300);

                    var bitmap = view.CaptureToBitmap(new Size(options.Width, options.Height));

                    var result = new CaptureResult(bitmap, "Segmentation");

                    if (!string.IsNullOrEmpty(options.OutputDirectory))
                    {
                        var filePath = Path.Combine(options.OutputDirectory, $"segmentation.{options.Format.ToLower()}");
                        result.Save(filePath);
                    }

                    return result;
                }
                finally
                {
                    // Restore original attributes
                    RhinoApp.WriteLine("[CaptureSegmentation] Restoring original object attributes.");
                    foreach (var kv in originalAttributes)
                    {
                        var id = kv.Key;
                        var attr = kv.Value;
                        doc.Objects.ModifyAttributes(id, attr, true);
                    }

                    // Restore display mode
                    if (viewport != null && originalMode != null)
                    {
                        viewport.DisplayMode = originalMode;
                        EnsureRedrawAndWait(view, 150);
                        RhinoApp.WriteLine("[CaptureSegmentation] Restored display mode to: " + originalMode.EnglishName);
                    }
                }
            }
            catch (System.Exception ex)
            {
                return new CaptureResult("Segmentation", ex.Message);
            }
        }

        /// <summary>
        /// Captures all three types (beauty, depth, segmentation).
        /// </summary>
        /// <param name="options">The capture options.</param>
        /// <returns>An array of capture results.</returns>
        public CaptureResult[] CaptureAll(CaptureOptions options)
        {
            return new[]
            {
                CaptureBeauty(options),
                CaptureDepth(options),
                CaptureSegmentation(options)
            };
        }
    }
}
