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

    public void Update(KeyboardState state, MouseState mouseState)
    {
        Up = state.IsKeyDown(Keys.Up) || state.IsKeyDown(Keys.W);
        Down = state.IsKeyDown(Keys.Down) || state.IsKeyDown(Keys.S);
        Left = state.IsKeyDown(Keys.Left) || state.IsKeyDown(Keys.A);
        Right = state.IsKeyDown(Keys.Right) || state.IsKeyDown(Keys.D);
        Space = state.IsKeyDown(Keys.Space);
        SpacePulse = state.IsKeyPressed(Keys.Space);
        SpaceHold = !SpacePulse && Space; // not just pulsed BUT still down


        LeftClick = mouseState.IsButtonPressed(MouseButton.Left);
        RightClick = mouseState.IsButtonPressed(MouseButton.Right);
        MiddleClick = mouseState.IsButtonPressed(MouseButton.Middle);
        LeftHold = mouseState.IsButtonDown(MouseButton.Left);
        RightHold = mouseState.IsButtonDown(MouseButton.Right);
        MiddleHold = mouseState.IsButtonDown(MouseButton.Middle);
        MousePosition = mouseState.Position;

        Shoot = SpacePulse || LeftClick;
    }
}


