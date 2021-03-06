//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d9407eb3-0107-4c75-b3f9-7b10cd157ba4
//        CLR Version:              4.0.30319.18063
//        Name:                     IChecksum
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.SharpZips.Checksums
//        File Name:                IChecksum
//
//        created by Charley at 2015/7/22 16:59:43
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.SharpZips.Checksums
{
    /// <summary>
    /// Interface to compute a data checksum used by checked input/output streams.
    /// A data checksum can be updated by one byte or with a byte array. After each
    /// update the value of the current checksum can be returned by calling
    /// <code>getValue</code>. The complete checksum object can also be reset
    /// so it can be used again with new data.
    /// </summary>
    public interface IChecksum
    {
        /// <summary>
        /// Returns the data checksum computed so far.
        /// </summary>
        long Value
        {
            get;
        }

        /// <summary>
        /// Resets the data checksum as if no update was ever called.
        /// </summary>
        void Reset();

        /// <summary>
        /// Adds one byte to the data checksum.
        /// </summary>
        /// <param name = "value">
        /// the data value to add. The high byte of the int is ignored.
        /// </param>
        void Update(int value);

        /// <summary>
        /// Updates the data checksum with the bytes taken from the array.
        /// </summary>
        /// <param name="buffer">
        /// buffer an array of bytes
        /// </param>
        void Update(byte[] buffer);

        /// <summary>
        /// Adds the byte array to the data checksum.
        /// </summary>
        /// <param name = "buffer">
        /// The buffer which contains the data
        /// </param>
        /// <param name = "offset">
        /// The offset in the buffer where the data starts
        /// </param>
        /// <param name = "count">
        /// the number of data bytes to add.
        /// </param>
        void Update(byte[] buffer, int offset, int count);
    }
}
