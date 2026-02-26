# G2 — Rendering & Graphics
> **Category:** Guide · **Related:** [G1 Custom Code Recipes](./G1_custom_code_recipes.md) · [R2 Capability Matrix](../R/R2_capability_matrix.md) · [G8 Content Pipeline](./G8_content_pipeline.md) · [G27 Shaders & Visual Effects](./G27_shaders_and_effects.md)

---

## Render Pipeline Overview

With Nez removed, the rendering stack is:

- **MonoGame SpriteBatch** — core sprite drawing
- **Custom render layer system** — ordered layers with optional per-layer post-processing → [G1](./G1_custom_code_recipes.md)
- **MonoGame.Extended OrthographicCamera** — transformation matrix, zoom, rotation
- **Custom post-processor pipeline** — chain of HLSL shader effects → [G1](./G1_custom_code_recipes.md)

---

## Sprite Rendering

**MonoGame.Aseprite v6.3.1** handles sprite sheets and animation directly from .ase/.aseprite files. See [G8 Content Pipeline](./G8_content_pipeline.md) for import setup.

**MonoGame.Extended** also provides `SpriteSheet` and `AnimatedSprite` classes with frame-based animation as an alternative.

Sprite features: static sprites, animated sprites, scrolling/repeating backgrounds, sprite trails (render previous positions at decreasing opacity).

---

## Post-Processor Shaders

Each is an HLSL .fx file compiled via MGFXC (cross-compiled to GLSL for OpenGL). The pattern is always: render scene to RenderTarget2D → apply shader → draw result to screen.

| Effect | HLSL Lines | Description |
|--------|-----------|-------------|
| Bloom | ~80 | Threshold bright pixels → Gaussian blur → additive blend |
| Vignette | ~15 | Distance-from-center darkening |
| Gaussian blur | ~40 | Two-pass separable filter (horizontal then vertical) |
| Scanlines | ~10 | Sine wave modulation over screen |
| CRT / chromatic aberration | ~30 | Channel offset + barrel distortion |
| Palette swap | ~15 | Lookup table texture remap |
| Dissolve | ~20 | Noise texture threshold discard |
| Outline | ~25 | Edge detection on alpha channel |
| Heat haze | ~10 | Sine-wave UV distortion near heat sources |
| Flash white | ~5 | Lerp to premultiplied white for damage indicator |
| Screen flash | ~5 | Additive color overlay with decay |
| Shockwave | ~15 | Expanding ring UV displacement from impact point |

Complete HLSL implementations for all effects above, plus elemental effects (fire, water, wind, earth, lightning, ice) and performance guidance, are in [G27 Shaders & Visual Effects](./G27_shaders_and_effects.md).

---

## Deferred 2D Lighting

This is the most complex visual feature. Three options:

### Option A: MonoGame.Penumbra (WindowsDX + DesktopGL)
- Mature 2D soft shadow library for MonoGame
- Point lights, hull-based shadow casting
- **Install:** `dotnet add package MonoGame.Penumbra.DesktopGL` (or `.WindowsDX`)
- No longer in active development, but bugs are still fixed

### Option B: Custom Deferred Lighting (~400-600 lines)
- Render normal maps and a light accumulation buffer
- For each light: draw a fullscreen quad with a lighting shader that reads the normal map and computes diffuse contribution
- Composite light buffer with albedo buffer
- This is two render targets and a multiply blend — not magic

### Option C: Forward Lighting with Light Maps (~200 lines)
- Simpler: pre-bake or runtime-compute a light intensity texture
- Apply as a multiplicative overlay
- Good enough for most 2D games that don't need per-pixel normal-mapped lighting

**Recommendation:** Unless your game specifically needs normal-mapped deferred lighting, forward lighting with a simple light accumulation pass covers 90% of 2D use cases in ~200 lines. If you need the full deferred pipeline, budget 2-3 days.

---

## Camera

MonoGame.Extended v5.3.1 `OrthographicCamera` provides:
- Position, zoom, rotation
- World-to-screen and screen-to-world coordinate conversion
- Transformation matrix for SpriteBatch.Begin()
- Viewport bounds queries

**Add your own** (minimal custom code):
- Follow logic (~30 lines): lerp camera position toward target with deadzone
- Screen shake (~20 lines): offset camera by Perlin noise * intensity, decay over time
- Dead zones (~15 lines): only move camera when target exits inner rectangle

---

## Parallax Scrolling

MonoGame.Extended supports multi-layer parallax. The concept: draw background layers at reduced scroll rates relative to the camera. Layer 0 (farthest) moves slowest; foreground layers move at full camera speed.
