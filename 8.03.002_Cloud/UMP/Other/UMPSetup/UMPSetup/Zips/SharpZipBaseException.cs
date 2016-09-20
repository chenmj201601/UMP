//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7c3af93c-d589-48a7-a159-0a16c83020b5
//        CLR Version:              4.0.30319.18063
//        Name:                     SharpZipBaseException
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPSetup.Zips
//        File Name:                SharpZipBaseException
//
//        created by Charley at 2015/12/29 9:49:56
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
    public class SharpZipBaseException : ApplicationException
    {
#if !NETCF_1_0 && !NETCF_2_0
        /// <summary>
        /// Deserialization constructor 
        /// </summary>
        /// <param name="info"><see cref="System.Runtime.Serialization.SerializationInfo"/> for this constructor</param>
        /// <param name="context"><see cref="StreamingContext"/> for this constructor</param>
        protected SharpZipBaseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif

        /// <summary>
        /// Initializes a new instance of the SharpZipBaseException class.
        /// </summary>
        public SharpZipBaseException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the SharpZipBaseException class with a specified error message.
        /// </summary>
        /// <param name="message">A message describing the exception.</param>
        public SharpZipBaseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the SharpZipBaseException class with a specified
        /// error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">A message describing the exception.</param>
        /// <param name="innerException">The inner exception</param>
        public SharpZipBaseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
