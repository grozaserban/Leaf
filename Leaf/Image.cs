using SimpleImageEditing;
using System.Diagnostics.Contracts;
using Windows.Graphics.Imaging;

namespace Leaf
{
    public class Image
    {
        private SoftwareBitmap _softwareBitmap;

        public SoftwareBitmapEditor Editor { get; set; }

        public int[,] Gradient { get; set; }

        public SoftwareBitmap SoftwareBitmap
        {
            get
            {
                SoftwareBitmap returnSoftwarebitmap = new SoftwareBitmap(_softwareBitmap.BitmapPixelFormat,
                                                             _softwareBitmap.PixelWidth,
                                                             _softwareBitmap.PixelHeight, _softwareBitmap.BitmapAlphaMode);
                Editor.Dispose();
                _softwareBitmap.CopyTo(returnSoftwarebitmap);
                Editor = new SoftwareBitmapEditor(_softwareBitmap);
                return returnSoftwarebitmap;
            }
            set
            {
                Contract.Requires(value != null);
                _softwareBitmap = new SoftwareBitmap(value.BitmapPixelFormat,
                                         value.PixelWidth,
                                         value.PixelHeight, value.BitmapAlphaMode);
                value.CopyTo(_softwareBitmap);
                Editor = new SoftwareBitmapEditor(_softwareBitmap);
            }
        }

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