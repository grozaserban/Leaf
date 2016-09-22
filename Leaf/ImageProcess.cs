using SimpleImageEditing;
using System;
using System.Diagnostics.Contracts;
using Windows.Graphics.Imaging;


namespace Leaf
{
    internal class ImageProcess
    {
        private SoftwareBitmap _softwareBitmap;
        private SoftwareBitmapEditor _editor;
        private int[,] _gradient;

        public ImageProcess()
        {
            _softwareBitmap = null;
            _editor = null;
        }

        public ImageProcess(SoftwareBitmap softwareBitmap)
        {
            _softwareBitmap = softwareBitmap;
            _editor = new SoftwareBitmapEditor(_softwareBitmap);
        }

        public SoftwareBitmap GetSoftwareBitmap()
        {
            SoftwareBitmap returnSoftwarebitmap = new SoftwareBitmap(_softwareBitmap.BitmapPixelFormat,
                                                                     _softwareBitmap.PixelWidth,
                                                                     _softwareBitmap.PixelHeight, _softwareBitmap.BitmapAlphaMode);
            _editor.Dispose();
            _softwareBitmap.CopyTo(returnSoftwarebitmap);
            _editor = new SoftwareBitmapEditor(_softwareBitmap);
            return returnSoftwarebitmap;
        }

        public void SetSoftwareBitmap(SoftwareBitmap softwareBitmap)
        {
            _softwareBitmap = new SoftwareBitmap(softwareBitmap.BitmapPixelFormat,
                                                 softwareBitmap.PixelWidth,
                                                 softwareBitmap.PixelHeight, softwareBitmap.BitmapAlphaMode);
            softwareBitmap.CopyTo(_softwareBitmap);
            _editor = new SoftwareBitmapEditor(_softwareBitmap);
        }

        public void Greenify()
        {
            for (uint y = 0; y < _editor.height; y++)
            {
                for (uint x = 0; x < _editor.width; x++)
                {
                    SoftwareBitmapPixel pixel = getPixel(x, y);
                    _editor.setPixel(x, y, pixel.r, (byte)Math.Min(pixel.g + 100, 255), pixel.b);
                }
            }
        }

        public void Grayscale()
        {
            for (uint y = 0; y < _editor.height; y++)
            {
                for (uint x = 0; x < _editor.width; x++)
                {
                    SoftwareBitmapPixel pixel = getPixel(x, y);
                    var gray = pixel.r + pixel.g + pixel.b;
                    gray = gray / 3;
                    _editor.setPixel(x, y, (byte)gray, (byte)gray, (byte)gray);
                }
            }
        }

        public void GaussianFilter()
        {
            var initBitmap = new SoftwareBitmap(_softwareBitmap.BitmapPixelFormat, _softwareBitmap.PixelWidth, _softwareBitmap.PixelHeight);
            _editor.Dispose();
            _softwareBitmap.CopyTo(initBitmap);
            _editor = new SoftwareBitmapEditor(_softwareBitmap);
            var initEditor = new SoftwareBitmapEditor(initBitmap);
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

            for (uint y = 2; y < _editor.height - 2; y++)
            {
                for (uint x = 2; x < _editor.width - 2; x++)
                {
                    aux = 0;
                    for (int yvar = -2; yvar < 3; yvar++)
                    {
                        for (int xvar = -2; xvar < 3; xvar++)
                        {
                            pixel = getPixel((uint)(x + xvar), (uint)(y + yvar), initEditor);
                            aux = aux + pixel.r * gaus[xvar + 2, yvar + 2];
                        }
                    }
                    aux = aux / gausSum;
                    _editor.setPixel(x, y, (byte)aux, (byte)aux, (byte)aux);
                }
            }
        }

