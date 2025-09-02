
using System.Drawing;

using OpenTK.Mathematics;

public static class Color4Extensions
{
    public static Vector4 ToVector(this Color4 color)
    {
        return new Vector4(color.R, color.G, color.B, color.A);
    }
}

public static class Vector2Extensions
{
    public static Rectangle ToBoundingBox(this Vector2 origin, Vector2 size)
    {
        return new Rectangle((int)origin.X, (int)origin.Y, (int)size.X, (int)size.Y);
    }
    public static Rectangle ToCenteredBoundingBox(this Vector2 cornerOrigin, Vector2 entitySize, Vector2 size)
    {

        return new Rectangle((int)(cornerOrigin.X + entitySize.X / 2 - size.X / 2), (int)(cornerOrigin.Y + entitySize.Y / 2 - size.Y / 2), (int)size.X, (int)size.Y);
    }
}