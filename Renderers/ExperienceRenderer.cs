
using OpenTK.Mathematics;

public class ExperienceRenderer : IDisposable
{
    private List<ExperienceEntity> _nuggets;

    public ExperienceRenderer(List<ExperienceEntity> nuggets)
    {
        _nuggets = nuggets;
    }

    public void BeginFrame(Vector2i screenSize)
    {
        Simple2D.Begin(screenSize);
    }
    public void Render()
    {
        foreach (var nugget in _nuggets.Where(x => x.Active))
        {
            Simple2D.Diamond(nugget.Position,
                          new Vector2(10.0f),
                          nugget.Color.ToVector());
        }
    }

    public void EndFrame()
    {
        Simple2D.Flush();
    }

    public void Dispose() => Simple2D.Dispose();
}
