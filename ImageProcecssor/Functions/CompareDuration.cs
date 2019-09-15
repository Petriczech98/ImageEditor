using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace ImageProcecssor.Functions
{
    public class CompareDuration
    {
        private readonly string originalImagePath;
        private readonly Bitmap originalImage;
        private readonly Bitmap newImage;
        private readonly Stopwatch stopWatch;
        private readonly int width;
        private long durationOfBitlock;
        private long durationOfGetPixel;

        public CompareDuration(string imagePath)
        {
            originalImagePath = imagePath;
            originalImage = new Bitmap(imagePath);
            width = originalImage.Width;
            newImage = new Bitmap(originalImage.Width, originalImage.Height);
            stopWatch = new Stopwatch();
        }

        public void DoComparison()
        {
            ConvertToBlackWhiteWithLockBit();
            ConvertToBlackWhiteWithGetPixel();
            Console.WriteLine($"Converting image {Path.GetFileName(originalImagePath)} to grayscale took:\n" +
                $"{durationOfGetPixel} ms with Get/SetPixel functions,\n" +
                $"{durationOfBitlock} ms with parallel LockBit.");
            originalImage.Dispose();
        }

        private unsafe void ConvertToBlackWhiteWithLockBit()
        {
            stopWatch.Start();
            BitmapData bitmapData = originalImage.LockBits(new Rectangle(
                0, 0, originalImage.Width, originalImage.Height), ImageLockMode.ReadWrite, originalImage.PixelFormat);
            int bitsPerPixel = Bitmap.GetPixelFormatSize(originalImage.PixelFormat);
            int heightInPixels = bitmapData.Height;
            int bytesPerPixel = (bitsPerPixel / 8);
            int widthInBytes = bitmapData.Width * bytesPerPixel;
            byte* PtrFirstPixel = (byte*)bitmapData.Scan0;

            Parallel.For(0, heightInPixels, y =>
            {
                byte* currentLine = PtrFirstPixel + (y * bitmapData.Stride);
                for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                {
                    int oldBlue = currentLine[x];
                    int oldGreen = currentLine[x + 1];
                    int oldRed = currentLine[x + 2];

                    int blackWhiteColor = (int)Math.Round(oldRed * .299 + oldGreen * .587 + oldBlue * .114);

                    currentLine[x] = (byte)blackWhiteColor;
                    currentLine[x + 1] = (byte)blackWhiteColor;
                    currentLine[x + 2] = (byte)blackWhiteColor;
                }
            });
            originalImage.UnlockBits(bitmapData);
            stopWatch.Stop();
            durationOfBitlock = stopWatch.ElapsedMilliseconds;
            stopWatch.Reset();
        }

        private void ConvertToBlackWhiteWithGetPixel()
        {
            stopWatch.Start();
            for(int y = 0; y < originalImage.Height; y++)
            {
                for(int x = 0; x < originalImage.Width; x++)
                {
                    Color originalColor = originalImage.GetPixel(x, y);
                    Color newColor = Color.FromArgb(
                        (int)(originalColor.R * .299),
                        (int)(originalColor.G * .587),
                        (int)(originalColor.B * .114));
                    newImage.SetPixel(x, y, newColor);
                }
            }
            stopWatch.Stop();
            durationOfGetPixel = stopWatch.ElapsedMilliseconds;
            stopWatch.Reset();
        }
    }
}
