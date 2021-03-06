//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    cdd6f24e-37c4-413e-b17d-5adad24192dd
//        CLR Version:              4.0.30319.18444
//        Name:                     WaveFileChunkReader
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave
//        File Name:                WaveFileChunkReader
//
//        created by Charley at 2014/12/8 15:18:32
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace VoiceCyber.NAudio.Wave
{
    class WaveFileChunkReader
    {
        private WaveFormat waveFormat;
        private long dataChunkPosition;
        private long dataChunkLength;
        private List<RiffChunk> riffChunks;
        private bool strictMode;
        private bool isRf64;
        private bool storeAllChunks;
        private long riffSize;

        public WaveFileChunkReader()
        {
            storeAllChunks = true;
            strictMode = false;
        }

        public void ReadWaveHeader(Stream stream)
        {
            dataChunkPosition = -1;
            waveFormat = null;
            riffChunks = new List<RiffChunk>();
            dataChunkLength = 0;

            BinaryReader br = new BinaryReader(stream);
            ReadRiffHeader(br);
            riffSize = br.ReadUInt32(); // read the file size (minus 8 bytes)

            if (br.ReadInt32() != WaveInterop.mmioStringToFOURCC("WAVE", 0))
            {
                throw new FormatException("Not a WAVE file - no WAVE header");
            }

            if (isRf64)
            {
                ReadDs64Chunk(br);
            }

            int dataChunkID = WaveInterop.mmioStringToFOURCC("data", 0);
            int formatChunkId = WaveInterop.mmioStringToFOURCC("fmt ", 0);

            // sometimes a file has more data than is specified after the RIFF header
            long stopPosition = Math.Min(riffSize + 8, stream.Length);

            // this -8 is so we can be sure that there are at least 8 bytes for a chunk id and length
            while (stream.Position <= stopPosition - 8)
            {
                Int32 chunkIdentifier = br.ReadInt32();
                Int32 chunkLength = br.ReadInt32();
                if (chunkIdentifier == dataChunkID)
                {
                    dataChunkPosition = stream.Position;
                    if (!isRf64) // we already know the dataChunkLength if this is an RF64 file
                    {
                        dataChunkLength = chunkLength;
                    }
                    stream.Position += chunkLength;
                }
                else if (chunkIdentifier == formatChunkId)
                {
                    waveFormat = WaveFormat.FromFormatChunk(br, chunkLength);
                }
                else
                {
                    // check for invalid chunk length
                    if (chunkLength < 0 || chunkLength > stream.Length - stream.Position)
                    {
                        if (strictMode)
                        {
                            Debug.Assert(false, String.Format("Invalid chunk length {0}, pos: {1}. length: {2}",
                                chunkLength, stream.Position, stream.Length));
                        }
                        // an exception will be thrown further down if we haven't got a format and data chunk yet,
                        // otherwise we will tolerate this file despite it having corrupt data at the end
                        break;
                    }
                    if (storeAllChunks)
                    {
                        riffChunks.Add(GetRiffChunk(stream, chunkIdentifier, chunkLength));
                    }
                    stream.Position += chunkLength;
                }
            }

            if (waveFormat == null)
            {
                throw new FormatException("Invalid WAV file - No fmt chunk found");
            }
            if (dataChunkPosition == -1)
            {
                throw new FormatException("Invalid WAV file - No data chunk found");
            }
        }

        /// <summary>
        /// http://tech.ebu.ch/docs/tech/tech3306-2009.pdf
        /// </summary>
        private void ReadDs64Chunk(BinaryReader reader)
        {
            int ds64ChunkId = WaveInterop.mmioStringToFOURCC("ds64", 0);
            int chunkId = reader.ReadInt32();
            if (chunkId != ds64ChunkId)
            {
                throw new FormatException("Invalid RF64 WAV file - No ds64 chunk found");
            }
            int chunkSize = reader.ReadInt32();
            riffSize = reader.ReadInt64();
            dataChunkLength = reader.ReadInt64();
            // ReSharper disable once UnusedVariable
            long sampleCount = reader.ReadInt64(); // replaces the value in the fact chunk
            reader.ReadBytes(chunkSize - 24); // get to the end of this chunk (should parse extra stuff later)
        }

        private static RiffChunk GetRiffChunk(Stream stream, Int32 chunkIdentifier, Int32 chunkLength)
        {
            return new RiffChunk(chunkIdentifier, chunkLength, stream.Position);
        }

        private void ReadRiffHeader(BinaryReader br)
        {
            int header = br.ReadInt32();
            if (header == WaveInterop.mmioStringToFOURCC("RF64", 0))
            {
                isRf64 = true;
            }
            else if (header != WaveInterop.mmioStringToFOURCC("RIFF", 0))
            {
                throw new FormatException("Not a WAVE file - no RIFF header");
            }
        }

        /// <summary>
        /// WaveFormat
        /// </summary>
        public WaveFormat WaveFormat { get { return waveFormat; } }

        /// <summary>
        /// Data Chunk Position
        /// </summary>
        public long DataChunkPosition { get { return dataChunkPosition; } }

        /// <summary>
        /// Data Chunk Length
        /// </summary>
        public long DataChunkLength { get { return dataChunkLength; } }

        /// <summary>
        /// Riff Chunks
        /// </summary>
        public List<RiffChunk> RiffChunks { get { return riffChunks; } }
    }
}
