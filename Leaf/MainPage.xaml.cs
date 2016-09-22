using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;      //For MediaCapture
using Windows.Media.MediaProperties;  //For Encoding Image in JPEG format
using Windows.Storage;         //For storing Capture Image in App storage or in Picture Library
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Leaf
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MediaCapture captureManager;

        private int focus = 10;

        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            captureManager = new MediaCapture();    //Define MediaCapture object
                                                    //    MediaCaptureInitializationSettings mediaCaptureSettings;

            await captureManager.InitializeAsync();   //Initialize MediaCapture and
            capturePreview.Source = captureManager; 
            //Start previewing on CaptureElement
            await captureManager.StartPreviewAsync();  //Start camera capturing

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private async void Open_Photo(object sender, RoutedEventArgs e)
        {
            ImageProcess image = new ImageProcess();
            image.SetSoftwareBitmap(await ImageIO.LoadSoftwareBitmapFromFile());

            var source = new SoftwareBitmapSource();
            var timeSpan = TimeSpan.FromMilliseconds(500);

            SetSoftwareBitmapSource(image.GetSoftwareBitmap(), source);
            editPreview.Source = source;
            await Task.Delay(timeSpan);

            image.Grayscale();
            SetSoftwareBitmapSource(image.GetSoftwareBitmap(), source);

            await Task.Delay(timeSpan);

            image.GaussianFilter();
            SetSoftwareBitmapSource(image.GetSoftwareBitmap(), source);
            await Task.Delay(timeSpan);

            image.ComputeGradient();
            SetSoftwareBitmapSource(image.GetSoftwareBitmap(), source);
            await Task.Delay(timeSpan);

            image.HistogramOfOrientedGradients();
            SetSoftwareBitmapSource(image.GetSoftwareBitmap(), source);
            await Task.Delay(timeSpan);

            ImageIO.SaveSoftwareBitmapToFile(image.GetSoftwareBitmap());
        }

        private async void LoadImages(object sender, RoutedEventArgs e)
        {
            var storageFolder = await KnownFolders.PicturesLibrary.GetFolderAsync("Leaf");
            var storageFiles = await storageFolder.GetFilesAsync(Windows.Storage.Search.CommonFileQuery.OrderByDate);
            var image = new ImageProcess();
            foreach (var storageFile in storageFiles)
            {
                image.SetSoftwareBitmap(await ImageIO.LoadSoftwareBitmapFromFile(storageFile));
                image.Grayscale();
                image.GaussianFilter();
                image.ComputeGradient();
                image.HistogramOfOrientedGradients();

                var file = await KnownFolders.PicturesLibrary.CreateFileAsync("HOGleaf.bmp", CreationCollisionOption.GenerateUniqueName);
                await ImageIO.SaveSoftwareBitmapToFile(image.GetSoftwareBitmap(), file);
            }
        }

        private async void MatchImage(object sender, RoutedEventArgs e)
        {
            var storageFolder = await KnownFolders.PicturesLibrary.GetFolderAsync("HOG60");
            var storageFiles = await storageFolder.GetFilesAsync(Windows.Storage.Search.CommonFileQuery.OrderByDate);
            /* var lowLagCapture = await captureManager.PrepareLowLagPhotoCaptureAsync(
                                                             ImageEncodingProperties.CreateUncompressed(MediaPixelFormat.Bgra8));
             var capturedPhoto = await lowLagCapture.CaptureAsync();
             var softwareBitmap = capturedPhoto.Frame.SoftwareBitmap;
             ImageProcess capture = new ImageProcess(softwareBitmap);
             */
            ImageProcess capture = new ImageProcess();
            capture.SetSoftwareBitmap(await ImageIO.LoadSoftwareBitmapFromFile());
            capture.Grayscale();
            capture.GaussianFilter();
            capture.ComputeGradient();
            capture.HistogramOfOrientedGradients();
            ComputeScores(storageFiles, capture);
            //  lowLagCapture.FinishAsync();
            /*
          var source = new SoftwareBitmapSource();
          SetSoftwareBitmapSource(capture.GetSoftwareBitmap(), source);
          editPreview.Source = source;
          await Task.Delay(TimeSpan.FromSeconds(3));
          capture.Check();*/
            var source = new SoftwareBitmapSource();
            SetSoftwareBitmapSource(capture.GetSoftwareBitmap(), source);
            editPreview.Source = source;
        }

        private async void Capture_Photo_Click(object sender, RoutedEventArgs e)
        {
            //Create JPEG image Encoding format for storing image in JPEG type
            ImageEncodingProperties imgFormat = ImageEncodingProperties.CreateBmp();
            imgFormat.Height = 480;
            imgFormat.Width = 480;
            try
            {
                captureManager.VideoDeviceController.FlashControl.Enabled = false;
            }
            catch
            {
            }
            //captureManager.VideoDeviceController.Focus.TrySetValue(0.5);

            // create storage file in local app storage
            var documentsLibrary = KnownFolders.PicturesLibrary.Path.ToString();
            // StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("Photo.jpg", CreationCollisionOption.ReplaceExisting);
            var file = await KnownFolders.PicturesLibrary.CreateFileAsync("leaf.bmp", CreationCollisionOption.GenerateUniqueName);
            // take photo and store it on file location.
            await captureManager.CapturePhotoToStorageFileAsync(imgFormat, file);
        }

        private async void Stop_Capture_Preview_Click(object sender, RoutedEventArgs e)
        {
            await captureManager.StopPreviewAsync();  //stop camera capturing
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            minFocusTextBlock.Text = "Min:" + captureManager.VideoDeviceController.FocusControl.Min.ToString();
            maxFocusTextBlock.Text = "Max:" + captureManager.VideoDeviceController.FocusControl.Max.ToString();
            stepFocusTextBlock.Text = "Step:" + captureManager.VideoDeviceController.FocusControl.Step.ToString();
            focusTextBlock.Text = "Current:" + captureManager.VideoDeviceController.FocusControl.Value.ToString();
            TextBlock02.Text = "State:" + captureManager.VideoDeviceController.FocusControl.FocusState.ToString();
            TextBlock12.Text = "Mode:" + captureManager.VideoDeviceController.FocusControl.Mode.ToString();
            TextBlock22.Text = "Supported:" + captureManager.VideoDeviceController.FocusControl.Supported.ToString();
            TextBlock32.Text = "Wait:" + captureManager.VideoDeviceController.FocusControl.WaitForFocusSupported.ToString();
        }

        private async void CaptureAndProcessBitmap(object sender, RoutedEventArgs e)
        {
            var lowLagCapture = await captureManager.PrepareLowLagPhotoCaptureAsync(
                ImageEncodingProperties.CreateUncompressed(MediaPixelFormat.Bgra8));
            var capturedPhoto = await lowLagCapture.CaptureAsync();
            var softwareBitmap = capturedPhoto.Frame.SoftwareBitmap;

            ImageProcess image = new ImageProcess(softwareBitmap);
            image.Grayscale();
            image.GaussianFilter();
            image.ComputeGradient();
            image.HistogramOfOrientedGradients();

            ImageIO.SaveSoftwareBitmapToFile(image.GetSoftwareBitmap());

            var source = new SoftwareBitmapSource();
            SetSoftwareBitmapSource(image.GetSoftwareBitmap(), source);
            editPreview.Source = source;

            await lowLagCapture.FinishAsync();
        }

        private async void SetSoftwareBitmapSource(SoftwareBitmap bitmap, SoftwareBitmapSource source)
        {
            if (bitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8 || bitmap.BitmapAlphaMode == BitmapAlphaMode.Straight)
            {
                bitmap = SoftwareBitmap.Convert(bitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
            }
            await source.SetBitmapAsync(bitmap);
        }

        private async void ComputeScores(System.Collections.Generic.IReadOnlyList<StorageFile> storageFiles, ImageProcess capture)
        {
            var imageFromStorage = new ImageProcess();
            var count = 0;
            foreach (var storageFile in storageFiles)
            {
                imageFromStorage.SetSoftwareBitmap(await ImageIO.LoadSoftwareBitmapFromFile(storageFile));
                long score = capture.Compare(imageFromStorage);
                score -= 210 * 480; // because 210 lines are red and they will match
                long percent = score / (48 * 48) * 480 / 270; // because only 270 lines are usefull
                Debug.WriteLine(count + " Score: " + score + " Percent: " + percent);
                count++;
            }
        }
    }
}