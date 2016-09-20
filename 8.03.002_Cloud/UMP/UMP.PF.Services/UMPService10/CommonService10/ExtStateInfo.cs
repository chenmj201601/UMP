//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b4584980-66b7-486c-b7af-c531488a7a78
//        CLR Version:              4.0.30319.18408
//        Name:                     ExtStateInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.CommonService10
//        File Name:                ExtStateInfo
//
//        created by Charley at 2016/6/28 14:06:20
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.CommonService10
{
    /// <summary>
    /// 分机状态信息
    /// </summary>
    public class ExtStateInfo
    {
        /// <summary>
        /// 分机ID
        /// </summary>
        public long ObjID { get; set; }
        /// <summary>
        /// 分机号（设备名）
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 状态类型
        /// </summary>
        public int StateType { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int State { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}][{1}][{2}][{3}]", ObjID, Name, StateType, State);
        }
    }
}
