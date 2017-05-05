//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    8737c6eb-ee27-44ed-9931-6dfb5476b4c2
//        CLR Version:              4.0.30319.42000
//        Name:                     ILittleEndianInput
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.Util
//        File Name:                ILittleEndianInput
//
//        Created by Charley at 2016/9/30 16:07:25
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.NPOI.Util
{
    /**
    * 
    * @author Josh Micich
    */
    public interface ILittleEndianInput
    {
        int Available();
        int ReadByte();
        int ReadUByte();
        short ReadShort();
        int ReadUShort();
        int ReadInt();
        long ReadLong();
        double ReadDouble();
        void ReadFully(byte[] buf);
        void ReadFully(byte[] buf, int off, int len);
    }
}
