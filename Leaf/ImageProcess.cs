using SimpleImageEditing;
using System;
using System.Diagnostics.Contracts;
using Windows.Graphics.Imaging;

namespace Leaf
{
    internal class ImageProcess
    {
        private SoftwareBitmap _softwareBitmap;
        private int[,] _gradient;

        public SoftwareBitmapEditor Editor { get; set; }

        public ImageProcess()
        {
            _softwareBitmap = null;
            Editor = null;
        }

        public ImageProcess(SoftwareBitmap softwareBitmap)
        {
            _softwareBitmap = softwareBitmap;
            Editor = new SoftwareBitmapEditor(_softwareBitmap);
        }

        public SoftwareBitmap GetSoftwareBitmap()
        {
            SoftwareBitmap returnSoftwarebitmap = new SoftwareBitmap(_softwareBitmap.BitmapPixelFormat,
                                                                     _softwareBitmap.PixelWidth,
                                                                     _softwareBitmap.PixelHeight, _softwareBitmap.BitmapAlphaMode);
            Editor.Dispose();
            _softwareBitmap.CopyTo(returnSoftwarebitmap);
            Editor = new SoftwareBitmapEditor(_softwareBitmap);
            return returnSoftwarebitmap;
        }

        public void SetSoftwareBitmap(SoftwareBitmap softwareBitmap)
        {
            _softwareBitmap = new SoftwareBitmap(softwareBitmap.BitmapPixelFormat,
                                                 softwareBitmap.PixelWidth,
                                                 softwareBitmap.PixelHeight, softwareBitmap.BitmapAlphaMode);
            softwareBitmap.CopyTo(_softwareBitmap);
            Editor = new SoftwareBitmapEditor(_softwareBitmap);
        }

        private SoftwareBitmapPixel getPixel(uint x, uint y)
        {
            return Editor.getPixel(x, y);
        }
    }


    /* var lowLagCapture = await captureManager.PrepareLowLagPhotoCaptureAsync(
                                                 ImageEncodingProperties.CreateUncompressed(MediaPixelFormat.Bgra8));
 var capturedPhoto = await lowLagCapture.CaptureAsync();
 var softwareBitmap = capturedPhoto.Frame.SoftwareBitmap;
 ImageProcess capture = new ImageProcess(softwareBitmap);
 */

    /*
var source = new SoftwareBitmapSource();
SetSoftwareBitmapSource(capture.GetSoftwareBitmap(), source);
editPreview.Source = source;
await Task.Delay(TimeSpan.FromSeconds(3));
capture.Check();*/
}