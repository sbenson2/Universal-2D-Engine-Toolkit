# G6 — Audio
> **Category:** Guide · **Related:** [R1 Library Stack](../R/R1_library_stack.md) · [C1 Genre Reference](../C/C1_genre_reference.md)

---

## MonoGame Built-in Audio (Start Here)

Adequate for most games. Cross-platform. No advanced features.

- **SoundEffect:** Short clips, multiple simultaneous instances. Load via Content Pipeline.
- **Song:** Streaming music playback. One at a time via MediaPlayer.

**Good enough for:** Platformers, RPGs, puzzles, card games, most genres.

---

## FMOD via FmodForFoxes (Upgrade When Needed)

Professional audio engine. Free for indie (<$200K revenue).

**Install:**
```bash
dotnet add package FmodForFoxes
dotnet add package FmodForFoxes.Desktop
```

**Setup note:** Requires manual native lib copying due to FMOD licensing. FMOD native libraries must be downloaded separately from the FMOD website.

### Features
- Real-time parameter control
- Beat callbacks (essential for rhythm games)
- DSP effects: reverb, lowpass, chorus, and more
- Bus mixing (music bus, SFX bus, ambient bus)
- 3D spatialization
- Adaptive music (change music layers based on game state)

### Platform Support
- Windows, Linux, Android: Supported via FmodForFoxes
- iOS: Requires manual integration (no NuGet wrapper)

---

## When to Upgrade

| Trigger | Stick with MonoGame | Switch to FMOD |
|---------|-------------------|----------------|
| Basic SFX + background music | ✓ | |
| Beat-synced gameplay (rhythm games) | | ✓ |
| Adaptive/layered music | | ✓ |
| Advanced DSP (reverb, filters) | | ✓ |
| 3D audio spatialization | | ✓ |
| Dynamic mixing (duck music during dialogue) | | ✓ |

**Recommendation:** Start with MonoGame audio. Upgrade to FMOD when you need beat-synced gameplay, adaptive music, or advanced DSP.
