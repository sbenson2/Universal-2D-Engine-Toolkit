# E4 — Solo Project Management
> **Category:** Explanation · **Related:** [E5 AI Workflow](./E5_ai_workflow.md) · [E9 Solo Dev Playbook](./E9_solo_dev_playbook.md) · [R3 Project Structure](../R/R3_project_structure.md)

---

## Vertical Slice Development

Build vertically, not horizontally. A vertical slice is a fully-polished, feature-complete thin cross-section — not a prototype with placeholders, but a small piece built to final quality across code, art, audio, and UI.

If you run out of time with 10 complete features and 10 unstarted, cutting is straightforward. With 20 half-finished features, everything breaks.

Each 1-2 week sprint should end with a playable build:
1. Pick 1-2 vertical slices
2. Break into tasks (code, art, audio, integration)
3. Execute
4. Playtest

**Build the Minimum Viable Game Loop first** — the absolute core mechanic stripped of UI, story, and polish. Validate it's fun before investing in anything else.

> **Deep dive:** [E9 Solo Dev Playbook](./E9_solo_dev_playbook.md) — 5-level goal hierarchy, Kanban vs sprints for solo dev, case studies from Stardew Valley/Balatro/Vampire Survivors

---

## Scope Management

Apply MoSCoW ruthlessly:
- **Must Have:** Core loop, win/lose conditions, basic UI
- **Should Have:** Sound effects, particles, tutorial
- **Could Have:** Achievements, extra content
- **Won't Have:** Multiplayer, level editor, mod support

Multiply every time estimate by 2-3x. Bug fixing typically consumes 30% of development time.

Write a design doc (DESIGN.md) with game pillars, target audience, core loop, and feature list with MoSCoW priorities. Cross out old decisions with dates when they change. This prevents scope creep by making every addition a conscious decision against a documented plan.

> **Deep dive:** [E9 Solo Dev Playbook](./E9_solo_dev_playbook.md) — scope creep as the universal killer, AI amplification risk, Polaris Framework for fix/polish phase, design pillars as filter

---

## Version Control Workflow

Use trunk-based Git with direct commits to main for day-to-day work. Create short-lived branches only for risky experiments you might abandon.

**Tag releases** with semantic versioning adapted for games:
- MAJOR: milestones (alpha → beta → release)
- MINOR: new features
- PATCH: bug fixes

**Set up Git LFS on day one** for binary assets. Use `git pull --rebase` instead of merge commits. Use `git rebase -i` to squash messy WIP commits.

**Commit hygiene:** Each commit should be one logical change. *"Add fire propagation system"* not *"work on stuff"*. This makes `git bisect` usable for finding regression sources.

---

## Technical Debt Management

Refactor code you're actively working in; leave stable code alone. The **Three Strikes Rule:** First duplication is fine, second you wince, third you refactor.

Allocate 10-20% of each sprint to tech debt. Use the **Strangler Fig pattern** for larger refactors: build the new system alongside the old, migrate callers gradually, delete the old. (This is exactly how [E2](./E2_why_nez_was_dropped.md) recommends migrating away from Nez.)

Prototype code written to answer "is this fun?" should be thrown away. Prototype code written to solve a known problem cleanly can be kept.

---

## Build Automation

Set up GitHub Actions for automated builds:

```yaml
# .github/workflows/build.yml
name: Build
on: [push, pull_request]
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      - run: dotnet build -c Release
      - run: dotnet publish -c Release -o ./publish
      - uses: actions/upload-artifact@v4
        with:
          name: game-build
          path: ./publish
```

Add Butler for automated itch.io deployment on tagged releases.

---

## Documentation That Compounds

Write **Architecture Decision Records** for framework choices, architectural patterns, and major library selections:

```
# ADR-001: Use Arch ECS for all entities
Status: Accepted
Date: 2026-02-09
Context: Need entity management for both mass and unique entities...
Decision: Use Arch ECS exclusively, no separate EC system...
Consequences: One entity model, simpler architecture, no bridge code...
```

Keep a CHANGELOG.md. Comment code with *why*, not *what*.

Create a **CONTEXT.md** file in your project root for AI-assisted development → [E5](./E5_ai_workflow.md).
