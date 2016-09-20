//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    753f1502-be39-4e54-ba12-9431436bafac
//        CLR Version:              4.0.30319.18444
//        Name:                     IconConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Ribbon
//        File Name:                IconConverter
//
//        created by Charley at 2014/5/27 22:54:18
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VoiceCyber.Ribbon
{
    /// <summary>
    /// Icon converter provides default icon if user-defined is not present
    /// </summary>
    public sealed class IconConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                if (Application.Current != null
                    && Application.Current.MainWindow != null)
                {
                    try
                    {
                        return GetDefaultIcon((new WindowInteropHelper(Application.Current.MainWindow)).Handle) as BitmapFrame;
                    }
                    catch (InvalidOperationException)
                    {
                        return null;
                    }
                }

                var p = Process.GetCurrentProcess();
                if (p.MainWindowHandle != IntPtr.Zero)
                {
                    return GetDefaultIcon(p.MainWindowHandle/*(new WindowInteropHelper(Application.Current.MainWindow)).Handle*/) as BitmapFrame;
                }
            }

            var bitmapFrame = value as BitmapFrame;

            if (bitmapFrame == null
                || bitmapFrame.Decoder == null)
            {
                return null;
            }

            foreach (var frame in bitmapFrame.Decoder.Frames)
            {
                var source = GetThumbnail(frame);

                if (source != null)
                {
                    return source;
                }
            }

            return value;
        }

        /// <summary>
        /// ThumbnailExceptionWorkArround when image cause a format exception by accessing the Thumbnail
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        static BitmapSource GetThumbnail(BitmapSource frame)
        {
            try
            {
                if (frame != null
                    && frame.PixelWidth == 16
                    && frame.PixelHeight == 16
                    && (frame.Format == PixelFormats.Bgra32 || frame.Format == PixelFormats.Bgr24))
                {
                    return frame;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031")]
        static ImageSource GetDefaultIcon(IntPtr hwnd)
        {
            if (hwnd != IntPtr.Zero)
            {
                try
                {
                    var zero = NativeMethods.SendMessage(hwnd, 0x7f, new IntPtr(2), IntPtr.Zero);

                    if (zero == IntPtr.Zero)
                    {
                        zero = NativeMethods.GetClassLongPtr(hwnd, -34);
                    }

                    if (zero == IntPtr.Zero)
                    {
                        zero = NativeMethods.LoadImage(IntPtr.Zero, new IntPtr(0x7f00), 1, (int)SystemParameters.SmallIconWidth, (int)SystemParameters.SmallIconHeight, 0x8000);
                    }

                    if (zero != IntPtr.Zero)
                    {
                        return BitmapFrame.Create(Imaging.CreateBitmapSourceFromHIcon(zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight((int)SystemParameters.SmallIconWidth, (int)SystemParameters.SmallIconHeight)));
                    }
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        #endregion
    }
}
