namespace Leaf
{
    public class ColorPixel
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
    }

    public class PositionedPixel
    {
        public int X { get; }
        public int Y { get; }
        public byte Value { get; }

        public PositionedPixel(int x, int y, byte value)
        {
            X = x;
            Y = y;
            Value = value;
        }
    }
}