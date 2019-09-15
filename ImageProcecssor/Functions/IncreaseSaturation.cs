using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcecssor.Functions
{
    public static class IncreaseSaturation
    {
        public static unsafe void Proceed(BitmapData bitmapData, int bitsPerPixel)
        {
            int bytesPerPixel = bitsPerPixel / 8;
            int heightInPixels = bitmapData.Height;
            int widthInBytes = bitmapData.Width * bytesPerPixel;
            byte* PtrFirstPixel = (byte*)bitmapData.Scan0;

            Parallel.For(0, heightInPixels, y =>
            {
                byte* currentLine = PtrFirstPixel + (y * bitmapData.Stride);
                for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                {
                    int[] previousColor = new int[] { currentLine[x], currentLine[x + 1], currentLine[x + 2] };
                    int[] newColor = new HelpFunctions(bitsPerPixel).SaturatePixel(previousColor);

                    currentLine[x] = (byte)newColor[0];
                    currentLine[x + 1] = (byte)newColor[1];
                    currentLine[x + 2] = (byte)newColor[2];
                }
            });
        }
    }
}