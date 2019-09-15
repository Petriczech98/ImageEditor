using System;
using System.Linq;

namespace ImageProcecssor.Functions
{
    public class HelpFunctions
    {
        private readonly double maximumColorValue;

        public HelpFunctions(int bitsPerPixel)
        {
            maximumColorValue = Math.Pow(2, bitsPerPixel);
        }

        public int[] BrightenPixel(int[] color)
        {
            int[] ret = new int[3];
            for (int i = 0; i < color.Length; i++)
            {
                ret[i] = color[i] + 40 > maximumColorValue ? ret[i] = (int)maximumColorValue : color[i] + 40;
            }
            return ret;
        }

        public int[] DarkenPixel(int[] color)
        {
            int[] ret = new int[3];
            for (int i = 0; i < color.Length; i++)
            {
                ret[i] = color[i] - 40 <= 0 ? ret[i] = 0 : ret[i] = color[i] - 40;
            }
            return ret;
        }

        public bool IsPixelBrightOrDark(int[] color)
        {
            return color.Sum() > maximumColorValue / 3;
        }

        public int[] SaturatePixel(int[] pixel)
        {
            int saturationIncrease = (int)(maximumColorValue / 6);
            bool pixelIsShadeOfGray = pixel.Skip(1).All(i => int.Equals(i, pixel[0]));
            if (pixelIsShadeOfGray)
            {
                return IsPixelBrightOrDark(pixel) ? BrightenPixel(pixel) : DarkenPixel(pixel);
            }
            if (pixel.Max() + saturationIncrease <= maximumColorValue)
            {
                for (int i = 0; i < pixel.Length; i++)
                {
                    if (pixel[i] == pixel.Max())
                    {
                        pixel[i] += saturationIncrease;
                    }
                }
            }
            else
            {
                int howMuchCanIIncrease = (int)maximumColorValue - pixel.Max();
                int increase = howMuchCanIIncrease >= saturationIncrease ?
                    increase = saturationIncrease : howMuchCanIIncrease;
                for (int i = 0; i < pixel.Length; i++)
                {
                    if (pixel[i] == pixel.Max())
                    {
                        pixel[i] += increase;
                    }
                    else
                    {
                        pixel[i] = pixel[i] - (saturationIncrease - increase) < 0 ?
                            pixel[i] = 0 : pixel[i] -= (saturationIncrease - increase);
                    }
                }
            }
            return pixel;
        }
    }
}
