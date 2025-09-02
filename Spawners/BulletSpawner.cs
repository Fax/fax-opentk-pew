

using OpenTK.Mathematics;

public class BulletSpawner : ISpawner<Bullet>
{

    public Bullet CreateBullet(ShootEvent e)
    {
        switch (e.BulletType)
        {
            case 2:
                return new MagicalBullet
                {
                    Position = e.Origin,
                    Velocity = e.Direction * e.Speed,
                };
            case 1:
                return new ExplosiveBullet
                {
                    Position = e.Origin,
                    Velocity = e.Direction * e.Speed,
                    Color = new Vector4(.8f,.3f,.2f,.9f)
                };

            case 0:
            default:
                return new Bullet
                {
                    Position = e.Origin,
                    Velocity = e.Direction * e.Speed,
                    BulletType = e.BulletType
                };
        }
    }
    private readonly List<Bullet> _bullets;
    public BulletSpawner(EventBus bus, List<Bullet> bullets)
    {
        _bullets = bullets;
        bus.Subscribe<ShootEvent>(OnShoot);
    }
    void OnShoot(ShootEvent e)
        => _bullets.Add(CreateBullet(e));

    public void Update(float dt)
    {
        _bullets.ForEach(b =>
        {
            b.Life -= dt;
            b.Position += dt * b.Velocity;
        }); // decrease life

        _bullets.RemoveAll(b => b.Life <= 0);

    }
}