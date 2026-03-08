using Arch.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyGame.Core;
using MyGame.ECS;
using MyGame.ECS.Components;
using MyGame.ECS.Systems;
using MyGame.ECS.Tags;

namespace MyGame.Scenes;

/// <summary>
/// Placeholder gameplay scene with an ECS world.
/// Spawns a player entity and runs the movement system.
/// Replace this with your actual gameplay logic.
/// </summary>
public class GameplayScene : Scene
{
    private WorldManager _worldManager = null!;

    public override void Initialize()
    {
        base.Initialize();

        _worldManager = new WorldManager();

        // Register systems.
        _worldManager.AddUpdateSystem(MovementSystem.Update);

        // Spawn a player entity.
        _worldManager.World.Create(
            new Position(100f, 100f),
            new Velocity(50f, 0f),
            new PlayerTag()
        );
    }

    public override void LoadContent()
    {
        base.LoadContent();
        // TODO: Load gameplay assets (tilemap, sprites, audio).
    }

    public override void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();

        // Press Escape to return to main menu.
        if (keyboard.IsKeyDown(Keys.Escape))
        {
            SceneManager.Switch(new MainMenuScene());
            return;
        }

        _worldManager.Update(gameTime);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        _worldManager.Draw(gameTime);

        // TODO: Draw gameplay — sprites, tilemap, UI overlay, etc.
        // Example: query entities with Position + Sprite and draw them.
    }

    public override void Unload()
    {
        _worldManager.Dispose();
        base.Unload();
    }
}
