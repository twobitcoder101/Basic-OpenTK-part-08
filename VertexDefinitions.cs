using System;
using OpenTK.Mathematics;

namespace BasicOpenTK
{
    public readonly struct VertexAttribute
    {
        public readonly string Name;
        public readonly int Index;
        public readonly int ComponentCount;
        public readonly int Offset;

        public VertexAttribute(string name, int index, int componentCount, int offset)
        {
            this.Name = name;
            this.Index = index;
            this.ComponentCount = componentCount;
            this.Offset = offset;
        }
    }

    public sealed class VertexInfo
    {
        public readonly Type Type;
        public readonly int SizeInBytes;
        public readonly VertexAttribute[] VertexAttributes;

        public VertexInfo(Type type, params VertexAttribute[] attributes)
        {
            this.Type = type;
            this.SizeInBytes = 0;

            this.VertexAttributes = attributes;

            for(int i = 0; i < this.VertexAttributes.Length; i++)
            {
                VertexAttribute attribute = this.VertexAttributes[i];
                this.SizeInBytes += attribute.ComponentCount * sizeof(float);
            }
        }
    }


    public readonly struct VertexPositionColor
    {
        public readonly Vector2 Position;
        public readonly Color4 Color;

        public static readonly VertexInfo VertexInfo = new VertexInfo(
            typeof(VertexPositionColor),
            new VertexAttribute("Position", 0, 2, 0),
            new VertexAttribute("Color", 1, 4, 2 * sizeof(float))
            );

        public VertexPositionColor(Vector2 position, Color4 color)
        {
            this.Position = position;
            this.Color = color;
        }
    }

    public readonly struct VertexPositionTexture
    {
        public readonly Vector2 Position;
        public readonly Vector2 TexCoord;

        public static readonly VertexInfo VertexInfo = new VertexInfo(
            typeof(VertexPositionTexture),
            new VertexAttribute("Positon", 0, 2, 0),
            new VertexAttribute("TexCoord", 1, 2, 2 * sizeof(float))
            );

        public VertexPositionTexture(Vector2 position, Vector2 texCoord)
        {
            this.Position = position;
            this.TexCoord = texCoord;
        }
    }
}
