using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leaf
{
    public class Pixel
    {
        public uint X { get; set; }
        public uint Y { get; set; }
        public byte Value { get; set; }

        public Pixel(uint x, uint y, byte value)
        {
            X = x;
            Y = y;
            Value = value;
        }
    }

    public class PixelComparer : IComparer<Pixel>
    {
        public int Compare(Pixel x, Pixel y)
        {
            return x.Value.CompareTo(y.Value);
        }
    }
}
