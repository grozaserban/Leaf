using SimpleImageEditing;
using System;
using System.Diagnostics.Contracts;
using Windows.Foundation;
using Windows.Graphics.Imaging;

namespace Leaf
{
    internal static class ImageOperations
    {
        public static Image ToGrayScale(this Image image)
        {
            for (uint y = 0; y < image.Editor.height; y++)
            {
                for (uint x = 0; x < image.Editor.width; x++)
                {
                    PixelOperations.SetPixelToGrayScale(image.Editor, x, y);
                }
            }
            return image;
        }

        public static Image ToBlackAndWhite(this Image image, uint tresholdPercent)
        {
            var treshold = image.Histogram().TresholdForPercent(tresholdPercent);

            for (uint y = 0; y < image.Editor.height; y++)
            {
                for (uint x = 0; x < image.Editor.width; x++)
                {
                    if (image.getPixel(x, y).g > treshold)
                    {
                        image.setPixel(x, y, 255);
                    }
                    else
                    {
                        image.setPixel(x, y, 0);
                    }
                }
            }
            return image;
        }

        public static uint[] Histogram(this Image image)
        {
            uint[] histogram = new uint[257];

            for (uint y = 0; y < image.Editor.height; y++)
            {
                for (uint x = 0; x < image.Editor.width; x++)
                {
                    histogram[image.getPixel(x, y).g]++;
                }
            }

            histogram[256] = (uint)(image.Editor.height * image.Editor.width);

            return histogram;
        }

        public static uint TresholdForPercent(this uint[] histogram, uint percent)
        {
            Contract.Assert(percent <= 100);
            var percentInPoints = histogram[256] * percent / 100;

            uint count = 0;
            for (uint i = 255; i >= 0; i--)
            {
                count += histogram[i];
                if (count >= percentInPoints)
                    return i;
            }

            return 0;
        }

        public static SortedPixelList ComputeMaxPoints(this Image image, int count, int range)
        {
            var pixels = new SortedPixelList(count, range);

            for (uint y = 0; y < image.Editor.height; y++)
            {
                for (uint x = 0; x < image.Editor.width; x++)
                {
                    pixels.Add(x, y, image.getPixel(x, y).g);
                }
            }
            return pixels;
        }

        public static Image DrawMaxPoints(this Image image, int count, int range)
        {
            foreach (var pixel in image.ComputeMaxPoints(count, range))
            {
                image.Editor.setPixel(pixel.X, pixel.Y, 255, 0, 0);
            }

            return image;
        }

        public static Image Normalize(this Image image)
        {
            var max = 0;
            for (uint y = 0; y < image.Editor.height; y++)
            {
                for (uint x = 0; x < image.Editor.width; x++)
                {
                    if (image.getPixel(x, y).g > max)
                        max = image.getPixel(x, y).g;
                }
            }
            var normalizationFactor = (double)255 / max;

            for (uint y = 0; y < image.Editor.height; y++)
            {
                for (uint x = 0; x < image.Editor.width; x++)
                {
                    image.setPixel(x, y, (byte)(image.getPixel(x, y).g * normalizationFactor));
                }
            }
            return image;
        }

