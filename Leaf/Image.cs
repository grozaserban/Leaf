using System.Diagnostics.Contracts;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;

namespace Leaf
{
    public class Image
    {
        private SoftwareBitmap _softwareBitmap;

        private WriteableBitmap _writeableBitmap;

        public int PixelHeight => _softwareBitmap.PixelHeight;

        public int PixelWidth => _softwareBitmap.PixelWidth;

        public WriteableBitmap Editor
        {
            get
            {
                return _writeableBitmap;
            }
            private set { }
        }

        public SoftwareBitmap SoftwareBitmap
        {
            get
            {
                SoftwareBitmap returnSoftwarebitmap = null;

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
            }
        }

        public int[,] Gradient { get; set; }

        public Image(SoftwareBitmap softwareBitmap)
        {
            Contract.Requires(softwareBitmap != null);
            SoftwareBitmap =softwareBitmap;
        }

        public Image(Image image)
        {
            Contract.Requires(image != null);
            SoftwareBitmap = image.SoftwareBitmap;
            Gradient = image.Gradient;
        }
    }
}