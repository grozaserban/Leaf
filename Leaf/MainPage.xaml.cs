using Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using static Leaf.MediaCaptureWrapper;
using static Net.NeuralNet;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Leaf
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MediaCaptureWrapper _captureManager;

        private NeuralNet _classifier;

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
            _captureManager = await new MediaCaptureWrapperFactory().Create();

            capturePreview.Source = _captureManager.MediaCapture;
        //    _captureManager.StartPreview();

            var picturesLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);
            await Task.Run(() =>
            {
                _classifier = NeuralNetFactory.CreateNetWithPredefinedWeights(picturesLibrary.SaveFolder.Path + @"\weights.txt", 13, 6);
            });
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _captureManager.Dispose();
        }

        private async void Open_Photo(object sender, RoutedEventArgs e)
        {
            Image image = new Image(await ImageIO.LoadSoftwareBitmapFromFile());
            var bitmapSource = new SoftwareBitmapSource();
            editPreview.Source = bitmapSource;
            await ComputeHistogramWithDisplay(image, bitmapSource, TimeSpan.FromMilliseconds(1000));
        }

        private async Task<List<double>> ComputeHistogramWithDisplay(Image image, SoftwareBitmapSource source, TimeSpan timeSpan)
        {
            await SetSoftwareBitmapSource(image.SoftwareBitmap, source);
            await Task.Delay(timeSpan);

            image.GaussianFilter();
            await SetSoftwareBitmapSource(image.SoftwareBitmap, source);
            await Task.Delay(timeSpan);

            image.ComputeGradient();
            await SetSoftwareBitmapSource(image.SoftwareBitmap, source);
            await Task.Delay(timeSpan);

            image.DeleteSquare();
            await SetSoftwareBitmapSource(image.SoftwareBitmap, source);
            await Task.Delay(timeSpan);

            var hist = image.HistogramOfOrientedGradients(240, 13);
            image.DrawHistogramOfOrientedGradients(240, 9);
            await SetSoftwareBitmapSource(image.SoftwareBitmap, source);
            await Task.Delay(timeSpan);

            return hist.Normalize().ToList();
          //  await ImageIO.SaveSoftwareBitmapToFile(image.SoftwareBitmap);
        }

        private async Task<bool> Classify(Image image)
        {
            var bitmapSource = new SoftwareBitmapSource();
            editPreview.Source = bitmapSource;
            var histogram = await ComputeHistogramWithDisplay(image, bitmapSource, TimeSpan.FromMilliseconds(500));

            var emptyOutputs = new List<double>() { 0, 0, 0, 0, 0, 0 };
            _classifier.ChangeData(histogram, emptyOutputs);
            var classification = _classifier.GetClassification();
            
            ClassTextBlock.Text = Folders.PlantNames[classification.Item1] + " Class";
            ConfidenceTextBlock.Text = classification.Item2 + "% Confidence";
            return true;
        }


        private async void LoadImages(object sender, RoutedEventArgs e)
        {
            //   await Task.Run(() => DataManager.CreateSubfoldersAndHough("20+"));
         //   await Task.Run(() => Reading.SplitDataInFiles(@"C:\Users\Serban\Pictures\20+\HistogramsOnPixelsDouble.txt",
         //       @"C:\Users\Serban\Pictures\20+\MinNormalizedTestData\", 5));

        await Task.Run(() => new Tests().TestHogOnAllNewDataWithPerformanceAndConfidence(@"C:\Users\Serban\Pictures\20+\MinNormalizedTestData"));

        //         DataManager.CreateHistogramAndWriteThemToFiles("20+", "HistogramsOnPixelsDouble.txt");
         
        }

        private async void CaptureAndClassify()
        {
            var capturedPhoto = await _captureManager.GetCapture();

            Image image = new Image(capturedPhoto.Frame.SoftwareBitmap);
            await Classify(image);
        }

        private void Capture_Photo_Click(object sender, RoutedEventArgs e)
        {
            CaptureAndClassify();
        }

        private void Preview_Tap(object sender, RoutedEventArgs e)
        {
            _captureManager.Focus();
        }

        private async Task<bool> SetSoftwareBitmapSource(SoftwareBitmap bitmap, SoftwareBitmapSource source)
        {
            if (bitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8 || bitmap.BitmapAlphaMode == BitmapAlphaMode.Straight)
            {
                bitmap = SoftwareBitmap.Convert(bitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
            }

            await source.SetBitmapAsync(bitmap);
            return true;
        }
    }
}