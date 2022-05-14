using System;
using OpenTK;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace BasicOpenTK
{
    public class Game : GameWindow
    {
        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;
        private VertexArray vertexArray;
        private ShaderProgram shaderProgram;

        private int vertexCount;
        private int indexCount;

        private float colorFactor = 1f;
        private float deltaColorFactor = 1f / 240f;

        public Game(int width = 1280, int height = 768, string title = "Game1")
            : base(
                  GameWindowSettings.Default, 
                  new NativeWindowSettings()
                    {
                        Title = title,
                        Size = new Vector2i(width, height),
                        WindowBorder = WindowBorder.Fixed,
                        StartVisible = false,
                        StartFocused = true,
                        API = ContextAPI.OpenGL,
                        Profile = ContextProfile.Core,
                        APIVersion = new Version(3, 3)
                    })
        {
            this.CenterWindow();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            base.OnResize(e);
        }

        protected override void OnLoad()
        {
            this.IsVisible = true;

            //GL.ClearColor(new Color4(0.3f, 0.4f, 0.5f, 1f));
            GL.ClearColor(0.8f, 0.8f, 0.8f, 1f);

            Random rand = new Random();

            int windowWidth = this.ClientSize.X;
            int windowHeight = this.ClientSize.Y;

            int boxCount = 1_00;

            VertexPositionColor[] vertices = new VertexPositionColor[boxCount * 4];
            this.vertexCount = 0;

            for(int i = 0; i < boxCount; i++)
            {
                int w = rand.Next(32, 128);
                int h = rand.Next(32, 128);
                int x = rand.Next(0, windowWidth - w);
                int y = rand.Next(0, windowHeight - h);

                float r = (float)rand.NextDouble();
                float g = (float)rand.NextDouble();
                float b = (float)rand.NextDouble();

                vertices[this.vertexCount++] = new VertexPositionColor(new Vector2(x, y + h), new Color4(r, g, b, 1f));
                vertices[this.vertexCount++] = new VertexPositionColor(new Vector2(x + w, y + h), new Color4(r, g, b, 1f));
                vertices[this.vertexCount++] = new VertexPositionColor(new Vector2(x + w, y), new Color4(r, g, b, 1f));
                vertices[this.vertexCount++] = new VertexPositionColor(new Vector2(x, y), new Color4(r, g, b, 1f));
            }


            int[] indices = new int[boxCount * 6];
            this.indexCount = 0;
            this.vertexCount = 0;

            for(int i = 0; i < boxCount; i++)
            {
                indices[this.indexCount++] = 0 + this.vertexCount;
                indices[this.indexCount++] = 1 + this.vertexCount;
                indices[this.indexCount++] = 2 + this.vertexCount;
                indices[this.indexCount++] = 0 + this.vertexCount;
                indices[this.indexCount++] = 2 + this.vertexCount;
                indices[this.indexCount++] = 3 + this.vertexCount;

                this.vertexCount += 4;
            }


            this.vertexBuffer = new VertexBuffer(VertexPositionColor.VertexInfo, vertices.Length, true);
            this.vertexBuffer.SetData(vertices, vertices.Length);

            this.indexBuffer = new IndexBuffer(indices.Length, true);
            this.indexBuffer.SetData(indices, indices.Length);

            this.vertexArray = new VertexArray(this.vertexBuffer);





            string vertexShaderCode = 
                @"
                #version 330 core

                uniform vec2 ViewportSize;
                uniform float ColorFactor;

                layout (location = 0) in vec2 aPosition;
                layout (location = 1) in vec4 aColor;

                out vec4 vColor;

                void main()
                {
                    float nx = aPosition.x / ViewportSize.x * 2f - 1f;
                    float ny = aPosition.y / ViewportSize.y * 2f - 1f;
                    gl_Position = vec4(nx, ny, 0f, 1f);

                    vColor = aColor * ColorFactor;
                }
                ";

            string pixelShaderCode =
                @"
                #version 330 core

                in vec4 vColor;

                out vec4 pixelColor;

                void main()
                {
                    pixelColor = vColor;
                }
                ";

            this.shaderProgram = new ShaderProgram(vertexShaderCode, pixelShaderCode);



            int[] viewport = new int[4];
            GL.GetInteger(GetPName.Viewport, viewport);

            this.shaderProgram.SetUniform("ViewportSize", (float)viewport[2], (float)viewport[3]);
            this.shaderProgram.SetUniform("ColorFactor", this.colorFactor);


            base.OnLoad();
        }

        protected override void OnUnload()
        {
            this.vertexArray?.Dispose();
            this.indexBuffer?.Dispose();
            this.vertexBuffer?.Dispose();



            base.OnUnload();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            this.colorFactor += this.deltaColorFactor;

            if(this.colorFactor >= 1f)
            {
                this.colorFactor = 1f;
                this.deltaColorFactor *= -1f;
            }

            if(this.colorFactor <= 0f)
            {
                this.colorFactor = 0f;
                this.deltaColorFactor *= -1f;
            }

            this.shaderProgram.SetUniform("ColorFactor", this.colorFactor);

            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.UseProgram(this.shaderProgram.ShaderProgramHandle);
            GL.BindVertexArray(this.vertexArray.VertexArrayHandle);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexBuffer.IndexBufferHandle);
            GL.DrawElements(PrimitiveType.Triangles, this.indexCount, DrawElementsType.UnsignedInt, 0);

            this.Context.SwapBuffers();
            base.OnRenderFrame(args);
        }
    }
}
