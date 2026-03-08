using Apos.Input;
using Arch.Core;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Core;
using MyGame.ECS;
using MyGame.Roguelike.Components;
using MyGame.Roguelike.Map;
using MyGame.Roguelike.Systems;
using MyGame.Roguelike.Tags;

namespace MyGame.Roguelike.Scenes;

/// <summary>
/// Main roguelike scene. Generates a dungeon, spawns the player, enemies, and items,
/// then runs the turn-based game loop. Layers on top of the project template's
/// <see cref="Scene"/> base class and <see cref="WorldManager"/>.
/// </summary>
public class DungeonScene : Scene
{
    // ECS
    private WorldManager _worldManager = null!;

    // Map
    private GameMap _map = null!;
    private DungeonGenerator.DungeonResult _dungeonResult;

    // Systems
    private readonly TurnSystem _turnSystem = new();
    private readonly PlayerInputSystem _inputSystem = new();
    private readonly AiSystem _aiSystem = new();
    private readonly MovementSystem _movementSystem = new();
    private readonly CombatSystem _combatSystem = new();
    private readonly FovSystem _fovSystem = new();
    private readonly RenderSystem _renderSystem = new();
    private readonly HudSystem _hudSystem = new();

    // Game state
    private readonly MessageLog _log = new();
    private int _dungeonDepth = 1;
    private int _seed;
    private Entity _playerEntity;
    private bool _gameOver;
    private readonly Random _rng = new();

    /// <summary>Create a dungeon scene with an optional seed.</summary>
    public DungeonScene(int? seed = null)
    {
        _seed = seed ?? Random.Shared.Next();
    }

    public override void Initialize()
    {
        base.Initialize();
        _worldManager = new WorldManager();
    }

    public override void LoadContent()
    {
        base.LoadContent();

        var graphicsDevice = ServiceLocator.Get<GraphicsDevice>();

        // Load a SpriteFont — you'll need a "DefaultFont" in your Content pipeline.
        // If not available, the HUD will be text-less but the map still renders.
        SpriteFont? font = null;
        try
        {
            var content = ServiceLocator.Get<Microsoft.Xna.Framework.Content.ContentManager>();
            font = content.Load<SpriteFont>("DefaultFont");
        }
        catch
        {
            // Font not available — HUD text won't render, but game works
        }

        _renderSystem.Initialize(graphicsDevice, font);
        if (font != null)
            _hudSystem.Initialize(graphicsDevice, font);

        GenerateFloor();
    }

