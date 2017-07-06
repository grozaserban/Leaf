using SimpleImageEditing;
using System;

namespace Leaf
{
    internal static class HOGPixels
    {
        private static readonly ColorPixel[] _pixels = new ColorPixel[]
       {
                    new ColorPixel(0,0 ,255, 128, 0  ),
                    new ColorPixel(0,0 ,255, 128, 0  ),
                    new ColorPixel(0,0 ,255, 255, 0  ),
                    new ColorPixel(0,0 ,0,   255, 0  ),
                    new ColorPixel(0,0 ,0,   255, 128),
                    new ColorPixel(0,0 ,0,   255, 255),
                    new ColorPixel(0,0 ,0,   0,   255),
                    new ColorPixel(0,0 ,128, 0,   255),
                    new ColorPixel(0,0 ,255, 0,   255)
       };

        public static int MaxScore = 16;

        public static ColorPixel[] getHogPixels()
        {
            return _pixels;
        }

        private static int getPixelNumber(ColorPixel inputPixel)
        {
            var count = 0;
            foreach (var pixel in _pixels)
            {
                if ((inputPixel.R == pixel.R) && (inputPixel.G == pixel.G) && (inputPixel.B == pixel.B))
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
        public static int comparePixels(ColorPixel pixel1, ColorPixel pixel2)
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