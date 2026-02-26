# G8 — Content Pipeline & Asset Workflow
> **Category:** Guide · **Related:** [R1 Library Stack](../R/R1_library_stack.md) · [R3 Project Structure](../R/R3_project_structure.md)

---

## MonoGame Content Builder (MGCB)

Compiles assets at build time:
- Textures → .xnb
- Audio → .xnb
- Effects → compiled shaders

MonoGame.Extended.Content.Pipeline adds importers for:
- Tiled .tmx maps
- Texture atlases
- Aseprite files
- Bitmap fonts

---

## Recommended Art Pipelines

### Pixel Art (Aseprite)
```
Aseprite → .aseprite → MonoGame.Aseprite importer → SpriteSheet + AnimationController
```
MonoGame.Aseprite v6.3.1 handles sprite sheets and animation directly from .ase/.aseprite files, including frame durations, tags, and layers.

### Pixel Art (Manual Export)
```
Aseprite → export spritesheet PNG + JSON → TextureAtlas via content pipeline
```

### Level Design (Tiled)
```
Tiled → .tmx → MonoGame.Extended Tiled loader → TiledMap object
```
Supports orthographic and isometric maps.

### Custom Data
```
Custom tools → .json level data → System.Text.Json deserialize → custom level objects
```
Store in `Resources/` folder (not compiled by MGCB, loaded at runtime).

---

## Font Pipeline

**Build-time (not recommended):** MonoGame SpriteFont — limited sizes, ugly compression.

**Runtime (recommended):** FontStashSharp loads .ttf/.otf at any size on demand, generates glyph atlases automatically. Superior quality.

**Install:** `dotnet add package FontStashSharp.MonoGame --version 1.3.7`

### Cross-Platform Font Loading

**Do NOT use system font paths** (`File.ReadAllBytes("/System/Library/Fonts/...")`). This fails on iOS due to sandbox restrictions, and system fonts differ across platforms.

**Pattern:** Bundle a .ttf via MGCB `/copy`, load with `TitleContainer.OpenStream()`:

**1. Add to Content project (.mgcb):**
```
#begin fonts/JetBrainsMono-Regular.ttf
/copy:fonts/JetBrainsMono-Regular.ttf
```

**2. Load at runtime (works on Desktop, iOS, Android):**
```csharp
FontSystem fontSystem = new FontSystem();

using Stream fontStream = TitleContainer.OpenStream(
    Path.Combine("Content", "fonts", "JetBrainsMono-Regular.ttf"));
using MemoryStream ms = new();
fontStream.CopyTo(ms);
fontSystem.AddFont(ms.ToArray());

DynamicSpriteFont font = fontSystem.GetFont(24);
```

**Why this works:** `TitleContainer.OpenStream()` reads from the app bundle on iOS, the content directory on Desktop. MGCB `/copy` embeds the file without XNB compilation. FontStashSharp's `AddFont()` takes `byte[]`.

**FontStashSharp gotcha:** `.ttc` (TrueType Collection) files are NOT supported — causes `stbtt_InitFont failed`. Use individual `.ttf` files (JetBrains Mono, Roboto, etc.).

---

## Content Folder Layout

```
Content/
├── sprites/          # Sprite sheets, individual sprites
├── tilemaps/         # .tmx Tiled map files
├── shaders/          # Custom HLSL .fx files
├── fonts/            # .ttf/.otf font files (for FontStashSharp)
└── audio/            # Sound effects and music
```

**Resources/ (runtime data, separate from Content/):**
```
Resources/
├── items.json        # Item database
├── dialogue/         # Dialogue tree JSON files
├── levels/           # Level definition JSON files
└── waves/            # Wave/spawn data JSON files
```

---

## iOS Content Pipeline

iOS projects reference Core's Content folder via `MonoGameContentReference` in the .csproj — identical to Desktop:

```xml
<MonoGameContentReference Include="..\MyGame.Core\Content\MyGame.mgcb" />
```

MGCB compiles content at build time and embeds it in the iOS app bundle. Runtime content loading is identical across platforms:

```csharp
Texture2D sprite = Content.Load<Texture2D>("sprites/player");
```

No iOS-specific content loading code is needed. See [R3 Project Structure](../R/R3_project_structure.md) for the full iOS .csproj.
