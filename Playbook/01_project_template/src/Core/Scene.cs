using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyGame.Core;

/// <summary>
/// Abstract base class for all game scenes.
/// Each scene has a full lifecycle: Initialize → LoadContent → Update/Draw → Unload.
/// </summary>
public abstract class Scene
{
    /// <summary>Whether this scene has been initialized.</summary>
    public bool IsInitialized { get; private set; }

    /// <summary>Whether content has been loaded for this scene.</summary>
    public bool IsContentLoaded { get; private set; }

    /// <summary>Called once when the scene is first pushed onto the stack.</summary>
    public virtual void Initialize()
    {
        IsInitialized = true;
    }

    /// <summary>Called after Initialize to load assets and content.</summary>
    public virtual void LoadContent()
    {
        IsContentLoaded = true;
    }

    /// <summary>Called every frame to update game logic.</summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    public abstract void Update(GameTime gameTime);

    /// <summary>Called every frame to render the scene.</summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    /// <param name="spriteBatch">Shared SpriteBatch for 2D rendering.</param>
    public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

    /// <summary>Called when the scene is removed from the stack. Clean up resources here.</summary>
    public virtual void Unload()
    {
        IsContentLoaded = false;
        IsInitialized = false;
    }
}
