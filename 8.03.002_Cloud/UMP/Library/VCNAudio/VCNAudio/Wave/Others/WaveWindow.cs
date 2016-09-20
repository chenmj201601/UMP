//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b9eab1e6-36d6-4ddf-8b12-46d5ec875d09
//        CLR Version:              4.0.30319.18444
//        Name:                     WaveWindow
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave
//        File Name:                WaveWindow
//
//        created by Charley at 2014/12/8 15:58:28
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VoiceCyber.NAudio.Wave
{
    class WaveWindowNative : NativeWindow
    {
        private WaveInterop.WaveCallback waveCallback;

        public WaveWindowNative(WaveInterop.WaveCallback waveCallback)
        {
            this.waveCallback = waveCallback;
        }

        protected override void WndProc(ref Message m)
        {
            WaveInterop.WaveMessage message = (WaveInterop.WaveMessage)m.Msg;

            switch (message)
            {
                case WaveInterop.WaveMessage.WaveOutDone:
                case WaveInterop.WaveMessage.WaveInData:
                    IntPtr hOutputDevice = m.WParam;
                    WaveHeader waveHeader = new WaveHeader();
                    Marshal.PtrToStructure(m.LParam, waveHeader);
                    waveCallback(hOutputDevice, message, IntPtr.Zero, waveHeader, IntPtr.Zero);
                    break;
                case WaveInterop.WaveMessage.WaveOutOpen:
                case WaveInterop.WaveMessage.WaveOutClose:
                case WaveInterop.WaveMessage.WaveInClose:
                case WaveInterop.WaveMessage.WaveInOpen:
                    waveCallback(m.WParam, message, IntPtr.Zero, null, IntPtr.Zero);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }

    class WaveWindow : Form
    {
        private WaveInterop.WaveCallback waveCallback;

        public WaveWindow(WaveInterop.WaveCallback waveCallback)
        {
            this.waveCallback = waveCallback;
        }

        protected override void WndProc(ref Message m)
        {
            WaveInterop.WaveMessage message = (WaveInterop.WaveMessage)m.Msg;

            switch (message)
            {
                case WaveInterop.WaveMessage.WaveOutDone:
                case WaveInterop.WaveMessage.WaveInData:
                    IntPtr hOutputDevice = m.WParam;
                    WaveHeader waveHeader = new WaveHeader();
                    Marshal.PtrToStructure(m.LParam, waveHeader);
                    waveCallback(hOutputDevice, message, IntPtr.Zero, waveHeader, IntPtr.Zero);
                    break;
                case WaveInterop.WaveMessage.WaveOutOpen:
                case WaveInterop.WaveMessage.WaveOutClose:
                case WaveInterop.WaveMessage.WaveInClose:
                case WaveInterop.WaveMessage.WaveInOpen:
                    waveCallback(m.WParam, message, IntPtr.Zero, null, IntPtr.Zero);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }
}
