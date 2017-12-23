using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using SmartXCore;
using SmartXCore.Event;
using SmartXCore.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SimpleConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            WebWeChatClient.Build(listener)
                            .Start();

            Console.ReadKey();
        }


        static NotifyEventListener listener = async (client, notifyEvent) =>
        {
            var logger = client.Logger;
            switch (notifyEvent.Type)
            {
                case NotifyEventType.LoginSuccess:
                    Console.WriteLine("登录成功");
                    break;

                case NotifyEventType.QRCodeReady:
                    {
                        var bytes = notifyEvent.Target as byte[];

                        //var qrEncode = new QrEncoder(ErrorCorrectionLevel.H);
                        //var qrCode = qrEncode.Encode("http://www.baidu.com");
                        //BitMatrix matrix = qrCode.Matrix;
                        //GraphicsRenderer gRender = new GraphicsRenderer(new FixedModuleSize(6, QuietZoneModules.Two));
                        //var stream = new MemoryStream();
                        //gRender.WriteToStream(matrix, ImageFormat.Png, stream);
                        ////获取图像
                        //var image = new Bitmap(Image.FromStream(stream));

                        //ConsoleWriteImage(image);


                        //var bitmap = QrCodeHelper.Encode("http://www.baidu.com");
                        //ConsoleWriteImage(bitmap);

                        using (var ms = new MemoryStream(bytes))
                        {
                            //获取图像
                            var image = new Bitmap(Image.FromStream(ms));
                            var a = QrCodeHelper.Decode(image);
                            Console.WriteLine(a);

                        }

                        //using (var ms = new MemoryStream(bytes))
                        //{
                        //    //获取图像
                        //    var image = new Bitmap(Image.FromStream(ms));
                        //    //灰度化
                        //    //image = ToGray(image);
                        //    //反转
                        //    //image = GrayReverse(image);
                        //    //image = GrayReverse(image);
                        //    //二值化
                        //    //image = ConvertTo1Bpp1(image);
                        //    //打印
                        //    ConsoleWriteImage(image);

                        //}
                        Console.WriteLine("二维码已打印在屏幕，请使用手机QQ扫描。");
                        break;
                    }

                case NotifyEventType.QRCodeSuccess:
                    Console.WriteLine("请在手机上点击确认以登录");
                    break;

                case NotifyEventType.QRCodeInvalid:
                    Console.WriteLine("二维码已失效");
                    break;

                case NotifyEventType.Message:
                    {
                        //
                        break;
                    }

                case NotifyEventType.Offline:
                    Console.WriteLine("微信已离线");
                    break;

                default:
                    Console.WriteLine(notifyEvent.Type.GetFullDescription());
                    break;

            }
        };


        /// <summary>
        /// 打印二维码
        /// </summary>
        static void ConsoleWriteImage(Bitmap bmp)
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