        public void ComputeGradient()
        {
            var initBitmap = new SoftwareBitmap(_softwareBitmap.BitmapPixelFormat, _softwareBitmap.PixelWidth, _softwareBitmap.PixelHeight);
            _editor.Dispose();
            _softwareBitmap.CopyTo(initBitmap);
            _editor = new SoftwareBitmapEditor(_softwareBitmap);
            var initEditor = new SoftwareBitmapEditor(initBitmap);

            byte[,] magn = new byte[_editor.width, _editor.height];
            _gradient = new int[_editor.width, _editor.height];
            int magnX;
            int magnY;
            double max = 0;

            for (int i = 0; i < _editor.height; i++)
                for (int j = 0; j < _editor.width; j++)
                {
                    magn[j, i] = 0;
                    _gradient[j, i] = 0;
                }

            for (uint y = 1; y < _editor.height - 1; y++)
            {
                for (uint x = 1; x < _editor.width - 1; x++)
                {
                    magnX = getPixel(x, y + 1,initEditor).r - getPixel(x, y - 1,initEditor).r;
                    magnY = getPixel(x + 1, y,initEditor).r - getPixel(x - 1, y,initEditor).r;
                    magn[x, y] = Math.Sqrt(magnX * magnX + magnY * magnY) > 255 ? (byte)255 : (byte)Math.Sqrt(magnX * magnX + magnY * magnY);
                    _gradient[x, y] = (int)(Math.Atan2(magnY, magnX) * 180 / Math.PI);
                    _gradient[x, y] = _gradient[x, y] > 0 ? _gradient[x, y] : _gradient[x, y] + 360;
                    max = magn[x, y] > max ? magn[x, y] : max;
                }
            }

            max = 255 / max;
            for (uint y = 1; y < _editor.height - 1; y++)
            {
                for (uint x = 1; x < _editor.width - 1; x++)
                {
                    magn[x, y] = (byte)(magn[x, y] * max);
                    _editor.setPixel(x, y, magn[x, y], magn[x, y], magn[x, y]);
                }
            }
        }

        public void HistogramOfOrientedGradients()
        {
            Contract.Assert(_gradient != null, "gradient is not computed");

            SoftwareBitmapPixel[] colorPixel = HOGPixels.getHogPixels();

            var histogramSizeX = 60;
            var histogramSizeY = 60;
            byte[,,] gradHist = new byte[histogramSizeX, histogramSizeY, 10];
            int angle;
            uint sizex, sizey, binx, biny;

            sizex = (uint)(_editor.width / histogramSizeX + 1);
            sizey = (uint)(_editor.height / histogramSizeY + 1);

            for (int i = 0; i < (uint)histogramSizeX; i++)
                for (int j = 0; j < (uint)histogramSizeY; j++)
                    for (int k = 0; k < 9; k++)
                    {
                        gradHist[i, j, k] = 0;
                    }
            for (uint y = 0; y < _editor.height; y++)
            {
                for (uint x = 0; x < _editor.width; x++)
                {
                    binx = x / sizex;
                    biny = y / sizey;
                    angle = _gradient[x, y] / 40;
                    angle = angle >= 9 ? 0 : angle;
                    gradHist[binx, biny, angle]++;
                }
            }
            for (uint y = 0; y < (uint)histogramSizeX; y++)
            {
                for (uint x = 0; x < (uint)histogramSizeY; x++)
                {
                    var max = 0;
                    var maxindex = 0;
                    for (int k = 0; k < 9; k++)
                    {
                        if (gradHist[x, y, k] > max)
                        {
                            max = gradHist[x, y, k];
                            maxindex = k;
                        }
                    }
                    gradHist[x, y, 9] = (byte)maxindex;
                }
            }
            for (uint y = 0; y < _editor.height; y++)
            {
                for (uint x = 0; x < _editor.width; x++)
                {
                    binx = x / sizex;
                    biny = y / sizey;
                    var pixel = colorPixel[gradHist[binx, biny, 9]];
                    _editor.setPixel(x, y, pixel.r, pixel.g, pixel.b);               
                }
            }
        }

        public long Compare(ImageProcess imageProcess)
        { 
            if (imageProcess._editor.height != _editor.height)
                return 0;
            if (imageProcess._editor.width != _editor.width)
                return 0;
            //     uint height = (uint)Math.Min(_softwareBitmap.PixelHeight, softwareBitmap.PixelHeight);
            //     uint width = (uint)Math.Min(_softwareBitmap.PixelWidth, softwareBitmap.PixelWidth);
            var softwareBitmap = imageProcess.GetSoftwareBitmap();
            long score = 0;

            for (uint y = 0; y < _editor.height; y++)
            {
                for (uint x = 0; x < _editor.width; x++)
                {
                    SoftwareBitmapPixel pixel = getPixel(x, y);
                    SoftwareBitmapPixel secondpixel = imageProcess.getPixel(x, y);
                    score+=HOGPixels.comparePixels(pixel, secondpixel);  
                }
            }

            return score / HOGPixels.MaxScore;
        }

        private SoftwareBitmapPixel getPixel(uint x, uint y)
        {
            return getPixel(x, y, _editor);
        }

        private SoftwareBitmapPixel getPixel(uint x, uint y, SoftwareBitmapEditor editor)
        {
            var pixel = editor.getPixel(x, y);
            var aux = pixel.g;
            pixel.g = pixel.b;
            pixel.b = aux;
            return pixel;
        }
    }
}