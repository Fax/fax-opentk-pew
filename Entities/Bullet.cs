using OpenTK.Mathematics;

public class Bullet
{
    public Vector2 Position, Velocity;
    public float Life = 2.0f;
    public int BulletType = 0;

    public Vector4 Color = new(1.0f);
    public Vector2 Size = new(10, 10);
    public float Friction = 0.0f;
}


public class ExplosiveBullet : Bullet
{
    public ExplosiveBullet()
    {
        BulletType = 1;
        Friction = 10.5f;
    }
}

public class MagicalBullet : Bullet
{
    public MagicalBullet()
    {
        BulletType = 2;
        Color = new(.4f, .1f, 1.0f, 1.0f);
    }
}