        public static Image DeleteSquare(this Image image)
        {
            uint heightMidle = (uint)image.Editor.height / 2;
            uint widthMidle = (uint)image.Editor.width / 2;
            for (uint y = 0; y < image.Editor.height; y++)
            {
                if (image.getPixel(widthMidle, y).g > 10)
                {
                    for (uint x = 0; x < image.Editor.width; x++)
                    {
                        image.setPixel(x, y, 0);
                        image.setPixel(x, y + 1, 0);
                        image.setPixel(x, y + 2, 0);
                        image.setPixel(x, y + 3, 0);
                        image.setPixel(x, y + 4, 0);
                    }
                    break;
                }
            }

            for (uint y = (uint)image.Editor.height - 1; y >= 0; y--)
            {
                if (image.getPixel(widthMidle, y).g > 10)
                {
                    for (uint x = 0; x < image.Editor.width; x++)
                    {
                        image.setPixel(x, y, 0);
                        image.setPixel(x, y - 1, 0);
                        image.setPixel(x, y - 2, 0);
                        image.setPixel(x, y - 3, 0);
                        image.setPixel(x, y - 4, 0);
                    }
                    break;
                }
            }

            for (uint x = 0; x < image.Editor.width; x++)
            {
                if (image.getPixel(x, heightMidle).g > 10)
                {
                    for (uint y = 0; y < image.Editor.width; y++)
                    {
                        image.setPixel(x, y, 0);
                        image.setPixel(x + 1, y, 0);
                        image.setPixel(x + 2, y, 0);
                        image.setPixel(x + 3, y, 0);
                        image.setPixel(x + 4, y, 0);
                    }
                    break;
                }
            }

            for (uint x = (uint)image.Editor.width - 1; x >= 0; x--)
            {
                if (image.getPixel(x, heightMidle).g > 10)
                {
                    for (uint y = 0; y < image.Editor.width; y++)
                    {
                        image.setPixel(x, y, 0);
                        image.setPixel(x - 1, y, 0);
                        image.setPixel(x - 2, y, 0);
                        image.setPixel(x - 3, y, 0);
                        image.setPixel(x - 4, y, 0);
                    }
                    break;
                }
            }

            return image;
        }

        public static Image GaussianFilter(this Image image)
        {
            var initialImage = new Image(image);
            SoftwareBitmapPixel pixel;

            double sigma = 0.8;
            int n = (int)(6 * sigma);
            double aux;
            double gausSum = 0;
            int p;

            if (n % 2 == 0) p = n + 1;
            else p = n;
            p = p / 2;

            // Compute the gaussian filter
            double[,] gaus = new double[5, 5]
                              { { 0, 0, 0, 0, 0 },
                                { 0, 0, 0, 0, 0 },
                                { 0, 0, 0, 0, 0 },
                                { 0, 0, 0, 0, 0 },
                                { 0, 0, 0, 0, 0 } };
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                {
                    gaus[i, j] = (1 / (2 * Math.PI * sigma * sigma)) * Math.Exp(-(((i - p) * (i - p) + (j - p) * (j - p)) / (2 * sigma * sigma)));
                    gausSum = gausSum + gaus[i, j];
                }

            for (uint y = 2; y < image.Editor.height - 2; y++)
            {
                for (uint x = 2; x < image.Editor.width - 2; x++)
                {
                    aux = 0;
                    for (int yvar = -2; yvar < 3; yvar++)
                    {
                        for (int xvar = -2; xvar < 3; xvar++)
                        {
                            pixel = initialImage.getPixel((uint)(x + xvar), (uint)(y + yvar));
                            aux = aux + pixel.r * gaus[xvar + 2, yvar + 2];
                        }
                    }
                    aux = aux / gausSum;
                    image.setPixel(x, y, (byte)aux);
                }
            }
            return image;
        }

        public static Image HoughFilter(this Image image)
        {
            // wrong filter
            var initialImage = new Image(image);
            SoftwareBitmapPixel pixel;

            double sigma = 0.8;
            int n = (int)(6 * sigma);
            double aux;
            double gausSum = 0;
            int p;

            if (n % 2 == 0) p = n + 1;
            else p = n;
            p = p / 2;

            // Compute the gaussian filter
            double[,] gaus = new double[5, 5]
                              { { 0, 0, 1, 0, 0 },
                                { 0, 3, 3, 3, 0 },
                                { 0, 2, 6, 2, 0 },
                                { 0, 3, 3, 3, 0 },
                                { 0, 0, 1, 0, 0 } };

            for (int yvar = 0; yvar < 5; yvar++)
            {
                for (int xvar = 0; xvar < 5; xvar++)
                {
                    gausSum += gaus[xvar, yvar];
                }
            }
            for (uint y = 2; y < image.Editor.height - 2; y++)
            {
                for (uint x = 2; x < image.Editor.width - 2; x++)
                {
                    aux = 0;
                    for (int yvar = -2; yvar < 3; yvar++)
                    {
                        for (int xvar = -2; xvar < 3; xvar++)
                        {
                            pixel = initialImage.getPixel((uint)(x + xvar), (uint)(y + yvar));
                            aux = aux + pixel.g * gaus[xvar + 2, yvar + 2];
                        }
                    }
                    aux = aux / gausSum;
                    image.setPixel(x, y, (byte)aux);
                }
            }
            return image;
        }

