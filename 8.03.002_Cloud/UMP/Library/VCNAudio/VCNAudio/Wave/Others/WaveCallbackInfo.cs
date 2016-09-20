//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    03f0563d-69b0-4c1d-863d-a09fee47b323
//        CLR Version:              4.0.30319.18444
//        Name:                     WaveCallbackInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave
//        File Name:                WaveCallbackInfo
//
//        created by Charley at 2014/12/8 15:53:25
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.NAudio.Wave
{
    /// <summary>
    /// Wave Callback Info
    /// </summary>
    public class WaveCallbackInfo
    {
        /// <summary>
        /// Callback Strategy
        /// </summary>
        public WaveCallbackStrategy Strategy { get; private set; }
        /// <summary>
        /// Window Handle (if applicable)
        /// </summary>
        public IntPtr Handle { get; private set; }

        private WaveWindow waveOutWindow;
        private WaveWindowNative waveOutWindowNative;

        /// <summary>
        /// Sets up a new WaveCallbackInfo for function callbacks
        /// </summary>
        public static WaveCallbackInfo FunctionCallback()
        {
            return new WaveCallbackInfo(WaveCallbackStrategy.FunctionCallback, IntPtr.Zero);
        }

        /// <summary>
        /// Sets up a new WaveCallbackInfo to use a New Window
        /// IMPORTANT: only use this on the GUI thread
        /// </summary>
        public static WaveCallbackInfo NewWindow()
        {
            return new WaveCallbackInfo(WaveCallbackStrategy.NewWindow, IntPtr.Zero);
        }

        /// <summary>
        /// Sets up a new WaveCallbackInfo to use an existing window
        /// IMPORTANT: only use this on the GUI thread
        /// </summary>
        public static WaveCallbackInfo ExistingWindow(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
            {
                throw new ArgumentException("Handle cannot be zero");
            }
            return new WaveCallbackInfo(WaveCallbackStrategy.ExistingWindow, handle);
        }

        private WaveCallbackInfo(WaveCallbackStrategy strategy, IntPtr handle)
        {
            Strategy = strategy;
            Handle = handle;
        }

        internal void Connect(WaveInterop.WaveCallback callback)
        {
            if (Strategy == WaveCallbackStrategy.NewWindow)
            {
                waveOutWindow = new WaveWindow(callback);
                waveOutWindow.CreateControl();
                Handle = waveOutWindow.Handle;
            }
            else if (Strategy == WaveCallbackStrategy.ExistingWindow)
            {
                waveOutWindowNative = new WaveWindowNative(callback);
                waveOutWindowNative.AssignHandle(Handle);
            }
        }

        internal MmResult WaveOutOpen(out IntPtr waveOutHandle, int deviceNumber, WaveFormat waveFormat, WaveInterop.WaveCallback callback)
        {
            MmResult result;
            if (Strategy == WaveCallbackStrategy.FunctionCallback)
            {
                result = WaveInterop.waveOutOpen(out waveOutHandle, (IntPtr)deviceNumber, waveFormat, callback, IntPtr.Zero, WaveInterop.WaveInOutOpenFlags.CallbackFunction);
            }
            else
            {
                result = WaveInterop.waveOutOpenWindow(out waveOutHandle, (IntPtr)deviceNumber, waveFormat, Handle, IntPtr.Zero, WaveInterop.WaveInOutOpenFlags.CallbackWindow);
            }
            return result;
        }

        internal MmResult WaveInOpen(out IntPtr waveInHandle, int deviceNumber, WaveFormat waveFormat, WaveInterop.WaveCallback callback)
        {
            MmResult result;
            if (Strategy == WaveCallbackStrategy.FunctionCallback)
            {
                result = WaveInterop.waveInOpen(out waveInHandle, (IntPtr)deviceNumber, waveFormat, callback, IntPtr.Zero, WaveInterop.WaveInOutOpenFlags.CallbackFunction);
            }
            else
            {
                result = WaveInterop.waveInOpenWindow(out waveInHandle, (IntPtr)deviceNumber, waveFormat, Handle, IntPtr.Zero, WaveInterop.WaveInOutOpenFlags.CallbackWindow);
            }
            return result;
        }

        internal void Disconnect()
        {
            if (waveOutWindow != null)
            {
                waveOutWindow.Close();
                waveOutWindow = null;
            }
            if (waveOutWindowNative != null)
            {
                waveOutWindowNative.ReleaseHandle();
                waveOutWindowNative = null;
            }
        }
    }
}
