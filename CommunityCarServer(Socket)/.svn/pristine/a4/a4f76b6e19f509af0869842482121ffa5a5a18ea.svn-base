using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
namespace ServerWindowsForms
{
    class Common
    {
        public static byte[] Bitmap2Byte(string path)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Bitmap bm = new Bitmap(path);
                bm.Save(stream, ImageFormat.Jpeg);
                byte[] data = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(data, 0, Convert.ToInt32(stream.Length));
                return data;
            }
        }

    }
}
