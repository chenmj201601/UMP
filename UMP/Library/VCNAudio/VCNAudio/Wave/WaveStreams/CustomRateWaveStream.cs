//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a8e61fab-9d9e-4630-be51-0f44cab45fe0
//        CLR Version:              4.0.30319.18444
//        Name:                     CustomRateWaveStream
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave
//        File Name:                CustomRateWaveStream
//
//        created by Charley at 2014/12/8 16:19:04
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.NAudio.Wave
{
    /// <summary>
    /// 实现可变速播放的音频流
    /// </summary>
    public class CustomRateWaveStream : WaveStream
    {
        private double mRate;
        private WaveStream mSourceStream;

        /// <summary>
        /// Play rate
        /// </summary>
        public double Rate
        {
            get { return mRate; }
            set { mRate = value; }
        }
        /// <summary>
        /// Gets the WaveFormat of this stream
        /// </summary>
        public override WaveFormat WaveFormat
        {
            get
            {
                return new WaveFormat((int)(mSourceStream.WaveFormat.SampleRate * mRate)
                    , mSourceStream.WaveFormat.BitsPerSample
                    , mSourceStream.WaveFormat.Channels);
            }
        }
        /// <summary>
        /// 构造一个默认速率为1的音频流
        /// </summary>
        /// <param name="sourceStream"></param>
        public CustomRateWaveStream(WaveStream sourceStream)
        {
            mSourceStream = sourceStream;
            mRate = 1;
        }
        /// <summary>
        /// 指定播放速率构造音频流
        /// </summary>
        /// <param name="sourceStream"></param>
        /// <param name="rate"></param>
        public CustomRateWaveStream(WaveStream sourceStream, double rate)
            : this(sourceStream)
        {
            mRate = rate;
        }
        /// <summary>
        /// Returns the stream length
        /// </summary>
        public override long Length
        {
            //get { return (long)(mSourceStream.Length * mRate); }
            get { return mSourceStream.Length; }
        }
        /// <summary>
        /// Gets or sets the current position in the stream
        /// </summary>
        public override long Position
        {
            get
            {
                return mSourceStream.Position;
            }
            set
            {
                mSourceStream.Position = value;
            }
        }
        /// <summary>
        /// Reads bytes from this stream
        /// </summary>
        /// <param name="buffer">Buffer to read into</param>
        /// <param name="offset">Offset in array to read into</param>
        /// <param name="count">Number of bytes to read</param>
        /// <returns>Number of bytes read</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            //return mSourceStream.Read(buffer, offset, (int)(count * mRate));
            return mSourceStream.Read(buffer, offset, count);
        }

        /// <summary>
        /// Disposes this WaveStream
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (mSourceStream != null)
                {
                    mSourceStream.Dispose();
                    mSourceStream = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}
