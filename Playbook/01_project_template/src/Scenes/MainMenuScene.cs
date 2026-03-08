using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyGame.Core;

namespace MyGame.Scenes;

/// <summary>
/// Placeholder main menu scene. Press Enter or Space to start the game.
/// Replace this with your actual menu UI.
/// </summary>
public class MainMenuScene : Scene
{
    public override void Initialize()
    {
        base.Initialize();
        // TODO: Set up menu UI, buttons, background, etc.
    }

    public override void LoadContent()
    {
        base.LoadContent();
        // TODO: Load menu assets (fonts, background art, music).
    }

    public override void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();

        // Press Enter or Space to switch to gameplay.
        if (keyboard.IsKeyDown(Keys.Enter) || keyboard.IsKeyDown(Keys.Space))
        {
            SceneManager.Switch(new GameplayScene());
        }

        // Press Escape to exit.
        if (keyboard.IsKeyDown(Keys.Escape))
        {
            ServiceLocator.Get<Game>().Exit();
        }
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        // TODO: Draw menu UI.
        // For now, the screen just shows the clear color from GameApp.
    }

    public override void Unload()
    {
        base.Unload();
    }
}
