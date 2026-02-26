# E8 — MonoGameStudio Post-Mortem
> **Category:** Explanation · **Related:** [G29 Game Editor](../G/G29_game_editor.md) · [E1 Architecture Overview](../E/E1_architecture_overview.md) · [E4 Solo Project Management](../E/E4_project_management.md)

---

**MonoGameStudio was a 2D game editor built with MonoGame + Arch ECS + ImGui. It reached 134 files (67 Core, 66 Editor, 1 Desktop) covering v0.1–v0.9 of its roadmap before being deliberately stopped.** The project validated the tech stack described in the Universal 2D Engine Toolkit docs, produced battle-tested implementation patterns captured in [G29](../G/G29_game_editor.md), and taught a critical lesson about tool-building traps. This post-mortem preserves the full decision history, architecture, and file map so the knowledge survives source deletion.

---

## What was built

### Stats
- **134 source files** across 3 projects (Core, Editor, Desktop)
- **24 ECS component types** (transforms, hierarchy, rendering, physics, audio, particles, materials, metadata, tags, UI)
- **11 systems** (transform propagation, sprite rendering, animation, camera, particles, tilemaps, scene management, screen transitions, timers, tweens, Gum UI)
- **17 ImGui editor panels** (hierarchy, inspector, viewport, console, asset browser, sprite sheet, animation, start screen, settings, game run, particle editor, post-process, shader preview, collision matrix, tilemap editor, toolbar, menu bar)
- **13 undo/redo command types** (move, create, delete, duplicate, rename, transform, modify component, add/remove component, paint tile, paint tiles, fill tiles)
- Development period: February 2026

### Feature inventory

**Core engine (zero editor dependencies)**:
- Full Arch ECS integration with entity CRUD, hierarchy, parent/child with circular-parenting prevention
- Scene serialization — two-pass JSON with GUID-based parent linking, generic for any registered component
- Component descriptor system — typed Has/Get/Set/Add/Remove without reflection, plus dynamic descriptors for user-defined components from game assemblies
- Physics (PhysicsWorld2D, collision layer matrix, tile collision generation)
- Particles (emitter runtime, object pooling)
- Post-processing pipeline, material system, render layer configuration
- Sprite sheet and animation data formats (`.spritesheet.json`, `.animation.json`)
- Asset caching (textures, audio, effects) with path-based lookup
- Scene manager with transitions, tween system with easing, timer system
- Atlas packing, texture import settings, build profiles
- Logging (static circular buffer, 1024 max, `OnLog` event)

**Editor (ImGui overlay on MonoGame window)**:
- Full docking layout with profile save/load
- Viewport rendering with camera pan/zoom, grid, entity markers
- Gizmo tools (move, rotate, scale) with hit detection and drag
- Selection system (click, box select, Ctrl multi-select)
- 100-deep undo/redo stack with 13 command types
- Play mode (serialize scene → snapshot, stop → restore)
- Asset browser with folder tree, grid/list view, search, type filters, thumbnails, FileSystemWatcher auto-refresh, drag-drop
- Sprite sheet editor (texture preview, auto-slice, frame editing, zoom)
- Animation editor (timeline, clip tabs, playback preview)
- Start screen with new project wizard, recent projects, and templates (Empty, 2D Platformer, Top-Down RPG)
- Project management (.mgstudio files, `dotnet new mgdesktopgl` scaffolding)
- Inspector with component drawer, field widgets, Add Component picker with category search
- Copy/paste entities, prefab system (.prefab.json), multi-select transform
- Tilemap editor (palette, paint/erase/fill, auto-tiling, Tiled .tmx import)
- Particle editor, shader preview, post-process stack, collision matrix editor
- macOS native integration (ObjC interop for menu bar, title bar, file dialogs, toolbar)
- Hot reload (FileSystemWatcher for assembly changes)
- Build & run game from editor with console output

### Tech stack

