using OpenTK.Mathematics;

public class EnemyRenderer : IDisposable
{
    private List<EnemyEntity> _enemies;

    public EnemyRenderer(List<EnemyEntity> enemies)
    {
        _enemies = enemies;
    }

    public void BeginFrame(Vector2i screenSize)
    {
        Simple2D.Begin(screenSize);
    }
    public void Render()
    {
        foreach (var enemy in _enemies.Where(x => x.Active))
        {

            Simple2D.Quad(enemy.Position,
                          enemy.Size,
                          enemy.Color.ToVector());
        }
    }

    public void EndFrame()
    {
        Simple2D.Flush();
    }

    public void Dispose() => Simple2D.Dispose();
}
