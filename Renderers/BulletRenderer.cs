
using OpenTK.Mathematics;

public class BulletRenderer : IDisposable
{
    readonly Vector2 size;

    public BulletRenderer(Vector2i screenSize, int pixelSize = 20)
    {
        size = new Vector2(pixelSize, pixelSize);
        Simple2D.Init(screenSize);
    }
    public void BeginFrame(Vector2i screenSize)
    {
        Simple2D.Begin(screenSize);
    }
    public void Render(Bullet p)
    {
        Simple2D.Quad(p.Position,
                      p.Size,
                      p.Color);
    }

    public void EndFrame()
    {
        Simple2D.Flush();
    }

    public void Dispose() => Simple2D.Dispose();
}
