//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    30830a2a-bd59-4870-8a66-53ad6bbd2438
//        CLR Version:              4.0.30319.18063
//        Name:                     MessageString
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.SDKs.DEC
//        File Name:                MessageString
//
//        created by Charley at 2015/6/17 18:18:51
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.SDKs.DEC
{
    /// <summary>
    /// DEC 消息部分
    /// </summary>
    public class MessageString
    {
        public ushort SourceModule { get; set; }
        public ushort SourceNumber { get; set; }
        public ushort TargetModule { get; set; }
        public ushort TargetNumber { get; set; }
        public ushort Number { get; set; }
        public ushort SmallType { get; set; }
        public ushort MiddleType { get; set; }
        public ushort LargeType { get; set; }

        internal _TAG_NETPACK_MESSAGE GetMessage()
        {
            return Helpers.CreateMessage(SourceModule, SourceNumber, TargetModule, TargetNumber, Number, SmallType,
                MiddleType, LargeType);
        }

        internal static MessageString FromMessage(_TAG_NETPACK_MESSAGE message)
        {
            MessageString str=new MessageString();
            str.TargetNumber = message._targetNumber;
            str.TargetModule = message._targetModule;
            str.SourceNumber = message._sourceNumber;
            str.SourceModule = message._sourceModule;
            str.Number = message._number;
            str.SmallType = message._samllType;
            str.MiddleType = message._middleType;
            str.LargeType = message._largeType;
            return str;
        }

        public override string ToString()
        {
            return string.Format("{0:X4}:{1:X4}:{2:X4}:{3:X4}-{4:X4}:{5:X4}:{6:X4}:{7:X4}",
                SourceModule,
                SourceNumber,
                TargetModule,
                TargetNumber,
                LargeType,
                MiddleType,
                SmallType,
                Number);
        }
    }
}
