using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;

namespace Leaf
{
    public class GrayScaleImage
    {
        private List<List<byte>> _image;
        private int _pixelHeight;
        private int _pixelWidth;

        public int PixelHeight => _pixelHeight;

        public int PixelWidth => _pixelWidth;

        public GrayScaleImage(int width, int height, Stream pixelStream)
        {
            Contract.Equals(width * height * 4, pixelStream.Length);
            _image = new List<List<byte>>();
            _pixelHeight = height;
            _pixelWidth = width;

            for (uint y = 0; y < height; y++)
            {
                var pixelLine = new List<byte>();
                for (uint x = 0; x < width; x++)
                {
                    var b = (byte)pixelStream.ReadByte();
                    var g = (byte)pixelStream.ReadByte();
                    var r = (byte)pixelStream.ReadByte();
                    var a = (byte)pixelStream.ReadByte();
                    pixelLine.Add((byte)((r + g + b)/3));
                }
                _image.Add(pixelLine);
            }
        }

        public byte GetPixel(int x, int y)
        {
            return _image[y][x];
        }

        public void SetPixel(int x, int y, byte pixel)
        {
            _image[y][x] = pixel;
        }

        public void CopyToStream(Stream stream)
        {
            stream.Flush();
            for (int y = 0; y < PixelHeight; y++)
            {
                for (int x = 0; x < PixelWidth; x++)
                {
                    stream.WriteByte(_image[y][x]);
                    stream.WriteByte(_image[y][x]);
                    stream.WriteByte(_image[y][x]);
                    stream.WriteByte(255);
                }
            }
        }
    }
}