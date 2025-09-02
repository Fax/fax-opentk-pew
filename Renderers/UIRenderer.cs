using OpenTK.Graphics.ES11;
using OpenTK.Mathematics;



class UIRenderer : IDisposable
{
    private Vector2i _screenSize;
    private readonly Player _player;

    public UIRenderer(Vector2i screenSize, Player player)
    {
        _screenSize = screenSize;
        _player = player;
        Simple2D.Init(screenSize);

    }
    public void BeginFrame(Vector2i screenSize)
    {
        Simple2D.Begin(screenSize);
    }
    int lastLevel = 0;
    int lastStart = 0;
    int lastCap = 0;
    public void Render()
    {
        Simple2D.Quad(new(10, _screenSize.Y - 20), new(_screenSize.X - 20, 10), Color4.AliceBlue.ToVector());

        var l = _player.Level;
        if (l != lastLevel || lastCap == 0)
        {
            lastLevel = _player.Level;
            lastCap = Levels.NextLevelExperience(l);
            lastStart = l == 0 ? 0 : Levels.NextLevelExperience(l - 1);
        }
        float exp = _player.Experience;

        Simple2D.Quad(new(10, _screenSize.Y - 20), new(_screenSize.X - 20, 10), Color4.AliceBlue.ToVector());
        // width
        float end = lastCap - lastStart;
        float fillPc = ((exp-lastStart) / end);
        Simple2D.Quad(new(10, _screenSize.Y - 20), new((_screenSize.X - 20) * fillPc, 10), Color4.BlueViolet.ToVector());
    }

    public void EndFrame()
    {
        Simple2D.Flush();
    }

    public void Dispose() => Simple2D.Dispose();
}