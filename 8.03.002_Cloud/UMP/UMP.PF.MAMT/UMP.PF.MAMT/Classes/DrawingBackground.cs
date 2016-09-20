using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UMP.PF.MAMT.Classes
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

        public static void DrawWindowsBackgond(Window AWindowTarget, string strImagePath)
        {
            try
            {
                System.Drawing.Bitmap LBitmapBackgroud = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(System.IO.Path.Combine(App.GStrApplicationDirectory, strImagePath));
                //System.Drawing.Bitmap LBitmapBackgroud = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000000.png"));
                BitmapSource bitmapSource = GetBitmapSource(LBitmapBackgroud);
                ImageBrush LImageBrushBackground = new ImageBrush();
                LImageBrushBackground.ImageSource = bitmapSource;
                AWindowTarget.Background = LImageBrushBackground;
            }
            catch { }
        }

        public static void DrawWindowsBackgond(System.Windows.Controls.UserControl AWindowTarget, string strImagePath)
        {
            try
            {
                System.Drawing.Bitmap LBitmapBackgroud = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(System.IO.Path.Combine(App.GStrApplicationDirectory, strImagePath));
                //System.Drawing.Bitmap LBitmapBackgroud = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000000.png"));
                BitmapSource bitmapSource = GetBitmapSource(LBitmapBackgroud);
                ImageBrush LImageBrushBackground = new ImageBrush();
                LImageBrushBackground.ImageSource = bitmapSource;
                AWindowTarget.Background = LImageBrushBackground;
            }
            catch { }
        }
    }
}
