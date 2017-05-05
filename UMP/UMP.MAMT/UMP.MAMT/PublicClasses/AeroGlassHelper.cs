using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace UMP.MAMT.PublicClasses
{
    [StructLayout(LayoutKind.Sequential)]
    public struct StructMargins
    {
        public int Left;
        public int Right;
        public int Top;
        public int Bottom;

        public StructMargins(Thickness AThickness)
        {
            Left = (int)AThickness.Left;
            Right = (int)AThickness.Right;
            Top = (int)AThickness.Top;
            Bottom = (int)AThickness.Bottom;
        }
    }

    public class AeroGlassHelper
    {
        [DllImport("dwmapi.dll", PreserveSig = false)]
        static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref StructMargins pMarInset);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        static extern bool DwmIsCompositionEnabled();

        public static bool ExtendGlassFrame(Window AWindow, Thickness AThicknessMargin)
        {
            try
            {
                if (!DwmIsCompositionEnabled()) { return false; }

                IntPtr LIntPtrHandle = new WindowInteropHelper(AWindow).Handle;
                if (LIntPtrHandle == IntPtr.Zero) { return false; }

                AWindow.Background = Brushes.Transparent;
                HwndSource.FromHwnd(LIntPtrHandle).CompositionTarget.BackgroundColor = Colors.Transparent;

                StructMargins LStructMargins = new StructMargins(AThicknessMargin);
                DwmExtendFrameIntoClientArea(LIntPtrHandle, ref LStructMargins);
            }
            catch { return false; }

            return true;
        }
    }
}
