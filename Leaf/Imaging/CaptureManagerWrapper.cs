using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;

namespace Leaf
{
    public class MediaCaptureWrapper :IDisposable
    {
        public MediaCapture MediaCapture => _mediaCapture;
        private MediaCapture _mediaCapture;
        private LowLagPhotoCapture _lowLagPhotoCapture;

        private MediaCaptureWrapper()
        {
            _mediaCapture = new MediaCapture();
        }

        private async Task<bool> Initialize()
        {
            await _mediaCapture.InitializeAsync();
            _lowLagPhotoCapture = await _mediaCapture.PrepareLowLagPhotoCaptureAsync(GetLowestFormat());
            _mediaCapture.VideoDeviceController.Focus.TrySetAuto(true);
            return true;
        }
        /***
         * Source should be set
         ***/
        public async void StartPreview()
        {
            if (IsMobile)
                _mediaCapture.SetPreviewRotation(VideoRotation.Clockwise90Degrees);
            await _mediaCapture.StartPreviewAsync();
        }

        public async void Focus()
        {
            if (_mediaCapture.VideoDeviceController.FocusControl.Supported)
                await _mediaCapture.VideoDeviceController.FocusControl.FocusAsync();
        }

        public async Task<CapturedPhoto> GetCapture()
        {
            return await _lowLagPhotoCapture.CaptureAsync();
        }

        private static bool IsMobile
        {
            get
            {
                var qualifiers = Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().QualifierValues;
                return (qualifiers.ContainsKey("DeviceFamily") && qualifiers["DeviceFamily"] == "Mobile");
            }
        }

        private ImageEncodingProperties GetLowestFormat()
        {
            var format = ImageEncodingProperties.CreateUncompressed(MediaPixelFormat.Bgra8);

            var resolutions = _mediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.Photo).ToList();
            var resolutionObj = resolutions.Last();
            var resolution = resolutionObj as VideoEncodingProperties;

            format.Height = resolution.Height;
            format.Width = resolution.Width;
            return format;
        }

        public async void Dispose()
        {
            await _lowLagPhotoCapture.FinishAsync();
            _mediaCapture.Dispose();
        }

        public class MediaCaptureWrapperFactory
        {
            public async Task<MediaCaptureWrapper> Create()
            {
                var wrapper = new MediaCaptureWrapper();
                await wrapper.Initialize();
                return wrapper;

            }
        }
    }
}
