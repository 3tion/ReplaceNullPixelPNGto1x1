using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReplaceNullBMP
{
    class Program
    {
        static unsafe void Main(string[] args)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            Stream imageStream1 = asm.GetManifestResourceStream("ReplaceNullBMP.1x1.png");
            int len = (int)imageStream1.Length;
            byte[] b= new byte[len];
            imageStream1.Read(b, 0, len);
            foreach (string path in args)
            {
                if (File.Exists(path))
                {
                    Bitmap bmp;
                    try
                    {
                        bmp = new Bitmap(path);
                    }
                    catch
                    {
                        continue;
                    }
                    bool flag = true;
                    int w = bmp.Width;
                    int h = bmp.Height;
                    Rectangle rect = new Rectangle(0, 0, w, h);
                    System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                    byte* ptr = (byte*)(bmpData.Scan0);
                    for (int y = 0; y < h; y++) 
                    {
                        for (int x = 0; x < w; x++)
                        {
                            if ((byte)ptr[3] != 0)//bmp.GetPixel(x, y).A != 0)
                            {
                                flag = false;
                                break;
                            }
                            ptr += 4;
                        }
                        ptr += bmpData.Stride - w * 4;
                    }
                    bmp.Dispose();
                    if (flag)//全部透明，说明是空文件
                    {
                        //用1*1像素透明png文件覆盖
                        FileStream file = new FileStream(path, FileMode.Create);
                        file.Write(b, 0, len);
                        file.Flush();
                        file.Close();
                    }
                }
            }
        }
    }
}
