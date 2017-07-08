using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Leaf
{
    public class HistogramOfOrientedGradients
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
    }
}
