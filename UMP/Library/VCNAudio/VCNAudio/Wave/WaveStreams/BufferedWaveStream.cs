//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7e5e8e8d-1d82-4de2-b55b-2f2b897c4335
//        CLR Version:              4.0.30319.18444
//        Name:                     BufferedWaveStream
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave
//        File Name:                BufferedWaveStream
//
//        created by Charley at 2014/12/8 16:18:17
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.IO;

namespace VoiceCyber.NAudio.Wave
{
    /// <summary>
    /// Buffered wave stream which inherit WaveStream,this stream is memorystream and can be circled.
    /// Call write to write data to its stream and call Read to read data from its stream.
    /// Be careful its Read function,it will return 0 if we have readed all avaliable data.
    /// Create by Charley at 2013/4/4
    /// </summary>
    public class BufferedWaveStream : WaveStream
    {
        private const int BUFFERSIZE = 1024000;  //1000K

        private WaveFormat mWaveFormat;
        private MemoryStream mWaveStream;
        private int mReadPosition;
        private int mWritePosition;
        private int mLength;

        /// <summary>
        /// 创建一个循环内存流
        /// </summary>
        /// <param name="waveFormat"></param>
        public BufferedWaveStream(WaveFormat waveFormat)
        {
            mWaveFormat = waveFormat;
            mWaveStream = new MemoryStream(BUFFERSIZE);
            mReadPosition = 0;
            mWritePosition = 0;
            mLength = 0;
        }

        public override WaveFormat WaveFormat
        {
            get { return mWaveFormat; }
        }
        /// <summary>
        /// Length
        /// </summary>
        public override long Length
        {
            get
            {
                return mLength;
            }
        }
        /// <summary>
        /// Position
        /// </summary>
        public override long Position
        {
            get
            {
                return mReadPosition;
            }
            set
            {
                mReadPosition = (int)value;
            }
        }
        /// <summary>
        /// 向流中写入数据，达到最大值则从0位置接着写
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (mReadPosition - mWritePosition > 0 && mReadPosition - mWritePosition < BUFFERSIZE / 2)
            {
                throw new Exception("Buffer is full");
            }
            //Write this block,write position still lower than the memorystream's capacity
            if ((mWritePosition + count) <= BUFFERSIZE)
            {
                mWaveStream.Position = mWritePosition;
                mWaveStream.Write(buffer, 0, count);
                mWritePosition += count;
            }
            //Write this block,memorystream will be full,so write the rest data circle
            else
            {
                int rest = BUFFERSIZE - mWritePosition;
                mWaveStream.Position = mWritePosition;
                mWaveStream.Write(buffer, 0, rest);
                mWaveStream.Position = 0;
                mWritePosition = count - rest;
                mWaveStream.Write(buffer, rest, mWritePosition);
            }
            //base.Write(buffer, offset, count);
        }
        /// <summary>
        /// 从流中读取数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (count % mWaveFormat.BlockAlign != 0)
            {
                throw new ArgumentException(String.Format("Must read complete blocks: requested {0}, block align is {1}", count, WaveFormat.BlockAlign));
            }
            if (mReadPosition <= mWritePosition)
            {
                if (mReadPosition + count <= mWritePosition)
                {
                    mWaveStream.Position = mReadPosition;
                    mReadPosition += count;
                    return mWaveStream.Read(buffer, offset, count);
                }
                mWaveStream.Position = mReadPosition;
                count = mWritePosition - mReadPosition;
                mReadPosition += count;
                return mWaveStream.Read(buffer, offset, count);
            }
            if (mReadPosition + count <= BUFFERSIZE)
            {
                mWaveStream.Position = mReadPosition;
                mReadPosition += count;
                return mWaveStream.Read(buffer, offset, count);
            }
            mWaveStream.Position = mReadPosition;
            int already = BUFFERSIZE - mReadPosition;
            mWaveStream.Read(buffer, offset, already);
            if (count - already <= mWritePosition)
            {
                int rest = count - already;
                mWaveStream.Position = 0;
                mWaveStream.Read(buffer, already, rest);
                mReadPosition = rest;
                return count;
            }
            else
            {
                int rest = mWritePosition;
                mWaveStream.Position = 0;
                mWaveStream.Read(buffer, already, rest);
                mReadPosition = rest;
                return already + rest;
            }
            //throw new NotImplementedException();
        }
        /// <summary>
        /// 释放对象
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (mWaveStream != null)
                {
                    mWaveStream.Dispose();
                    mWaveStream = null;
                }
            }
            else
            {
                System.Diagnostics.Debug.Assert(false, "BufferedWaveStream was not disposed");
            }
            base.Dispose(disposing);
        }
    }
}
