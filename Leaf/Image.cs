using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;

namespace Leaf
{
    public class Image
    {
        private SoftwareBitmap _softwareBitmap;

        private WriteableBitmap _writeableBitmap;

        private GrayScaleImage _grayscaleImage;

        public int PixelHeight => _softwareBitmap.PixelHeight;

        public int PixelWidth => _softwareBitmap.PixelWidth;

        private byte[] getBitmap(WriteableBitmap writeableBitmap)
        {
            var memoryStream = new MemoryStream();
            writeableBitmap.PixelBuffer.AsStream().CopyTo(memoryStream);
            return memoryStream.ToArray();
        }

        private void BitmapToWriteableBitmap(WriteableBitmap writeableBitmap, byte[] bitmap)
        {
            //writeableBitmap.PixelBuffer.AsStream().Flush();
            writeableBitmap.PixelBuffer.AsStream().Write(bitmap, 0, bitmap.Length);
        }

        public GrayScaleImage GrayScale
        {
            get
            {
                return _grayscaleImage;
            }
            private set { }
        }

        public SoftwareBitmap SoftwareBitmap
        {
            get
            {
                SoftwareBitmap returnSoftwarebitmap = null;

                _grayscaleImage.CopyToStream(_writeableBitmap.PixelBuffer.AsStream());

                _softwareBitmap.CopyFromBuffer(_writeableBitmap.PixelBuffer);
                returnSoftwarebitmap = new SoftwareBitmap(_softwareBitmap.BitmapPixelFormat,
                                                            _softwareBitmap.PixelWidth,
                                                            _softwareBitmap.PixelHeight, _softwareBitmap.BitmapAlphaMode);

                _softwareBitmap.CopyTo(returnSoftwarebitmap);

                return returnSoftwarebitmap;
            }
            set
            {
                Contract.Requires(value != null);
                _softwareBitmap = new SoftwareBitmap(value.BitmapPixelFormat,
                                             value.PixelWidth,
                                             value.PixelHeight, value.BitmapAlphaMode);
                value.CopyTo(_softwareBitmap);

                _writeableBitmap = new WriteableBitmap(_softwareBitmap.PixelWidth, _softwareBitmap.PixelHeight);
                _softwareBitmap.CopyToBuffer(_writeableBitmap.PixelBuffer);

                _grayscaleImage = new GrayScaleImage(_softwareBitmap.PixelWidth,
                                                     _softwareBitmap.PixelHeight,
                                                     _writeableBitmap.PixelBuffer.AsStream());
            }
        }

        public int[,] Gradient { get; set; }

        public Image(SoftwareBitmap softwareBitmap)
        {
            Contract.Requires(softwareBitmap != null);
            SoftwareBitmap = softwareBitmap;
        }

        public Image(Image image)
        {
            Contract.Requires(image != null);
            SoftwareBitmap = image.SoftwareBitmap;
            Gradient = image.Gradient;
        }
    }
}