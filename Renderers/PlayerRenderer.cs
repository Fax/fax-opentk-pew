// See https://aka.ms/new-console-template for more information
using OpenTK.Mathematics;

class PlayerRenderer : IDisposable
{
    readonly Vector2 size;

    public PlayerRenderer(Vector2i screenSize, int pixelSize = 20)
    {
        size = new Vector2(pixelSize, pixelSize);
        Simple2D.Init(screenSize);
    }
    public void BeginFrame(Vector2i screenSize)
    {
        Simple2D.Begin(screenSize);
    }
    public void Render(Player p, Color4 color)
    {
        Simple2D.Quad(new Vector2(p.Position.X, p.Position.Y),
                      size,
                      new Vector4(color.R, color.G, color.B, color.A),
                      p.Rotation);
    }

    public void EndFrame()
    {
        Simple2D.Flush();
    }

    public void Dispose() => Simple2D.Dispose();
}


