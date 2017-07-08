using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

namespace Leaf
{
    public static class ImageIO
    {
        private static async Task<StorageFile> SetOutputStorageFile()
        {
            FileSavePicker _fileSavePicker = new FileSavePicker();
            _fileSavePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            _fileSavePicker.FileTypeChoices.Add("BMP files", new List<string>() { ".bmp" });
            _fileSavePicker.SuggestedFileName = "savedLeaf";

            return await _fileSavePicker.PickSaveFileAsync();
        }

        private static async Task<StorageFile> SetAutomaticOutputStorageFile()
        {
            return await KnownFolders.PicturesLibrary.CreateFileAsync("leaf.bmp", CreationCollisionOption.GenerateUniqueName);
        }

        public static async void WriteToFile(IStorageFolder folder, string fileName, SoftwareBitmap bitmap)
        {
            var file = await folder.CreateFileAsync(fileName + ".bmp", CreationCollisionOption.GenerateUniqueName);
            await ImageIO.SaveSoftwareBitmapToFile(bitmap, file);
        }

        public static async Task<bool> SaveSoftwareBitmapToFile(SoftwareBitmap softwareBitmap, bool automatic = false)
        {
            if (!automatic)
                return SaveSoftwareBitmapToFile(softwareBitmap).Result;

            return await SaveSoftwareBitmapToFile(softwareBitmap, await SetAutomaticOutputStorageFile());
        }

        public static async Task<bool> SaveSoftwareBitmapToFile(SoftwareBitmap softwareBitmap)
        {
            return await SaveSoftwareBitmapToFile(softwareBitmap, await SetOutputStorageFile());
        }

        public static async Task<bool> SaveSoftwareBitmapToFile(SoftwareBitmap softwareBitmap, StorageFile storageFile)
        {
            if (storageFile == null)
                return false;
            using (IRandomAccessStream stream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                // Create an encoder with the desired format
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, stream);

                // Set the software bitmap
                encoder.SetSoftwareBitmap(softwareBitmap);

                // Set additional encoding parameters, if needed
                if (softwareBitmap.PixelHeight > 480 && softwareBitmap.PixelWidth > 480)
                {
                    encoder.BitmapTransform.ScaledWidth = 480;
                    encoder.BitmapTransform.ScaledHeight = 480;
                    encoder.BitmapTransform.Rotation = BitmapRotation.None;//BitmapRotation.Clockwise90Degrees;
                    encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Linear;
                }
                encoder.IsThumbnailGenerated = false;
                try
                {
                    await encoder.FlushAsync();
                }
                catch (Exception err)
                {
                    switch (err.HResult)
                    {
                        case unchecked((int)0x88982F81): //WINCODEC_ERR_UNSUPPORTEDOPERATION
                                                         // If the encoder does not support writing a thumbnail, then try again
                                                         // but disable thumbnail generation.
                            encoder.IsThumbnailGenerated = false;
                            break;

                        default:
                            throw err;
                    }
                }
                // put new softwarebitmap to release the lock from the other one
                encoder.SetSoftwareBitmap(new SoftwareBitmap(BitmapPixelFormat.Bgra8, 10, 10, BitmapAlphaMode.Premultiplied));
            }
            return true;
        }

        private static async Task<StorageFile> SetInputStorageFile()
        {
            FileOpenPicker fileOpenPicker = new FileOpenPicker();
            fileOpenPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            fileOpenPicker.FileTypeFilter.Add(".bmp");
            fileOpenPicker.ViewMode = PickerViewMode.Thumbnail;

            return await fileOpenPicker.PickSingleFileAsync();
        }

        public static async Task<SoftwareBitmap> LoadSoftwareBitmapFromFile()
        {
            return await LoadSoftwareBitmapFromFile(await SetInputStorageFile());
        }

        public static async Task<SoftwareBitmap> LoadSoftwareBitmapFromFile(StorageFile storageFile)
        {
            if (storageFile == null)
                return null;
            using (IRandomAccessStream stream = await storageFile.OpenAsync(FileAccessMode.Read))
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);

                return await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight);
            }
        }
    }
}