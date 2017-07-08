using System;
using System.Diagnostics.Contracts;
using Windows.Foundation;
using Windows.Graphics.Imaging;

namespace Leaf
{
    internal static class ImageOperations
    {
        public static Image ToBlackAndWhite(this Image image, int tresholdPercent)
        {
            var treshold = image.Histogram().TresholdForPercent(tresholdPercent);

            for (int y = 0; y < image.PixelHeight; y++)
            {
                for (int x = 0; x < image.PixelWidth; x++)
                {
                    if (image.GrayScale.GetPixel(x, y) > treshold)
                    {
                        image.GrayScale.SetPixel(x, y, 255);
                    }
                    else
                    {
                        image.GrayScale.SetPixel(x, y, 0);
                    }
                }
            }
            return image;
        }

        public static int[] Histogram(this Image image)
        {
            int[] histogram = new int[257];

            for (int y = 0; y < image.PixelHeight; y++)
            {
                for (int x = 0; x < image.PixelWidth; x++)
                {
                    histogram[image.GrayScale.GetPixel(x, y)]++;
                }
            }

            histogram[256] = (int)(image.PixelHeight * image.PixelWidth);

            return histogram;
        }

        public static int TresholdForPercent(this int[] histogram, int percent)
        {
            Contract.Assert(percent <= 100);
            var percentInPoints = histogram[256] * percent / 100;

            int count = 0;
            for (int i = 255; i >= 0; i--)
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

            for (int y = 0; y < image.PixelHeight; y++)
            {
                for (int x = 0; x < image.PixelWidth; x++)
                {
                    pixels.Add(x, y, image.GrayScale.GetPixel(x, y));
                }
            }
            return pixels;
        }

        public static Image DrawMaxPoints(this Image image, int count, int range)
        {
            foreach (var pixel in image.ComputeMaxPoints(count, range))
            {
                image.GrayScale.SetPixel(pixel.X - 1, pixel.Y + 1, 255);
                image.GrayScale.SetPixel(pixel.X, pixel.Y + 1, 255);
                image.GrayScale.SetPixel(pixel.X + 1, pixel.Y + 1, 255);

                image.GrayScale.SetPixel(pixel.X - 1, pixel.Y, 255);
                image.GrayScale.SetPixel(pixel.X, pixel.Y, 255);
                image.GrayScale.SetPixel(pixel.X + 1, pixel.Y, 255);

                image.GrayScale.SetPixel(pixel.X - 1, pixel.Y - 1, 255);
                image.GrayScale.SetPixel(pixel.X, pixel.Y - 1, 255);
                image.GrayScale.SetPixel(pixel.X - 1, pixel.Y - 1, 255);
            }

            return image;
        }

        public static Image Normalize(this Image image)
        {
            var max = 0;
            for (int y = 0; y < image.PixelHeight; y++)
            {
                for (int x = 0; x < image.PixelWidth; x++)
                {
                    if (image.GrayScale.GetPixel(x, y) > max)
                        max = image.GrayScale.GetPixel(x, y);
                }
            }
            var normalizationFactor = (double)255 / max;

            for (int y = 0; y < image.PixelHeight; y++)
            {
                for (int x = 0; x < image.PixelWidth; x++)
                {
                    image.GrayScale.SetPixel(x, y, (byte)(image.GrayScale.GetPixel(x, y) * normalizationFactor));
                }
            }
            return image;
        }

