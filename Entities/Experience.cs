using System.Runtime.CompilerServices;
using OpenTK.Mathematics;

public class ExperienceEntity
{
    public int Amount = 100;
    public bool Active = true;
    public Vector2 Position;
    public float Speed = 0;


    public Color4 Color
    {
        get
        {
            return Amount switch
            {
                < 1000 => Color4.Blue,
                < 10000 => Color4.Purple,
                < 100000 => Color4.Green,
                _ => Color4.Blue
            };
        }
    }
}