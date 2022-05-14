using System;
using OpenTK.Graphics.OpenGL;

namespace BasicOpenTK
{
    public sealed class VertexBuffer : IDisposable
    {
        public static readonly int MinVertexCount = 1;
        public static readonly int MaxVertexCount = 100_000;

        private bool disposed;

        public readonly int VertexBufferHandle;
        public readonly VertexInfo VertexInfo;
        public readonly int VertexCount;
        public readonly bool IsStatic;

        public VertexBuffer(VertexInfo vertexInfo, int vertexCount, bool isStatic = true)
        {
            this.disposed = false;

            if(vertexCount < VertexBuffer.MinVertexCount ||
                vertexCount > VertexBuffer.MaxVertexCount)
            {
                throw new ArgumentOutOfRangeException(nameof(vertexCount));
            }

            this.VertexInfo = vertexInfo;
            this.VertexCount = vertexCount;
            this.IsStatic = isStatic;

            BufferUsageHint hint = BufferUsageHint.StaticDraw;
            if(!this.IsStatic)
            {
                hint = BufferUsageHint.StreamDraw;
            }

            int vertexSizeInBytes = VertexPositionColor.VertexInfo.SizeInBytes;

            this.VertexBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.VertexBufferHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, this.VertexCount * this.VertexInfo.SizeInBytes, IntPtr.Zero, hint);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        ~VertexBuffer()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            if(this.disposed)
            {
                return;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(this.VertexBufferHandle);

            this.disposed = true;
            GC.SuppressFinalize(this);
        }

        public void SetData<T>(T[] data, int count) where T : struct
        {
            if(typeof(T) != this.VertexInfo.Type)
            {
                throw new ArgumentException("Generic type 'T' does not match the vertex type of the vertex buffer.");
            }

            if(data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if(data.Length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(data));
            }

            if(count <= 0||
                count > this.VertexCount||
                count > data.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, this.VertexBufferHandle);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, count * this.VertexInfo.SizeInBytes, data);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
