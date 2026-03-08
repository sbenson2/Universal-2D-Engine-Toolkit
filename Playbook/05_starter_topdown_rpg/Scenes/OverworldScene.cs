using Arch.Core;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Core;
using MyGame.ECS;
using MyGame.ECS.Components;
using MyGame.TopDown.Components;
using MyGame.TopDown.Dialogue;
using MyGame.TopDown.Inventory;
using MyGame.TopDown.Systems;
using MyGame.TopDown.Tags;

namespace MyGame.TopDown.Scenes;

/// <summary>
/// Starter overworld scene that wires up all top-down RPG systems.
/// Spawns a player, places NPCs with dialogue, and creates wall boundaries.
/// Copy this as a starting point and customize to build your world.
/// </summary>
public class OverworldScene : Scene
{
    private WorldManager _worldManager = null!;
    private DialogueBox _dialogueBox = null!;
    private InventoryManager _inventory = null!;
    private SpriteBatch _spriteBatch = null!;
    private SpriteFont _font = null!;

    // World bounds for camera clamping (pixels).
    private const int WorldWidth = 480;
    private const int WorldHeight = 270;

    public override void Initialize()
    {
        _worldManager = new WorldManager();
        _inventory = new InventoryManager();

        base.Initialize();
    }

    public override void LoadContent()
    {
        _spriteBatch = ServiceLocator.Get<SpriteBatch>();

        // Load a SpriteFont — replace "Fonts/Default" with your actual font asset path.
        var content = ServiceLocator.Get<Microsoft.Xna.Framework.Content.ContentManager>();
        // _font = content.Load<SpriteFont>("Fonts/Default");
        // TODO: Uncomment above and add a SpriteFont to your Content pipeline.
        // For now, dialogue rendering requires a font — add one to use DialogueBox.

        // ── Seed Item Database ───────────────────────────────
        ItemDatabase.SeedDefaults();

        // ── Build Dialogue Database ──────────────────────────
        var dialogueDb = new Dictionary<string, DialogueData>
        {
            ["elder_greeting"] = new DialogueData
            {
                Id = "elder_greeting",
                Lines = new List<DialogueLine>
                {
                    new() { Text = "Welcome, traveler. This village has\nseen better days..." },
                    new() { Text = "Strange creatures have appeared\nin the forest to the north." },
                    new() { Text = "If you're brave enough, perhaps\nyou could investigate?" }
                }
            },
            ["guard_hint"] = new DialogueData
            {
                Id = "guard_hint",
                Lines = new List<DialogueLine>
                {
                    new() { Text = "The bridge east of here is broken.\nYou'll need a key to use the ferry." },
                    new() { Text = "I heard the old man in the\nhouse has one..." }
                }
            }
        };

        // _dialogueBox = new DialogueBox(_font, dialogueDb);
        // TODO: Uncomment when font is loaded.

        // ── Wire up Interaction System ────────────────────────
        // InteractionSystem.ActiveDialogueBox = _dialogueBox;

        // ── Register Systems (order matters!) ─────────────────
        _worldManager.AddUpdateSystem(InputSystem.Update);
        _worldManager.AddUpdateSystem(TopDownMovementSystem.Update);
        _worldManager.AddUpdateSystem(CollisionSystem.Update);
        _worldManager.AddUpdateSystem(InteractionSystem.Update);
        _worldManager.AddUpdateSystem(AnimationStateSystem.Update);
        _worldManager.AddUpdateSystem(CameraFollowSystem.Update);

        // ── Spawn Player ─────────────────────────────────────
        var world = _worldManager.World;

        world.Create(
            new Position(WorldWidth / 2f, WorldHeight / 2f),
            new Velocity(0f, 0f),
            new CharacterMotion(
                TopDownConfig.DefaultMoveSpeed,
                TopDownConfig.DefaultAcceleration,
                TopDownConfig.DefaultFriction),
            new CharacterBody(TopDownConfig.PlayerBodyWidth, TopDownConfig.PlayerBodyHeight),
            new FacingDirection(0, 1), // Facing down
            new AnimationState("idle_down", false),
            new Stats(
                TopDownConfig.DefaultMaxHp,
                TopDownConfig.DefaultMaxHp,
                TopDownConfig.DefaultAttack,
                TopDownConfig.DefaultDefense,
                TopDownConfig.DefaultSpeed,
                Level: 1,
                Exp: 0),
            new InventoryComponent(new List<string>()),
            new PlayerTag()
        );

        // ── Spawn NPCs ──────────────────────────────────────
        // Village Elder
        world.Create(
            new Position(200f, 120f),
            new Velocity(0f, 0f),
            new CharacterBody(10f, 6f),
            new FacingDirection(0, 1),
            new AnimationState("idle_down", false),
            new DialogueSpeaker("Elder Rowan", "", "elder_greeting"),
            new Interactable(TopDownConfig.DefaultInteractionRadius, "dialogue"),
            new NpcTag(),
            new InteractableTag(),
            new SolidTag()
        );

        // Town Guard
        world.Create(
            new Position(320f, 160f),
            new Velocity(0f, 0f),
            new CharacterBody(10f, 6f),
            new FacingDirection(-1, 0),
            new AnimationState("idle_side", true),
            new DialogueSpeaker("Guard", "", "guard_hint"),
            new Interactable(TopDownConfig.DefaultInteractionRadius, "dialogue"),
            new NpcTag(),
            new InteractableTag(),
            new SolidTag()
        );

        // ── Create World Boundary Walls ──────────────────────
        // Top wall
        CreateWall(world, WorldWidth / 2f, -4f, WorldWidth, 8f);
        // Bottom wall
        CreateWall(world, WorldWidth / 2f, WorldHeight + 4f, WorldWidth, 8f);
        // Left wall
        CreateWall(world, -4f, WorldHeight / 2f, 8f, WorldHeight);
        // Right wall
        CreateWall(world, WorldWidth + 4f, WorldHeight / 2f, 8f, WorldHeight);

        // ── Scatter some solid objects (rocks, trees) ────────
        CreateWall(world, 100f, 80f, 16f, 16f);  // Rock
        CreateWall(world, 350f, 90f, 16f, 16f);  // Rock
        CreateWall(world, 150f, 200f, 12f, 8f);  // Tree base

        // ── Camera Setup ─────────────────────────────────────
        CameraFollowSystem.WorldBounds = new Rectangle(0, 0, WorldWidth, WorldHeight);
        CameraFollowSystem.ViewportWidth = TopDownConfig.NativeWidth;
        CameraFollowSystem.ViewportHeight = TopDownConfig.NativeHeight;

        base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
        // Update Apos.Input (must be called once per frame at the game level).
        // If GameApp already calls InputHelper.UpdateSetup/UpdateCleanup, skip here.
        // Apos.Input.InputHelper.UpdateSetup();

        _worldManager.Update(gameTime);
        _dialogueBox?.Update(gameTime);

        // Apos.Input.InputHelper.UpdateCleanup();
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        // ── World pass (with camera transform) ───────────────
        var transform = CameraFollowSystem.GetTransformMatrix();

        spriteBatch.Begin(
            SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            null, null, null,
            transform);

        // TODO: Draw tile map layers here (ground, below-entities).
        // TODO: Draw Y-sorted entities (collect, sort by foot Y, draw).
        // For now, the WorldManager.Draw systems would go here if you add render systems.

        _worldManager.Draw(gameTime);

        spriteBatch.End();

        // ── UI pass (no camera transform) ────────────────────
        spriteBatch.Begin(
            SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointClamp);

        _dialogueBox?.Draw(spriteBatch, TopDownConfig.NativeWidth, TopDownConfig.NativeHeight);

        spriteBatch.End();
    }

    public override void Unload()
    {
        _worldManager.Dispose();
        base.Unload();
    }

    /// <summary>
    /// Helper to create an invisible solid wall entity.
    /// </summary>
    private static void CreateWall(World world, float x, float y, float width, float height)
    {
        world.Create(
            new Position(x, y),
            new CharacterBody(width, height),
            new SolidTag()
        );
    }
}
