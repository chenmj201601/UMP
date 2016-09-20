//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    74f1b4c8-975d-427a-9710-172adb75a49a
//        CLR Version:              4.0.30319.18444
//        Name:                     RiffChunk
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave
//        File Name:                RiffChunk
//
//        created by Charley at 2014/12/8 15:17:45
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Text;

namespace VoiceCyber.NAudio.Wave
{
    /// <summary>
    /// Holds information about a RIFF file chunk
    /// </summary>
    public class RiffChunk
    {
        int identifier;
        int length;
        long streamPosition;

        /// <summary>
        /// Creates a RiffChunk object
        /// </summary>
        public RiffChunk(int identifier, int length, long streamPosition)
        {
            this.identifier = identifier;
            this.length = length;
            this.streamPosition = streamPosition;
        }

        /// <summary>
        /// The chunk identifier
        /// </summary>
        public int Identifier
        {
            get
            {
                return identifier;
            }
        }

        /// <summary>
        /// The chunk identifier converted to a string
        /// </summary>
        public string IdentifierAsString
        {
            get
            {
                return Encoding.ASCII.GetString(BitConverter.GetBytes(identifier));
            }
        }

        /// <summary>
        /// The chunk length
        /// </summary>
        public int Length
        {
            get
            {
                return length;
            }
        }

        /// <summary>
        /// The stream position this chunk is located at
        /// </summary>
        public long StreamPosition
        {
            get
            {
                return streamPosition;
            }
        }
    }
}
