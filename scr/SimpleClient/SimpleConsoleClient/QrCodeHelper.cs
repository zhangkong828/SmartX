using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
