// See https://aka.ms/new-console-template for more information
using System.Drawing;
using OpenTK.Mathematics;



public class Player
{
    public readonly int EntityId = IdManager.NextId();
    public Player(EventBus bus)
    {
        _bus = bus;
        _bus.Subscribe<WeaponPickupEvent>(PickUpWeapon);
    }
    public void PickUpWeapon(WeaponPickupEvent evt)
    {
        if (evt.PlayerId != EntityId) return;
        // do not subscribe again
        if (weapon == null)
        {
            weapon = new Weapon(_bus) { BulletType = evt.BulletType };
        }
    }
    public Vector2 Position = new Vector2(100, 100);

    public float Rotation = 0.0f;

    public Rectangle BoundingBox => new Rectangle
    (
        (int)Position.X,
        (int)Position.Y,
        20,
        20
    );
    public float Speed = 10f;

    public Weapon? weapon;
    private readonly EventBus _bus;

    public void Update(InputManager inputManager, float dt)
    {
        if (inputManager.Up)
        {
            Position.Y -= 10 * Speed * dt;
        }
        if (inputManager.Down)
        {
            Position.Y += 10 * Speed * dt;
        }
        if (inputManager.Right)
        {
            Position.X += 10 * Speed * dt;
        }
        if (inputManager.Left)
        {
            Position.X -= 10 * Speed * dt;
        }
        if (weapon != null)
        {
            if (inputManager.Shoot)
            {
                weapon.TryShoot(Position, inputManager.MousePosition - Position, 250.0f);
            }
            weapon.Update(dt);
        }

        Vector2 direction = (inputManager.MousePosition - Position).Normalized();
        float angle = MathF.Atan2(direction.X, -direction.Y);
        Rotation = angle;
    }
}