        public static Image DeleteSquare(this Image image)
        {
            int heightMidle = (int)image.PixelHeight / 2;
            int widthMidle = (int)image.PixelWidth / 2;
            for (int y = 0; y < image.PixelHeight; y++)
            {
                if (image.GrayScale.GetPixel(widthMidle, y) > 10)
                {
                    for (int x = 0; x < image.PixelWidth; x++)
                    {
                        image.GrayScale.SetPixel(x, y, 0);
                        image.GrayScale.SetPixel(x, y + 1, 0);
                        image.GrayScale.SetPixel(x, y + 2, 0);
                        image.GrayScale.SetPixel(x, y + 3, 0);
                        image.GrayScale.SetPixel(x, y + 4, 0);
                    }
                    break;
                }
            }

            for (int y = (int)image.PixelHeight - 1; y > 0; y--)
            {
                if (image.GrayScale.GetPixel(widthMidle, y) > 10)
                {
                    for (int x = 0; x < image.PixelWidth; x++)
                    {
                        image.GrayScale.SetPixel(x, y, 0);
                        image.GrayScale.SetPixel(x, y - 1, 0);
                        image.GrayScale.SetPixel(x, y - 2, 0);
                        image.GrayScale.SetPixel(x, y - 3, 0);
                        image.GrayScale.SetPixel(x, y - 4, 0);
                    }
                    break;
                }
            }

            for (int x = 0; x < image.PixelWidth; x++)
            {
                if (image.GrayScale.GetPixel(x, heightMidle) > 10)
                {
                    for (int y = 0; y < image.PixelHeight; y++)
                    {
                        image.GrayScale.SetPixel(x, y, 0);
                        image.GrayScale.SetPixel(x + 1, y, 0);
                        image.GrayScale.SetPixel(x + 2, y, 0);
                        image.GrayScale.SetPixel(x + 3, y, 0);
                        image.GrayScale.SetPixel(x + 4, y, 0);
                    }
                    break;
                }
            }

            for (int x = (int)image.PixelWidth - 1; x > 0; x--)
            {
                if (image.GrayScale.GetPixel(x, heightMidle) > 10)
                {
                    for (int y = 0; y < image.PixelHeight; y++)
                    {
                        image.GrayScale.SetPixel(x, y, 0);
                        image.GrayScale.SetPixel(x - 1, y, 0);
                        image.GrayScale.SetPixel(x - 2, y, 0);
                        image.GrayScale.SetPixel(x - 3, y, 0);
                        image.GrayScale.SetPixel(x - 4, y, 0);
                    }
                    break;
                }
            }

            return image;
        }

        public static Image GaussianFilter(this Image image)
        {
            var initialImage = new Image(image);
            byte pixel;

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

            for (int y = 2; y < image.PixelHeight - 2; y++)
            {
                for (int x = 2; x < image.PixelWidth - 2; x++)
                {
                    aux = 0;
                    for (int yvar = -2; yvar < 3; yvar++)
                    {
                        for (int xvar = -2; xvar < 3; xvar++)
                        {
                            pixel = initialImage.GrayScale.GetPixel(x + xvar, y + yvar);
                            aux = aux + pixel * gaus[xvar + 2, yvar + 2];
                        }
                    }
                    aux = aux / gausSum;
                    image.GrayScale.SetPixel(x, y, (byte)aux);
                }
            }
            return image;
        }

        public static Image HoughFilter(this Image image)
        {
            // wrong filter
            var initialImage = new Image(image);
            byte pixel;

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
            for (int y = 2; y < image.PixelHeight - 2; y++)
            {
                for (int x = 2; x < image.PixelWidth - 2; x++)
                {
                    aux = 0;
                    for (int yvar = -2; yvar < 3; yvar++)
                    {
                        for (int xvar = -2; xvar < 3; xvar++)
                        {
                            pixel = initialImage.GrayScale.GetPixel(x + xvar, y + yvar);
                            aux = aux + pixel * gaus[xvar + 2, yvar + 2];
                        }
                    }
                    aux = aux / gausSum;
                    image.GrayScale.SetPixel(x, y, (byte)aux);
                }
            }
            return image;
        }

        public static Image Expand(this Image image)
        {
            var initialImage = new Image(image);

            for (int y = 1; y < image.PixelHeight - 1; y++)
            {
                for (int x = 1; x < image.PixelWidth - 1; x++)
                {
                    var neighbors =
                        initialImage.GrayScale.GetPixel(x - 1, y - 1) +
                        initialImage.GrayScale.GetPixel(x - 1, y) +
                        initialImage.GrayScale.GetPixel(x - 1, y + 1) +
                        initialImage.GrayScale.GetPixel(x, y - 1) +
                        initialImage.GrayScale.GetPixel(x, y) +
                        initialImage.GrayScale.GetPixel(x, y + 1) +
                        initialImage.GrayScale.GetPixel(x + 1, y - 1) +
                        initialImage.GrayScale.GetPixel(x + 1, y) +
                        initialImage.GrayScale.GetPixel(x + 1, y + 1);
                    image.GrayScale.SetPixel(x, y, (byte)(neighbors != 0 ? 255 : 0));
                }
            }
            return image;
        }

