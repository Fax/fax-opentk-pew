// See https://aka.ms/new-console-template for more information
using System.Drawing;
using OpenTK.Compute.OpenCL;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

Console.WriteLine("Hello, World!");

new GOLWindow().Run();



public class CollisionManager
{

    public bool CheckCollision(Rectangle a, Rectangle b)
    {
        return a.IntersectsWith(b);
    }
}

class GOLWindow : GameWindow
{
    InputManager inputManager = new InputManager();
    Player player;
    PlayerRenderer prenderer;
    BulletRenderer bulletRenderer;

    EventBus eventBus = new();
    List<Bullet> bullets = new List<Bullet>();

    List<EnemyEntity> enemies = new List<EnemyEntity>();
    EnemyManager enemyManager;
    EnemyRenderer enemyRenderer;
    BulletSpawner BulletSpawner;
    PickupSpawner PickupSpawner;
    PickupManager PickupManager;
    PickupRenderer PickupRenderer;
    ExperienceManager expManager;
    ExperienceRenderer expRenderer;
    CollisionManager collisionManager = new CollisionManager();
    List<PickupEntity> pickups = new();
    List<ExperienceEntity> nuggets = new();

    public GOLWindow() : base(GameWindowSettings.Default, new NativeWindowSettings
    {
        ClientSize = new Vector2i(800, 700),
        Title = "GOL"
    })
    {
        player = new Player(eventBus) { weapon = new Weapon(eventBus) };
        prenderer = new PlayerRenderer(this.ClientSize, pixelSize: 20);
        bulletRenderer = new BulletRenderer(this.ClientSize, pixelSize: 20);
        BulletSpawner = new BulletSpawner(eventBus, bullets);
        PickupSpawner = new PickupSpawner(eventBus, pickups);
        PickupManager = new PickupManager(pickups);
        PickupRenderer = new PickupRenderer(ClientSize);
        enemyManager = new EnemyManager(eventBus, enemies);
        enemyRenderer = new EnemyRenderer(enemies);
        expManager = new ExperienceManager(eventBus, nuggets, player);
        expRenderer = new ExperienceRenderer(nuggets);
        Sfx.Init();
    }
    protected override void OnLoad()
    {
        base.OnLoad();
    }
    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        GL.ClearColor(Color4.CornflowerBlue);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        prenderer.BeginFrame(ClientSize);
        prenderer.Render(player, Color4.Red);
        prenderer.EndFrame();
        bulletRenderer.BeginFrame(ClientSize);
        foreach (var b in bullets)
        {
            bulletRenderer.Render(b);
        }
        bulletRenderer.EndFrame();
        PickupRenderer.BeginFrame(ClientSize);
        foreach (var p in pickups)
        {
            PickupRenderer.Render(p);
        }
        PickupRenderer.EndFrame();
        enemyRenderer.BeginFrame(ClientSize);
        enemyRenderer.Render();
        enemyRenderer.EndFrame();
        expRenderer.BeginFrame(ClientSize);
        expRenderer.Render();
        expRenderer.EndFrame();
        this.SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {

        var dt = (float)args.Time;
        inputManager.Update(KeyboardState, MouseState);

        player.Update(inputManager, dt);

        BulletSpawner.Update(dt);
        PickupSpawner.Update(dt);
        PickupManager.Update(dt);
        enemyManager.Update(dt, player.Position);

        if (this.KeyboardState.IsKeyDown(Keys.Escape)) this.Close();

        // I need to check bullet collisions

        foreach (var b in bullets)
        {
            var box = b.Position.ToBoundingBox(b.Size);
            foreach (var enemy in enemies)
            {
                var enemyBox = enemy.Position.ToBoundingBox(enemy.Size);
                if (collisionManager.CheckCollision(box, enemyBox))
                {
                    eventBus.Publish<BulletCollisionEvent>(new BulletCollisionEvent(b.EntityId, enemy.EntityId));
                }
            }
        }

        foreach (var x in pickups)
        {
            var box = new Rectangle((int)x.Position.X, (int)x.Position.Y, (int)x.Size.X, (int)x.Size.Y);
            if (collisionManager.CheckCollision(player.BoundingBox, box))
            {
                switch (x)
                {
                    case WeaponPickup we:
                        eventBus.Publish<WeaponPickupEvent>(new WeaponPickupEvent(we.BulletType, we.Id, player.EntityId));
                        break;
                }
            }
        }

        var playerRangeBox = player.Position.ToCenteredBoundingBox(player.Size, new(150.0f));
        var playerBox = player.BoundingBox;
        foreach (var nugget in nuggets.Where(x => x.Active))
        {
            if (nugget.Speed > 0)
            {
                // this is already moving
                if (collisionManager.CheckCollision(playerBox, nugget.Position.ToBoundingBox(new(15.0f))))
                {
                    nugget.Active = false;
                    eventBus.Publish<CollectExperienceEvent>(new CollectExperienceEvent(nugget.Amount, nugget.Position));
                }
            }
            else
            {
                if (collisionManager.CheckCollision(playerRangeBox, nugget.Position.ToBoundingBox(new(15f))))
                {
                    nugget.Speed += .9f;
                }
            }
        }

        expManager.Update(dt);
        base.OnUpdateFrame(args);
        eventBus.Drain();
    }


    public override void Close()
    {
        Sfx.Shutdown();
        base.Close();
    }
}


