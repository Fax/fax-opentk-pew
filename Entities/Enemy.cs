using System.Drawing;
using OpenTK.Graphics.ES11;
using OpenTK.Mathematics;

public class EnemyEntity
{
    public readonly int EntityId = IdManager.NextId();
    public float Life = 2.0f;
    public Vector2 Size = new(30f);
    public Color4 Color = Color4.Red;
    public float Speed = 10.0f;
    public Vector2 Position;
    public bool Active = true;
}