    /// <summary>
    /// Generate a new dungeon floor, spawning player, enemies, and items.
    /// </summary>
    private void GenerateFloor()
    {
        // Dispose old world and create fresh
        _worldManager.Dispose();
        _worldManager = new WorldManager();
        var world = _worldManager.World;

        // Generate dungeon
        var generator = new DungeonGenerator(_seed + _dungeonDepth);
        _dungeonResult = generator.Generate(
            RoguelikeConfig.MapWidth,
            RoguelikeConfig.MapHeight,
            RoguelikeConfig.MaxRooms,
            RoguelikeConfig.RoomMinSize,
            RoguelikeConfig.RoomMaxSize);

        _map = _dungeonResult.Map;

        // Spawn player
        _playerEntity = world.Create(
            new GridPosition(_dungeonResult.PlayerSpawn.X, _dungeonResult.PlayerSpawn.Y),
            new TurnActor(
                RoguelikeConfig.PlayerSpeed,
                0,
                RoguelikeConfig.PlayerSpeed * RoguelikeConfig.BaseEnergyPerTurn / 10),
            new Stats(
                RoguelikeConfig.PlayerMaxHp,
                RoguelikeConfig.PlayerMaxHp,
                RoguelikeConfig.PlayerAttack,
                RoguelikeConfig.PlayerDefense,
                1, 0,
                RoguelikeConfig.PlayerExpToNext),
            new FieldOfView(RoguelikeConfig.PlayerFovRadius, new HashSet<(int, int)>()),
            Inventory.Create(RoguelikeConfig.PlayerInventorySlots),
            new PlayerTag(),
            new BlocksMovementTag());

        // Spawn enemies in random rooms (skip first room — that's the player's)
        int enemyCount = _rng.Next(
            RoguelikeConfig.MinEnemiesPerFloor,
            RoguelikeConfig.MaxEnemiesPerFloor + 1);

        for (int i = 0; i < enemyCount && _dungeonResult.Rooms.Count > 1; i++)
        {
            int roomIdx = _rng.Next(1, _dungeonResult.Rooms.Count);
            var room = _dungeonResult.Rooms[roomIdx];
            int ex = _rng.Next(room.X + 1, room.X + room.Width - 1);
            int ey = _rng.Next(room.Y + 1, room.Y + room.Height - 1);

            // Randomly choose goblin or orc
            bool isOrc = _rng.NextDouble() > 0.6;

            world.Create(
                new GridPosition(ex, ey),
                new TurnActor(
                    isOrc ? RoguelikeConfig.OrcSpeed : RoguelikeConfig.GoblinSpeed,
                    0,
                    (isOrc ? RoguelikeConfig.OrcSpeed : RoguelikeConfig.GoblinSpeed)
                        * RoguelikeConfig.BaseEnergyPerTurn / 10),
                new Stats(
                    isOrc ? RoguelikeConfig.OrcMaxHp : RoguelikeConfig.GoblinMaxHp,
                    isOrc ? RoguelikeConfig.OrcMaxHp : RoguelikeConfig.GoblinMaxHp,
                    isOrc ? RoguelikeConfig.OrcAttack : RoguelikeConfig.GoblinAttack,
                    isOrc ? RoguelikeConfig.OrcDefense : RoguelikeConfig.GoblinDefense,
                    1, 0, 0),
                new AiIntent(AiBehavior.Wander),
                new EnemyTag(),
                new BlocksMovementTag());
        }

        // Spawn items in random rooms
        int itemCount = _rng.Next(
            RoguelikeConfig.MinItemsPerFloor,
            RoguelikeConfig.MaxItemsPerFloor + 1);

        for (int i = 0; i < itemCount && _dungeonResult.Rooms.Count > 1; i++)
        {
            int roomIdx = _rng.Next(1, _dungeonResult.Rooms.Count);
            var room = _dungeonResult.Rooms[roomIdx];
            int ix = _rng.Next(room.X + 1, room.X + room.Width - 1);
            int iy = _rng.Next(room.Y + 1, room.Y + room.Height - 1);

            world.Create(
                new GridPosition(ix, iy),
                new ItemTag());
        }

        // Initial FOV calculation
        _fovSystem.Update(world, _map);

        _log.Add($"You descend to depth {_dungeonDepth}.", Color.LightBlue);
    }

    public override void Update(GameTime gameTime)
    {
        if (_gameOver) return;

        InputHelper.UpdateSetup();
        var world = _worldManager.World;

        // Turn system tick
        _turnSystem.Update(world, gameTime);

        if (_turnSystem.IsPlayerTurn && _turnSystem.WaitingForInput)
        {
            // Player input
            _inputSystem.Update(world, gameTime, _turnSystem);

            if (_inputSystem.PendingMove.HasValue)
            {
                var (dx, dy) = _inputSystem.PendingMove.Value;
                ProcessPlayerMove(world, dx, dy);
            }
            else if (_inputSystem.DidWait)
            {
                _log.Add("You wait...", Color.Gray);
                _turnSystem.ConsumeEnergy(_playerEntity);
                _fovSystem.Update(world, _map);
            }
            else if (_inputSystem.DidInteract)
            {
                TryInteract(world);
            }
        }
        else if (_turnSystem.CurrentActor.HasValue && !_turnSystem.IsPlayerTurn)
        {
            // AI turn
            ProcessAiTurn(world, _turnSystem.CurrentActor.Value);
        }

        InputHelper.UpdateCleanup();
    }

