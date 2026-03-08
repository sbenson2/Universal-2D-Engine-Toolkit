using Arch.Core;
using Microsoft.Xna.Framework;
using Apos.Input;
using MyGame.ECS.Components;
using MyGame.TopDown.Components;
using MyGame.TopDown.Dialogue;
using MyGame.TopDown.Tags;

namespace MyGame.TopDown.Systems;

/// <summary>
/// Detects when the player presses the interact button near an interactable entity.
/// Checks proximity using <see cref="Interactable.Radius"/> and triggers the
/// appropriate action (dialogue, pickup, examine, etc.).
/// </summary>
public static class InteractionSystem
{
    private static readonly QueryDescription PlayerQuery = new QueryDescription()
        .WithAll<Position, FacingDirection, PlayerTag>();

    private static readonly QueryDescription InteractableQuery = new QueryDescription()
        .WithAll<Position, Interactable, InteractableTag>();

    /// <summary>
    /// The dialogue box instance to open when a "dialogue" interaction triggers.
    /// Must be set by the scene before this system runs.
    /// </summary>
    public static DialogueBox? ActiveDialogueBox { get; set; }

    /// <summary>
    /// Register with <see cref="MyGame.ECS.WorldManager.AddUpdateSystem"/>.
    /// </summary>
    public static void Update(World world, GameTime gameTime)
    {
        // Don't process interactions while dialogue is active.
        if (ActiveDialogueBox is { IsActive: true }) return;

        if (!InputSystem.Interact.Pressed()) return;

        // Get player position.
        float playerX = 0f, playerY = 0f;
        int facingX = 0, facingY = 1;
        bool hasPlayer = false;

        world.Query(in PlayerQuery, (ref Position pos, ref FacingDirection facing) =>
        {
            playerX = pos.X;
            playerY = pos.Y;
            facingX = facing.X;
            facingY = facing.Y;
            hasPlayer = true;
        });

        if (!hasPlayer) return;

        // Check facing-biased proximity: offset the check point slightly in the facing direction.
        float checkX = playerX + facingX * TopDownConfig.InteractionFacingOffset;
        float checkY = playerY + facingY * TopDownConfig.InteractionFacingOffset;

        // Find the closest interactable within range.
        Entity closestEntity = Entity.Null;
        float closestDist = float.MaxValue;
        Interactable closestInteract = default;

        world.Query(in InteractableQuery, (Entity entity, ref Position pos, ref Interactable inter) =>
        {
            float dx = checkX - pos.X;
            float dy = checkY - pos.Y;
            float dist = MathF.Sqrt(dx * dx + dy * dy);

            if (dist < inter.Radius && dist < closestDist)
            {
                closestDist = dist;
                closestEntity = entity;
                closestInteract = inter;
            }
        });

        if (closestEntity == Entity.Null) return;

        // Dispatch by action type.
        switch (closestInteract.ActionType)
        {
            case "dialogue":
                if (ActiveDialogueBox != null && world.Has<DialogueSpeaker>(closestEntity))
                {
                    var speaker = world.Get<DialogueSpeaker>(closestEntity);
                    ActiveDialogueBox.StartDialogue(speaker.Name, speaker.DialogueId);
                }
                break;

            case "examine":
                // Extend: show a text popup with item/object description.
                break;

            case "pickup":
                // Extend: add item to player inventory, destroy entity.
                break;

            case "transition":
                // Extend: trigger scene change.
                break;
        }
    }
}
