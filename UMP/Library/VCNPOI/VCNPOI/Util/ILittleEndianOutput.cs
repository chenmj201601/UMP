//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    74bdf972-5e10-461a-b529-eba04c8d09a2
//        CLR Version:              4.0.30319.42000
//        Name:                     ILittleEndianOutput
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.Util
//        File Name:                ILittleEndianOutput
//
//        Created by Charley at 2016/9/30 16:09:57
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.NPOI.Util
{
    /**
      * 
      * @author Josh Micich
      */
    public interface ILittleEndianOutput
    {
        void WriteByte(int v);
        void WriteShort(int v);
        void WriteInt(int v);
        void WriteLong(long v);
        void WriteDouble(double v);
        void Write(byte[] b);
        void Write(byte[] b, int offset, int len);
    }
}