        public static Image Expand(this Image image)
        {
            var initialImage = new Image(image);

            for (uint y = 1; y < image.Editor.height - 1; y++)
            {
                for (uint x = 1; x < image.Editor.width - 1; x++)
                {
                    var neighbors =
                        initialImage.getPixel(x - 1, y - 1).g +
                        initialImage.getPixel(x - 1, y).g +
                        initialImage.getPixel(x - 1, y + 1).g +
                        initialImage.getPixel(x, y - 1).g +
                        initialImage.getPixel(x, y).g +
                        initialImage.getPixel(x, y + 1).g +
                        initialImage.getPixel(x + 1, y - 1).g +
                        initialImage.getPixel(x + 1, y).g +
                        initialImage.getPixel(x + 1, y + 1).g;
                    image.setPixel(x, y, (byte)(neighbors != 0 ? 255 : 0));
                }
            }
            return image;
        }

        public static Image Contraction(this Image image)
        {
            var initialImage = new Image(image);

            for (uint y = 1; y < image.Editor.height - 1; y++)
            {
                for (uint x = 1; x < image.Editor.width - 1; x++)
                {
                    var neighbors =
                        initialImage.getPixel(x - 1, y - 1).g +
                        initialImage.getPixel(x - 1, y).g +
                        initialImage.getPixel(x - 1, y + 1).g +
                        initialImage.getPixel(x, y - 1).g +
                        initialImage.getPixel(x, y).g +
                        initialImage.getPixel(x, y + 1).g +
                        initialImage.getPixel(x + 1, y - 1).g +
                        initialImage.getPixel(x + 1, y).g +
                        initialImage.getPixel(x + 1, y + 1).g;
                    neighbors /= 255;
                    image.setPixel(x, y, (byte)(neighbors == 9 ? 255 : 0));
                }
            }
            return image;
        }

        public static Image Salt(this Image image)
        {
            var initialImage = new Image(image);

            for (uint y = 1; y < image.Editor.height - 1; y++)
            {
                for (uint x = 1; x < image.Editor.width - 1; x++)
                {
                    if (initialImage.getPixel(x, y).g == 255)
                    {
                        var neighbors =
                            initialImage.getPixel(x - 1, y).g +
                            initialImage.getPixel(x, y - 1).g +
                            initialImage.getPixel(x, y + 1).g +
                            initialImage.getPixel(x + 1, y).g;
                        neighbors /= 255;
                        image.setPixel(x, y, (byte)(neighbors == 0 ? 0 : 255));
                    }
                }
            }
            return image;
        }

