using SimpleImageEditing;
using System;
using System.Collections.Generic;
namespace Leaf
{
    public static class PixelOperations
    {
        public static void SetPixelToGrayScale(SoftwareBitmapEditor editor, uint x, uint y)
        {
            SoftwareBitmapPixel pixel = getPixel(editor, x, y);
            var gray = pixel.r + pixel.g + pixel.b;
            gray = gray / 3;
            editor.setPixel(x, y, (byte)gray, (byte)gray, (byte)gray);
        }

        public static SoftwareBitmapPixel getPixel(SoftwareBitmapEditor editor, uint x, uint y)
        {
            var pixel = editor.getPixel(x, y);
            var aux = pixel.g;
            pixel.g = pixel.b;
            pixel.b = aux;
            return pixel;
        }

        public static SoftwareBitmapPixel getPixel(this Image Image, uint x, uint y)
        {
            return getPixel(Image.Editor, x, y);
        }

        public static void setPixel(this SoftwareBitmapEditor editor, uint x, uint y, byte value)
        {
            editor.setPixel(x, y, value, value, value);
        }

        public static void setPixel(this Image image, uint x, uint y, byte value)
        {
            image.Editor.setPixel(x, y, value, value, value);
        }
    }

    class PixelEqualityComparer : IEqualityComparer<SoftwareBitmapPixel>
    {
        public bool Equals(SoftwareBitmapPixel x, SoftwareBitmapPixel y)
        {
            return x.b == y.b &&
                x.g == y.g &&
                x.r == y.r;
        }

        public int GetHashCode(SoftwareBitmapPixel obj)
        {
            throw new NotImplementedException();
        }
    }
}
