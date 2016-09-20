//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7753acf2-a397-453a-a372-06c30efd1956
//        CLR Version:              4.0.30319.18444
//        Name:                     BufferHelpers
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Utils
//        File Name:                BufferHelpers
//
//        created by Charley at 2014/12/8 14:54:51
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.NAudio.Utils
{
    /// <summary>
    /// Helper methods for working with audio buffers
    /// </summary>
    public static class BufferHelpers
    {
        /// <summary>
        /// Ensures the buffer is big enough
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="bytesRequired"></param>
        /// <returns></returns>
        public static byte[] Ensure(byte[] buffer, int bytesRequired)
        {
            if (buffer == null || buffer.Length < bytesRequired)
            {
                buffer = new byte[bytesRequired];
            }
            return buffer;
        }

        /// <summary>
        /// Ensures the buffer is big enough
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="samplesRequired"></param>
        /// <returns></returns>
        public static float[] Ensure(float[] buffer, int samplesRequired)
        {
            if (buffer == null || buffer.Length < samplesRequired)
            {
                buffer = new float[samplesRequired];
            }
            return buffer;
        }
    }
}