        public static Image EqualizeEdgesWidths(this Image image)
        {
            //not working properly
            var initialImage = new Image(image);

            for (uint y = 1; y < image.Editor.height - 1; y++)
            {
                for (uint x = 1; x < image.Editor.width - 1; x++)
                {
                    if (initialImage.getPixel(x, y).g == 255)
                    {
                        var widthDifference =
                            initialImage.getPixel(x + 1, y).g -
                            initialImage.getPixel(x - 1, y).g;
                        var diagonalDifference =
                            initialImage.getPixel(x + 1, y + 1).g -
                            initialImage.getPixel(x - 1, y - 1).g;
                        var heightDifference =
                            initialImage.getPixel(x, y + 1).g -
                            initialImage.getPixel(x, y - 1).g;
                        var smallDiagonal =
                            initialImage.getPixel(x - 1, y + 1).g -
                            initialImage.getPixel(x + 1, y - 1).g;

                        var count = 0;
                        count = widthDifference == 255 ? count++ : count;
                        count = diagonalDifference == 255 ? count++ : count;
                        count = heightDifference == 255 ? count++ : count;
                        count = smallDiagonal == 255 ? count++ : count;

                        if (count > 1)
                            image.setPixel(x, y, 0);
                    }
                }
            }

            for (uint y = 1; y < image.Editor.height - 1; y++)
            {
                for (uint x = 1; x < image.Editor.width - 1; x++)
                {
                    if (initialImage.getPixel(x, y).g == 255)
                    {
                        var widthDifference =
                            initialImage.getPixel(x - 1, y).g -
                            initialImage.getPixel(x + 1, y).g;
                        var diagonalDifference =
                            initialImage.getPixel(x - 1, y - 1).g -
                            initialImage.getPixel(x + 1, y + 1).g;
                        var heightDifference =
                            initialImage.getPixel(x, y - 1).g -
                            initialImage.getPixel(x, y + 1).g;
                        var smallDiagonal =
                            initialImage.getPixel(x + 1, y - 1).g -
                            initialImage.getPixel(x - 1, y + 1).g;

                        var count = 0;
                        count = widthDifference == 255 ? count++ : count;
                        count = diagonalDifference == 255 ? count++ : count;
                        count = heightDifference == 255 ? count++ : count;
                        count = smallDiagonal == 255 ? count++ : count;

                        if (count > 1)
                            image.setPixel(x, y, 0);
                    }
                }
            }
            return image;
        }

        public static Image ComputeGradient(this Image image)
        {
            var initialImage = new Image(image);

            byte[,] magn = new byte[image.Editor.width, image.Editor.height];
            int[,] gradient = new int[image.Editor.width, image.Editor.height];
            int magnX;
            int magnY;
            double max = 0;

            for (int i = 0; i < image.Editor.height; i++)
                for (int j = 0; j < image.Editor.width; j++)
                {
                    magn[j, i] = 0;
                    gradient[j, i] = 0;
                }

            for (uint y = 1; y < image.Editor.height - 1; y++)
            {
                for (uint x = 1; x < image.Editor.width - 1; x++)
                {
                    magnX = initialImage.getPixel(x, y + 1).r - initialImage.getPixel(x, y - 1).r;
                    magnY = initialImage.getPixel(x + 1, y).r - initialImage.getPixel(x - 1, y).r;
                    magn[x, y] = Math.Sqrt(magnX * magnX + magnY * magnY) > 255 ? (byte)255 : (byte)Math.Sqrt(magnX * magnX + magnY * magnY);
                    gradient[x, y] = (int)(Math.Atan2(magnY, magnX) * 180 / Math.PI);
                    gradient[x, y] = gradient[x, y] > 0 ? gradient[x, y] : gradient[x, y] + 360;
                    max = magn[x, y] > max ? magn[x, y] : max;
                }
            }

            max = 255 / max;
            for (uint y = 1; y < image.Editor.height - 1; y++)
            {
                for (uint x = 1; x < image.Editor.width - 1; x++)
                {
                    magn[x, y] = (byte)(magn[x, y] * max);
                    image.setPixel(x, y, magn[x, y]);
                }
            }

            image.Gradient = gradient;
            return image;
        }

