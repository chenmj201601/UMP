//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    1c0f9e53-13a8-4274-a6e0-9d6ac40317a1
//        CLR Version:              4.0.30319.18063
//        Name:                     ZipException
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.SharpZips.Zip
//        File Name:                ZipException
//
//        created by Charley at 2015/7/22 17:19:55
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Runtime.Serialization;

namespace VoiceCyber.SharpZips.Zip
{

    /// <summary>
    /// Represents exception conditions specific to Zip archive handling
    /// </summary>
#if !NETCF_1_0 && !NETCF_2_0
    [Serializable]
#endif
    public class ZipException : SharpZipBaseException
    {
#if !NETCF_1_0 && !NETCF_2_0
        /// <summary>
        /// Deserialization constructor 
        /// </summary>
        /// <param name="info"><see cref="SerializationInfo"/> for this constructor</param>
        /// <param name="context"><see cref="StreamingContext"/> for this constructor</param>
        protected ZipException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif

        /// <summary>
        /// Initializes a new instance of the ZipException class.
        /// </summary>
        public ZipException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ZipException class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public ZipException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initialise a new instance of ZipException.
        /// </summary>
        /// <param name="message">A message describing the error.</param>
        /// <param name="exception">The exception that is the cause of the current exception.</param>
        public ZipException(string message, Exception exception)
            : base(message, exception)
        {
        }
    }
}
