using System;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace ImageProcecssor.Functions
{
    public static class ConvertToBlackWhite
    {
        public static unsafe void Proceed(BitmapData bitmapData, int bitsPerPixel)
        {
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
        }
    }
}
