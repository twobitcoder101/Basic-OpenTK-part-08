using System;
using OpenTK.Graphics.OpenGL;

namespace BasicOpenTK
{
    public sealed class IndexBuffer : IDisposable
    {
        public static readonly int MinIndexCount = 1;
        public static readonly int MaxIndexCount = 250_000;

        private bool disposed;

        public readonly int IndexBufferHandle;
        public readonly int IndexCount;
        public readonly bool IsStatic;

        public IndexBuffer(int indexCount, bool isStatic = true)
        {
            if(indexCount < IndexBuffer.MinIndexCount ||
                indexCount > IndexBuffer.MaxIndexCount)
            {
                throw new ArgumentOutOfRangeException(nameof(indexCount));
            }

            this.IndexCount = indexCount;
            this.IsStatic = isStatic;

            BufferUsageHint hint = BufferUsageHint.StaticDraw;
            if (!this.IsStatic)
            {
                hint = BufferUsageHint.StreamDraw;
            }

            this.IndexBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.IndexBufferHandle);
            GL.BufferData(BufferTarget.ElementArrayBuffer, this.IndexCount * sizeof(int), IntPtr.Zero, hint);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        ~IndexBuffer()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            if(this.disposed)
            {
                return;
            }

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.DeleteBuffer(this.IndexBufferHandle);

            this.disposed = true;
            GC.SuppressFinalize(this);
        }

        public void SetData(int[] data, int count)
        {
            if(data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if(data.Length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(data));
            }

            if(count <= 0 ||
                count > this.IndexCount ||
                count > data.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.IndexBufferHandle);
            GL.BufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, count * sizeof(int), data);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }
    }
}
