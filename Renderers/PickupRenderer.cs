
using OpenTK.Mathematics;

public class PickupRenderer : IDisposable
{
    public PickupRenderer(Vector2i screenSize)
    {
        Simple2D.Init(screenSize);
    }
    public void BeginFrame(Vector2i screenSize)
    {
        Simple2D.Begin(screenSize);
    }
    public void Render(PickupEntity p)
    {
        Simple2D.Quad(p.Position,
                      p.Size,
                    new Vector4(p.Color.R, p.Color.G, p.Color.B, p.Color.A),
                    p.Rotation
                    );
    }

    public void EndFrame()
    {
        Simple2D.Flush();
    }

    public void Dispose() => Simple2D.Dispose();
}
