//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    002d47db-4555-4762-b270-a187d0862b89
//        CLR Version:              4.0.30319.34003
//        Name:                     MmException
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio
//        File Name:                MmException
//
//        created by Charley at 2013/12/1 11:55:16
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
        private string mFunction;

        /// <summary>
        /// Creates a new MmException
        /// </summary>
        /// <param name="result">The result returned by the Windows API call</param>
        /// <param name="function">The name of the Windows API that failed</param>
        public MmException(MmResult result, string function)
            : base(ErrorMessage(result, function))
        {
            this.result = result;
            mFunction = function;
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
