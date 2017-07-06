using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leaf
{
    public class ColorPixel
    {
        public uint X { get; }
        public uint Y { get; }
        public byte R { get; }
        public byte G { get; }
        public byte B { get; }

        public ColorPixel(uint x, uint y, byte r, byte g, byte b)
        {
            X = x;
            Y = y;
            R = r;
            G = g;
            B = b;
        }

        public GrayscalePixel ToGrayscale()
        {
            return new GrayscalePixel(X, Y, (byte)((R + G + B) / 3));
        }
    }

    public class GrayscalePixel
    {
        public uint X { get; }
        public uint Y { get; }
        public byte Value { get; }

        public GrayscalePixel(uint x, uint y, byte value)
        {
            X = x;
            Y = y;
            Value = value;
        }

        public ColorPixel ToColor()
        {
            return new ColorPixel(X, Y, Value, Value, Value);
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
