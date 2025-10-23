📘 Project Title

Rhino AI Render — Architectural Visualization Plug-in
(RhinoCommon + Stable Diffusion img2img + GPT-4o Multimodal)

🎯 Objective

Create a RhinoCommon C# plug-in that transforms the active viewport into an intelligent rendering interface.
The system captures the viewport as:

a beauty render (current view),

a depth map, and

a segmentation map,

then uses GPT-4o (text + image input) to refine the user’s prompt and send all assets to a Stable Diffusion ControlNet API for final generation.
The rendered result is streamed and composited directly over the Rhino viewport.

🧩 Feature Summary
Module	Purpose
Capture System	Generate beauty, depth, and segmentation images from the active viewport.
GPT-4o Prompt Enhancer	Rewrite the user prompt using vision + style presets + segmentation legend.
Stable Diffusion Client	Interface to cloud img2img endpoint with ControlNet Depth + Seg.
Viewport Overlay	Stream progress frames + show final image overlay with interactivity.
Style System	Dynamic /styles folder of baked system prompts selectable at runtime.
ADE20K Mapper (GPT-only)	Map Rhino layer names to ADE20K classes/colors via GPT-4o API; cache results.
Settings Manager	Persist API keys, defaults, style, and overlay preferences.
🧱 Core Architecture
RhinoAiRender/
├── RhinoAiRenderPlugIn.cs
├── Commands/
│   ├── AiRenderViewCommand.cs
│   ├── AiSettingsCommand.cs
│   └── AiRetryCommand.cs
├── UI/
│   ├── PromptDialog.cs
│   └── ProgressOverlayConduit.cs
├── Capture/
│   ├── CaptureBundle.cs
│   ├── DepthCapture.cs
│   ├── SegmentationCapture.cs
│   ├── ScopedDisplayMode.cs
│   └── Ade20kMapper.cs
├── Net/
│   ├── PromptEnhancer.cs
│   ├── StableDiffusionClient.cs
│   ├── SettingsManager.cs
│   ├── GptMapper.cs
│   └── ApiSettings.cs
└── Util/
    ├── ImageUtils.cs
    ├── Ade20kCache.cs
    ├── JsonUtils.cs
    └── Logger.cs

🧰 Functional Specifications
1️⃣ Viewport Capture

Inputs

Active viewport camera, clipping planes, render mode.

Outputs

Map	Description	Format
beauty.png	rendered/shaded color pass	24-bit PNG
depth.png	normalized Z-buffer (white = far)	8-bit grayscale
segmentation.png	per-layer color (ADE20K)	24-bit flat

Implementation notes

Use DisplayPipelineAttributes overrides or custom DisplayModeDescription.

Disable edges & lighting for segmentation capture.

Save temp files under %AppData%\RhinoAiRender\tmp.

2️⃣ GPT-4o Semantic ADE20K Remapping (🔥 GPT-only)
Purpose

Automatically translate arbitrary Rhino layer names into ADE20K segmentation classes & colors using GPT-4o semantic understanding.

Workflow

Collect all layer names in the document.

Load cache (ade20k_layer_map.json).

Identify layers not in cache.

Send only unknown names to GPT-4o.

Receive structured JSON → merge → save.

Use cached RGBs to render segmentation map.

GPT Request
{
 "model": "gpt-4o",
 "messages": [
  {
   "role": "system",
   "content": "You map 3D-model layer names to ADE20K segmentation classes. Return JSON: [{layer_name, ade20k_class, rgb}]. Use [0,0,0] for unknowns."
  },
  {
   "role": "user",
   "content": "Layers: [\"Glass Walls\",\"Railing_Guard\",\"Ceiling Grid\"]"
  }
 ]
}


Example Response

[
 {"layer_name":"Glass Walls","ade20k_class":"window","rgb":[190,153,153]},
 {"layer_name":"Railing_Guard","ade20k_class":"fence","rgb":[153,153,153]},
 {"layer_name":"Ceiling Grid","ade20k_class":"ceiling","rgb":[204,204,204]}
]


Cache Structure

{
 "Walls":{"ade20k_class":"wall","rgb":[120,120,120]},
 "Glass Walls":{"ade20k_class":"window","rgb":[190,153,153]},
 "Landscape":{"ade20k_class":"grass","rgb":[152,251,152]}
}


Key Features

Incremental updates (only new/renamed layers).

Robust JSON validation & merge.

Cached locally → offline safe.

Zero hardcoded keyword logic.

Settings

{
 "ade20k_mapping_mode": "gpt4o",
 "mapping_cache_path": "%AppData%/RhinoAiRender/ade20k_layer_map.json",
 "default_color": [0,0,0]
}

3️⃣ GPT-4o Multimodal Prompt Enhancement
Purpose

Use GPT-4o to rewrite the user’s prompt in the selected style, informed by three images (beauty, depth, segmentation).

Payload
{
 "model": "gpt-4o",
 "messages": [
  {
   "role": "system",
   "content": "<contents of selected style .txt>"
  },
  {
   "role": "user",
   "content": [
     {"type": "text", "text": "User prompt: curved timber sauna interior"},
     {"type": "image_url","image_url":"data:image/png;base64,<beauty>"},
     {"type": "image_url","image_url":"data:image/png;base64,<depth>"},
     {"type": "image_url","image_url":"data:image/png;base64,<segmentation>"}
   ]
  }
 ]
}

