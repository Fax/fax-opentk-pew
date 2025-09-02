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
    BulletSpawner BulletSpawner;
    PickupSpawner PickupSpawner;
    PickupManager PickupManager;
    PickupRenderer PickupRenderer;
    CollisionManager collisionManager = new CollisionManager();
    List<PickupEntity> pickups = new();
    public GOLWindow() : base(GameWindowSettings.Default, new NativeWindowSettings
    {
        ClientSize = new Vector2i(800, 700),
        Title = "GOL"
    })
    {
        prenderer = new PlayerRenderer(this.ClientSize, pixelSize: 20);
        bulletRenderer = new BulletRenderer(this.ClientSize, pixelSize: 20);
        BulletSpawner = new BulletSpawner(eventBus, bullets);
        PickupSpawner = new PickupSpawner(eventBus, pickups);
        PickupManager = new PickupManager(pickups);
        PickupRenderer = new PickupRenderer(ClientSize);
        player = new Player(eventBus) { weapon = new Weapon(eventBus) };
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
        this.SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {

        var dt = (float)args.Time;
        inputManager.Update(KeyboardState);
        inputManager.Update(MouseState);
        player.Update(inputManager, dt);

        BulletSpawner.Update(dt);
        PickupSpawner.Update(dt);
        PickupManager.Update(dt);
        if (this.KeyboardState.IsKeyDown(Keys.Escape)) this.Close();

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

        base.OnUpdateFrame(args);


        eventBus.Drain();
    }


    public override void Close()
    {
        base.Close();
    }
}


