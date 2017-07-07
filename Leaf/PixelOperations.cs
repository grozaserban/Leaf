using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml.Media.Imaging;

namespace Leaf
{
    public static class PixelOperations
    {
        static int bytesPerPixel = 4;
        public static void SetPixelToGrayScale(WriteableBitmap editor, uint x, uint y)
        {
            var index = y * editor.PixelWidth * bytesPerPixel + x * bytesPerPixel;
            var pixels = editor.PixelBuffer.AsStream();

            pixels.Seek(index, System.IO.SeekOrigin.Begin);

            var b = (byte)pixels.ReadByte();
            var g = (byte)pixels.ReadByte();
            var r = (byte)pixels.ReadByte();
            //alfa
            var average = (byte)((r + g + b)/3);

            pixels.Seek(index, System.IO.SeekOrigin.Begin);
            pixels.WriteByte(average);
            pixels.WriteByte(average);
            pixels.WriteByte(average);
        }

        public static GrayscalePixel getPixel(WriteableBitmap editor, uint x, uint y)
        {
            var index = y * editor.PixelWidth * bytesPerPixel + x * bytesPerPixel;
            var pixels = editor.PixelBuffer.AsStream();

            pixels.Seek(index, System.IO.SeekOrigin.Begin);

            var b = (byte)pixels.ReadByte();

            return new GrayscalePixel(b);
        }

        public static GrayscalePixel getPixel(this Image Image, uint x, uint y)
        {
            return getPixel(Image.Editor, x, y);
        }

        public static void setPixel(this Image image, uint x, uint y, byte value)
        {
            image.Editor.setPixel(x, y, value);
        }

        public static void setPixel(this Image image, uint x, uint y, byte r, byte g, byte b)
        {
            image.Editor.setPixel(x, y, r, g, b);
        }

        private static void setPixel(this WriteableBitmap editor, uint x, uint y, byte value)
        {
            var index = y * editor.PixelWidth * bytesPerPixel + x * bytesPerPixel;
            var pixels = editor.PixelBuffer.AsStream();

            pixels.Seek(index, System.IO.SeekOrigin.Begin);
            pixels.WriteByte(value);
            pixels.WriteByte(value);
            pixels.WriteByte(value);
        }

        private static void setPixel(this WriteableBitmap editor, uint x, uint y, byte r, byte g, byte b)
        {
            var index = y * editor.PixelWidth * bytesPerPixel + x * bytesPerPixel;
            var pixels = editor.PixelBuffer.AsStream();

            pixels.Seek(index, System.IO.SeekOrigin.Begin);
            pixels.WriteByte(b);
            pixels.WriteByte(g);
            pixels.WriteByte(r);
        }
    }

    class PixelEqualityComparer : IEqualityComparer<GrayscalePixel>
    {
        public bool Equals(GrayscalePixel x, GrayscalePixel y)
        {
            return x.Value == y.Value;
        }

        public int GetHashCode(GrayscalePixel obj)
        {
            throw new NotImplementedException();
        }
    }
}