    private void ProcessPlayerMove(World world, int dx, int dy)
    {
        var (result, bumpTarget) = _movementSystem.TryMove(world, _playerEntity, dx, dy, _map);

        switch (result)
        {
            case MovementSystem.MoveResult.Moved:
                // Check if stepped on stairs
                ref var pos = ref _playerEntity.Get<GridPosition>();
                if (_map.GetTileAt(pos.X, pos.Y) == TileType.StairsDown)
                {
                    _log.Add("You see stairs leading down. Press E to descend.", Color.Gold);
                }
                break;

            case MovementSystem.MoveResult.BumpAttack when bumpTarget.HasValue:
                var (damage, killed) = _combatSystem.Attack(_playerEntity, bumpTarget.Value, _log);
                if (killed)
                {
                    int expValue = bumpTarget.Value.Has<EnemyTag>()
                        ? RoguelikeConfig.GoblinExpValue
                        : 0;
                    _combatSystem.AwardExp(_playerEntity, expValue, _log);
                    world.Destroy(bumpTarget.Value);
                }
                break;

            case MovementSystem.MoveResult.Blocked:
                // No-op — don't consume the turn
                return;
        }

        _turnSystem.ConsumeEnergy(_playerEntity);
        _fovSystem.Update(world, _map);
    }

    private void ProcessAiTurn(World world, Entity enemy)
    {
        if (!enemy.IsAlive()) return;

        ref var playerPos = ref _playerEntity.Get<GridPosition>();
        ref var playerFov = ref _playerEntity.Get<FieldOfView>();

        var (dx, dy) = _aiSystem.DecideMove(
            world, enemy, playerPos, playerFov.VisibleTiles);

        var (result, bumpTarget) = _movementSystem.TryMove(world, enemy, dx, dy, _map);

        if (result == MovementSystem.MoveResult.BumpAttack && bumpTarget.HasValue)
        {
            var (damage, killed) = _combatSystem.Attack(enemy, bumpTarget.Value, _log);
            if (killed && bumpTarget.Value.Has<PlayerTag>())
            {
                _gameOver = true;
                _log.Add("Game Over! Press R to restart.", Color.Red);
            }
        }

        _turnSystem.ConsumeEnergy(enemy);
    }

    private void TryInteract(World world)
    {
        ref var pos = ref _playerEntity.Get<GridPosition>();

        // Check for stairs
        if (_map.GetTileAt(pos.X, pos.Y) == TileType.StairsDown)
        {
            _dungeonDepth++;
            GenerateFloor();
            return;
        }

        // Check for items on the ground
        var itemQuery = new QueryDescription().WithAll<GridPosition, ItemTag>();
        Entity? foundItem = null;
        world.Query(in itemQuery, (Entity entity, ref GridPosition itemPos) =>
        {
            if (itemPos.X == pos.X && itemPos.Y == pos.Y)
                foundItem = entity;
        });

        if (foundItem.HasValue)
        {
            ref var inventory = ref _playerEntity.Get<Inventory>();
            if (inventory.HasRoom)
            {
                inventory.Items.Add(world.Reference(foundItem.Value));
                world.Destroy(foundItem.Value);
                _log.Add("You pick up an item.", Color.Cyan);
            }
            else
            {
                _log.Add("Your inventory is full!", Color.Orange);
            }
        }

        _turnSystem.ConsumeEnergy(_playerEntity);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        var world = _worldManager.World;

        // Center camera on player
        ref var playerPos = ref _playerEntity.Get<GridPosition>();
        var viewport = spriteBatch.GraphicsDevice.Viewport;
        var cameraOffset = new Vector2(
            viewport.Width / 2f - playerPos.X * RoguelikeConfig.TileSize,
            viewport.Height / 2f - playerPos.Y * RoguelikeConfig.TileSize);

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
            SamplerState.PointClamp);

        _renderSystem.Draw(spriteBatch, world, _map, cameraOffset);
        _hudSystem.Draw(spriteBatch, world, _log, _dungeonDepth);

        spriteBatch.End();
    }

    public override void Unload()
    {
        _renderSystem.Dispose();
        _hudSystem.Dispose();
        _worldManager.Dispose();
        base.Unload();
    }
}
