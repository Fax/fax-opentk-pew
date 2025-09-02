using OpenTK.Mathematics;

public readonly struct ShootEvent : IGameEvent
{
    private readonly int id = IdManager.NextId();
    public readonly Vector2 Origin, Direction;
    public readonly float Speed;
    public readonly int BulletType;
    public ShootEvent(Vector2 origin, Vector2 direction, float speed, int bulletType)
    {
        Origin = origin;
        Direction = direction;
        Speed = speed;
        BulletType = bulletType;
    }

    int IGameEvent.EventID => id;
}


public readonly struct BulletCollisionEvent : IGameEvent
{
    private readonly int id = IdManager.NextId();
    int IGameEvent.EventID => id;
    public readonly int BulletId;
    public readonly int CollisionEntityId;
    public BulletCollisionEvent(int entityId, int collisionEntityId)
    {
        BulletId = entityId;
        CollisionEntityId = collisionEntityId;
    }
}

public readonly struct ScoreEvent : IGameEvent
{
    public readonly int Score = 10;
    private readonly int id = IdManager.NextId();
    int IGameEvent.EventID => id;
    public ScoreEvent(){}
}
public readonly struct WeaponPickupEvent : IGameEvent
{

    private readonly int id = IdManager.NextId();
    int IGameEvent.EventID => id;
    public readonly int BulletType = 0;
    public readonly int EntityId;
    public readonly int PlayerId;
    public WeaponPickupEvent(int btype, int entityId, int playerId)
    {
        EntityId = entityId;
        PlayerId = playerId;
        BulletType = btype;
    }
}