using SimpleImageEditing;
using System;

namespace Leaf
{
    internal static class HOGPixels
    {
        private static readonly SoftwareBitmapPixel[] _pixels = new SoftwareBitmapPixel[]
       {
                    new SoftwareBitmapPixel() { r = 255, g = 128, b = 0   },
                    new SoftwareBitmapPixel() { r = 255, g = 128, b = 0   },
                    new SoftwareBitmapPixel() { r = 255, g = 255, b = 0   },
                    new SoftwareBitmapPixel() { r = 0,   g = 255, b = 0   },
                    new SoftwareBitmapPixel() { r = 0,   g = 255, b = 128 },
                    new SoftwareBitmapPixel() { r = 0,   g = 255, b = 255 },
                    new SoftwareBitmapPixel() { r = 0,   g = 0,   b = 255 },
                    new SoftwareBitmapPixel() { r = 128, g = 0,   b = 255 },
                    new SoftwareBitmapPixel() { r = 255, g = 0,   b = 255 }
       };

        public static int MaxScore = 16;

        public static SoftwareBitmapPixel[] getHogPixels()
        {
            return _pixels;
        }

        private static int getPixelNumber(SoftwareBitmapPixel inputPixel)
        {
            var count = 0;
            foreach (var pixel in _pixels)
            {
                if ((inputPixel.r == pixel.r) && (inputPixel.g == pixel.g) && (inputPixel.b == pixel.b))
                    return count;
                count++;
            }
            return -100;
        }

        /// <summary>
        /// Returns a score from 0 to 4 representing the similarity of the two pixels
        /// </summary>
        /// <param name="pixel1"></param>
        /// <param name="pixel2"></param>
        /// <returns></returns>
        public static int comparePixels(SoftwareBitmapPixel pixel1, SoftwareBitmapPixel pixel2)
        {
            var pixel1Id = getPixelNumber(pixel1);
            var pixel2Id = getPixelNumber(pixel2);
            if (pixel1Id > pixel2Id)
            {
                var aux = pixel1Id;
                pixel1Id = pixel2Id;
                pixel2Id = aux;
            }
            if (pixel1Id == -100)
                return 0;
            var directDistance = pixel2Id - pixel1Id;
            var inverseDistance = 9 - pixel2Id + pixel1Id;
            var distance = Math.Min(directDistance, inverseDistance);
            return (int)Math.Pow(2, 4 - distance);
        }
    }
}