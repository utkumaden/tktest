using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace roguerunner
{
    public class RogueRunner
    {
        public GameWindow Window = new GameWindow(800, 600);
        public Player Player = new Player();
        public List<Room> Rooms;
        public int RoomIndex;
        public Random RNG = new Random();

        public int Points = 0;
        public int Lives = 3;

        public const float ClipTolarence = 0.01f;

        public const string VertexShader = @"#version 330
in vec2 position;
uniform mat4 matrix;

void main()
{
    gl_Position = vec4(position.x, position.y, 0, 1) * matrix;
}
";
        public const string FragmentShader = @"#version 330
out vec3 fragment;

void main()
{
    fragment = vec3(0.2,0.0,0.8);
}
";

        private int sp;
        private int tex_sprite;
        private int tex_platform;
        private int tex_bg;
        private int vbo;
        private int vao;

        public RogueRunner()
        {
            Window.Load += (s, a) => Load();
            Window.RenderFrame += (s, a) => Render();
            Window.UpdateFrame+= (s, a) => Update();
        }

        public unsafe void Load()
        {
            Window.Title = "RogueRunner";
            Player.State = PlayerState.Grounded;

            var vs = GL.CreateShader(ShaderType.VertexShader);
            var fs = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(vs, VertexShader);
            GL.CompileShader(vs);

            GL.ShaderSource(fs, FragmentShader);
            GL.CompileShader(fs);

            sp = GL.CreateProgram();
            GL.AttachShader(sp, vs);
            GL.AttachShader(sp, fs);
            GL.LinkProgram(sp);

            GL.DeleteShader(vs);
            GL.DeleteShader(fs);

            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

            var ar = RectangleToArray(Player.BoundingBox);
            fixed(float *pin = ar)
                GL.BufferData(BufferTarget.ArrayBuffer, ar.Length * sizeof(float), (IntPtr)pin, BufferUsageHint.DynamicDraw);
            
            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);
            GL.EnableVertexAttribArray(0);
        }

        public void Update()
        {
            var kbd = Keyboard.GetState();

            var dir = 0;
            if (kbd.IsKeyDown(Key.Left))
                dir = -1;
            else if (kbd.IsKeyDown(Key.Right))
                dir = 1;

            if (Player.Location.Y <= 0)
            {
                Player.Location.Y = 0;
                Player.Grounded = true;
            }
            else
            {
                Player.Grounded = false;
            }

            bool jumping = false;
            if (kbd.IsKeyDown(Key.Up))
            {
                if (Player.Grounded)
                {
                    Player.Jump();
                    Player.Grounded = false;
                }
                jumping = true;
            }

            Player.Upkeep(jumping, dir);
        }

        public unsafe void Render()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.UseProgram(sp);
            var mat = Matrix4.Identity * Matrix4.CreateTranslation(new Vector3(Player.Location.X, Player.Location.Y, 0));
            GL.UniformMatrix4(0, true, ref mat);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            Window.SwapBuffers();
        }

        private float[] RectangleToArray(RectangleF rectangle)
        {
            return new float[]
            {
                rectangle.X,                    rectangle.Y,
                rectangle.X + rectangle.Width,  rectangle.Y,
                rectangle.X + rectangle.Width,  rectangle.Y + rectangle.Height,
                rectangle.X,                    rectangle.Y,
                rectangle.X + rectangle.Width,  rectangle.Y + rectangle.Height,
                rectangle.X,                    rectangle.Y + rectangle.Height
            };
        }

        public void Run() => Window.Run(60);
    }
}
