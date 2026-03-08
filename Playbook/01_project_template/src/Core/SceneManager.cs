using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyGame.Core;

/// <summary>
/// Manages a stack of scenes. The topmost scene receives Update and Draw calls.
/// Supports push, pop, and switch (replace top) operations.
/// </summary>
public static class SceneManager
{
    private static readonly Stack<Scene> _scenes = new();

    /// <summary>The currently active scene (top of stack), or null if empty.</summary>
    public static Scene? ActiveScene => _scenes.Count > 0 ? _scenes.Peek() : null;

    /// <summary>Number of scenes on the stack.</summary>
    public static int Count => _scenes.Count;

    /// <summary>
    /// Push a new scene onto the stack. Initializes and loads content immediately.
    /// </summary>
    public static void Push(Scene scene)
    {
        _scenes.Push(scene);
        scene.Initialize();
        scene.LoadContent();
    }

    /// <summary>
    /// Pop the top scene off the stack and unload it.
    /// </summary>
    /// <returns>The popped scene, or null if stack was empty.</returns>
    public static Scene? Pop()
    {
        if (_scenes.Count == 0) return null;

        var scene = _scenes.Pop();
        scene.Unload();
        return scene;
    }

    /// <summary>
    /// Replace the top scene with a new one. Pops the current and pushes the new.
    /// </summary>
    public static void Switch(Scene scene)
    {
        Pop();
        Push(scene);
    }

    /// <summary>
    /// Clear all scenes from the stack, unloading each one.
    /// </summary>
    public static void Clear()
    {
        while (_scenes.Count > 0)
        {
            _scenes.Pop().Unload();
        }
    }

    /// <summary>Update the active scene.</summary>
    public static void Update(GameTime gameTime)
    {
        ActiveScene?.Update(gameTime);
    }

    /// <summary>Draw the active scene.</summary>
    public static void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        ActiveScene?.Draw(gameTime, spriteBatch);
    }
}
