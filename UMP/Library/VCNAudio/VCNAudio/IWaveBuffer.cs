//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    c150d159-49d0-4159-aa21-c801c21d0443
//        CLR Version:              4.0.30319.18444
//        Name:                     IWaveBuffer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio
//        File Name:                IWaveBuffer
//
//        created by Charley at 2014/12/8 15:51:16
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.NAudio
{
    /// <summary>
    /// IWaveBuffer interface use to store wave datas. 
    /// Data can be manipulated with arrays (<see cref="ByteBuffer"/>,<see cref="FloatBuffer"/>,
    /// <see cref="ShortBuffer"/>,<see cref="IntBuffer"/> ) that are pointing to the same memory buffer.
    /// This is a requirement for all subclasses.
    /// 
    /// Use the associated Count property based on the type of buffer to get the number of data in the 
    /// buffer.
    /// 
    /// </summary>
    public interface IWaveBuffer
    {
        /// <summary>
        /// Gets the byte buffer.
        /// </summary>
        /// <value>The byte buffer.</value>
        byte[] ByteBuffer { get; }

        /// <summary>
        /// Gets the float buffer.
        /// </summary>
        /// <value>The float buffer.</value>
        float[] FloatBuffer { get; }

        /// <summary>
        /// Gets the short buffer.
        /// </summary>
        /// <value>The short buffer.</value>
        short[] ShortBuffer { get; }

        /// <summary>
        /// Gets the int buffer.
        /// </summary>
        /// <value>The int buffer.</value>
        int[] IntBuffer { get; }

        /// <summary>
        /// Gets the max size in bytes of the byte buffer..
        /// </summary>
        /// <value>Maximum number of bytes in the buffer.</value>
        int MaxSize { get; }

        /// <summary>
        /// Gets the byte buffer count.
        /// </summary>
        /// <value>The byte buffer count.</value>
        int ByteBufferCount { get; }

        /// <summary>
        /// Gets the float buffer count.
        /// </summary>
        /// <value>The float buffer count.</value>
        int FloatBufferCount { get; }

        /// <summary>
        /// Gets the short buffer count.
        /// </summary>
        /// <value>The short buffer count.</value>
        int ShortBufferCount { get; }

        /// <summary>
        /// Gets the int buffer count.
        /// </summary>
        /// <value>The int buffer count.</value>
        int IntBufferCount { get; }
    }
}
