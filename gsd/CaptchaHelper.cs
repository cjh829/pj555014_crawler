using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Net;
using System.IO;
using NReco.PhantomJS;

namespace gsd
{
    public static class CaptchaHelper
    {
        //make image easier to OCR
        public static Bitmap process(string pngPath)
        {
            using (Bitmap bimg = new Bitmap(pngPath))
            {
                convert2GrayScale(bimg);
                threshoding(bimg,50);
                var img = Image.FromHbitmap(bimg.GetHbitmap());
                return img;
            }
        }
        private static void convert2GrayScale(Bitmap bimg)
        {
            for (int i = 0; i < bimg.Width; i++)
            {
                for (int j = 0; j < bimg.Height; j++)
                {
                    Color pixelColor = bimg.GetPixel(i, j);
                    byte r = pixelColor.R;
                    byte g = pixelColor.G;
                    byte b = pixelColor.B;

                    byte gray = (byte)(0.299 * (float)r + 0.587 * (float)g + 0.114 * (float)b);
                    r = g = b = gray;
                    pixelColor = Color.FromArgb(r, g, b);

                    bimg.SetPixel(i, j, pixelColor);
                }
            }
        }
        private static void threshoding(Bitmap bimg, int minPoints)
        {
            int x, y;
            var width = bimg.Width;
            var height = bimg.Height;
            var analystable = new SortedDictionary<int, int>();
            for (x = 1; x < width - 1; x++)
            {
                for (y = 1; y < height - 1; y++)
                {
                    var value = bimg.GetPixel(x, y).R;
                    if (!analystable.ContainsKey(value))
                    {
                        analystable.Add(value, 0);
                    }
                    analystable[value] = analystable[value] + 1;
                }
            }

            var min = analystable.Where(kvp => kvp.Value >= minPoints).First().Key;
            for (x = 1; x < width - 1; x++)
            {
                for (y = 1; y < height - 1; y++)
                {
                    var value = bimg.GetPixel(x, y).R;
                    if (value != min)
                    {
                        bimg.SetPixel(x, y, Color.FromArgb(255, 255, 255));
                    }
                }
            }
        }
    }
}
