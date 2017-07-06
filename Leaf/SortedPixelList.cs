﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Leaf
{
    public class SortedPixelList : IEnumerable<GrayscalePixel>
    {
        private List<GrayscalePixel> _pixels;

        private int _capacity;
        private int _range;
        private byte _minValue;

        public SortedPixelList(int capacity, int range)
        {
            _capacity = capacity;
            _minValue = 0;
            _range = range;
            _pixels = new List<GrayscalePixel>(capacity);
        }

        public void Add(uint x, uint y, byte value)
        {
            if ((value > _minValue) || (_pixels.Count < _capacity))
                Add(new GrayscalePixel(x, y, value));
        }

        public void Add(GrayscalePixel pixel)
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

        private bool WeakNeighbour(GrayscalePixel ex, GrayscalePixel pixel)
        {
            return Neighour(ex, pixel) && ex.Value < pixel.Value;
        }

        private bool StrongNeighbour(GrayscalePixel ex, GrayscalePixel pixel)
        {
            return Neighour(ex, pixel) && ex.Value >= pixel.Value;
        }

        private bool Neighour(GrayscalePixel a, GrayscalePixel b)
        {
            var distX = Math.Abs((int)a.X - b.X);
            var distY = Math.Abs((int)a.Y - b.Y);

            return distX < _range && distY < _range;
        }

        public GrayscalePixel Get(int index)
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

        public IEnumerator<GrayscalePixel> GetEnumerator()
        {
            return ((IEnumerable<GrayscalePixel>)_pixels).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<GrayscalePixel>)_pixels).GetEnumerator();
        }
    }
}