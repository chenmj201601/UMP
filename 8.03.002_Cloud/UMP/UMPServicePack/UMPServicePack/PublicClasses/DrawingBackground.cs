using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UMPServicePack.PublicClasses
{
    public static class DrawingBackground
    {
        [DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);

        private static BitmapSource GetBitmapSource(System.Drawing.Bitmap m_Bitmap)
        {
            IntPtr inptr = m_Bitmap.GetHbitmap();
            BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                inptr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            DeleteObject(inptr);
            return bitmapSource;
        }

        public static void DrawWindowsBackgond(Window AWindowTarget)
        {
            try
            {
                //System.Drawing.Bitmap LBitmapBackgroud = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(@"../../Images/Background02.jpg");
                //BitmapSource bitmapSource = GetBitmapSource(LBitmapBackgroud);
                //ImageBrush LImageBrushBackground = new ImageBrush();
                //LImageBrushBackground.ImageSource = bitmapSource;
                //AWindowTarget.Background = LImageBrushBackground;

                ImageBrush b = new ImageBrush();
                b.ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/Images/Background02.jpg", UriKind.Absolute));
                b.Stretch = Stretch.Fill;
                AWindowTarget.Background = b;
            }
            catch { }
        }
    }
}
