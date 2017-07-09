using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Leaf
{
    public class DataManager
    {
        public static async void WriteToFile(string leafTypeMaxPoints, StorageFolder folder, string filename)
        {
            var file = await folder.CreateFileAsync(filename, CreationCollisionOption.GenerateUniqueName);
            await Task.Run(() =>
            {
                Task.Yield();
                using (var fileStream = File.OpenWrite(file.Path))
                {
                    byte[] points = new UTF8Encoding(true).GetBytes(leafTypeMaxPoints);
                    fileStream.Write(points, 0, points.Length);
                }
            });
        }

        public static async void CreateHistogramAndWriteThemToFiles(string rootFolder, string fileName)
        {
            var appFolder = await KnownFolders.PicturesLibrary.GetFolderAsync(rootFolder);
            string leafTypeMaxPoints = Folders.Names.Length + Environment.NewLine;
            foreach (var folderName in Folders.Names)
            {
                var leafTypeFolder = await appFolder.GetFolderAsync(folderName);
                var leafStorageFiles = await leafTypeFolder.GetFilesAsync(Windows.Storage.Search.CommonFileQuery.OrderByDate);

                foreach (var storageFile in leafStorageFiles)
                {
                    var image = new Image(await ImageIO.LoadSoftwareBitmapFromFile(storageFile));
                    var histogram = image
                        .GaussianFilter()
                        .ComputeGradient()
                        .DeleteSquare()
                        .HistogramOfOrientedGradients(240, 13)
                        .Normalize();

                    leafTypeMaxPoints += folderName.ToString();
                    foreach (var value in histogram)
                    {
                        leafTypeMaxPoints += " " + value;
                    }
                    leafTypeMaxPoints += Environment.NewLine;
                }
            }
            DataManager.WriteToFile(leafTypeMaxPoints, appFolder, fileName);
        }

        public static async void CreateHistogramAveragesAndWriteThemToFile(string rootFolder, string fileName)
        {
            var appFolder = await KnownFolders.PicturesLibrary.GetFolderAsync(rootFolder);
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
                        .GaussianFilter()
                        .ComputeGradient()
                        .DeleteSquare()
                        .HistogramOfOrientedGradients(240, 13)
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
            WriteToFile(leafTypeMaxPoints, appFolder, fileName);
        }

        public static async void CreateSubfoldersAndHough(string rootFolder)
        {
            string leafTypeMaxPoints = string.Empty;
            var appFolder = await KnownFolders.PicturesLibrary.GetFolderAsync(rootFolder);
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
                    ImageIO.WriteToFile(gray, "leaf", image.SoftwareBitmap);

                    image.GaussianFilter();
                    ImageIO.WriteToFile(filtered, "leaf", image.SoftwareBitmap);

                    image.ComputeGradient();
                    ImageIO.WriteToFile(Gradient, "leaf", image.SoftwareBitmap);

                    image.DrawHistogramOfOrientedGradients(240, 9);
                    ImageIO.WriteToFile(HOG, "leaf", image.SoftwareBitmap);
                }
            }
        }
    }
}
