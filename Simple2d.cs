using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
static class Simple2D
{
  static int vao, vbo, prog, uScreen;
  static readonly List<float> verts = new(); // [x,y,r,g,b,a] per vertex
  static Vector2 screen;

  public static void Init(Vector2i screenSize)
  {
    screen = screenSize;
    prog = MakeProgram(
    @"#version 330 core
          layout(location=0) in vec2 aPos;
          layout(location=1) in vec4 aCol;
          uniform vec2 uScreen;
          out vec4 vCol;
          void main(){
            vec2 ndc = (aPos/uScreen)*2.0 - 1.0;
            gl_Position = vec4(ndc*vec2(1,-1),0,1);
            vCol = aCol;
          }",
    @"#version 330 core
          in vec4 vCol; out vec4 FragColor;
          void main(){ FragColor = vCol; }");

    uScreen = GL.GetUniformLocation(prog, "uScreen");

    vao = GL.GenVertexArray();
    vbo = GL.GenBuffer();
    GL.BindVertexArray(vao);
    GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
    GL.BufferData(BufferTarget.ArrayBuffer, 1024, IntPtr.Zero, BufferUsageHint.DynamicDraw);

    int stride = (2 + 4) * sizeof(float);
    GL.EnableVertexAttribArray(0);
    GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, stride, 0);
    GL.EnableVertexAttribArray(1);
    GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, stride, 2 * sizeof(float));
  }

  public static void Begin(Vector2i screenSize) { screen = screenSize; verts.Clear(); }

  public static void Quad(Vector2 pos, Vector2 size, Vector4 color, float angle)
  => Quad(pos, size, color, angle, pos + size * 0.5f);
  public static void Quad(Vector2 pos, Vector2 size, Vector4 color, float angle, Vector2 origin)
  {
    float x = pos.X, y = pos.Y, w = size.X, h = size.Y;
    float c = MathF.Cos(angle), s = MathF.Sin(angle);

    static Vector2 Rot(Vector2 p, Vector2 o, float c, float s)
    {
      float dx = p.X - o.X, dy = p.Y - o.Y;
      return new Vector2(o.X + dx * c - dy * s, o.Y + dx * s + dy * c);
    }

    Vector2 tl = Rot(new Vector2(x, y), origin, c, s);
    Vector2 tr = Rot(new Vector2(x + w, y), origin, c, s);
    Vector2 br = Rot(new Vector2(x + w, y + h), origin, c, s);
    Vector2 bl = Rot(new Vector2(x, y + h), origin, c, s);

    // same triangle order as your original
    Push(tl.X, tl.Y, color); Push(tr.X, tr.Y, color); Push(br.X, br.Y, color);
    Push(tl.X, tl.Y, color); Push(br.X, br.Y, color); Push(bl.X, bl.Y, color);
  }
  public static void Quad(Vector2 pos, Vector2 size, Vector4 color)
  {
    float x = pos.X, y = pos.Y, w = size.X, h = size.Y;
    Push(x, y, color); Push(x + w, y, color); Push(x + w, y + h, color);
    Push(x, y, color); Push(x + w, y + h, color); Push(x, y + h, color);
  }

public static void Diamond(Vector2 pos, Vector2 size, Vector4 color)
{
    float cx = pos.X + size.X / 2f;
    float cy = pos.Y + size.Y / 2f;
    float hw = size.X / 2f;
    float hh = size.Y / 2f;

    // Four vertices (top, right, bottom, left)
    Vector2 top    = new Vector2(cx, cy - hh);
    Vector2 right  = new Vector2(cx + hw, cy);
    Vector2 bottom = new Vector2(cx, cy + hh);
    Vector2 left   = new Vector2(cx - hw, cy);

    // Triangles: top-right-bottom, top-bottom-left
    Push(top.X,    top.Y,    color);
    Push(right.X,  right.Y,  color);
    Push(bottom.X, bottom.Y, color);

    Push(top.X,    top.Y,    color);
    Push(bottom.X, bottom.Y, color);
    Push(left.X,   left.Y,   color);
}

  public static void Flush()
  {
    if (verts.Count == 0) return;
    GL.UseProgram(prog);
    GL.Uniform2(uScreen, screen);
    GL.BindVertexArray(vao);
    GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
    GL.BufferData(BufferTarget.ArrayBuffer, verts.Count * sizeof(float), verts.ToArray(), BufferUsageHint.DynamicDraw);
    GL.DrawArrays(PrimitiveType.Triangles, 0, verts.Count / 6);
    verts.Clear();
  }

  public static void Dispose()
  { GL.DeleteBuffer(vbo); GL.DeleteVertexArray(vao); GL.DeleteProgram(prog); }

  static void Push(float x, float y, Vector4 c)
  {
    verts.Add(x);
    verts.Add(y);
    verts.Add(c.X);
    verts.Add(c.Y);
    verts.Add(c.Z);
    verts.Add(c.W);
  }

  static int MakeProgram(string vsSrc, string fsSrc)
  {
    int vs = GL.CreateShader(ShaderType.VertexShader);
    GL.ShaderSource(vs, vsSrc); GL.CompileShader(vs); Check(vs);
    int fs = GL.CreateShader(ShaderType.FragmentShader);
    GL.ShaderSource(fs, fsSrc); GL.CompileShader(fs); Check(fs);
    int p = GL.CreateProgram();
    GL.AttachShader(p, vs); GL.AttachShader(p, fs); GL.LinkProgram(p);
    GL.DeleteShader(vs); GL.DeleteShader(fs);
    return p;
    static void Check(int s) { GL.GetShader(s, ShaderParameter.CompileStatus, out int ok); if (ok == 0) throw new Exception(GL.GetShaderInfoLog(s)); }
  }
}
