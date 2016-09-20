//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6034ab52-10a1-4f56-9dcb-1f3d358719a9
//        CLR Version:              4.0.30319.34003
//        Name:                     AcmStreamHeader
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave.Compression
//        File Name:                AcmStreamHeader
//
//        created by Charley at 2013/12/1 13:11:11
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Runtime.InteropServices;

namespace VoiceCyber.NAudio.Wave.Compression
{
    class AcmStreamHeader : IDisposable
    {
        private AcmStreamHeaderStruct streamHeader;
        private byte[] sourceBuffer;
        private GCHandle hSourceBuffer;
        private byte[] destBuffer;
        private GCHandle hDestBuffer;
        private IntPtr streamHandle;
        private bool firstTime;

        public AcmStreamHeader(IntPtr streamHandle, int sourceBufferLength, int destBufferLength)
        {
            streamHeader = new AcmStreamHeaderStruct();
            sourceBuffer = new byte[sourceBufferLength];
            hSourceBuffer = GCHandle.Alloc(sourceBuffer, GCHandleType.Pinned);

            destBuffer = new byte[destBufferLength];
            hDestBuffer = GCHandle.Alloc(destBuffer, GCHandleType.Pinned);

            this.streamHandle = streamHandle;
            firstTime = true;
            //Prepare();
        }

        private void Prepare()
        {
            streamHeader.cbStruct = Marshal.SizeOf(streamHeader);
            streamHeader.sourceBufferLength = sourceBuffer.Length;
            streamHeader.sourceBufferPointer = hSourceBuffer.AddrOfPinnedObject();
            streamHeader.destBufferLength = destBuffer.Length;
            streamHeader.destBufferPointer = hDestBuffer.AddrOfPinnedObject();
            MmException.Try(AcmInterop.acmStreamPrepareHeader(streamHandle, streamHeader, 0), "acmStreamPrepareHeader");
        }

        private void Unprepare()
        {
            streamHeader.sourceBufferLength = sourceBuffer.Length;
            streamHeader.sourceBufferPointer = hSourceBuffer.AddrOfPinnedObject();
            streamHeader.destBufferLength = destBuffer.Length;
            streamHeader.destBufferPointer = hDestBuffer.AddrOfPinnedObject();

            MmResult result = AcmInterop.acmStreamUnprepareHeader(streamHandle, streamHeader, 0);
            if (result != MmResult.NoError)
            {
                //if (result == MmResult.AcmHeaderUnprepared)
                throw new MmException(result, "acmStreamUnprepareHeader");
            }
        }

        public int Convert(int bytesToConvert, out int sourceBytesConverted)
        {
            Prepare();
            try
            {
                streamHeader.sourceBufferLength = bytesToConvert;
                streamHeader.sourceBufferLengthUsed = bytesToConvert;
                AcmStreamConvertFlags flags = firstTime ? (AcmStreamConvertFlags.Start | AcmStreamConvertFlags.BlockAlign) : AcmStreamConvertFlags.BlockAlign;
                MmException.Try(AcmInterop.acmStreamConvert(streamHandle, streamHeader, flags), "acmStreamConvert");
                firstTime = false;
                System.Diagnostics.Debug.Assert(streamHeader.destBufferLength == destBuffer.Length, "Codecs should not change dest buffer length");
                sourceBytesConverted = streamHeader.sourceBufferLengthUsed;
            }
            finally
            {
                Unprepare();
            }

            return streamHeader.destBufferLengthUsed;
        }

        public byte[] SourceBuffer
        {
            get
            {
                return sourceBuffer;
            }
        }

        public byte[] DestBuffer
        {
            get
            {
                return destBuffer;
            }
        }

        #region IDisposable Members

        bool disposed;

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                //Unprepare();
                sourceBuffer = null;
                destBuffer = null;
                hSourceBuffer.Free();
                hDestBuffer.Free();
            }
            disposed = true;
        }

        ~AcmStreamHeader()
        {
            //-------Comment this line so client will not get the error. Edit by Charley at 2013/4/4
            //System.Diagnostics.Debug.Assert(false, "AcmStreamHeader dispose was not called");
            Dispose(false);
        }
        #endregion
    }
}
