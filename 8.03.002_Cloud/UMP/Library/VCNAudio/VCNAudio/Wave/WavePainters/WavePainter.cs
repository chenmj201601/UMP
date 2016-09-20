//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    1425cb94-9b4e-4e0e-9ad8-eeee4b49d8ee
//        CLR Version:              4.0.30319.18444
//        Name:                     WavePainter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave
//        File Name:                WavePainter
//
//        created by Charley at 2014/12/9 11:51:54
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Drawing;
using System.IO;
using VoiceCyber.NAudio.Wave.SampleProviders;

namespace VoiceCyber.NAudio.Wave
{
    /// <summary>
    /// 绘制音频波形图
    /// </summary>
    public class WavePainter
    {
        private int mPngWidth;
        private int mPngHeight;
        private string mSrcPath;
        private string mBitmapPath;
        private string mBitmapPath1;
        private int mVolumnNum;
        private Graphics mGraphics;
        private Bitmap mBitmap;
        private Graphics mGraphics1;
        private Bitmap mBitmap1;
        private float mVolume;
        private float mVolume1;
        private float mPos;
        private Brush mDrawBrush;
        private WaveStreamType mSourceType;
        private bool mIsSaveFile;

        #region Properties

        /// <summary>
        /// Png file's Width,default is 1000
        /// </summary>
        public int PngWidth
        {
            set { mPngWidth = value; }
        }
        /// <summary>
        /// Png file's Height,default is 300
        /// </summary>
        public int PngHeight
        {
            set { mPngHeight = value; }
        }
        /// <summary>
        /// Source wave file path
        /// </summary>
        public string SrcPath
        {
            set { mSrcPath = value; }
        }
        /// <summary>
        /// Path for Saving png file
        /// </summary>
        public string BitmapPath
        {
            set { mBitmapPath = value; }
        }
        /// <summary>
        /// Path for saving png file (right volumn for stereo wave)
        /// </summary>
        public string BitmapPath1
        {
            set { mBitmapPath1 = value; }
        }
        /// <summary>
        /// Get the bitmap image
        /// </summary>
        public Bitmap Bitmap
        {
            get { return mBitmap; }
        }
        /// <summary>
        /// Get the bitmap image (right volumn for stereo wave)
        /// </summary>
        public Bitmap Bitmap1
        {
            get { return mBitmap; }
        }
        /// <summary>
        /// Force draw single volumn image
        /// </summary>
        public int VolumnNum
        {
            set
            {
                mVolumnNum = value == 2 ? 2 : 1;
            }
        }
        /// <summary>
        /// Set draw brush of bitmap
        /// </summary>
        public Brush DrawBrush
        {
            set { mDrawBrush = value; }
        }
        /// <summary>
        /// Set media source type
        /// </summary>
        public WaveStreamType SourceType
        {
            set { mSourceType = value; }
        }
        /// <summary>
        /// Save Png File
        /// </summary>
        public bool IsSaveFile
        {
            set { mIsSaveFile = value; }
        }

        #endregion