| Component | Choice | Notes |
|-----------|--------|-------|
| Runtime | .NET 10 (C# 13) | AllowUnsafeBlocks, Nullable, ImplicitUsings enabled |
| Framework | MonoGame.Framework.DesktopGL 3.8.x | Cross-platform OpenGL backend |
| ECS | Arch 2.1.0 | Fast, C#-native, struct components |
| ImGui | Hexa.NET.ImGui 2.2.9 | Migrated from ImGui.NET — better .NET 10 support, native docking |
| Theming | ktsu.ImGuiStyler 1.3.12 | Catppuccin.Mocha theme + runtime theme selector |
| Game UI | MonoGameGum (Gum.MonoGame 2026.2.*) | GumScreen component for in-game UI |
| Serialization | System.Text.Json | Scene files, project files, sprite/animation data |
| Fonts | Inter (UI), JetBrains Mono (console), FontAwesome 6 (icons) | DPI-aware loading, icon merge |

---

## What worked well

### ImGui for editor tools
ImGui (via Hexa.NET.ImGui) was the right call. The immediate-mode paradigm made rapid iteration trivial — add a panel, see it instantly. Docking came free. The learning curve was gentle. Industry-proven by Blizzard, Rockstar, id Software, Valve for internal tools. The Hexa.NET binding was better maintained than ImGui.NET for modern .NET.

### Component descriptor pattern
The `ComponentDescriptor<T>` system — providing typed component access without reflection — was the cleanest architectural win. It made the inspector, serialization, and Add Component picker all generic: register a new component type once, and it appears everywhere automatically. `FieldDescriptor` with compiled delegates gave near-zero-overhead field access.

### Two-pass scene serialization
The GUID-based two-pass serialization pattern (create all entities first, link parents second) was simple and bulletproof. No ordering dependencies, no special cases for new entity types. System.Text.Json was fast enough and produced human-readable, git-diffable files.

### Core/Editor separation
Strict rule: Core never references Editor. This was enforced from day one and never violated. It meant the Core library was genuinely reusable — a game could use it without any editor code compiled in.

### macOS native integration via raw ObjC interop
Direct `objc_msgSend` P/Invoke calls to Cocoa APIs (NSOpenPanel, NSMenu, NSWindow) worked flawlessly without any binding framework. Simpler than Xamarin/MAUI, zero dependencies, and the interface abstraction (`IFileDialogService`) kept platform code isolated.

---

## What didn't work / lessons learned

### The SwiftUI/NativeAOT trap (2026-02-24)
Mid-project, attempted to rearchitect with a SwiftUI native wrapper around a NativeAOT-compiled MonoGame core. Created MonoGameStudio.Native (NativeAOT) and MonoGameStudio.macOS (Swift/SwiftUI). This was a classic tool-building trap:
- NativeAOT + MonoGame had unresolved interop issues
- SwiftUI layout for a game editor viewport was fighting the framework
- The whole effort was rebuilding something that already worked (ImGui docking)
- Time spent: ~1 day before recognizing the trap and reverting

**Lesson**: If the current tool works, don't rebuild it in a shinier technology. ImGui is ugly but functional. SwiftUI is pretty but wrong for this use case.

### Scope creep through roadmap completion
The roadmap (v0.1–v0.9) was meant to be built incrementally as game development demanded features. Instead, the entire roadmap was implemented in a concentrated burst without building any actual game. This produced a comprehensive editor that was never validated against real game development needs.

**Lesson**: Build editor features only when a specific game development pain point demands them. The doc G29 says this explicitly ("the goal is making games, not making engines") — but it's easy to ignore when editor development feels productive.

### 134 files for a tool nobody uses
The editor was feature-complete but had zero users (including its author — no game was built with it). Every feature worked in isolation but the workflow was never stress-tested against actual game development. Some features (particle editor, shader preview, post-processing stack) may have been unnecessary for the first game.

**Lesson**: Ship the game first. Build tools when they save more time than they cost.

---

## The key decision: stop building tools, start making games

On 2026-02-25, the decision was made to:
1. Capture all MonoGameStudio knowledge into the Universal 2D Engine Toolkit docs
2. Delete the MonoGameStudio source code
3. Shift focus to FireStarter (the actual game project)

The editor validated the tech stack and produced valuable implementation patterns. But continuing to build it was a trap — polishing tools instead of shipping games. The knowledge is preserved in:
- [G29 — Game Editor](../G/G29_game_editor.md): Implementation notes section with all battle-tested patterns
- This post-mortem (E8): Full feature inventory, decision log, architecture reference

If a visual editor is needed in the future, it can be rebuilt from these docs in a fraction of the original time — but only when a specific game demands it.

---

## Decision log

| Date | Decision | Rationale |
|------|----------|-----------|
| 2026-02-25 | Stop building editor, capture knowledge, delete source | Tool-building trap. No game built. Focus on FireStarter. |
| 2026-02-25 | Docs audit — STATUS.md + CLAUDE.md updated to reflect 134 files | Codebase had grown far beyond original docs. |
| 2026-02-25 | v0.1 feature-complete, batch push through v0.2–v0.9 | Concentrated implementation burst. |
| 2026-02-24 | Abandoned SwiftUI/NativeAOT rearchitecture | Tool-building trap within a tool-building trap. Deleted MonoGameStudio.Native and MonoGameStudio.macOS. |
| 2026-02-24 | Chose Hexa.NET.ImGui over ImGui.NET | Active development, better .NET 10 support, native docking/tables. |
| 2026-02-24 | Chose Arch ECS 2.1.0 | Fast, well-maintained, C#-native. Custom EntityRef for safe entity storage. |

---

## Complete file map (preserved from CLAUDE.md)

This is the full architecture reference — 134 files organized by project and feature area. Preserved here so the architecture survives source deletion.

### Core (67 files)

#### Components (12 files, 24 component types)
| File | Purpose |
|------|---------|
| `Components/Transform.cs` | Position, Rotation, Scale, LocalTransform, WorldTransform (5 structs) |
| `Components/Hierarchy.cs` | EntityRef, Parent, Children |
| `Components/EntityMetadata.cs` | EntityName, EntityGuid |
| `Components/Tags.cs` | SelectedTag, EditorOnlyTag, EntityTag |
| `Components/Rendering.cs` | SpriteRenderer, Animator, Camera2D, TilemapRenderer |
| `Components/Physics.cs` | BoxCollider, CircleCollider, RigidBody2D |
| `Components/Audio.cs` | AudioSource |
| `Components/Particles.cs` | ParticleEmitter |
| `Components/Material.cs` | MaterialComponent |
| `Components/GumScreen.cs` | GumScreen (Gum UI integration) |
| `Components/ComponentCategory.cs` | `[ComponentCategory("...")]` attribute |
| `Components/GameComponentAttribute.cs` | Attribute for marking game components |

#### Systems (11 files)
| File | Purpose |
|------|---------|
| `Systems/TransformPropagationSystem.cs` | Recursive Local→World transform |
| `Systems/SpriteRenderingSystem.cs` | Query SpriteRenderer + Position, sort by SortOrder, draw |
| `Systems/AnimationSystem.cs` | Advance frame timer, update SourceRect |
| `Systems/GumUISystem.cs` | Load/activate Gum screens |
| `Systems/CameraSystem.cs` | Follow target, deadzone, lookahead |
| `Systems/ParticleSystem.cs` | Emission, update, rendering |
| `Systems/TilemapRenderingSystem.cs` | Tilemap layer rendering |
| `Systems/SceneManager.cs` | Scene load/unload/transition |
| `Systems/ScreenTransitionSystem.cs` | Fade, slide transitions |
| `Systems/TimerSystem.cs` | Delayed/repeating actions |
| `Systems/TweenSystem.cs` | Property tweening with easing |

#### Serialization (11 files)
| File | Purpose |
|------|---------|
| `Serialization/ComponentRegistry.cs` | Type registry with categories |
| `Serialization/ComponentDescriptor.cs` | Generic typed Has/Get/Set/Add/Remove |
| `Serialization/IComponentDescriptor.cs` | Descriptor interface |
| `Serialization/DynamicComponentDescriptor.cs` | Runtime descriptor for user components |
| `Serialization/FieldDescriptor.cs` | Field metadata + compiled get/set delegates |
| `Serialization/ComponentRegistrations.cs` | Built-in registration definitions |
| `Serialization/SceneSerializer.cs` | JSON save/load via descriptors |
| `Serialization/SceneData.cs` | Scene DTOs |
| `Serialization/PrefabSerializer.cs` | Prefab save/load (`.prefab.json`) |
| `Serialization/ExternalComponentLoader.cs` | Load user components from assemblies |
| `Serialization/JsonConverters.cs` | Custom System.Text.Json converters |

#### Data (15 files)
| File | Purpose |
|------|---------|
| `Data/EditorMode.cs` | Edit, Play, Pause enum |
| `Data/ApplicationPhase.cs` | StartScreen, Editor enum |
| `Data/SpriteSheetData.cs` | SpriteSheetDocument, SpriteFrame |
| `Data/AnimationData.cs` | AnimationDocument, AnimationClip |
| `Data/TilemapData.cs` | Tilemap layer/tile structures |
| `Data/AutoTileRules.cs` | Bitmask auto-tiling |
| `Data/TiledImporter.cs` | Tiled `.tmx` import |
| `Data/ParticleData.cs` | Particle emitter config |
| `Data/MaterialData.cs` | Material/shader parameters |
| `Data/PostProcessStackData.cs` | Post-processing stack config |
| `Data/BuildProfileData.cs` | Build profiles |
| `Data/RenderLayerConfig.cs` | Sort layer config |
| `Data/Easing.cs` | Easing functions |
| `Data/AtlasData.cs` | Texture atlas format |
| `Data/TextureImportSettings.cs` | Sprite import settings |

#### Assets (4 files)
TextureCache, AudioCache, EffectCache (path-based loading + caching), AtlasPacker (combine loose sprites).

#### Physics (4 files)
PhysicsSystem, PhysicsWorld2D, CollisionLayerSettings, TileCollisionGenerator.

#### Particles (3 files)
Particle struct, ParticleEmitterRuntime, ParticlePool.

#### Other Core
PostProcessorPipeline (1 file), ProjectInfo/RecentProject/ProjectSerializer (3 files), WorldManager (1 file), GumUIManager (1 file), Log.cs (1 file).

### Editor (66 files)

#### Editor Core (5 files)
EditorGame (main Game class), EditorState (mode, selection, visibility), EditorPreferences, PlayModeManager (snapshot/restore), ShortcutManager.

#### Commands (6 files, 13 command types)
ICommand interface, CommandHistory (undo/redo stacks, 100 max), EntityCommands (Move, Create, Delete, Rename, Transform, MoveMultiple, Duplicate, ModifyComponent\<T\>), ComponentCommands (Add, Remove), TilemapCommands (PaintTile, PaintTiles, FillTiles), ClipboardManager (copy/paste).

#### Panels (18 files, 17 panels)
MenuBar, Toolbar, SceneHierarchy, Inspector, GameViewport, Console, AssetBrowser, SpriteSheet, Animation, StartScreen, Settings, GameRun, ParticleEditor, PostProcess, ShaderPreview, CollisionMatrix, TilemapEditor + TilemapEditorTool.

#### ImGui (4 files)
ImGuiManager (context, fonts, theming, DPI), ImGuiRenderer (Hexa.NET.ImGui render backend for MonoGame/OpenGL), DrawVertDeclaration, FontAwesomeIcons.

#### Layout (3 files)
DockingLayout (dockspace), LayoutDefinitions (defaults), LayoutProfileManager (save/load).

#### Gizmos & Visualization (6 files)
GizmoManager, GizmoRenderer, SelectionSystem, ColliderVisualization, AudioVisualization, PhysicsDebugOverlay.

#### Inspector (3 files)
ComponentDrawer (field reflection + ImGui controls), FieldDrawers (type→widget dispatch), ComponentPicker (searchable Add Component popup).

#### Viewport (4 files)
EditorCamera (pan/zoom), ViewportRenderer (RenderTarget2D), GridRenderer, TilemapPaintHandler.

#### Assets (2 files)
AssetDatabase (filesystem scan + FileSystemWatcher), AssetEntry (DTO + type classification).

#### Project (4 files)
ProjectManager, ProjectTemplate, ProjectScaffolder, UserDataManager.

#### Platform (9 files)
IFileDialogService, MacFileDialogService, FallbackFileDialogService, ObjCRuntime (P/Invoke), MacMenuBar, MacMenuCallbacks, MacTitleBar, MacToolbar, MacToolbarCallbacks.

#### Runtime (2 files)
GameProcessManager (build & run), HotReloadWatcher.

### Desktop (1 file)
`Program.cs` — `new EditorGame(args).Run()`.

---

## Roadmap status at time of deletion

| Milestone | Theme | Status |
|-----------|-------|--------|
| v0.1 | Editor Foundation | Complete |
| v0.2 | Sprite Workflow | ~90% (missing pivot editor) |
| v0.3 | Scene Editing QoL | Complete |
| v0.4 | Custom Code Integration | Complete |
| v0.5 | Physics & Collision | ~90% (missing collider drag handles) |
| v0.6 | Tilemap Editing | Complete |
| v0.7 | Camera & Display | ~50% (missing multi-camera, safe areas, resolution presets) |
| v0.8 | Particle System | ~90% (missing particle presets save/load) |
| v0.9 | Audio & Shaders | ~90% (missing audio playback preview) |
| v1.0 | Game Runtime Bridge | ~70% (missing in-editor game viewport, console piping) |
