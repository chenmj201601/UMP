//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    269f1e70-9b80-4219-9cf0-5c81548db78b
//        CLR Version:              4.0.30319.18444
//        Name:                     MmException
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio
//        File Name:                MmException
//
//        created by Charley at 2014/12/8 14:38:27
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.NAudio
{
    /// <summary>
    /// Summary description for MmException.
    /// </summary>
    public class MmException : Exception
    {
        private MmResult result;

        /// <summary>
        /// Creates a new MmException
        /// </summary>
        /// <param name="result">The result returned by the Windows API call</param>
        /// <param name="function">The name of the Windows API that failed</param>
        public MmException(MmResult result, string function)
            : base(ErrorMessage(result, function))
        {
            this.result = result;
        }


        private static string ErrorMessage(MmResult result, string function)
        {
            return String.Format("{0} calling {1}", result, function);
        }

        /// <summary>
        /// Helper function to automatically raise an exception on failure
        /// </summary>
        /// <param name="result">The result of the API call</param>
        /// <param name="function">The API function name</param>
        public static void Try(MmResult result, string function)
        {
            if (result != MmResult.NoError)
                throw new MmException(result, function);
        }

        /// <summary>
        /// Returns the Windows API result
        /// </summary>
        public MmResult Result
        {
            get
            {
                return result;
            }
        }
    }
}
