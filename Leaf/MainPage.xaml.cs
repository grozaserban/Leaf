﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
            if (IsMobile)
                captureManager.SetPreviewRotation(VideoRotation.Clockwise90Degrees);
            await captureManager.StartPreviewAsync();  //Start camera capturing

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        public static bool IsMobile
        {
            get
            {
                var qualifiers = Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().QualifierValues;
                return (qualifiers.ContainsKey("DeviceFamily") && qualifiers["DeviceFamily"] == "Mobile");
            }
        }

        private async void Open_Photo(object sender, RoutedEventArgs e)
        {
            Image image = new Image(await ImageIO.LoadSoftwareBitmapFromFile());
            var source = new SoftwareBitmapSource();
            var timeSpan = TimeSpan.FromMilliseconds(1000);

            SetSoftwareBitmapSource(image.SoftwareBitmap, source);
            editPreview.Source = source;
            await Task.Delay(timeSpan);

            image.ToGrayScale();
            SetSoftwareBitmapSource(image.SoftwareBitmap, source);
            await Task.Delay(timeSpan);

            image.GaussianFilter();
            SetSoftwareBitmapSource(image.SoftwareBitmap, source);
            await Task.Delay(timeSpan);

            image.ComputeGradient();
            SetSoftwareBitmapSource(image.SoftwareBitmap, source);
            await Task.Delay(timeSpan);

            image.DeleteSquare();
            SetSoftwareBitmapSource(image.SoftwareBitmap, source);
            await Task.Delay(timeSpan);

            image.HistogramOfOrientedGradients(60, 14);
            SetSoftwareBitmapSource(image.SoftwareBitmap, source);
            await Task.Delay(timeSpan);

            ImageIO.SaveSoftwareBitmapToFile(image.SoftwareBitmap);
        }

        private async void CreateHough(Image image)
        {
            for (int i = 1; i < 10; i ++)
                for (int j = 1; j < 10; j++)
                {
                    ImageIO.SaveSoftwareBitmapToFile(image.HoughGradient(i, j).SoftwareBitmap);
                }
        }

        private async void LoadImages(object sender, RoutedEventArgs e)
        {
            Task.Run(() => CreateSubfoldersAndHough(sender, e));
        
            //  Task.Run(() =>WriteHOGToFiles(sender, e));
      
            //WriteHOGAverageToFiles(sender, e);
        }

        private async void WriteHOGAverageToFiles(object sender, RoutedEventArgs e)
        {
            var appFolder = await KnownFolders.PicturesLibrary.GetFolderAsync("LeafHogs");
            string leafTypeMaxPoints = Folders.Names.Length + Environment.NewLine;
            foreach (var folderName in Folders.Names)
            {
                var leafTypeFolder = await appFolder.GetFolderAsync(folderName);
                var leafStorageFiles = await leafTypeFolder.GetFilesAsync(Windows.Storage.Search.CommonFileQuery.OrderByDate);
                double[] histogram = new double[13];

                foreach (var storageFile in leafStorageFiles)
                {
                    var image = new Image(await ImageIO.LoadSoftwareBitmapFromFile(storageFile));
                    var imageHistogram = image
                        .ToGrayScale()
                        .GaussianFilter()
                        .ComputeGradient()
                        .DeleteSquare()
                        .HistogramOfOrientedGradients(60, 13)
                        .Normalize();
                    for (int i = 0; i < imageHistogram.Length; i++)
                        histogram[i] += imageHistogram[i];
                }

                for (int i = 0; i < histogram.Length; i++)
                    histogram[i] /= leafStorageFiles.Count;

                leafTypeMaxPoints += folderName.ToString();
                foreach (var value in histogram)
                {
                    leafTypeMaxPoints += " " + value;
                }
                leafTypeMaxPoints += Environment.NewLine;
            }
            WriteToFile(leafTypeMaxPoints, appFolder, "HistogramsAveragesDouble.txt");
        }

        private async void WriteHOGToFiles(object sender, RoutedEventArgs e)
        {
            var appFolder = await KnownFolders.PicturesLibrary.GetFolderAsync("20+");
            string leafTypeMaxPoints = Folders.Names.Length + Environment.NewLine;
            foreach (var folderName in Folders.Names)
            {
                var leafTypeFolder = await appFolder.GetFolderAsync(folderName);
                var leafStorageFiles = await leafTypeFolder.GetFilesAsync(Windows.Storage.Search.CommonFileQuery.OrderByDate);

                foreach (var storageFile in leafStorageFiles)
                {
                    var image = new Image(await ImageIO.LoadSoftwareBitmapFromFile(storageFile));
                    var histogram = image
                        .ToGrayScale()
                        .GaussianFilter()
                        .ComputeGradient()
                        .DeleteSquare()
                        .HistogramOfOrientedGradients(60, 13)
                        .Normalize();

                    leafTypeMaxPoints += folderName.ToString();
                    foreach(var value in histogram)
                    {
                        leafTypeMaxPoints += " " + value;
                    }
                    leafTypeMaxPoints += Environment.NewLine;
                }
            }
            WriteToFile(leafTypeMaxPoints, appFolder, "HistogramsDouble.txt");
        }

        private async void CreateSubfoldersAndHough(object sender, RoutedEventArgs e)
        {
            string leafTypeMaxPoints = string.Empty;
            var appFolder = await KnownFolders.PicturesLibrary.GetFolderAsync("20+");
            foreach (var folderName in Folders.Names)
            {
                var leafTypeFolder = await appFolder.GetFolderAsync(folderName);
                var leafStorageFiles = await leafTypeFolder.GetFilesAsync(Windows.Storage.Search.CommonFileQuery.OrderByDate);
                var gray = await leafTypeFolder.CreateFolderAsync("Grayscale");
                var filtered = await leafTypeFolder.CreateFolderAsync("GaussianFilter");
                var Gradient = await leafTypeFolder.CreateFolderAsync("Gradient");
                var HOG = await leafTypeFolder.CreateFolderAsync("HOG");

                foreach (var storageFile in leafStorageFiles)
                {
                    var image = new Image(await ImageIO.LoadSoftwareBitmapFromFile(storageFile));
                    image.ToGrayScale();
                    ImageIO.WriteToFile(gray, "leaf", image.SoftwareBitmap);

                    image.GaussianFilter();
                    ImageIO.WriteToFile(filtered, "leaf", image.SoftwareBitmap);

                    image.ComputeGradient();
                    ImageIO.WriteToFile(Gradient, "leaf", image.SoftwareBitmap);

                    image.DrawHistogramOfOrientedGradients(60, 9);
                    ImageIO.WriteToFile(HOG, "leaf", image.SoftwareBitmap);
                }
            }
        }

        private async void WriteToFile(string leafTypeMaxPoints, StorageFolder folder, string filename)
        {
            var file = await folder.CreateFileAsync(filename, CreationCollisionOption.GenerateUniqueName);
            await Task.Run(() =>
            {
                Task.Yield();
                using (var tw = File.OpenWrite(file.Path))
                {
                    byte[] points = new UTF8Encoding(true).GetBytes(leafTypeMaxPoints);
                    tw.Write(points, 0, points.Length);
                }
            });
        }

        private async void MatchImage(object sender, RoutedEventArgs e)
        {
            var storageFolder = await KnownFolders.PicturesLibrary.GetFolderAsync("HOG60");
            var storageFiles = await storageFolder.GetFilesAsync(Windows.Storage.Search.CommonFileQuery.OrderByDate);

            var image = new Image(await ImageIO.LoadSoftwareBitmapFromFile());
            image.ToGrayScale();  
            image.GaussianFilter();
            image.ComputeGradient();
            image.DrawHistogramOfOrientedGradients(20, 9);
            ComputeScores(storageFiles, image);
 
            var source = new SoftwareBitmapSource();
            SetSoftwareBitmapSource(image.SoftwareBitmap, source);
            editPreview.Source = source;
        }

        private async void Capture_Photo_Click(object sender, RoutedEventArgs e)
        {
            //Create JPEG image Encoding format for storing image in JPEG type
            ImageEncodingProperties imgFormat = ImageEncodingProperties.CreateBmp();

            var rotation = captureManager.GetPreviewRotation();
            var resolutions = captureManager.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.Photo).ToList();
            var resolutionObj = resolutions.Last();
            var resolution = resolutionObj as VideoEncodingProperties;

            imgFormat.Height = resolution.Height;
            imgFormat.Width = resolution.Width;
          //  captureManager.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.Photo, imgFormat);
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
                                                                                                                                     captureManager.CapturePhotoToStorageFileAsync(imgFormat, file);
        }

        private async void Stop_Capture_Preview_Click(object sender, RoutedEventArgs e)
        {
            await captureManager.StopPreviewAsync();  //stop camera capturing
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            captureManager.VideoDeviceController.Focus.TrySetAuto(true);
            captureManager.VideoDeviceController.FocusControl.FocusAsync();
        }

        private async void CaptureAndProcessBitmap(object sender, RoutedEventArgs e)
        {
            var lowLagCapture = await captureManager.PrepareLowLagPhotoCaptureAsync(
                ImageEncodingProperties.CreateUncompressed(MediaPixelFormat.Bgra8));
            var capturedPhoto = await lowLagCapture.CaptureAsync();
            await lowLagCapture.FinishAsync();

            Image image = new Image(capturedPhoto.Frame.SoftwareBitmap);

           // image.ToGrayScale()
           //      .GaussianFilter()
           //      .ComputeGradient()
           //      .DrawHistogramOfOrientedGradients(30, 9);

            await ImageIO.SaveSoftwareBitmapToFile(image.SoftwareBitmap, true);

            var source = new SoftwareBitmapSource();
            SetSoftwareBitmapSource(image.SoftwareBitmap, source);
            editPreview.Source = source;


        }

        private async void SetSoftwareBitmapSource(SoftwareBitmap bitmap, SoftwareBitmapSource source)
        {
            if (bitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8 || bitmap.BitmapAlphaMode == BitmapAlphaMode.Straight)
            {
                bitmap = SoftwareBitmap.Convert(bitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
            }

            await source.SetBitmapAsync(bitmap);
        }

        private async void ComputeScores(System.Collections.Generic.IReadOnlyList<StorageFile> storageFiles, Image image)
        {
            var count = 0;
            var timer = new Stopwatch();
    /*        timer.Start();
            foreach (var storageFile in storageFiles)
            {
                var imageFromStorage = new Image(await ImageIO.LoadSoftwareBitmapFromFile(storageFile));
                long score = image.MatchExactely(imageFromStorage);
                score -= 210 * 480; // because 210 lines are red and they will match
                long percent = score / (48 * 48) * 480 / 270; // because only 270 lines are usefull
                Debug.WriteLine(count + " Score: " + score + " Percent: " + percent);
                count++;
            }
            timer.Stop();
            Debug.WriteLine("Duration sequential: %d", timer.ElapsedMilliseconds); */

            timer.Reset();
            Parallel.ForEach(storageFiles, async storageFile => {

                var imageFromStorage = new Image(await ImageIO.LoadSoftwareBitmapFromFile(storageFile));
                long score = image.MatchExactely(imageFromStorage);
                score -= 210 * 480; // because 210 lines are red and they will match
                long percent = score / (48 * 48) * 480 / 270; // because only 270 lines are usefull
                Debug.WriteLine(" Score: " + score + " Percent: " + percent);
            });
            timer.Stop();
            Debug.WriteLine("Duration parallel: %d", timer.ElapsedMilliseconds);
        }
    }
}