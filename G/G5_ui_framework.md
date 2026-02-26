# G5 — UI Framework
> **Category:** Guide · **Related:** [R1 Library Stack](../R/R1_library_stack.md) · [C1 Genre Reference](../C/C1_genre_reference.md)

---

## Gum (Primary — MonoGame's Official Recommendation)

The MonoGame official tutorial (Chapter 20) now uses Gum for UI. MonoGame.Extended is also migrating its UI system to Gum. This is the biggest upgrade over Nez.

**Install:** `dotnet add package Gum.MonoGame`

### Features
- WYSIWYG visual editor for layout design
- Code-first approach (no editor required)
- Forms controls: Button, TextBox, ListBox, CheckBox, Slider, ComboBox, ScrollViewer, TreeView
- Flexible anchor-based layout engine (position relative to parent edges, centers, percentages)
- Mouse, keyboard, gamepad, and touch input handling
- Used in production on multiple commercial FlatRedBall games
- Actively maintained with near-daily commits

### Why Gum Over Myra
- Official MonoGame ecosystem backing
- Both visual editor AND code-first API
- Active maintenance with near-daily commits
- Official MonoGame documentation support

---

## Myra (Alternative — Still Viable)

Myra v1.5.10 is still a solid choice if you're already using it.

**Install:** `dotnet add package Myra --version 1.5.10`

**Features:** Buttons, grids, dialogs, text input, scroll viewers, combo boxes.

**When to prefer Myra:** If you have existing Myra code, or if Gum's API doesn't suit your style. Both work fine.

---

## UI Needs by Genre

| Genre | UI Complexity | Key Elements |
|-------|--------------|-------------|
| RPG | Heavy | Inventory grids, equipment slots, stat screens, dialogue boxes, shops |
| Card game | Heavy | Card rendering, drag-and-drop, hand layout, deck management |
| RTS | Heavy | Resource bars, build menus, minimap, unit selection info |
| Idle/Incremental | Heavy | Primary interface IS the UI — buttons, progress bars, upgrade trees |
| Platformer | Light | HUD (health, lives), pause menu |
| Bullet hell | Light | Score display, bomb count |
| Puzzle | Medium | Level select, score display, undo button |

For RPG/card/RTS/idle games, invest time in learning Gum's layout system properly. For platformers and action games, a minimal HUD drawn directly with SpriteBatch may be simpler.