        public static Image Contraction(this Image image)
        {
            var initialImage = new Image(image);

            for (int y = 1; y < image.PixelHeight - 1; y++)
            {
                for (int x = 1; x < image.PixelWidth - 1; x++)
                {
                    var neighbors =
                        initialImage.GrayScale.GetPixel(x - 1, y - 1) +
                        initialImage.GrayScale.GetPixel(x - 1, y) +
                        initialImage.GrayScale.GetPixel(x - 1, y + 1) +
                        initialImage.GrayScale.GetPixel(x, y - 1) +
                        initialImage.GrayScale.GetPixel(x, y) +
                        initialImage.GrayScale.GetPixel(x, y + 1) +
                        initialImage.GrayScale.GetPixel(x + 1, y - 1) +
                        initialImage.GrayScale.GetPixel(x + 1, y) +
                        initialImage.GrayScale.GetPixel(x + 1, y + 1);
                    neighbors /= 255;
                    image.GrayScale.SetPixel(x, y, (byte)(neighbors == 9 ? 255 : 0));
                }
            }
            return image;
        }

        public static Image Salt(this Image image)
        {
            var initialImage = new Image(image);

            for (int y = 1; y < image.PixelHeight - 1; y++)
            {
                for (int x = 1; x < image.PixelWidth - 1; x++)
                {
                    if (initialImage.GrayScale.GetPixel(x, y) == 255)
                    {
                        var neighbors =
                            initialImage.GrayScale.GetPixel(x - 1, y) +
                            initialImage.GrayScale.GetPixel(x, y - 1) +
                            initialImage.GrayScale.GetPixel(x, y + 1) +
                            initialImage.GrayScale.GetPixel(x + 1, y);
                        neighbors /= 255;
                        image.GrayScale.SetPixel(x, y, (byte)(neighbors == 0 ? 0 : 255));
                    }
                }
            }
            return image;
        }

        public static Image EqualizeEdgesWidths(this Image image)
        {
            //not working properly
            var initialImage = new Image(image);

            for (int y = 1; y < image.PixelHeight - 1; y++)
            {
                for (int x = 1; x < image.PixelWidth - 1; x++)
                {
                    if (initialImage.GrayScale.GetPixel(x, y) == 255)
                    {
                        var widthDifference =
                            initialImage.GrayScale.GetPixel(x + 1, y) -
                            initialImage.GrayScale.GetPixel(x - 1, y);
                        var diagonalDifference =
                            initialImage.GrayScale.GetPixel(x + 1, y + 1) -
                            initialImage.GrayScale.GetPixel(x - 1, y - 1);
                        var heightDifference =
                            initialImage.GrayScale.GetPixel(x, y + 1) -
                            initialImage.GrayScale.GetPixel(x, y - 1);
                        var smallDiagonal =
                            initialImage.GrayScale.GetPixel(x - 1, y + 1) -
                            initialImage.GrayScale.GetPixel(x + 1, y - 1);

                        var count = 0;
                        count = widthDifference == 255 ? count++ : count;
                        count = diagonalDifference == 255 ? count++ : count;
                        count = heightDifference == 255 ? count++ : count;
                        count = smallDiagonal == 255 ? count++ : count;

                        if (count > 1)
                            image.GrayScale.SetPixel(x, y, 0);
                    }
                }
            }

            for (int y = 1; y < image.PixelHeight - 1; y++)
            {
                for (int x = 1; x < image.PixelWidth - 1; x++)
                {
                    if (initialImage.GrayScale.GetPixel(x, y) == 255)
                    {
                        var widthDifference =
                            initialImage.GrayScale.GetPixel(x - 1, y) -
                            initialImage.GrayScale.GetPixel(x + 1, y);
                        var diagonalDifference =
                            initialImage.GrayScale.GetPixel(x - 1, y - 1) -
                            initialImage.GrayScale.GetPixel(x + 1, y + 1);
                        var heightDifference =
                            initialImage.GrayScale.GetPixel(x, y - 1) -
                            initialImage.GrayScale.GetPixel(x, y + 1);
                        var smallDiagonal =
                            initialImage.GrayScale.GetPixel(x + 1, y - 1) -
                            initialImage.GrayScale.GetPixel(x - 1, y + 1);

                        var count = 0;
                        count = widthDifference == 255 ? count++ : count;
                        count = diagonalDifference == 255 ? count++ : count;
                        count = heightDifference == 255 ? count++ : count;
                        count = smallDiagonal == 255 ? count++ : count;

                        if (count > 1)
                            image.GrayScale.SetPixel(x, y, 0);
                    }
                }
            }
            return image;
        }

