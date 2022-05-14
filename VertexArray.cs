using System;
using OpenTK.Graphics.OpenGL;

namespace BasicOpenTK
{
    public sealed class VertexArray : IDisposable
    {
        private bool disposed;

        public readonly int VertexArrayHandle;
        public readonly VertexBuffer VertexBuffer;

        public VertexArray(VertexBuffer vertexBuffer)
        {
            this.disposed = false;

            if(vertexBuffer is null)
            {
                throw new ArgumentNullException(nameof(vertexBuffer));
            }

            this.VertexBuffer = vertexBuffer;

            int vertexSizeInBytes = this.VertexBuffer.VertexInfo.SizeInBytes;
            VertexAttribute[] attributes = this.VertexBuffer.VertexInfo.VertexAttributes;


            this.VertexArrayHandle = GL.GenVertexArray();
            GL.BindVertexArray(this.VertexArrayHandle);

            GL.BindBuffer(BufferTarget.ArrayBuffer, this.VertexBuffer.VertexBufferHandle);

            for(int i = 0; i < attributes.Length; i++)
            {
                VertexAttribute attribute = attributes[i];
                GL.VertexAttribPointer(attribute.Index, attribute.ComponentCount, VertexAttribPointerType.Float, false, vertexSizeInBytes, attribute.Offset);
                GL.EnableVertexAttribArray(attribute.Index);
            }

            GL.BindVertexArray(0);
        }

        ~VertexArray()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            if(this.disposed)
            {
                return;
            }

            GL.BindVertexArray(0);
            GL.DeleteVertexArray(this.VertexArrayHandle);

            this.disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