        public static Image HoughGradient(this Image image, int deltaP, int deltaTheta)
        {
            int theta = 0;
            int maxTheta = 180;

            int n = 0;

            int maxP = (int)Math.Ceiling(Math.Sqrt(Math.Pow(image.Editor.height, 2) + Math.Pow(image.Editor.width, 2))) / deltaP;
            Point[] P = new Point[image.Editor.height * image.Editor.width];
            Image hough = new Image(new SoftwareBitmap(BitmapPixelFormat.Bgra8, maxTheta, maxP * 2, BitmapAlphaMode.Premultiplied)); //*2
            int[][] houghAux = new int[maxTheta][];
            int maxPixel = 0;

            for (uint x = 0; x < hough.Editor.width; x++)
            {
                houghAux[x] = new int[maxP * 2]; //*2
                for (uint y = 0; y < hough.Editor.height; y++)
                {
                    houghAux[x][y] = 0;
                }
            }
            //numaram punctele
            for (uint y = 0; y < image.Editor.height; y++)
                for (uint x = 0; x < image.Editor.width; x++)
                {
                    if (image.getPixel(x, y).g > 40)
                    {
                        P[n] = new Point(x, y);
                        n++;
                    }
                }
            var c1 = 0;
            var c2 = 0;
            var c3 = 0;
            for (int i = 0; i < n; i++)
            {
                for (theta = 0; theta < maxTheta; theta += deltaTheta)
                {
                    var thetaCount = theta / deltaTheta;
                    double p = (P[i].X * Math.Cos(Math.PI * theta / 180) + P[i].Y * Math.Sin(Math.PI * theta / 180)) / deltaP + maxP;
                    //      thetaCount = thetaCount % (180 / deltaTheta);
                    //       if ((p > 0) && (p < maxP))
                    if ((p >= 0) && (p <= 2 * maxP))
                    {
                        houghAux[thetaCount][(int)p] += 1;
                        if (houghAux[thetaCount][(int)p] > maxPixel)
                            maxPixel = houghAux[thetaCount][(int)p];
                        c1++;
                    }
                    else
                    {
                        c2++;
                        if (theta > 180 && theta < 270)
                            c3++;
                    }
                }
            }
            double normalizationFactor = (double)255 / maxPixel;

            for (uint x = 0; x < hough.Editor.width; x++)
                for (uint y = 0; y < hough.Editor.height; y++)
                {
                    hough.setPixel(x, y, (byte)(houghAux[x][y] * normalizationFactor));
                }

            return hough;
        }

        public static int[] HistogramOfOrientedGradients(this Image image, int blockCount, int bucketCount)
        {
            Contract.Assert(image != null);
            Contract.Assert(image.Gradient != null, "gradient is not computed");

            SoftwareBitmapPixel[] colorPixel = HOGPixels.getHogPixels();

            var histogramSizeX = blockCount;
            var histogramSizeY = blockCount;
            var bucketAngles = (int)Math.Ceiling((decimal)360 / bucketCount);
            byte[,,] gradHist = new byte[histogramSizeX, histogramSizeY, bucketCount + 1];
            int[] histogram = new int[bucketCount];
            int angle;
            uint sizex, sizey, binx, biny;

            sizex = (uint)(image.Editor.width / histogramSizeX + 1);
            sizey = (uint)(image.Editor.height / histogramSizeY + 1);

            for (int i = 0; i < (uint)histogramSizeX; i++)
                for (int j = 0; j < (uint)histogramSizeY; j++)
                    for (int k = 0; k < 9; k++)
                    {
                        gradHist[i, j, k] = 0;
                    }
            // compute gradients for each angle
            for (uint y = 0; y < image.Editor.height; y++)
            {
                for (uint x = 0; x < image.Editor.width; x++)
                {
                    binx = x / sizex;
                    biny = y / sizey;
                    angle = image.Gradient[x, y] / bucketAngles;
                    if (angle >= bucketCount)
                        angle = 0; 
                    gradHist[binx, biny, angle]++;
                }
            }
            //compute max gradient
            for (uint y = 0; y < (uint)histogramSizeX; y++)
            {
                for (uint x = 0; x < (uint)histogramSizeY; x++)
                {
                    var max = 0;
                    var maxindex = 0;
                    for (int k = 0; k < bucketCount; k++)
                    {
                        if (gradHist[x, y, k] > max)
                        {
                            max = gradHist[x, y, k];
                            maxindex = k;
                        }
                    }
                    gradHist[x, y, bucketCount] = (byte)maxindex;
                }
            }
            //compute histogram
            for (uint y = 0; y < image.Editor.height; y++)
            {
                for (uint x = 0; x < image.Editor.width; x++)
                {
                    binx = x / sizex;
                    biny = y / sizey;
                    histogram[gradHist[binx, biny, bucketCount]] += image.Editor.getPixel(x, y).g;
                }
            }

            return histogram;
        }

