using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Scenes;

namespace MyGame.Core;

/// <summary>
/// Main game class. Configures graphics, manages the scene lifecycle,
/// and pumps Update/Draw through the active scene.
/// </summary>
public class GameApp : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;

    public GameApp()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = 1280,
            PreferredBackBufferHeight = 720,
            SynchronizeWithVerticalRetrace = true
        };

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 60.0);
        Window.Title = "MyGame";
    }

    protected override void Initialize()
    {
        // Register core services so scenes and systems can access them.
        ServiceLocator.Register<Game>(this);
        ServiceLocator.Register(GraphicsDevice);
        ServiceLocator.Register(_graphics);
        ServiceLocator.Register(Content);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        ServiceLocator.Register(_spriteBatch);

        // Push the initial scene.
        SceneManager.Push(new MainMenuScene());
    }

    protected override void Update(GameTime gameTime)
    {
        SceneManager.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        SceneManager.Draw(gameTime, _spriteBatch);
        base.Draw(gameTime);
    }

    protected override void UnloadContent()
    {
        SceneManager.Clear();
        base.UnloadContent();
    }
}
