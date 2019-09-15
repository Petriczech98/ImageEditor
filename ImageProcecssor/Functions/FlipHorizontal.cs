using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace ImageProcecssor.Functions
{
    public static class FlipHorizontal
    {
        public static unsafe void Proceed(BitmapData bitmapData, int bitsPerPixel)
        {
            int heightInPixels = bitmapData.Height;
            int bytesPerPixel = bitsPerPixel / 8;
            int widthInBytes = bitmapData.Width * bytesPerPixel;

            byte* PtrFirstPixel = (byte*)bitmapData.Scan0;

            Parallel.For(0, heightInPixels, y =>
            {
                byte* currentLine = PtrFirstPixel + (y * bitmapData.Stride);
                int inverted = widthInBytes - 1;
                for (int x = 0; x < widthInBytes / 2; x = x + bytesPerPixel)
                {
                    int oldBlue = currentLine[x];
                    int oldGreen = currentLine[x + 1];
                    int oldRed = currentLine[x + 2];

                    currentLine[x] = currentLine[inverted - 2];
                    currentLine[x + 1] = currentLine[inverted - 1];
                    currentLine[x + 2] = currentLine[inverted];

                    currentLine[inverted - 2] = (byte)oldBlue;
                    currentLine[inverted - 1] = (byte)oldGreen;
                    currentLine[inverted] = (byte)oldRed;

                    inverted -= bytesPerPixel;
                }
            });
        }
    }
}