        public static double[] Normalize(this int[] array)
        {
            var max = 0;
            double[] normalizedValues = new double[array.Length];
            foreach(var value in array)
            {
                if (value > max)
                    max = value;
            }
            for (int i= 0; i< array.Length; i++)
                normalizedValues[i] = (double)array[i] / max;

            return normalizedValues;
        }

        public static Image DrawHistogramOfOrientedGradients(this Image image, int blockCount, int bucketCount)
        {
            Contract.Assert(image != null);
            Contract.Assert(image.Gradient != null, "gradient is not computed");

            SoftwareBitmapPixel[] colorPixel = HOGPixels.getHogPixels();

            var histogramSizeX = blockCount;
            var histogramSizeY = blockCount;
            var bucketAngles = 360 / bucketCount;
            byte[,,] gradHist = new byte[histogramSizeX, histogramSizeY, bucketCount + 1];
            int angle;
            uint sizex, sizey, binx, biny;

            sizex = (uint)(image.Editor.width / histogramSizeX + 1);
            sizey = (uint)(image.Editor.height / histogramSizeY + 1);

            for (int i = 0; i < (uint)histogramSizeX; i++)
                for (int j = 0; j < (uint)histogramSizeY; j++)
                    for (int k = 0; k < 9; k++)
                    {
                        gradHist[i, j, k] = 0;
                    }
            for (uint y = 0; y < image.Editor.height; y++)
            {
                for (uint x = 0; x < image.Editor.width; x++)
                {
                    binx = x / sizex;
                    biny = y / sizey;
                    angle = image.Gradient[x, y] / bucketAngles;
                    angle = angle >= bucketCount ? 0 : angle;   // is this ok?
                    gradHist[binx, biny, angle]++;
                }
            }
            for (uint y = 0; y < (uint)histogramSizeX; y++)
            {
                for (uint x = 0; x < (uint)histogramSizeY; x++)
                {
                    var max = 0;
                    var maxindex = 0;
                    for (int k = 0; k < bucketCount; k++)
                    {
                        if (gradHist[x, y, k] > max)
                        {
                            max = gradHist[x, y, k];
                            maxindex = k;
                        }
                    }
                    gradHist[x, y, bucketCount] = (byte)maxindex;
                }
            }
            for (uint y = 0; y < image.Editor.height; y++)
            {
                for (uint x = 0; x < image.Editor.width; x++)
                {
                    binx = x / sizex;
                    biny = y / sizey;
                    var pixel = colorPixel[gradHist[binx, biny, bucketCount]];
                    image.Editor.setPixel(x, y, pixel.r, pixel.g, pixel.b);
                }
            }

            return image;
        }

        public static long MatchExactely(this Image first, Image second)
        {
            if (AreNotTheSameSize(first.Editor, second.Editor))
                return 0;

            long score = 0;

            var equalityComparer = new PixelEqualityComparer();

            for (uint y = 0; y < first.Editor.height; y++)
            {
                for (uint x = 0; x < first.Editor.width; x++)
                {
                    SoftwareBitmapPixel pixel = first.getPixel(x, y);
                    SoftwareBitmapPixel secondpixel = second.getPixel(x, y);
                    if (equalityComparer.Equals(pixel, secondpixel))
                        score += 1;
                }
            }
            return score;
        }

        public static long Match(this Image first, Image second)
        {
            if (AreNotTheSameSize(first.Editor, second.Editor))
                return 0;

            long score = 0;

            for (uint y = 0; y < first.Editor.height; y++)
            {
                for (uint x = 0; x < first.Editor.width; x++)
                {
                    SoftwareBitmapPixel pixel = first.getPixel(x, y);
                    SoftwareBitmapPixel secondpixel = second.getPixel(x, y);
                    score += HOGPixels.comparePixels(pixel, secondpixel);
                }
            }
            return score / HOGPixels.MaxScore;
        }

        private static bool AreNotTheSameSize(SoftwareBitmapEditor first, SoftwareBitmapEditor second)
        {
            return ((first.height != second.height) || (first.width != second.width));
        }
    }
}