        public static Image ComputeGradient(this Image image)
        {
            var initialImage = new Image(image);

            byte[,] magn = new byte[image.PixelWidth, image.PixelHeight];
            int[,] gradient = new int[image.PixelWidth, image.PixelHeight];
            int magnX;
            int magnY;
            double max = 0;

            for (int i = 0; i < image.PixelHeight; i++)
                for (int j = 0; j < image.PixelWidth; j++)
                {
                    magn[j, i] = 0;
                    gradient[j, i] = 0;
                }

            for (int y = 1; y < image.PixelHeight - 1; y++)
            {
                for (int x = 1; x < image.PixelWidth - 1; x++)
                {
                    magnX = initialImage.GrayScale.GetPixel(x, y + 1) - initialImage.GrayScale.GetPixel(x, y - 1);
                    magnY = initialImage.GrayScale.GetPixel(x + 1, y) - initialImage.GrayScale.GetPixel(x - 1, y);
                    magn[x, y] = Math.Sqrt(magnX * magnX + magnY * magnY) > 255 ? (byte)255 : (byte)Math.Sqrt(magnX * magnX + magnY * magnY);
                    gradient[x, y] = (int)(Math.Atan2(magnY, magnX) * 180 / Math.PI);
                    gradient[x, y] = gradient[x, y] > 0 ? gradient[x, y] : gradient[x, y] + 360;
                    max = magn[x, y] > max ? magn[x, y] : max;
                }
            }

            max = 255 / max;
            for (int y = 1; y < image.PixelHeight - 1; y++)
            {
                for (int x = 1; x < image.PixelWidth - 1; x++)
                {
                    magn[x, y] = (byte)(magn[x, y] * max);
                    image.GrayScale.SetPixel(x, y, magn[x, y]);
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

            int maxP = (int)Math.Ceiling(Math.Sqrt(Math.Pow(image.PixelHeight, 2) + Math.Pow(image.PixelWidth, 2))) / deltaP;
            Point[] P = new Point[image.PixelHeight * image.PixelWidth];
            Image hough = new Image(new SoftwareBitmap(BitmapPixelFormat.Bgra8, maxTheta, maxP * 2, BitmapAlphaMode.Premultiplied)); //*2
            int[][] houghAux = new int[maxTheta][];
            int maxPixel = 0;

            for (int x = 0; x < hough.PixelWidth; x++)
            {
                houghAux[x] = new int[maxP * 2]; //*2
                for (int y = 0; y < hough.PixelHeight; y++)
                {
                    houghAux[x][y] = 0;
                }
            }
            //numaram punctele
            for (int y = 0; y < image.PixelHeight; y++)
                for (int x = 0; x < image.PixelWidth; x++)
                {
                    if (image.GrayScale.GetPixel(x, y) > 40)
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

            for (int x = 0; x < hough.PixelWidth; x++)
                for (int y = 0; y < hough.PixelHeight; y++)
                {
                    hough.GrayScale.SetPixel(x, y, (byte)(houghAux[x][y] * normalizationFactor));
                }

            return hough;
        }

        public static int[] HistogramOfOrientedGradients(this Image image, int blockCount, int bucketCount)
        {
            Contract.Assert(image != null);
            Contract.Assert(image.Gradient != null, "gradient is not computed");

            ColorPixel[] colorPixel = HOGPixels.getHogPixels();

            var histogramSizeX = blockCount;
            var histogramSizeY = blockCount;
            var bucketAngles = (int)Math.Ceiling((decimal)360 / bucketCount);
            byte[,,] gradHist = new byte[histogramSizeX, histogramSizeY, bucketCount + 1];
            int[] histogram = new int[bucketCount];
            int angle;
            int sizex, sizey, binx, biny;

            sizex = (int)(image.PixelWidth / histogramSizeX + 1);
            sizey = (int)(image.PixelHeight / histogramSizeY + 1);

            for (int i = 0; i < (int)histogramSizeX; i++)
                for (int j = 0; j < (int)histogramSizeY; j++)
                    for (int k = 0; k < 9; k++)
                    {
                        gradHist[i, j, k] = 0;
                    }
            // compute gradients for each angle
            for (int y = 0; y < image.PixelHeight; y++)
            {
                for (int x = 0; x < image.PixelWidth; x++)
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
            for (int y = 0; y < (int)histogramSizeX; y++)
            {
                for (int x = 0; x < (int)histogramSizeY; x++)
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
            for (int y = 0; y < image.PixelHeight; y++)
            {
                for (int x = 0; x < image.PixelWidth; x++)
                {
                    binx = x / sizex;
                    biny = y / sizey;
                    histogram[gradHist[binx, biny, bucketCount]] += image.GrayScale.GetPixel(x, y);
                }
            }

            return histogram;
        }

        public static double[] Normalize(this int[] array)
        {
            var max = 0;
            double[] normalizedValues = new double[array.Length];
            foreach (var value in array)
            {
                if (value > max)
                    max = value;
            }
            for (int i = 0; i < array.Length; i++)
                normalizedValues[i] = (double)array[i] / max;

            return normalizedValues;
        }

        public static Image DrawHistogramOfOrientedGradients(this Image image, int blockCount, int bucketCount)
        {
            Contract.Assert(image != null);
            Contract.Assert(image.Gradient != null, "gradient is not computed");
            Contract.Assert(bucketCount <= 9, "Cannot draw gradient for more than 9 buckets");

            ColorPixel[] colorPixel = HOGPixels.getHogPixels();

            var histogramSizeX = blockCount;
            var histogramSizeY = blockCount;
            var bucketAngles = 360 / bucketCount;
            byte[,,] gradHist = new byte[histogramSizeX, histogramSizeY, bucketCount + 1];
            int angle;
            int sizex, sizey, binx, biny;

            sizex = (int)(image.PixelWidth / histogramSizeX + 1);
            sizey = (int)(image.PixelHeight / histogramSizeY + 1);

            for (int i = 0; i < (int)histogramSizeX; i++)
                for (int j = 0; j < (int)histogramSizeY; j++)
                    for (int k = 0; k < 9; k++)
                    {
                        gradHist[i, j, k] = 0;
                    }
            for (int y = 0; y < image.PixelHeight; y++)
            {
                for (int x = 0; x < image.PixelWidth; x++)
                {
                    binx = x / sizex;
                    biny = y / sizey;
                    angle = image.Gradient[x, y] / bucketAngles;
                    angle = angle >= bucketCount ? 0 : angle;   // is this ok?
                    gradHist[binx, biny, angle]++;
                }
            }
            for (int y = 0; y < (int)histogramSizeX; y++)
            {
                for (int x = 0; x < (int)histogramSizeY; x++)
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
            for (int y = 0; y < image.PixelHeight; y++)
            {
                for (int x = 0; x < image.PixelWidth; x++)
                {
                    binx = x / sizex;
                    biny = y / sizey;
                    var pixel = colorPixel[gradHist[binx, biny, bucketCount]];
                    image.GrayScale.SetPixel(x, y, (byte)((pixel.R + pixel.G + pixel.B) / 3));
                }
            }

            return image;
        }

        //public static long MatchExactely(this Image first, Image second)
        //{
        //    if (AreNotTheSameSize(first.Editor, second.Editor))
        //        return 0;

        //    long score = 0;

        //    var equalityComparer = new PixelEqualityComparer();

        //    for (int y = 0; y < first.PixelHeight; y++)
        //    {
        //        for (int x = 0; x < first.PixelWidth; x++)
        //        {
        //            SoftwareBitmapPixel pixel = first.Editor.GetPixel(x, y);
        //            SoftwareBitmapPixel secondpixel = second.Editor.GetPixel(x, y);
        //            if (equalityComparer.Equals(pixel, secondpixel))
        //                score += 1;
        //        }
        //    }
        //    return score;
        //}

        //public static long Match(this Image first, Image second)
        //{
        //    if (AreNotTheSameSize(first.Editor, second.Editor))
        //        return 0;

        //    long score = 0;

        //    for (int y = 0; y < first.PixelHeight; y++)
        //    {
        //        for (int x = 0; x < first.PixelWidth; x++)
        //        {
        //            SoftwareBitmapPixel pixel = first.Editor.GetPixel(x, y);
        //            SoftwareBitmapPixel secondpixel = second.Editor.GetPixel(x, y);
        //            score += HOGPixels.comparePixels(pixel, secondpixel);
        //        }
        //    }
        //    return score / HOGPixels.MaxScore;
        //}

        //private static bool AreNotTheSameSize(SoftwareBitmapEditor first, SoftwareBitmapEditor second)
        //{
        //    return ((first.height != second.height) || (first.width != second.width));
        //}
    }
}