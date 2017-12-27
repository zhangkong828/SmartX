using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Data;
using System;
using System.Drawing;

namespace SimpleConsoleClient
{
    public class QrCodeHelper
    {
        public static Bitmap Encode(string str)
        {
            var encoder = new QRCodeEncoder();
            return encoder.Encode(str);

        }

        public static string Decode(Bitmap bitmap)
        {
            var decoder = new QRCodeDecoder();
            var image = new QRCodeBitmapImage(bitmap);
            return decoder.Decode(image);

        }
        /// <summary>
        /// 打印二维码
        /// </summary>
        public static void ConsoleWriteImage(Bitmap bmp)
        {
            int w = bmp.Width;
            int h = bmp.Height;

            int max = 28;

            // we need to scale down high resolution images...
            int complexity = (int)Math.Floor(Convert.ToDecimal(((w / max) + (h / max)) / 2));

            if (complexity < 1) { complexity = 1; }

            for (var x = 0; x < w; x += complexity)
            {
                for (var y = 0; y < h; y += complexity)
                {
                    Color clr = bmp.GetPixel(x, y);
                    Console.ForegroundColor = getNearestConsoleColor(clr);
                    Console.Write("█");
                }
                Console.WriteLine();
            }

            Console.WriteLine();
        }

        private static ConsoleColor getNearestConsoleColor(Color color)
        {
            // this is very likely to be awful and hilarious
            int r = color.R;
            int g = color.G;
            int b = color.B;
            int total = r + g + b;
            decimal darkThreshold = 0.35m; // how dark a color has to be overall to be the dark version of a color

            ConsoleColor cons = ConsoleColor.White;
            if (total >= 39 && total < 100 && areClose(r, g) && areClose(g, b) && areClose(r, b))
            {
                cons = ConsoleColor.DarkGray;
            }

            if (total >= 100 && total < 180 && areClose(r, g) && areClose(g, b) && areClose(r, b))
            {
                cons = ConsoleColor.Gray;
            }
            // if green is the highest value
            if (g > b && g > r)
            {
                // ..and color is less that 25% of color
                if (Convert.ToDecimal(total / 765m) < darkThreshold)
                {
                    cons = ConsoleColor.DarkGreen;
                }
                else
                {
                    cons = ConsoleColor.Green;
                }
            }
            // if red is the highest value
            if (r > g && r > b)
            {

                // ..and color is less that 25% of color
                if (Convert.ToDecimal(total / 765m) < darkThreshold)
                {
                    cons = ConsoleColor.DarkRed;
                }
                else
                {
                    cons = ConsoleColor.Red;
                }
            }
            // if blue is the highest value
            if (b > g && b > r)
            {
                // ..and color is less that 25% of color
                if (Convert.ToDecimal(total / 765m) < darkThreshold)
                {
                    cons = ConsoleColor.DarkBlue;
                }
                else
                {
                    cons = ConsoleColor.Blue;
                }
            }
            if (r > b && g > b && areClose(r, g))
            {
                // ..and color is less that 25% of color
                if (Convert.ToDecimal(total / 765m) < darkThreshold)
                {
                    cons = ConsoleColor.DarkYellow;
                }
                else
                {
                    cons = ConsoleColor.Yellow;
                }
            }
            if (b > r && g > r && areClose(b, g))
            {
                // ..and color is less that 25% of color
                if (Convert.ToDecimal(total / 765m) < darkThreshold)
                {
                    cons = ConsoleColor.DarkCyan;
                }
                else
                {
                    cons = ConsoleColor.Cyan;
                }
            }
            if (r > g && b > g && areClose(r, b))
            {
                // ..and color is less that 25% of color
                if (Convert.ToDecimal(total / 765m) < darkThreshold)
                {
                    cons = ConsoleColor.DarkMagenta;
                }
                else
                {
                    cons = ConsoleColor.Magenta;
                }
            }

            if (total >= 180 && areClose(r, g) && areClose(g, b) && areClose(r, b))
            {
                cons = ConsoleColor.White;
            }
            // BLACK
            if (total < 39)
            {
                cons = ConsoleColor.Black;
            }
            return cons;
        }

        /// <summary>
        /// Returns true if the numbers are pretty close
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool areClose(int a, int b)
        {
            int diff = Math.Abs(a - b);
            if (diff < 30)
            {
                return true;
            }
            else return false;

        }
    }
}
