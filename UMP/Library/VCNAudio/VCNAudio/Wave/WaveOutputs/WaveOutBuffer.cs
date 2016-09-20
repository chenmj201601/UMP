//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    fe3377a5-4229-4f7c-adea-ceacaa85e83f
//        CLR Version:              4.0.30319.18444
//        Name:                     WaveOutBuffer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave
//        File Name:                WaveOutBuffer
//
//        created by Charley at 2014/12/8 16:12:33
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Runtime.InteropServices;

namespace VoiceCyber.NAudio.Wave
{
    /// <summary>
    /// A buffer of Wave samples for streaming to a Wave Output device
    /// </summary>
    class WaveOutBuffer : IDisposable
    {
        private WaveHeader header;
        private Int32 bufferSize; // allocated bytes, may not be the same as bytes read
        private byte[] buffer;
        private GCHandle hBuffer;
        private IntPtr hWaveOut;
        private GCHandle hHeader; // we need to pin the header structure
        private GCHandle hThis; // for the user callback
        private IWaveProvider waveStream;
        private object waveOutLock;

        /// <summary>
        /// Channel mode
        /// Added by charley at 2013/4/9
        /// </summary>
        public WaveOutChannelMode ChannelMode { get; set; }
        /// <summary>
        /// 通道间隔长度，这是由音频格式决定的，volumnsize=waveformat.bitpersample/8
        /// 1   8bit    一个字节
        /// 2   16bit   两个字节
        /// 3   32bit   三个字节
        /// 4   64bit   四个字节
        /// Added by charley at 2013/4/9
        /// </summary>
        private int mVolumnSize;
        /// <summary>
        /// Tag of current volumn data
        /// Added by charley at 2013/4/9
        /// </summary>
        private bool mLeftVolumn;

        /// <summary>
        /// creates a new wavebuffer
        /// </summary>
        /// <param name="hWaveOut">WaveOut device to write to</param>
        /// <param name="bufferSize">Buffer size in bytes</param>
        /// <param name="bufferFillStream">Stream to provide more data</param>
        /// <param name="waveOutLock">Lock to protect WaveOut API's from being called on >1 thread</param>
        public WaveOutBuffer(IntPtr hWaveOut, Int32 bufferSize, IWaveProvider bufferFillStream, object waveOutLock)
        {
            this.bufferSize = bufferSize;
            buffer = new byte[bufferSize];
            hBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            this.hWaveOut = hWaveOut;
            waveStream = bufferFillStream;
            this.waveOutLock = waveOutLock;

            // Added by charley at 2013/4/9
            ChannelMode = WaveOutChannelMode.Default;
            mVolumnSize = 1;
            mLeftVolumn = true;
            mVolumnSize = bufferFillStream.WaveFormat.BitsPerSample / 8;

            header = new WaveHeader();
            hHeader = GCHandle.Alloc(header);
            header.dataBuffer = hBuffer.AddrOfPinnedObject();
            header.bufferLength = bufferSize;
            header.loops = 1;
            hThis = GCHandle.Alloc(this);
            header.userData = (IntPtr)hThis;
            lock (waveOutLock)
            {
                MmException.Try(WaveInterop.waveOutPrepareHeader(hWaveOut, header, Marshal.SizeOf(header)), "waveOutPrepareHeader");
            }
        }

        #region Dispose Pattern

        /// <summary>
        /// Finalizer for this wave buffer
        /// </summary>
        ~WaveOutBuffer()
        {
            Dispose(false);
            System.Diagnostics.Debug.Assert(true, "WaveBuffer was not disposed");
        }

        /// <summary>
        /// Releases resources held by this WaveBuffer
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        /// <summary>
        /// Releases resources held by this WaveBuffer
        /// </summary>
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
            }
            // free unmanaged resources
            if (hHeader.IsAllocated)
                hHeader.Free();
            if (hBuffer.IsAllocated)
                hBuffer.Free();
            if (hThis.IsAllocated)
                hThis.Free();
            if (hWaveOut != IntPtr.Zero)
            {
                lock (waveOutLock)
                {
                    WaveInterop.waveOutUnprepareHeader(hWaveOut, header, Marshal.SizeOf(header));
                }
                hWaveOut = IntPtr.Zero;
            }
        }

        #endregion

        /// this is called by the WAVE callback and should be used to refill the buffer
        internal bool OnDone()
        {
            int bytes;
            lock (waveStream)
            {
                bytes = waveStream.Read(buffer, 0, buffer.Length);
            }
            if (bytes == 0)
            {
                return false;
            }
            //Edit by charley at 2013/4/9
            //左右声道是交替出现的，所以在间隔处用0填充
            if (waveStream.WaveFormat.Channels > 1 && ChannelMode != WaveOutChannelMode.Default)
            {
                for (int i = 0; i < bytes - 1; i += mVolumnSize)
                {
                    if (ChannelMode == WaveOutChannelMode.Left)
                    {
                        for (int j = 0; j < mVolumnSize; j++)
                        {
                            buffer[i + j] = mLeftVolumn ? buffer[i + j] : (byte)0;
                        }
                        mLeftVolumn = !mLeftVolumn;
                    }
                    if (ChannelMode == WaveOutChannelMode.Right)
                    {
                        for (int j = 0; j < mVolumnSize; j++)
                        {
                            buffer[i + j] = !mLeftVolumn ? buffer[i + j] : (byte)0;
                        }
                        mLeftVolumn = !mLeftVolumn;
                    }
                }
            }

            for (int n = bytes; n < buffer.Length; n++)
            {
                buffer[n] = 0;
            }
            WriteToWaveOut();
            return true;
        }

        /// <summary>
        /// Whether the header's in queue flag is set
        /// </summary>
        public bool InQueue
        {
            get
            {
                return (header.flags & WaveHeaderFlags.InQueue) == WaveHeaderFlags.InQueue;
            }
        }

        /// <summary>
        /// The buffer size in bytes
        /// </summary>
        public Int32 BufferSize
        {
            get
            {
                return bufferSize;
            }
        }

        private void WriteToWaveOut()
        {
            MmResult result;

            lock (waveOutLock)
            {
                result = WaveInterop.waveOutWrite(hWaveOut, header, Marshal.SizeOf(header));
            }
            if (result != MmResult.NoError)
            {
                throw new MmException(result, "waveOutWrite");
            }

            GC.KeepAlive(this);
        }

    }
}
