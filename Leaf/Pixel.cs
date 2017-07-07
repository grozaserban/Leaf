using System.Collections.Generic;

namespace Leaf
{
    public abstract class Pixel
    {
        public enum Type { Color, Grayscale }
        public abstract Type GetKind();
    }

    public class ColorPixel : Pixel
    {
        public byte R { get; }
        public byte G { get; }
        public byte B { get; }

        public ColorPixel(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public GrayscalePixel ToGrayscale()
        {
            return new GrayscalePixel((byte)((R + G + B) / 3));
        }

        public override Type GetKind() => Type.Color;
    }

    public class GrayscalePixel : Pixel
    {
        public byte Value { get; }

        public GrayscalePixel(byte value)
        {
            Value = value;
        }

        public ColorPixel ToColor()
        {
            return new ColorPixel(Value, Value, Value);
        }

        public override Type GetKind() => Type.Grayscale;
    }

    public class PositionedPixel
    {
        public uint X { get; }
        public uint Y { get; }
        public byte Value { get; }

        public PositionedPixel(uint x, uint y, byte value)
        {
            X = x;
            Y = y;
            Value = value;
        }

        public ColorPixel ToColor()
        {
            return new ColorPixel(Value, Value, Value);
        }
    }

    public class PixelComparer : IComparer<GrayscalePixel>
    {
        public int Compare(GrayscalePixel x, GrayscalePixel y)
        {
            return x.Value.CompareTo(y.Value);
        }
    }
}