        /// <summary>
        /// 构造一个波形图绘制类的实例
        /// </summary>
        public WavePainter()
        {
            mPngWidth = Defines.PNG_WIDTH;
            mPngHeight = Defines.PNG_HEIGHT;
            mSrcPath = string.Empty;
            mBitmapPath = string.Empty;
            mBitmapPath1 = string.Empty;
            mVolumnNum = 1;
            mDrawBrush = Brushes.Green;
            mSourceType = WaveStreamType.LocalFile;
            mIsSaveFile = true;
        }
        /// <summary>
        /// 绘制波形图
        /// </summary>
        /// <returns></returns>
        public OptReturn Draw()
        {
            OptReturn optReturn = new OptReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            switch (mSourceType)
            {
                case WaveStreamType.LocalFile:
                    if (!Path.IsPathRooted(mSrcPath))
                    {
                        mSrcPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, mSrcPath);
                    }
                    if (!File.Exists(mSrcPath))
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_FILE_NOT_EXIST;
                        optReturn.Message = string.Format("Source wave file not exist.\tpath:{0}", mSrcPath);
                        return optReturn;
                    }
                    try
                    {
                        WaveStream waveStream = new WaveFileReader(File.OpenRead(mSrcPath));
                        return Draw(waveStream);
                    }
                    catch (Exception ex)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_FAIL;
                        optReturn.Message = ex.Message;
                        return optReturn;
                    }
                case WaveStreamType.NetworkStream:
                    try
                    {
                        WaveStream waveStream = new NetWorkWaveReader(mSrcPath);
                        return Draw(waveStream);
                    }
                    catch (Exception ex)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_FAIL;
                        optReturn.Message = ex.Message;
                        return optReturn;
                    }
                default:
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FAIL;
                    optReturn.Message = string.Format("Media source type invalid.\t{0}", mSourceType);
                    return optReturn;
            }
        }

        /// <summary>
        /// 使用指定的音频流绘制波形图
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public OptReturn Draw(WaveStream stream)
        {
            OptReturn optReturn = new OptReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            try
            {
                WaveStream reader = stream;
                reader.Position = 0;

                if (reader.WaveFormat.Encoding != WaveFormatEncoding.Pcm && reader.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
                {
                    reader = WaveFormatConversionStream.CreatePcmStream(reader);
                    reader = new BlockAlignReductionStream(reader);
                }
                InitDraw();
                var waveChannel = new SampleChannel(reader, true);
                int samples = (int)(reader.TotalTime.TotalSeconds) * reader.WaveFormat.SampleRate / mPngWidth;
                var postVolumeMeter = new MeteringSampleProvider(waveChannel, samples);
                postVolumeMeter.StreamVolume += postVolumeMeter_StreamVolume;
                var waveProvider = new SampleToWaveProvider(postVolumeMeter);
                while (true)
                {
                    byte[] buffer = new byte[waveProvider.WaveFormat.AverageBytesPerSecond];
                    int readcount = waveProvider.Read(buffer, 0, waveProvider.WaveFormat.AverageBytesPerSecond);
                    if (readcount <= 0)
                    {
                        break;
                    }
                }
                stream.Position = 0;
                if (mIsSaveFile)
                {
                    return GenerateImageFile();
                }
                Bitmap[] bitmaps = new Bitmap[2];
                bitmaps[0] = mBitmap;
                bitmaps[1] = mBitmap1;
                optReturn.Data = bitmaps;
                return optReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                return optReturn;
            }
        }

        private OptReturn GenerateImageFile()
        {
            OptReturn optReturn = new OptReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            try
            {
                if (string.IsNullOrEmpty(mBitmapPath))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_STRING_EMPTY;
                    optReturn.Message = string.Format("BitmapPath empty.");
                    return optReturn;
                }
                if (File.Exists(mBitmapPath)) { File.Delete(mBitmapPath); }
                mBitmap.Save(mBitmapPath);
                if (mVolumnNum == 2)
                {
                    if (string.IsNullOrEmpty(mBitmapPath1))
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_STRING_EMPTY;
                        optReturn.Message = string.Format("BitmapPath empty.");
                        return optReturn;
                    }
                    if (File.Exists(mBitmapPath1)) { File.Delete(mBitmapPath1); }
                    mBitmap1.Save(mBitmapPath1);
                }
                return optReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                return optReturn;
            }
        }

        void postVolumeMeter_StreamVolume(object sender, StreamVolumeEventArgs e)
        {
            //Draw bitmap
            if (mGraphics != null && mGraphics1 != null)
            {
                float volume;
                float volume1;

                if (e.MaxSampleValues.Length > 1)
                {
                    if (mVolumnNum == 2)
                    {
                        volume = e.MaxSampleValues[0] * (float)(mPngHeight / 2.0);
                        volume1 = e.MaxSampleValues[1] * (float)(mPngHeight / 2.0);
                    }
                    else
                    {
                        volume = Math.Max(e.MaxSampleValues[0], e.MaxSampleValues[1]) * (float)(mPngHeight / 2.0);
                        volume1 = 0;
                    }
                }
                else
                {
                    if (mVolumnNum == 2)
                    {
                        volume = e.MaxSampleValues[0] * (float)(mPngHeight / 2.0);
                        volume1 = volume;
                    }
                    else
                    {
                        volume = e.MaxSampleValues[0] * (float)(mPngHeight / 2.0);
                        volume1 = 0;
                    }
                }

                PointF volume11 = new PointF(mPos,(float)(mPngHeight / 2.0)- mVolume);
                PointF volume111 = new PointF(mPos, (float)(mPngHeight / 2.0) - mVolume1);
                PointF volume12 = new PointF(mPos, (float)(mPngHeight / 2.0) + mVolume);
                PointF volume112 = new PointF(mPos, (float)(mPngHeight / 2.0) + mVolume1);
                mPos += 1;
                PointF volume21 = new PointF(mPos, (float)(mPngHeight / 2.0) - volume);
                PointF volume121 = new PointF(mPos, (float)(mPngHeight / 2.0) - volume1);
                PointF volume22 = new PointF(mPos, (float)(mPngHeight / 2.0) + volume);
                PointF volume122 = new PointF(mPos, (float)(mPngHeight / 2.0) + volume1);
                PointF[] points = { volume11, volume21, volume22, volume12 };
                PointF[] points1 = { volume111, volume121, volume122, volume112 };
                mGraphics.FillPolygon(mDrawBrush, points);
                mGraphics1.FillPolygon(mDrawBrush, points1);
                //mGraphics.FillPolygon(Brushes.Blue, points);
                mVolume = volume;
                mVolume1 = volume1;
            }
        }
        private void InitDraw()
        {
            int width = mPngWidth;
            int height = mPngHeight;
            mVolume = 0;
            mVolume1 = 0;
            mPos = 0;
            mBitmap = new Bitmap(width, height);
            mBitmap1 = new Bitmap(width, height);
            mGraphics = Graphics.FromImage(mBitmap);
            mGraphics1 = Graphics.FromImage(mBitmap1);

        }
    }
}
