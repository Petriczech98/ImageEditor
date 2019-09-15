using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using ImageProcecssor.Functions;

namespace ImageProcecssor
{
    public class Processor
    {
        private string[] commands;
        private List<string> images;
        private BitmapData bitmapData;
        private Dictionary<string, Action<BitmapData, int>> funcContainer;

        public Processor()
        {
            FillFuncContainer();
        }

        private void FillFuncContainer()
        {
            funcContainer = new Dictionary<string, Action<BitmapData, int>>
            {
                { "blackwhite", (data, bitsPerPixel) => ConvertToBlackWhite.Proceed(data, bitsPerPixel) },
                { "horizontalflip", (data, bitsPerPixel) => FlipHorizontal.Proceed(data, bitsPerPixel) },
                { "increasecontrast", (data, bitsPerPixel) => IncreaseContrast.Proceed(data, bitsPerPixel) },
                { "increasebrightness", (data, bitsPerPixel) => IncreaseBrightness.Proceed(data, bitsPerPixel) },
                { "increasesaturation", (data, bitsPerPixel) => IncreaseSaturation.Proceed(data, bitsPerPixel) }
            };
        }

        public async void ProceedImage(string[] cmds, List<string> imgs)
        {
            commands = cmds;
            images = imgs;
            Task task = new Task(() => Proceed());
            task.Start();
            await task;
            Array.Clear(cmds, 0, cmds.Length);
            imgs.Clear();
        }

        private void Proceed()
        {
            foreach(string imagePath in images)
            {
                Bitmap originalImage = new Bitmap(imagePath);
                bitmapData = originalImage.LockBits(
                    new Rectangle(0, 0, originalImage.Width, originalImage.Height), ImageLockMode.ReadWrite, originalImage.PixelFormat);
                int bitsPerPixel = Bitmap.GetPixelFormatSize(originalImage.PixelFormat);

                foreach(string command in commands)
                {
                    if (command.Equals("comparedurations"))
                    {
                        new CompareDuration(imagePath).DoComparison();
                        return;
                    }
                    funcContainer[command].Invoke(bitmapData, bitsPerPixel);
                }

                originalImage.UnlockBits(bitmapData);
                originalImage.Save(GenerateNewFileName(imagePath));
                originalImage.Dispose();
                Console.WriteLine($"Image {Path.GetFileName(imagePath)} has been processed!");
            }
        }

        private string GenerateNewFileName(string filePath)
        {
            string folderPath = Path.GetDirectoryName(filePath);
            string fileExtension = Path.GetExtension(filePath);
            string nameOfFile = Path.GetFileName(filePath).Replace(fileExtension, "");

            return folderPath + Path.DirectorySeparatorChar + nameOfFile + "changed" + fileExtension;
        }
    }
}