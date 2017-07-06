using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Leaf
{
    public class SortedPixelList : IEnumerable<Pixel>
    {
        private List<Pixel> _pixels;

        private int _capacity;
        private int _range;
        private byte _minValue;

        public SortedPixelList(int capacity, int range)
        {
            _capacity = capacity;
            _minValue = 0;
            _range = range;
            _pixels = new List<Pixel>(capacity);
        }

        public void Add(uint x, uint y, byte value)
        {
            if ((value > _minValue) || (_pixels.Count < _capacity))
                Add(new Pixel(x, y, value));
        }

        public void Add(Pixel pixel)
        {
            _pixels.RemoveAll(ex => WeakNeighbour(ex, pixel));

            if (_pixels.Where(ex => StrongNeighbour(ex, pixel)).Any())
                return;

            if (_pixels.Count < _capacity)
                _pixels.Add(pixel);
            else
            {
                _pixels.RemoveAt(_capacity - 1);
                _pixels.Add(pixel);
            }
            _pixels = _pixels.OrderByDescending(p => p.Value).ToList();
            _minValue = _pixels[_pixels.Count - 1].Value;
        }

        private bool WeakNeighbour(Pixel ex, Pixel pixel)
        {
            return Neighour(ex, pixel) && ex.Value < pixel.Value;
        }

        private bool StrongNeighbour(Pixel ex, Pixel pixel)
        {
            return Neighour(ex, pixel) && ex.Value >= pixel.Value;
        }

        private bool Neighour(Pixel a, Pixel b)
        {
            var distX = Math.Abs((int)a.X - b.X);
            var distY = Math.Abs((int)a.Y - b.Y);

            return distX < _range && distY < _range;
        }

        public Pixel Get(int index)
        {
            return _pixels[index];
        }

        public string ToString(int width, int height)
        {
            string returnValue = string.Empty;

            foreach (var pixel in _pixels)
            {
                returnValue += (double)pixel.X/width + " " + (double)pixel.Y/height + " ";
            }

            return returnValue;
        }

        public IEnumerator<Pixel> GetEnumerator()
        {
            return ((IEnumerable<Pixel>)_pixels).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Pixel>)_pixels).GetEnumerator();
        }
    }
}