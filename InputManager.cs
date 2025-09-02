// See https://aka.ms/new-console-template for more information
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

public class InputManager
{
    public bool Up = false;
    public bool Down = false;
    public bool Left = false;
    public bool Right = false;
    public bool Space = false;
    public bool SpacePulse = false;
    public bool SpaceHold = false;
    public bool Esc = false;
    public bool LeftClick = false;
    public bool RightClick = false;
    public bool MiddleClick = false;
    public bool LeftHold = false;
    public bool RightHold = false;
    public bool MiddleHold = false;

    public bool Shoot = false;

    public Vector2 MousePosition;

    public void Update(KeyboardState state)
    {
        Up = state.IsKeyDown(Keys.Up) || state.IsKeyDown(Keys.W);
        Down = state.IsKeyDown(Keys.Down) || state.IsKeyDown(Keys.S);
        Left = state.IsKeyDown(Keys.Left) || state.IsKeyDown(Keys.A);
        Right = state.IsKeyDown(Keys.Right) || state.IsKeyDown(Keys.D);
        Space = state.IsKeyDown(Keys.Space);
        SpacePulse = state.IsKeyPressed(Keys.Space);
        SpaceHold = !SpacePulse && Space; // not just pulsed BUT still down
        Shoot = Shoot || SpacePulse;
    }
    public void Update(MouseState state)
    {
        LeftClick = state.IsButtonPressed(MouseButton.Left);
        RightClick = state.IsButtonPressed(MouseButton.Right);
        MiddleClick = state.IsButtonPressed(MouseButton.Middle);
        LeftHold = state.IsButtonDown(MouseButton.Left);
        RightHold = state.IsButtonDown(MouseButton.Right);
        MiddleHold = state.IsButtonDown(MouseButton.Middle);
        MousePosition = state.Position;
        Shoot = Shoot || LeftClick;
    }
}