Output

Single string — enhanced architectural rendering prompt.

Capabilities

Understands geometry, material, & lighting from images.

Integrates segmentation semantics (“cyan → windows”).

Applies style personality from selected preset.

Describes scene vividly + coherently for Stable Diffusion.

Fallback: text-only mode if image upload unsupported.

4️⃣ Style Preset System (/styles Folder)
File	Function
interiors.txt	sculptural interiors, warm lighting
exterior.txt	expressive exteriors & landscape context
furniture.txt	object/product render focus
sketch.txt	conceptual/linework/clay aesthetics

Behavior

Created automatically on first run.

Dropdown in PromptDialog.

Auto-reloads when new .txt files appear (FileSystemWatcher).

Text inside each file acts as GPT system prompt.

Editable without recompiling.

SettingsManager remembers last selection.

5️⃣ Stable Diffusion Cloud Integration
Endpoint

Any REST API supporting img2img + ControlNet (Depth + Segmentation)
e.g. Segmind, Stability AI, Replicate.

Payload
{
 "model": "sdxl-1.0-controlnet-depth-seg",
 "prompt": "<GPT-enhanced prompt>",
 "image": "<beauty_base64>",
 "control_images": [
   {"type":"depth","image":"<depth_base64>"},
   {"type":"segmentation","image":"<segmentation_base64>"}
 ],
 "strength":0.6,
 "guidance":7.5,
 "seed":42
}

Streaming / Polling

Poll or stream progress images.

Show via ProgressOverlayConduit.

Final image saved to temp folder.

Option to bake as PictureFrame in viewport.

6️⃣ Viewport Overlay System

Draw intermediate & final render frames.

Adjustable opacity slider.

Command-line controls:

S) Save R) Retry X) Exit


Non-blocking async UI updates.

Fade-in/out on completion.

7️⃣ Settings & Persistence

Stored in %AppData%\RhinoAiRender\settings.json

Keys → secure storage if possible.

Persistent fields:

{
 "openai_api_key": "...",
 "diffusion_api_key": "...",
 "last_style": "interiors",
 "default_model": "sdxl-1.0-controlnet-depth-seg",
 "default_strength": 0.65,
 "default_guidance": 7.5
}

🧭 Deployment Phases
Phase 1 – Project Scaffolding & Capture

Create plugin skeleton + commands.

Implement beauty/depth/segmentation capture.

Verify image exports + res control.

Phase 2 – GPT-4o Prompt Enhancer

Build multimodal request handler.

Input: text + 3 images + style prompt.

Output: enriched prompt.

Add fallback path.

Phase 3 – Stable Diffusion Client

Implement cloud API requests.

Stream progress images.

Overlay updates.

Handle save/retry/exit.

Phase 4 – Settings Manager

Store API keys, defaults, style.

Eto.Forms settings UI.

Phase 5 – UI Polish

Add progress bar, opacity slider.

Robust error handling.

Maintain async non-blocking.

Phase 6 – Documentation & Packaging

Write README, API Reference, User Guide.

Build .rhp for Rhino 8.

Phase 7 – Optional Enhancements

Material-ID mode

Batch named-views

Local ComfyUI backend

Segmentation editing overlay

User style learning

Phase 8 – GPT-4o ADE20K Remapping System

GPT-4o handles all semantic layer-to-class mapping.
No keyword matching. Cache incrementally updated.

Deliverables

Ade20kMapper.cs → handles API calls.

Ade20kCache.cs → persistent storage.

GptMapper.cs → formats prompts, parses JSON.

Phase 9 – Dynamic Style System

/styles folder + dropdown menu for style selection.
Auto-detect new files on the fly.

Phase 10 – Final QA & Release

End-to-end validation (Windows / macOS).

Stress-test prompt generation and overlay.

Deliver signed .rhp.

🧠 Default Style Prompt Templates (text files)
interiors.txt
You create prompts for sculptural, atmospheric interior visualizations.
Emphasize carved forms, warm timber, polished concrete, and cinematic
daylight or ambient dusk light. Maintain architectural realism and emotion.

exterior.txt
Generate prompts for expressive building exteriors integrated with landscape.
Describe bold geometry, contrast lighting, and photographic atmosphere.
Avoid surreal distortion; preserve credible context and material detail.

furniture.txt
Craft prompts for product and furniture visualization.
Highlight craftsmanship, material richness, and elegant studio lighting.
Focus on object scale; minimize environment distractions.

sketch.txt
Describe architectural concepts as if presented in a clay or pen sketch.
Focus on form and composition over realism; reference linework, graphite,
and concept rendering textures.

⚙️ Technical Notes for Cline
Category	Spec
Language	C#
Framework	.NET 8
SDK	RhinoCommon (Rhino 8)
UI Toolkit	Eto.Forms
Async Pattern	async/await, UI thread safe
APIs	OpenAI GPT-4o (multi-image) + Stable Diffusion img2img
Data Storage	JSON (caches + settings)
Compatibility	Windows & macOS Rhino 8
Error Handling	Try/catch + non-blocking logging
Logging	RhinoApp.WriteLine + file log in /logs