//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7e6d47f5-fe7b-4dc6-bca2-23f57755602c
//        CLR Version:              4.0.30319.18063
//        Name:                     ZipException
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPSetup.Zips
//        File Name:                ZipException
//
//        created by Charley at 2015/12/29 9:59:09
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Runtime.Serialization;

namespace UMPSetup.Zips
{
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
