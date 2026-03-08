using Arch.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyGame.Core;
using MyGame.ECS;
using MyGame.ECS.Components;
using MyGame.Platformer.Components;
using MyGame.Platformer.Systems;
using MyGame.Platformer.Tags;

namespace MyGame.Platformer.Scenes;

/// <summary>
/// Complete gameplay scene that wires up all platformer systems,
/// spawns a player, and creates a simple level layout.
/// Drop this into your project as-is or use it as a reference for your own scene.
/// </summary>
public class PlatformerScene : Scene
{
    private WorldManager _worldManager = null!;
    private SpriteBatch _spriteBatch = null!;
    private Texture2D _pixel = null!; // 1x1 white texture for debug drawing.

    public override void Initialize()
    {
        base.Initialize();

        _worldManager = new WorldManager();

        // ── Register systems in execution order ──
        // Order matters: input → movement → gravity → jump → ground detect → animation → camera
        _worldManager.AddUpdateSystem(InputSystem.Update);
        _worldManager.AddUpdateSystem(CharacterMovementSystem.Update);
        _worldManager.AddUpdateSystem(GravitySystem.Update);
        _worldManager.AddUpdateSystem(JumpSystem.Update);
        _worldManager.AddUpdateSystem(GroundDetectionSystem.Update);
        _worldManager.AddUpdateSystem(AnimationStateSystem.Update);
        _worldManager.AddUpdateSystem(CameraFollowSystem.Update);

        // ── Spawn the player ──
        SpawnPlayer(120f, 100f);

        // ── Build a simple level ──
        BuildLevel();
    }

    public override void LoadContent()
    {
        base.LoadContent();

        var gd = ServiceLocator.Get<GraphicsDevice>();
        _spriteBatch = new SpriteBatch(gd);

        // Create a 1×1 white pixel for debug rendering.
        _pixel = new Texture2D(gd, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    public override void Update(GameTime gameTime)
    {
        // Escape to quit / return to menu.
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            SceneManager.Pop();
            return;
        }

        _worldManager.Update(gameTime);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        var gd = ServiceLocator.Get<GraphicsDevice>();
        gd.Clear(new Color(24, 20, 37)); // Dark purple background.

        var viewMatrix = CameraFollowSystem.GetViewMatrix(
            gd.Viewport.Width, gd.Viewport.Height);

        _spriteBatch.Begin(
            sortMode: SpriteSortMode.Deferred,
            blendState: BlendState.AlphaBlend,
            samplerState: SamplerState.PointClamp,
            transformMatrix: viewMatrix);

        // ── Draw ground tiles ──
        DrawGroundTiles();

        // ── Draw the player ──
        DrawPlayer();

        _spriteBatch.End();
    }

    public override void Unload()
    {
        _pixel?.Dispose();
        _spriteBatch?.Dispose();
        _worldManager?.Dispose();
        base.Unload();
    }

    // ─────────────────────────────────────────────────────────
    //  SPAWNING
    // ─────────────────────────────────────────────────────────

    private void SpawnPlayer(float x, float y)
    {
        _worldManager.World.Create(
            new Position(x, y),
            new Velocity(0f, 0f),
            new CharacterBody(
                Width: PlatformerConfig.PlayerWidth,
                Height: PlatformerConfig.PlayerHeight,
                IsGrounded: false,
                WasGrounded: false,
                CoyoteTimer: 0f,
                JumpBufferTimer: 0f),
            PlatformerConfig.DefaultMotion(),
            new PlayerIntent(0f, false, false, false),
            new FacingDirection(1),
            new AnimationState("idle", false),
            new PlayerTag()
        );
    }

    /// <summary>
    /// Builds a simple test level with a floor, some platforms, and a gap.
    /// Each ground tile is a separate entity with Position + GroundTag.
    /// Replace this with your tilemap loader.
    /// </summary>
    private void BuildLevel()
    {
        int ts = PlatformerConfig.TileSize;

        // ── Floor (two sections with a gap) ──
        // Left section: tiles 0–14
        for (int i = 0; i < 15; i++)
            SpawnGroundTile(i * ts, 200);

        // Right section: tiles 18–35 (3-tile gap for jumping)
        for (int i = 18; i < 36; i++)
            SpawnGroundTile(i * ts, 200);

        // ── Floating platforms ──
        // Platform 1: above the gap
        for (int i = 15; i < 18; i++)
            SpawnGroundTile(i * ts, 160);

        // Platform 2: higher up, to the right
        for (int i = 22; i < 25; i++)
            SpawnGroundTile(i * ts, 120);

        // Staircase
        SpawnGroundTile(8 * ts, 180);
        SpawnGroundTile(9 * ts, 180);
        SpawnGroundTile(10 * ts, 160);
        SpawnGroundTile(11 * ts, 160);
        SpawnGroundTile(12 * ts, 140);
    }

    private void SpawnGroundTile(float x, float y)
    {
        _worldManager.World.Create(
            new Position(x, y),
            new GroundTag()
        );
    }

    // ─────────────────────────────────────────────────────────
    //  DEBUG RENDERING (replace with sprite rendering later)
    // ─────────────────────────────────────────────────────────

    private void DrawGroundTiles()
    {
        int ts = PlatformerConfig.TileSize;
        var query = new QueryDescription().WithAll<Position, GroundTag>();

        _worldManager.World.Query(in query, (ref Position pos) =>
        {
            _spriteBatch.Draw(_pixel,
                new Rectangle((int)pos.X, (int)pos.Y, ts, ts),
                new Color(80, 80, 110)); // Muted blue-grey tiles.
        });
    }

    private void DrawPlayer()
    {
        var query = new QueryDescription()
            .WithAll<Position, CharacterBody, AnimationState, PlayerTag>();

        _worldManager.World.Query(in query, (
            ref Position pos,
            ref CharacterBody body,
            ref AnimationState anim) =>
        {
            // Color-code by animation state for debug visibility.
            Color color = anim.CurrentAnim switch
            {
                "run"  => Color.LimeGreen,
                "jump" => Color.Cyan,
                "fall" => Color.Orange,
                _      => Color.White // idle
            };

            int drawX = (int)(pos.X - body.Width * 0.5f);
            int drawY = (int)(pos.Y - body.Height * 0.5f);

            _spriteBatch.Draw(_pixel,
                new Rectangle(drawX, drawY, (int)body.Width, (int)body.Height),
                color);
        });
    }
}
