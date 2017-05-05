//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7d2b1c4d-6361-4e2b-91f6-31a611b72d4c
//        CLR Version:              4.0.30319.18063
//        Name:                     RealExtInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService07
//        File Name:                RealExtInfo
//
//        created by Charley at 2015/11/24 14:00:35
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPService07
{
    public class RealExtInfo : ResourceConfigInfo
    {
        public long OrgID { get; set; }
        /// <summary>
        /// 状态
        /// 0：删除
        /// 1：正常
        /// 2：禁用
        /// </summary>
        public string Status { get; set; }
        public bool IsNew { get; set; }
        public bool IsLock { get; set; }
        public string LockMethod { get; set; }
        /// <summary>
        /// 来源类型
        /// 00：手动输入
        /// 21：录音表导入
        /// </summary>
        public string SourceType { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string ChanName { get; set; }

        public long VoiceObjID { get; set; }
        public int VoiceChanID { get; set; }
        public long VoiceChanObjID { get; set; }
        public long ScreenObjID { get; set; }
        public int ScreenChanID { get; set; }
        public long ScreenChanObjID { get; set; }

        public override string LogInfo()
        {
            string strInfo = base.LogInfo();
            strInfo =
                string.Format(
                    "{0};Name:{1};ChanName:{2};OrgID:{3};Status:{4};VoiceObjID:{5};VoiceChanID:{6};ScreenObjID:{7};ScreenChanID:{8}",
                    strInfo,
                    Name,
                    ChanName,
                    OrgID,
                    Status,
                    VoiceObjID,
                    VoiceChanID,
                    ScreenObjID,
                    ScreenChanID);
            return strInfo;
        }
    }
}
