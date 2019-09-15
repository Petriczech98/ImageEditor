using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace ImageProcecssor.Functions
{
    public static class IncreaseContrast
    {
        public static unsafe void Proceed(BitmapData bitmapData, int bitsPerPixel)
        {
            int bytesPerPixel = bitsPerPixel / 8;
            int heightInPixels = bitmapData.Height;
            int widthInBytes = bitmapData.Width * bytesPerPixel;
            HelpFunctions helpFunctions = new HelpFunctions(bitsPerPixel);
            byte* PtrFirstPixel = (byte*)bitmapData.Scan0;

            Parallel.For(0, heightInPixels, y =>
            {
                byte* currentLine = PtrFirstPixel + (y * bitmapData.Stride);
                for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                {
                    int[] previousColor = new int[] { currentLine[x], currentLine[x + 1], currentLine[x + 2] };
                    int[] newColor = helpFunctions.IsPixelBrightOrDark(previousColor) ?
                    newColor = helpFunctions.BrightenPixel(previousColor) :
                    newColor = helpFunctions.DarkenPixel(previousColor);

                    currentLine[x] = (byte)newColor[0];
                    currentLine[x + 1] = (byte)newColor[1];
                    currentLine[x + 2] = (byte)newColor[2];
                }
            });
        }
    }
}